/* global window, document, fetch, URLSearchParams */
(function () {
    'use strict';

    const CommunicationUI = {};
    const defaultConfig = {
        apiBase: '/Communication',
        pageSize: 50,
        pollIntervalMs: 5000,
        presencePollMs: 30000,
        currentUserId: 0
    };

    let cfg = { ...defaultConfig };

    // DOM refs
    let elConvList, elMessages, elTitle, elPresenceArea, elPresenceText, elPresenceDot, elBtnSend, elInput, elBtnStartDirect, elTxtDirectUserId, elBtnRefreshConvs;

    // timers
    let pollTimer = null;
    let presenceTimer = null;

    // state
    const state = {
        conversations: [],
        selectedConvId: null,
        // Per conversation: { messages: [], idSet: Set(), latestId, earliestId, hasMore, lastAckedOtherId, otherUserId }
        convData: new Map()
    };

    // API helpers
    const api = {
        async getConversations() {
            const res = await fetch(`${cfg.apiBase}/Conversations`, { credentials: 'same-origin' });
            if (!res.ok) throw new Error('Failed to load conversations');
            return res.json();
        },
        async getMessages(conversationId, beforeMessageId = null, pageSize = cfg.pageSize) {
            const qs = new URLSearchParams();
            qs.set('conversationId', String(conversationId));
            if (beforeMessageId != null) qs.set('beforeMessageId', String(beforeMessageId));
            if (pageSize != null) qs.set('pageSize', String(pageSize));
            const res = await fetch(`${cfg.apiBase}/Messages?${qs.toString()}`, { credentials: 'same-origin' });
            if (!res.ok) throw new Error('Failed to load messages');
            return res.json();
        },
        async startDirect(otherUserId) {
            const res = await fetch(`${cfg.apiBase}/StartDirect`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                // Send raw JSON number to match [FromBody] int otherUserId
                body: String(otherUserId),
                credentials: 'same-origin'
            });
            if (!res.ok) throw new Error('Failed to start direct conversation');
            return res.json();
        },
        async sendMessage(conversationId, body, contentType = 'text') {
            const res = await fetch(`${cfg.apiBase}/Send`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'same-origin',
                body: JSON.stringify({ conversationId, body, contentType })
            });
            if (!res.ok) throw new Error('Failed to send message');
            return res.json();
        },
        async ackRead(conversationId, messageId) {
            const res = await fetch(`${cfg.apiBase}/AckRead`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'same-origin',
                body: JSON.stringify({ conversationId, messageId })
            });
            if (!res.ok) throw new Error('Failed to ack read');
            return res.json();
        },
        async getPresence(userId) {
            const res = await fetch(`${cfg.apiBase}/Presence?userId=${encodeURIComponent(userId)}`, { credentials: 'same-origin' });
            if (!res.ok) throw new Error('Failed to get presence');
            return res.json();
        }
    };

    // Utils
    const camel = (o, a, b) => (o && (o[a] ?? o[b])) ?? null;
    const msgId = m => m.messageId ?? m.MessageId;
    const msgSender = m => m.senderUserId ?? m.SenderUserId;
    const msgBody = m => m.body ?? m.Body ?? '';
    const msgSentAt = m => m.sentAtUtc ?? m.SentAtUtc;
    const convId = c => c.conversationId ?? c.ConversationId;
    const convType = c => c.conversationType ?? c.ConversationType ?? 'Direct';
    const convTitle = (c) => {
        const id = convId(c);
        const type = convType(c);
        const name = c.title ?? c.Title ?? null;
        return name ? `${name}` : `${type} #${id}`;
    };

    function fmtTime(iso) {
        if (!iso) return '';
        const d = new Date(iso);
        const now = new Date();
        const sameDay = d.toDateString() === now.toDateString();
        const time = d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
        if (sameDay) return time;
        return `${d.toLocaleDateString()} ${time}`;
    }

    function atBottom(el, tolerance = 24) {
        return (el.scrollHeight - el.scrollTop - el.clientHeight) <= tolerance;
    }

    function ensureConvState(conversationId) {
        if (!state.convData.has(conversationId)) {
            state.convData.set(conversationId, {
                messages: [],
                idSet: new Set(),
                latestId: null,
                earliestId: null,
                hasMore: true,
                lastAckedOtherId: 0,
                otherUserId: null
            });
        }
        return state.convData.get(conversationId);
    }

    // Rendering
    function renderConversations() {
        elConvList.innerHTML = '';
        if (!state.conversations || !state.conversations.length) {
            elConvList.innerHTML = '<div class="item"><div class="meta">No conversations yet.</div></div>';
            return;
        }
        state.conversations.forEach(c => {
            const id = convId(c);
            const item = document.createElement('div');
            item.className = 'item' + (id === state.selectedConvId ? ' active' : '');
            item.dataset.conversationId = String(id);
            item.innerHTML = `
                <div class="title">
                    <span>${convTitle(c)}</span>
                    <small class="muted">#${id}</small>
                </div>
                <div class="meta">${convType(c)}</div>
            `;
            item.addEventListener('click', () => openConversation(id, c));
            elConvList.appendChild(item);
        });
    }

    function appendMessagesToDOM(conversationId, msgsAsc, { prepend = false } = {}) {
        // Remember scroll position if we are prepending
        let previousHeight = null;
        let previousTop = null;
        if (prepend) {
            previousHeight = elMessages.scrollHeight;
            previousTop = elMessages.scrollTop;
        }

        const frag = document.createDocumentFragment();
        msgsAsc.forEach(m => {
            const isMine = msgSender(m) === cfg.currentUserId;
            const row = document.createElement('div');
            row.className = 'msg-wrapper';

            const rowInner = document.createElement('div');
            rowInner.className = 'msg-row' + (isMine ? ' mine' : '');
            const bubble = document.createElement('div');
            bubble.className = 'msg' + (isMine ? ' mine' : '');
            bubble.setAttribute('data-message-id', msgId(m));
            bubble.innerHTML = `
                <div class="body">${escapeHtml(msgBody(m))}</div>
                <div class="meta">${isMine ? 'You' : 'User ' + msgSender(m)} · ${fmtTime(msgSentAt(m))}</div>
            `;
            rowInner.appendChild(bubble);
            row.appendChild(rowInner);
            frag.appendChild(row);
        });

        if (prepend) {
            elMessages.insertBefore(frag, elMessages.firstChild);
            // Adjust scroll to keep viewport stable
            const newHeight = elMessages.scrollHeight;
            elMessages.scrollTop = previousTop + (newHeight - previousHeight);
        } else {
            elMessages.appendChild(frag);
            // Scroll to bottom when appending batch if we were already near bottom
            if (atBottom(elMessages, 120)) {
                elMessages.scrollTop = elMessages.scrollHeight;
            }
        }
    }

    function escapeHtml(s) {
        return (s || '').replace(/[&<>"']/g, c => ({
            '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;'
        })[c]);
    }

    function setTitle(text) {
        elTitle.textContent = text || 'Select a conversation';
    }

    function setPresenceStatus(status) {
        // status: { isOnline, lastSeenUtc, lastActiveUtc } or null
        if (!status) {
            elPresenceDot.classList.remove('online');
            elPresenceDot.classList.add('offline');
            elPresenceText.textContent = '—';
            return;
        }
        const online = !!(status.isOnline ?? status.IsOnline);
        elPresenceDot.classList.toggle('online', online);
        elPresenceDot.classList.toggle('offline', !online);
        if (online) {
            elPresenceText.textContent = 'Online';
        } else {
            const lastSeen = status.lastSeenUtc ?? status.LastSeenUtc;
            elPresenceText.textContent = lastSeen ? `Last seen ${fmtTime(lastSeen)}` : 'Offline';
        }
    }

    async function refreshPresence() {
        const cid = state.selectedConvId;
        if (!cid) { setPresenceStatus(null); return; }
        const cs = ensureConvState(cid);
        const otherId = await getOtherUserId(cid);
        if (!otherId) { setPresenceStatus(null); return; }
        try {
            const p = await api.getPresence(otherId);
            setPresenceStatus(p);
        } catch {
            setPresenceStatus(null);
        }
    }

    async function openConversation(conversationId, convObj = null) {
        state.selectedConvId = conversationId;
        Array.from(elConvList.children).forEach(x => x.classList.toggle('active', Number(x.dataset.conversationId) === conversationId));
        setTitle(convObj ? convTitle(convObj) : `Conversation #${conversationId}`);
        elMessages.innerHTML = '';
        // Clear timers
        if (pollTimer) clearInterval(pollTimer);
        if (presenceTimer) clearInterval(presenceTimer);

        // Load initial page
        const cs = ensureConvState(conversationId);
        cs.messages = [];
        cs.idSet = new Set();
        cs.latestId = null;
        cs.earliestId = null;
        cs.hasMore = true;
        cs.lastAckedOtherId = 0;

        await loadInitialMessages(conversationId);
        await refreshPresence();

        // Set up polling
        pollTimer = setInterval(() => pullNewMessages(conversationId), cfg.pollIntervalMs);
        presenceTimer = setInterval(refreshPresence, cfg.presencePollMs);
    }

    async function loadInitialMessages(conversationId) {
        try {
            const data = await api.getMessages(conversationId, null, cfg.pageSize);
            const cs = ensureConvState(conversationId);
            const msgs = Array.isArray(data) ? data : [];
            // API returns newest-first; render oldest-first
            msgs.sort((a, b) => msgId(a) - msgId(b));
            msgs.forEach(m => {
                const id = msgId(m);
                if (!cs.idSet.has(id)) {
                    cs.idSet.add(id);
                    cs.messages.push(m);
                    cs.latestId = cs.latestId == null ? id : Math.max(cs.latestId, id);
                    cs.earliestId = cs.earliestId == null ? id : Math.min(cs.earliestId, id);
                }
            });
            appendMessagesToDOM(conversationId, msgs, { prepend: false });
            // Scroll fully bottom on initial load
            elMessages.scrollTop = elMessages.scrollHeight;

            // Ack read (highest other-user message)
            await maybeAckRead(conversationId, true);

        } catch (e) {
            console.error(e);
            elMessages.innerHTML = `<div class="muted small">Failed to load messages.</div>`;
        }
    }

    async function loadOlderMessages(conversationId) {
        const cs = ensureConvState(conversationId);
        if (!cs.hasMore || cs.earliestId == null) return;
        try {
            const older = await api.getMessages(conversationId, cs.earliestId, cfg.pageSize);
            const arr = Array.isArray(older) ? older : [];
            if (!arr.length) {
                cs.hasMore = false;
                return;
            }
            arr.sort((a, b) => msgId(a) - msgId(b));
            const newOnes = [];
            arr.forEach(m => {
                const id = msgId(m);
                if (!cs.idSet.has(id)) {
                    cs.idSet.add(id);
                    cs.messages.unshift(m);
                    cs.earliestId = Math.min(cs.earliestId, id);
                    newOnes.push(m);
                }
            });
            if (newOnes.length) {
                appendMessagesToDOM(conversationId, newOnes, { prepend: true });
            } else {
                // If all were duplicates, mark no-more to avoid loops
                cs.hasMore = arr.length >= cfg.pageSize ? cs.hasMore : false;
            }
        } catch (e) {
            console.error('loadOlderMessages', e);
        }
    }

    async function pullNewMessages(conversationId) {
        const cs = ensureConvState(conversationId);
        try {
            const latestBatch = await api.getMessages(conversationId, null, cfg.pageSize);
            const arr = Array.isArray(latestBatch) ? latestBatch : [];
            if (!arr.length) return;
            // Keep only messages newer than current latestId
            const newOnes = arr.filter(m => cs.latestId == null || msgId(m) > cs.latestId);
            if (!newOnes.length) return;
            newOnes.sort((a, b) => msgId(a) - msgId(b));
            newOnes.forEach(m => {
                const id = msgId(m);
                if (!cs.idSet.has(id)) {
                    cs.idSet.add(id);
                    cs.messages.push(m);
                    cs.latestId = Math.max(cs.latestId ?? id, id);
                }
            });
            const wasAtBottom = atBottom(elMessages, 160);
            appendMessagesToDOM(conversationId, newOnes, { prepend: false });
            if (wasAtBottom) {
                elMessages.scrollTop = elMessages.scrollHeight;
            }

            // Maybe ack read for newly received ones
            await maybeAckRead(conversationId, wasAtBottom);
        } catch (e) {
            console.error('pullNewMessages', e);
        }
    }

    async function maybeAckRead(conversationId, onlyIfBottom) {
        if (onlyIfBottom && !atBottom(elMessages, 200)) return;
        const cs = ensureConvState(conversationId);
        // Highest message ID from other user
        const other = [...cs.messages].filter(m => msgSender(m) !== cfg.currentUserId);
        if (!other.length) return;
        const highestOtherId = msgId(other[other.length - 1]);
        if (!highestOtherId || highestOtherId === cs.lastAckedOtherId) return;

        try {
            await api.ackRead(conversationId, highestOtherId);
            cs.lastAckedOtherId = highestOtherId;
        } catch (e) {
            // non-fatal
            console.warn('Ack read failed', e);
        }
    }

    async function getOtherUserId(conversationId) {
        // Try from conversation object (participants/otherUserId), else infer from messages
        const conv = state.conversations.find(c => convId(c) === conversationId);
        const cs = ensureConvState(conversationId);
        if (cs.otherUserId) return cs.otherUserId;

        // known mapping candidates:
        // conv.otherUserId / conv.OtherUserId
        let candidate = camel(conv, 'otherUserId', 'OtherUserId');
        if (!candidate) {
            // conv.participants / conv.Participants could be array of user IDs or objects
            const participants = camel(conv || {}, 'participants', 'Participants');
            if (Array.isArray(participants)) {
                for (const p of participants) {
                    const uid = typeof p === 'number' ? p : (p.userId ?? p.UserId);
                    if (uid && uid !== cfg.currentUserId) { candidate = uid; break; }
                }
            }
        }
        if (!candidate) {
            // infer from messages
            const msgs = cs.messages;
            for (let i = msgs.length - 1; i >= 0; i--) {
                const s = msgSender(msgs[i]);
                if (s !== cfg.currentUserId) { candidate = s; break; }
            }
        }
        if (candidate) cs.otherUserId = candidate;
        return candidate || null;
    }

    // Event wiring
    function wireEvents() {
        // Send on button
        elBtnSend.addEventListener('click', async () => {
            await handleSend();
        });
        // Send on Enter (Shift+Enter => newline)
        elInput.addEventListener('keydown', async (e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                await handleSend();
            }
        });
        // Load older on scroll top
        elMessages.addEventListener('scroll', async () => {
            if (elMessages.scrollTop <= 16 && state.selectedConvId) {
                await loadOlderMessages(state.selectedConvId);
            }
            // Opportunistic ack when user reaches bottom
            if (atBottom(elMessages, 12) && state.selectedConvId) {
                await maybeAckRead(state.selectedConvId, true);
            }
        });

        // Start direct
        elBtnStartDirect.addEventListener('click', async () => {
            await handleStartDirect();
        });
        elTxtDirectUserId.addEventListener('keydown', async (e) => {
            if (e.key === 'Enter') {
                await handleStartDirect();
            }
        });

        // Refresh conversations
        elBtnRefreshConvs.addEventListener('click', async () => {
            await reloadConversations();
        });
    }

    async function handleStartDirect() {
        const val = (elTxtDirectUserId.value || '').trim();
        const otherId = parseInt(val, 10);
        if (!otherId || otherId <= 0) {
            alert('Enter a valid user ID.');
            return;
        }
        try {
            const res = await api.startDirect(otherId);
            if (res && res.success && res.conversationId > 0) {
                // Refresh conversation list, then open the new one
                await reloadConversations();
                await openConversation(res.conversationId);
                elTxtDirectUserId.value = '';
            } else {
                alert('Failed to start direct conversation.');
            }
        } catch (e) {
            console.error(e);
            alert('Error starting direct conversation.');
        }
    }

    async function handleSend() {
        if (!state.selectedConvId) return;
        const text = (elInput.value || '').trim();
        if (!text) return;

        elBtnSend.disabled = true;
        try {
            const res = await api.sendMessage(state.selectedConvId, text, 'text');
            if (res && res.success) {
                elInput.value = '';
                // Quickly pull newest to reflect sent message with server ID/time
                await pullNewMessages(state.selectedConvId);
                // try ack (won't ack own msgs, but keeps behavior consistent)
                await maybeAckRead(state.selectedConvId, true);
            } else {
                alert('Message failed to send.');
            }
        } catch (e) {
            console.error(e);
            alert('Error sending message.');
        } finally {
            elBtnSend.disabled = false;
            elInput.focus();
        }
    }

    async function reloadConversations() {
        try {
            const list = await api.getConversations();
            state.conversations = Array.isArray(list) ? list : [];
            renderConversations();
        } catch (e) {
            console.error('reloadConversations', e);
        }
    }

    // init
    CommunicationUI.init = async function init(options = {}) {
        cfg = { ...defaultConfig, ...options };

        // DOM refs
        elConvList = document.getElementById('conversationList');
        elMessages = document.getElementById('messagesPane');
        elTitle = document.getElementById('convTitle');
        elPresenceArea = document.getElementById('presenceArea');
        elPresenceText = document.getElementById('presenceText');
        elPresenceDot = document.getElementById('presenceDot');
        elBtnSend = document.getElementById('btnSend');
        elInput = document.getElementById('messageInput');
        elBtnStartDirect = document.getElementById('btnStartDirect');
        elTxtDirectUserId = document.getElementById('txtDirectUserId');
        elBtnRefreshConvs = document.getElementById('btnRefreshConvs');

        wireEvents();

        await reloadConversations();

        // Optionally auto-open the latest conversation
        if (state.conversations.length) {
            const first = state.conversations[0];
            await openConversation(convId(first), first);
        }
    };

    // Expose
    window.CommunicationUI = CommunicationUI;
})();