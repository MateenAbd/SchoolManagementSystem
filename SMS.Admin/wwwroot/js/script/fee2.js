// wwwroot/js/script/fee.js
// PART 1/2
// Covers: Fee Heads, Fee Terms, Fee Structures (headers + details), Demand & Collection (generate + collect)
// Requires common.js (BASE_URL, $.ajaxSetup, showAlert, clearAlert)

(function () {
    "use strict";

    $(function () {
        if ($("#feePage").length === 0) return;

        const roles = ($("body").attr("data-roles") || "");
        const isAdmin = roles.includes("Admin");
        const isTeacher = roles.includes("Teacher");
        if (!(isAdmin || isTeacher)) return;

        // ---------------- Utilities ----------------
        function parseError(xhr) {
            if (xhr.responseJSON?.errors) return xhr.responseJSON.errors.join("<br/>");
            if (xhr.responseJSON?.error) return xhr.responseJSON.error;
            return xhr.status + " " + xhr.statusText;
        }
        function toLocalDate(d) {
            if (!d) return "-";
            const dt = new Date(d);
            return isNaN(dt) ? "-" : dt.toLocaleDateString();
        }
        function todayStr() { const t = new Date(); return t.toISOString().substring(0, 10); }
        function fmt(num) { return (Number(num || 0)).toFixed(2); }

        // Caches
        let feeHeadsCache = [];
        let feeTermsCache = [];

        // Fill selects
        function fillHeadsSelect($sel, includeAny = true) {
            const opts = [];
            if (includeAny) opts.push(`<option value="">Any</option>`);
            feeHeadsCache.sort((a, b) => (a.sortOrder || 0) - (b.sortOrder || 0)).forEach(h => {
                opts.push(`<option value="${h.headId}">${h.headCode} - ${h.headName}</option>`);
            });
            $sel.html(opts.join(""));
        }
        function fillHeadsSelectStrict($sel) {
            const opts = [`<option value="">Select</option>`];
            feeHeadsCache.sort((a, b) => (a.sortOrder || 0) - (b.sortOrder || 0)).forEach(h => {
                opts.push(`<option value="${h.headId}">${h.headCode} - ${h.headName}</option>`);
            });
            $sel.html(opts.join(""));
        }
        function termNameById(id) {
            const t = feeTermsCache.find(x => x.termId === id);
            return t ? (t.termCode + " - " + t.termName) : (id ?? "");
        }

        // Preload for dropdowns used across module
        function loadHeadsForOptions(cb) {
            $.ajax({
                url: "/Fee/GetFeeHeadList",
                type: "GET",
                dataType: "json",
                data: { isActive: "" }
            })
                .done(list => {
                    feeHeadsCache = Array.isArray(list) ? list : [];
                    // populate head selects used in structures and collection
                    fillHeadsSelectStrict($("#fsHeadSelectTemplate")); // hidden template element
                    // expose globally if needed by Part 2 (optional)
                    // window.feeHeadsCache = feeHeadsCache;
                })
                .always(() => cb && cb());
        }
        function loadTermsForOptions(cb) {
            $.ajax({
                url: "/Fee/GetFeeTermList",
                type: "GET",
                dataType: "json",
                data: { academicYear: "", isActive: "" }
            })
                .done(list => {
                    feeTermsCache = Array.isArray(list) ? list : [];
                    // populate structure term select
                    const opts = [`<option value="">Select</option>`];
                    feeTermsCache.sort((a, b) => (a.sequenceNo || 0) - (b.sequenceNo || 0)).forEach(t => {
                        opts.push(`<option value="${t.termId}">${t.academicYear} - ${t.termCode} - ${t.termName}</option>`);
                    });
                    $("#fsTerm").html(opts.join(""));
                })
                .always(() => cb && cb());
        }

        // =========================================================================
        // FEE HEADS
        // =========================================================================
        const fhModal = new bootstrap.Modal(document.getElementById("fhModal"), { backdrop: "static" });

        function renderHeads(list) {
            const $tb = $("#fhTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="7" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            const rows = list.sort((a, b) => (a.sortOrder || 0) - (b.sortOrder || 0)).map(h => `
        <tr data-id="${h.headId}">
          <td>${h.headId}</td>
          <td>${h.headCode}</td>
          <td>${h.headName}</td>
          <td>${h.sortOrder ?? ""}</td>
          <td>${h.description || ""}</td>
          <td>${h.isActive ? "Yes" : "No"}</td>
          <td>
            <div class="btn-group btn-group-sm">
              <button class="btn btn-outline-primary btn-fh-edit">Edit</button>
              ${isAdmin ? `<button class="btn btn-outline-danger btn-fh-del">Delete</button>` : ``}
            </div>
          </td>
        </tr>
      `).join("");
            $tb.html(rows);
        }
        function loadFeeHeads() {
            clearAlert("#fhMsg");
            const isActive = $("#fhFilterActive").val() || "";
            $("#fhTable tbody").html(`<tr><td colspan="7" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Fee/GetFeeHeadList",
                type: "GET",
                dataType: "json",
                data: { isActive }
            })
                .done(list => { renderHeads(list); feeHeadsCache = Array.isArray(list) ? list : []; })
                .fail(xhr => showAlert("#fhMsg", "danger", parseError(xhr)));
        }
        function openHeadModal(headId) {
            clearAlert("#fhFormMsg");
            $("#fhModalLabel").text(headId ? "Edit Fee Head" : "New Fee Head");
            $("#fhId").val(headId || 0);
            $("#fhCode,#fhName,#fhSort,#fhDesc").val("");
            $("#fhActive").val("true");
            if (headId && headId > 0) {
                $.ajax({
                    url: "/Fee/GetFeeHeadById",
                    type: "GET",
                    dataType: "json",
                    data: { headId }
                })
                    .done(h => {
                        $("#fhId").val(h.headId);
                        $("#fhCode").val(h.headCode);
                        $("#fhName").val(h.headName);
                        $("#fhSort").val(h.sortOrder ?? "");
                        $("#fhDesc").val(h.description || "");
                        $("#fhActive").val(h.isActive ? "true" : "false");
                    })
                    .fail(xhr => showAlert("#fhFormMsg", "danger", parseError(xhr)))
                    .always(() => fhModal.show());
            } else { fhModal.show(); }
        }
        function saveHead() {
            clearAlert("#fhFormMsg");
            const dto = {
                headId: parseInt($("#fhId").val(), 10) || 0,
                headCode: $("#fhCode").val().trim(),
                headName: $("#fhName").val().trim(),
                description: $("#fhDesc").val().trim() || null,
                sortOrder: $("#fhSort").val() ? parseInt($("#fhSort").val(), 10) : null,
                isActive: $("#fhActive").val() === "true"
            };
            if (!dto.headCode || !dto.headName) {
                showAlert("#fhFormMsg", "warning", "Head Code and Name are required.");
                return;
            }
            const $btn = $("#btnSaveFeeHead").prop("disabled", true).text("Saving...");
            const req = dto.headId > 0
                ? $.ajax({ url: "/Fee/UpdateFeeHead", type: "POST", dataType: "json", contentType: "application/json; charset=UTF-8", data: JSON.stringify(dto) })
                : $.ajax({ url: "/Fee/CreateFeeHead", type: "POST", dataType: "json", contentType: "application/json; charset=UTF-8", data: JSON.stringify(dto) });
            req.done(res => {
                if (res?.success) {
                    fhModal.hide();
                    showAlert("#fhMsg", "success", "Saved.");
                    loadFeeHeads();
                    loadHeadsForOptions(); // refresh for other tabs
                } else {
                    showAlert("#fhFormMsg", "warning", "Save failed.");
                }
            }).fail(xhr => showAlert("#fhFormMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Save"));
        }
        function deleteHead(headId) {
            $.ajax({
                url: "/Fee/DeleteFeeHead",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(headId)
            })
                .done(res => {
                    if (res?.success) {
                        showAlert("#fhMsg", "success", "Deleted.");
                        loadFeeHeads(); loadHeadsForOptions();
                    } else {
                        showAlert("#fhMsg", "warning", "Delete failed.");
                    }
                })
                .fail(xhr => showAlert("#fhMsg", "danger", parseError(xhr)));
        }

        $("#btnLoadFeeHeads").on("click", loadFeeHeads);
        $("#btnNewFeeHead").on("click", () => openHeadModal(0));
        $(document).on("click", ".btn-fh-edit", function () { const id = parseInt($(this).closest("tr").data("id"), 10); if (id) openHeadModal(id); });
        $(document).on("click", ".btn-fh-del", function () { const id = parseInt($(this).closest("tr").data("id"), 10); if (!id) return; if (!confirm("Delete head " + id + " ?")) return; deleteHead(id); });
        $("#btnSaveFeeHead").on("click", saveHead);

        // =========================================================================
        // FEE TERMS
        // =========================================================================
        const ftModal = new bootstrap.Modal(document.getElementById("ftModal"), { backdrop: "static" });

        function renderTerms(list) {
            const $tb = $("#ftTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="8" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            const rows = list.sort((a, b) => (a.sequenceNo || 0) - (b.sequenceNo || 0)).map(t => `
        <tr data-id="${t.termId}">
          <td>${t.termId}</td>
          <td>${t.academicYear}</td>
          <td>${t.termCode}</td>
          <td>${t.termName}</td>
          <td>${t.sequenceNo}</td>
          <td>${toLocalDate(t.dueDate)}</td>
          <td>${t.isActive ? "Yes" : "No"}</td>
          <td>
            <div class="btn-group btn-group-sm">
              <button class="btn btn-outline-primary btn-ft-edit">Edit</button>
              ${isAdmin ? `<button class="btn btn-outline-danger btn-ft-del">Delete</button>` : ``}
            </div>
          </td>
        </tr>
      `).join("");
            $tb.html(rows);
        }
        function loadFeeTerms() {
            clearAlert("#ftMsg");
            const academicYear = $("#ftFilterYear").val().trim() || "";
            const isActive = $("#ftFilterActive").val() || "";
            $("#ftTable tbody").html(`<tr><td colspan="8" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Fee/GetFeeTermList",
                type: "GET",
                dataType: "json",
                data: { academicYear, isActive }
            })
                .done(list => { renderTerms(list); feeTermsCache = Array.isArray(list) ? list : []; })
                .fail(xhr => showAlert("#ftMsg", "danger", parseError(xhr)));
        }
        function openTermModal(termId) {
            clearAlert("#ftFormMsg");
            $("#ftModalLabel").text(termId ? "Edit Fee Term" : "New Fee Term");
            $("#ftId").val(termId || 0);
            $("#ftYear,#ftCode,#ftName,#ftSeq,#ftDue").val("");
            $("#ftActive").val("true");
            if (termId && termId > 0) {
                $.ajax({
                    url: "/Fee/GetFeeTermById",
                    type: "GET",
                    dataType: "json",
                    data: { termId }
                })
                    .done(t => {
                        $("#ftId").val(t.termId);
                        $("#ftYear").val(t.academicYear || "");
                        $("#ftCode").val(t.termCode || "");
                        $("#ftName").val(t.termName || "");
                        $("#ftSeq").val(t.sequenceNo ?? "");
                        if (t.dueDate) $("#ftDue").val(new Date(t.dueDate).toISOString().substring(0, 10));
                        $("#ftActive").val(t.isActive ? "true" : "false");
                    })
                    .fail(xhr => showAlert("#ftFormMsg", "danger", parseError(xhr)))
                    .always(() => ftModal.show());
            } else { ftModal.show(); }
        }
        function saveTerm() {
            clearAlert("#ftFormMsg");
            const dto = {
                termId: parseInt($("#ftId").val(), 10) || 0,
                academicYear: $("#ftYear").val().trim(),
                termCode: $("#ftCode").val().trim(),
                termName: $("#ftName").val().trim(),
                sequenceNo: $("#ftSeq").val() ? parseInt($("#ftSeq").val(), 10) : 0,
                dueDate: $("#ftDue").val() || null,
                isActive: $("#ftActive").val() === "true"
            };
            if (!dto.academicYear || !dto.termCode || !dto.termName) {
                showAlert("#ftFormMsg", "warning", "AY, Term Code and Term Name are required.");
                return;
            }
            const $btn = $("#btnSaveFeeTerm").prop("disabled", true).text("Saving...");
            const req = dto.termId > 0
                ? $.ajax({ url: "/Fee/UpdateFeeTerm", type: "POST", dataType: "json", contentType: "application/json; charset=UTF-8", data: JSON.stringify(dto) })
                : $.ajax({ url: "/Fee/CreateFeeTerm", type: "POST", dataType: "json", contentType: "application/json; charset=UTF-8", data: JSON.stringify(dto) });
            req.done(res => {
                if (res?.success) {
                    ftModal.hide();
                    showAlert("#ftMsg", "success", "Saved.");
                    loadFeeTerms();
                    loadTermsForOptions(); // refresh structure term select
                } else {
                    showAlert("#ftFormMsg", "warning", "Save failed.");
                }
            }).fail(xhr => showAlert("#ftFormMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Save"));
        }
        function deleteTerm(termId) {
            $.ajax({
                url: "/Fee/DeleteFeeTerm",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(termId)
            })
                .done(res => {
                    if (res?.success) { showAlert("#ftMsg", "success", "Deleted."); loadFeeTerms(); loadTermsForOptions(); }
                    else showAlert("#ftMsg", "warning", "Delete failed.");
                })
                .fail(xhr => showAlert("#ftMsg", "danger", parseError(xhr)));
        }

        $("#btnLoadFeeTerms").on("click", loadFeeTerms);
        $("#btnNewFeeTerm").on("click", () => openTermModal(0));
        $(document).on("click", ".btn-ft-edit", function () { const id = parseInt($(this).closest("tr").data("id"), 10); if (id) openTermModal(id); });
        $(document).on("click", ".btn-ft-del", function () { const id = parseInt($(this).closest("tr").data("id"), 10); if (!id) return; if (!confirm("Delete term " + id + " ?")) return; deleteTerm(id); });
        $("#btnSaveFeeTerm").on("click", saveTerm);

        // =========================================================================
        // FEE STRUCTURES (headers + details)
        // =========================================================================
        const fsModal = new bootstrap.Modal(document.getElementById("fsModal"), { backdrop: "static" });
        let fsDetailRowSeq = 0;

        function renderStructures(list) {
            const $tb = $("#fsTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="7" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            const rows = list.map(s => `
        <tr data-id="${s.structureId}">
          <td>${s.structureId}</td>
          <td>${s.academicYear}</td>
          <td>${s.className}${s.section ? ("-" + s.section) : ""}</td>
          <td>${termNameById(s.termId)}</td>
          <td>${toLocalDate(s.effectiveFrom)}</td>
          <td>${s.isActive ? "Yes" : "No"}</td>
          <td>
            <div class="btn-group btn-group-sm">
              <button class="btn btn-outline-primary btn-fs-edit">Edit</button>
              ${isAdmin ? `<button class="btn btn-outline-danger btn-fs-del">Delete</button>` : ``}
            </div>
          </td>
        </tr>
      `).join("");
            $tb.html(rows);
        }

        function searchStructures() {
            clearAlert("#fsMsg");
            const q = {
                academicYear: $("#fsFilterYear").val().trim() || null,
                className: $("#fsFilterClass").val().trim() || null,
                section: $("#fsFilterSection").val().trim() || null,
                termId: $("#fsFilterTerm").val() ? parseInt($("#fsFilterTerm").val(), 10) : null,
                isActive: $("#fsFilterActive").val() || null
            };
            $("#fsTable tbody").html(`<tr><td colspan="7" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Fee/GetFeeStructureHeaders",
                type: "GET",
                dataType: "json",
                data: q
            })
                .done(renderStructures)
                .fail(xhr => showAlert("#fsMsg", "danger", parseError(xhr)));
        }

        function openStructureModal(structureId) {
            clearAlert("#fsFormMsg");
            $("#fsModalLabel").text(structureId ? "Edit Fee Structure" : "New Fee Structure");
            $("#fsId").val(structureId || 0);
            $("#fsYear,#fsClass,#fsSection,#fsEffective").val("");
            $("#fsTerm").val("");
            $("#fsActive").val("true");
            // reset details table
            $("#fsDetailsTable tbody").html(`<tr><td colspan="4" class="p-3 text-center text-muted">No rows</td></tr>`);
            fsDetailRowSeq = 0;

            if (structureId && structureId > 0) {
                $.ajax({
                    url: "/Fee/GetFeeStructureById",
                    type: "GET",
                    dataType: "json",
                    data: { structureId }
                })
                    .done(s => {
                        $("#fsId").val(s.structureId);
                        $("#fsYear").val(s.academicYear || "");
                        $("#fsClass").val(s.className || "");
                        $("#fsSection").val(s.section || "");
                        $("#fsTerm").val(s.termId || "");
                        $("#fsActive").val(s.isActive ? "true" : "false");
                        if (s.effectiveFrom) $("#fsEffective").val(new Date(s.effectiveFrom).toISOString().substring(0, 10));
                        if (Array.isArray(s.details) && s.details.length) {
                            $("#fsDetailsTable tbody").empty();
                            s.details.forEach((d, i) => addFsRow(d.headId, d.amount));
                        }
                    })
                    .fail(xhr => showAlert("#fsFormMsg", "danger", parseError(xhr)))
                    .always(() => fsModal.show());
            } else {
                fsModal.show();
            }
        }

        function addFsRow(headId, amount) {
            fsDetailRowSeq++;
            const idx = fsDetailRowSeq;
            const headOptions = feeHeadsCache.map(h => `<option value="${h.headId}" ${h.headId === headId ? 'selected' : ''}>${h.headCode} - ${h.headName}</option>`).join("");
            const row = `
        <tr data-row="${idx}">
          <td>${idx}</td>
          <td>
            <select class="form-select form-select-sm fs-head">
              <option value="">Select</option>
              ${headOptions}
            </select>
          </td>
          <td><input type="number" step="0.01" class="form-control form-control-sm fs-amount" value="${amount ?? ""}" /></td>
          <td><button class="btn btn-sm btn-outline-danger btn-fs-del-row">Remove</button></td>
        </tr>
      `;
            const $tb = $("#fsDetailsTable tbody");
            if ($tb.find("tr td").length === 1) $tb.empty();
            $tb.append(row);
        }

        function collectFsDetails() {
            const details = [];
            $("#fsDetailsTable tbody tr").each(function () {
                const headId = parseInt($(this).find(".fs-head").val(), 10);
                const amount = $(this).find(".fs-amount").val() ? parseFloat($(this).find(".fs-amount").val()) : 0;
                if (headId && amount > 0) details.push({ headId, amount });
            });
            return details;
        }

        function saveStructure() {
            clearAlert("#fsFormMsg");
            const dto = {
                structureId: parseInt($("#fsId").val(), 10) || 0,
                academicYear: $("#fsYear").val().trim(),
                className: $("#fsClass").val().trim(),
                section: $("#fsSection").val().trim() || null,
                termId: $("#fsTerm").val() ? parseInt($("#fsTerm").val(), 10) : 0,
                effectiveFrom: $("#fsEffective").val() || null,
                isActive: $("#fsActive").val() === "true",
                details: collectFsDetails()
            };
            if (!dto.academicYear || !dto.className || !dto.termId) {
                showAlert("#fsFormMsg", "warning", "AY, Class and Term are required.");
                return;
            }
            if (!dto.details.length) {
                showAlert("#fsFormMsg", "warning", "Add at least one detail row with head and amount.");
                return;
            }
            const $btn = $("#btnSaveStructure").prop("disabled", true).text("Saving...");
            $.ajax({
                url: "/Fee/UpsertFeeStructure",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(dto)
            })
                .done(res => {
                    if (res?.success) {
                        fsModal.hide();
                        showAlert("#fsMsg", "success", "Saved.");
                        searchStructures();
                    } else {
                        showAlert("#fsFormMsg", "warning", "Save failed.");
                    }
                })
                .fail(xhr => showAlert("#fsFormMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Save"));
        }
        function deleteStructure(structureId) {
            $.ajax({
                url: "/Fee/DeleteFeeStructure",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(structureId)
            })
                .done(res => {
                    if (res?.success) { showAlert("#fsMsg", "success", "Deleted."); searchStructures(); }
                    else showAlert("#fsMsg", "warning", "Delete failed.");
                })
                .fail(xhr => showAlert("#fsMsg", "danger", parseError(xhr)));
        }

        $("#btnLoadStructures, #btnSearchStructures").on("click", searchStructures);
        $("#btnNewStructure").on("click", () => openStructureModal(0));
        $(document).on("click", ".btn-fs-edit", function () { const id = parseInt($(this).closest("tr").data("id"), 10); if (id) openStructureModal(id); });
        $(document).on("click", ".btn-fs-del", function () { const id = parseInt($(this).closest("tr").data("id"), 10); if (!id) return; if (!confirm("Delete structure " + id + " ?")) return; deleteStructure(id); });
        $("#btnFsAddRow").on("click", () => addFsRow(null, null));
        $(document).on("click", ".btn-fs-del-row", function () { $(this).closest("tr").remove(); const $tb = $("#fsDetailsTable tbody"); if ($tb.find("tr").length === 0) $tb.html(`<tr><td colspan="4" class="p-3 text-center text-muted">No rows</td></tr>`); });
        $("#btnSaveStructure").on("click", saveStructure);

        // =========================================================================
        // DEMAND & COLLECTION
        // =========================================================================
        function loadBalanceText() {
            const studentId = $("#clStudentId").val() ? parseInt($("#clStudentId").val(), 10) : null;
            const academicYear = $("#clYear").val().trim() || null;
            const termId = $("#clTerm").val() ? parseInt($("#clTerm").val(), 10) : null;
            if (!studentId || !academicYear || !termId) { $("#clBalanceText").text("-"); return; }
            $.ajax({
                url: "/Fee/GetStudentFeeBalance",
                type: "GET",
                dataType: "json",
                data: { studentId, academicYear, termId }
            })
                .done(dto => {
                    if (!dto) { $("#clBalanceText").text("-"); return; }
                    const bal = dto.balance ?? dto.Balance ?? null;
                    $("#clBalanceText").text(bal != null ? fmt(bal) : "-");
                })
                .fail(() => $("#clBalanceText").text("-"));
        }
        $("#clStudentId, #clYear, #clTerm").on("change input", loadBalanceText);

        $("#btnGenerateDemand").on("click", function () {
            clearAlert("#gdMsg");
            const payload = {
                studentId: $("#gdStudentId").val() ? parseInt($("#gdStudentId").val(), 10) : 0,
                academicYear: $("#gdYear").val().trim(),
                termId: $("#gdTerm").val() ? parseInt($("#gdTerm").val(), 10) : 0
            };
            if (!payload.studentId || !payload.academicYear || !payload.termId) {
                showAlert("#gdMsg", "warning", "Student ID, AY and Term are required.");
                return;
            }
            const $btn = $(this).prop("disabled", true).text("Posting...");
            $.ajax({
                url: "/Fee/GenerateStudentTermFee",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(payload)
            })
                .done(res => {
                    if (res?.success) showAlert("#gdMsg", "success", "Demand generated/posted: " + res.posted);
                    else showAlert("#gdMsg", "warning", "Generate failed.");
                })
                .fail(xhr => showAlert("#gdMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Generate Demand"));
        });

        // Collect Fee - items table
        function renderClEmpty() {
            $("#clItemsTable tbody").html(`<tr><td colspan="4" class="p-3 text-center text-muted">No rows</td></tr>`);
            $("#clTotal").text("0.00");
        }
        function clRecalcTotal() {
            let total = 0;
            $("#clItemsTable tbody tr").each(function () {
                const amt = $(this).find(".cl-amount").val();
                if (amt) total += parseFloat(amt) || 0;
            });
            $("#clTotal").text(fmt(total));
        }
        function clAddRow(headId, amount) {
            const headOpts = feeHeadsCache.map(h => `<option value="${h.headId}" ${h.headId === headId ? 'selected' : ''}>${h.headCode} - ${h.headName}</option>`).join("");
            const row = `
        <tr>
          <td></td>
          <td>
            <select class="form-select form-select-sm cl-head">
              <option value="">Select</option>
              ${headOpts}
            </select>
          </td>
          <td><input type="number" step="0.01" class="form-control form-control-sm cl-amount" value="${amount ?? ""}" /></td>
          <td><button class="btn btn-sm btn-outline-danger btn-cl-del-row">Remove</button></td>
        </tr>
      `;
            const $tb = $("#clItemsTable tbody");
            if ($tb.find("tr td").length === 1) $tb.empty();
            $tb.append(row);
            clRecalcTotal();
        }
        $("#btnClAddRow").on("click", () => clAddRow(null, null));
        $(document).on("click", ".btn-cl-del-row", function () { $(this).closest("tr").remove(); const $tb = $("#clItemsTable tbody"); if ($tb.find("tr").length === 0) renderClEmpty(); clRecalcTotal(); });
        $(document).on("input", ".cl-amount", clRecalcTotal);

        $("#btnClClear").on("click", function () {
            $("#clStudentId,#clYear,#clTerm,#clRef").val("");
            $("#clMode").val("Cash");
            $("#clDate").val(todayStr());
            renderClEmpty();
            loadBalanceText();
        });

        $("#btnCollectFee").on("click", function () {
            clearAlert("#clMsg");
            const studentId = $("#clStudentId").val() ? parseInt($("#clStudentId").val(), 10) : 0;
            const academicYear = $("#clYear").val().trim();
            const termId = $("#clTerm").val() ? parseInt($("#clTerm").val(), 10) : 0;
            const receiptDate = $("#clDate").val() || todayStr();
            const paymentMode = $("#clMode").val();
            const referenceNo = $("#clRef").val().trim() || null;

            const items = [];
            $("#clItemsTable tbody tr").each(function () {
                const headId = parseInt($(this).find(".cl-head").val(), 10);
                const amount = $(this).find(".cl-amount").val() ? parseFloat($(this).find(".cl-amount").val()) : 0;
                if (headId && amount > 0) items.push({ headId, amount });
            });

            if (!studentId || !academicYear || !termId) {
                showAlert("#clMsg", "warning", "Student, AY and Term are required.");
                return;
            }
            if (items.length === 0) {
                showAlert("#clMsg", "warning", "Add at least one receipt item.");
                return;
            }

            const payload = {
                studentId, academicYear, termId,
                paymentMode, referenceNo, receiptDate,
                receivedByUserId: (parseInt($("body").attr("data-user-id") || "0", 10) || null),
                items
            };

            const $btn = $(this).prop("disabled", true).text("Collecting...");
            $.ajax({
                url: "/Fee/CollectStudentFee",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(payload)
            })
                .done(res => {
                    if (res?.success) {
                        showAlert("#clMsg", "success", "Collected. Receipt ID = " + res.receiptId);
                        $("#rcptTitle").text("#" + res.receiptId);
                        $("#btnLoadReceipts").trigger("click");
                        $("#btnClClear").trigger("click");
                    } else {
                        showAlert("#clMsg", "warning", "Collection failed.");
                    }
                })
                .fail(xhr => showAlert("#clMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Collect"));
        });

        // ---------------- Initial Loads ----------------
        $("#clDate").val(todayStr());
        loadHeadsForOptions(() => {
            loadFeeHeads();
        });
        loadTermsForOptions(() => {
            loadFeeTerms();
        });
        // structures header list initial
        searchStructures();
        // initial empty for collection
        renderClEmpty();
    });
})();


// wwwroot/js/script/fee.js
// PART 2/2
// Covers: Receipts & Ledger, Rules & Discounts (Fine Rules, Discount Schemes, Scholarships),
// Adjustments, Online Payment (initiate/check), Receipt viewer modal
// Requires common.js (BASE_URL, $.ajaxSetup, showAlert, clearAlert)

(function () {
    "use strict";

    $(function () {
        if ($("#feePage").length === 0) return;

        const roles = ($("body").attr("data-roles") || "");
        const isAdmin = roles.includes("Admin");
        const isTeacher = roles.includes("Teacher");
        if (!(isAdmin || isTeacher)) return;

        // ---------------- Utilities ----------------
        function parseError(xhr) {
            if (xhr.responseJSON?.errors) return xhr.responseJSON.errors.join("<br/>");
            if (xhr.responseJSON?.error) return xhr.responseJSON.error;
            return xhr.status + " " + xhr.statusText;
        }
        function toLocalDate(d) {
            if (!d) return "-";
            const dt = new Date(d);
            return isNaN(dt) ? "-" : dt.toLocaleDateString();
        }
        function toLocalDateTime(d) {
            if (!d) return "-";
            const dt = new Date(d);
            return isNaN(dt) ? "-" : dt.toLocaleString();
        }
        function todayStr() { const t = new Date(); return t.toISOString().substring(0, 10); }
        function fmt(num) { return (Number(num || 0)).toFixed(2); }

        // Reuse fee heads cache (if Part 1 loaded)
        let feeHeadsCache = window.feeHeadsCache || [];
        function headNameById(id) {
            const h = (feeHeadsCache || []).find(x => x.headId === id);
            return h ? (h.headCode + " - " + h.headName) : (id ?? "");
        }

        // =========================================================================
        // RECEIPTS & LEDGER
        // =========================================================================
        const rcptModal = new bootstrap.Modal(document.getElementById("rcptModal"), { backdrop: "static" });

        function renderReceipts(list) {
            const $tb = $("#rcTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="8" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            const rows = list.map(r => {
                const date = r.receiptDate || r.ReceiptDate || r.createdAtUtc;
                const amt = r.amount ?? r.totalAmount ?? r.Amount ?? 0;
                const student = r.studentId ?? "-";
                const term = r.termId ?? "-";
                const mode = r.paymentMode || r.mode || "-";
                return `
          <tr data-id="${r.receiptId}">
            <td>${r.receiptId}</td>
            <td>${r.academicYear || ""}</td>
            <td>${student}</td>
            <td>${term}</td>
            <td>${toLocalDate(date)}</td>
            <td>${mode}</td>
            <td>${fmt(amt)}</td>
            <td>
              <div class="btn-group btn-group-sm">
                <button class="btn btn-outline-primary btn-rc-view">View</button>
              </div>
            </td>
          </tr>
        `;
            }).join("");
            $tb.html(rows);
        }

        $("#btnLoadReceipts").on("click", function () {
            clearAlert("#rcMsg");
            const q = {
                academicYear: $("#rcYear").val().trim() || null,
                studentId: $("#rcStudentId").val() ? parseInt($("#rcStudentId").val(), 10) : null,
                termId: $("#rcTerm").val() ? parseInt($("#rcTerm").val(), 10) : null,
                fromDate: $("#rcFrom").val() || null,
                toDate: $("#rcTo").val() || null,
                paymentMode: $("#rcMode").val().trim() || null
            };
            $("#rcTable tbody").html(`<tr><td colspan="8" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Fee/GetFeeReceiptList",
                type: "GET",
                dataType: "json",
                data: q
            })
                .done(renderReceipts)
                .fail(xhr => showAlert("#rcMsg", "danger", parseError(xhr)));
        });

        $(document).on("click", ".btn-rc-view", function () {
            const id = parseInt($(this).closest("tr").data("id"), 10);
            if (!id) return;
            clearAlert("#rcptMsg");
            $("#rcptId").text("-");
            $("#rcptStudent").text("-");
            $("#rcptAY").text("-");
            $("#rcptTerm").text("-");
            $("#rcptDate").text("-");
            $("#rcptMode").text("-");
            $("#rcptRef").text("-");
            $("#rcptAmount").text("-");
            $("#rcptItemsTable tbody").html(`<tr><td colspan="3" class="p-3 text-center text-muted">Loading...</td></tr>`);

            $.ajax({
                url: "/Fee/GetFeeReceiptById",
                type: "GET",
                dataType: "json",
                data: { receiptId: id }
            })
                .done(r => {
                    $("#rcptTitle").text("#" + id);
                    $("#rcptId").text(r.receiptId);
                    $("#rcptStudent").text(r.studentId ?? "-");
                    $("#rcptAY").text(r.academicYear || "-");
                    $("#rcptTerm").text(r.termId ?? "-");
                    $("#rcptDate").text(toLocalDate(r.receiptDate || r.createdAtUtc));
                    $("#rcptMode").text(r.paymentMode || r.mode || "-");
                    $("#rcptRef").text(r.referenceNo || "-");
                    $("#rcptAmount").text(fmt(r.amount ?? r.totalAmount ?? 0));
                })
                .fail(xhr => showAlert("#rcptMsg", "danger", parseError(xhr)));

            $.ajax({
                url: "/Fee/GetFeeReceiptItems",
                type: "GET",
                dataType: "json",
                data: { receiptId: id }
            })
                .done(items => {
                    const $tb = $("#rcptItemsTable tbody");
                    if (!Array.isArray(items) || items.length === 0) {
                        $tb.html(`<tr><td colspan="3" class="p-3 text-center text-muted">No items</td></tr>`);
                        return;
                    }
                    const rows = items.map(i => `
            <tr>
              <td>${i.receiptItemId}</td>
              <td>${headNameById(i.headId)}</td>
              <td>${fmt(i.amount)}</td>
            </tr>
          `).join("");
                    $tb.html(rows);
                })
                .fail(xhr => $("#rcptItemsTable tbody").html(`<tr><td colspan="3" class="p-3 text-center text-danger">${parseError(xhr)}</td></tr>`));

            rcptModal.show();
        });

        // Ledger
        function renderLedger(list) {
            const $tb = $("#lgTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="7" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            const rows = list.map(l => {
                const date = l.entryDate || l.date || l.createdAtUtc;
                const type = l.type || l.entryType || "-";
                const head = l.headName || headNameById(l.headId) || "-";
                //const debit = l.debit ?? l.Debit ?? 0;
                //const credit = l.credit ?? l.Credit ?? 0;
                const amount = l.amount ?? l.amount ?? 0;
                const balance = l.balance ?? l.Balance ?? 0;
                const narr = l.narration || "-";
                return `
          <tr>
            <td>${toLocalDateTime(date)}</td>
            <td>${type}</td>
            <td>${head}</td>
            <td>${fmt(amount)}</td>
            <td>${fmt(balance)}</td>
            <td>${narr}</td>
          </tr>
        `;
            }).join("");
            $tb.html(rows);
        }

        $("#btnLoadLedger").on("click", function () {
            clearAlert("#rcMsg");
            const q = {
                studentId: $("#lgStudentId").val() ? parseInt($("#lgStudentId").val(), 10) : 0,
                academicYear: $("#lgYear").val().trim() || null,
                termId: $("#lgTerm").val() ? parseInt($("#lgTerm").val(), 10) : null
            };
            if (!q.studentId) { showAlert("#rcMsg", "warning", "Student ID is required for ledger."); return; }
            $("#lgTable tbody").html(`<tr><td colspan="7" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Fee/GetStudentLedger",
                type: "GET",
                dataType: "json",
                data: q
            })
                .done(renderLedger)
                .fail(xhr => showAlert("#rcMsg", "danger", parseError(xhr)));
        });

        // =========================================================================
        // RULES & DISCOUNTS (Fine Rules, Discount Schemes, Scholarships)
        // =========================================================================
        const frModal = new bootstrap.Modal(document.getElementById("frModal"), { backdrop: "static" });
        const dsModal = new bootstrap.Modal(document.getElementById("dsModal"), { backdrop: "static" });
        const schModal = new bootstrap.Modal(document.getElementById("schModal"), { backdrop: "static" });

        let fineRulesCache = [];
        let schemesCache = [];
        let scholarshipsCache = [];

        // Fine Rules list
        function renderFineRules(list) {
            const $tb = $("#frTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="10" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            fineRulesCache = list;
            const rows = list.map(r => `
        <tr data-id="${r.ruleId}">
          <td>${r.ruleId}</td>
          <td>${r.academicYear || ""}</td>
          <td>${(r.className || "") + (r.section ? ("-" + r.section) : "")}</td>
          <td>${r.termId}</td>
          <td>${r.mode}</td>
          <td>${fmt(r.rate)}</td>
          <td>${r.maxAmount != null ? fmt(r.maxAmount) : "-"}</td>
          <td>${r.fineHeadId}</td>
          <td>${r.isActive ? "Yes" : "No"}</td>
          <td>
            <div class="btn-group btn-group-sm">
              <button class="btn btn-outline-primary btn-fr-edit">Edit</button>
            </div>
          </td>
        </tr>
      `).join("");
            $tb.html(rows);
        }
        function loadFineRules() {
            clearAlert("#rdMsg");
            const q = {
                academicYear: $("#frYear").val().trim() || null,
                className: $("#frClass").val().trim() || null,
                section: $("#frSection").val().trim() || null,
                termId: $("#frTerm").val() ? parseInt($("#frTerm").val(), 10) : null,
                isActive: $("#frActive").val() || null
            };
            $("#frTable tbody").html(`<tr><td colspan="10" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Fee/GetFeeFineRules",
                type: "GET",
                dataType: "json",
                data: q
            })
                .done(renderFineRules)
                .fail(xhr => showAlert("#rdMsg", "danger", parseError(xhr)));
        }
        // Fine Rule modal open/save
        function openFineRuleModal(ruleId) {
            clearAlert("#frFormMsg");
            $("#frModalLabel").text(ruleId ? "Edit Fine Rule" : "New Fine Rule");
            $("#frId").val(ruleId || 0);
            $("#frFAy,#frFClass,#frFSection,#frFTerm,#frGrace,#frRate,#frMax,#frHead").val("");
            $("#frMode").val("PerDayFixed");
            $("#frFActive").val("true");

            if (ruleId && ruleId > 0) {
                const r = fineRulesCache.find(x => x.ruleId === ruleId);
                if (r) {
                    $("#frFAy").val(r.academicYear || "");
                    $("#frFClass").val(r.className || "");
                    $("#frFSection").val(r.section || "");
                    $("#frFTerm").val(r.termId || "");
                    $("#frFActive").val(r.isActive ? "true" : "false");
                    $("#frGrace").val(r.graceDays ?? "");
                    $("#frMode").val(r.mode || "PerDayFixed");
                    $("#frRate").val(r.rate ?? "");
                    $("#frMax").val(r.maxAmount ?? "");
                    $("#frHead").val(r.fineHeadId ?? "");
                }
            }
            frModal.show();
        }
        function saveFineRule() {
            clearAlert("#frFormMsg");
            const dto = {
                ruleId: parseInt($("#frId").val(), 10) || 0,
                academicYear: $("#frFAy").val().trim(),
                className: $("#frFClass").val().trim() || null,
                section: $("#frFSection").val().trim() || null,
                termId: $("#frFTerm").val() ? parseInt($("#frFTerm").val(), 10) : 0,
                graceDays: $("#frGrace").val() ? parseInt($("#frGrace").val(), 10) : 0,
                mode: $("#frMode").val(),
                rate: $("#frRate").val() ? parseFloat($("#frRate").val()) : 0,
                maxAmount: $("#frMax").val() ? parseFloat($("#frMax").val()) : null,
                fineHeadId: $("#frHead").val() ? parseInt($("#frHead").val(), 10) : 0,
                isActive: $("#frFActive").val() === "true"
            };
            if (!dto.academicYear || !dto.termId || !dto.mode || !dto.fineHeadId) {
                showAlert("#frFormMsg", "warning", "AY, Term, Mode, Fine Head are required.");
                return;
            }
            const $btn = $("#btnSaveFineRule").prop("disabled", true).text("Saving...");
            $.ajax({
                url: "/Fee/UpsertFeeFineRule",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(dto)
            })
                .done(res => {
                    if (res?.success) {
                        frModal.hide();
                        showAlert("#rdMsg", "success", "Saved.");
                        loadFineRules();
                    } else {
                        showAlert("#frFormMsg", "warning", "Save failed.");
                    }
                })
                .fail(xhr => showAlert("#frFormMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Save"));
        }

        $("#btnLoadFineRules, #btnSearchFineRules").on("click", loadFineRules);
        $("#btnNewFineRule").on("click", () => openFineRuleModal(0));
        $(document).on("click", ".btn-fr-edit", function () { const id = parseInt($(this).closest("tr").data("id"), 10); if (id) openFineRuleModal(id); });
        $("#btnSaveFineRule").on("click", saveFineRule);

        // Discount Schemes
        function renderSchemes(list) {
            const $tb = $("#dsTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="10" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            schemesCache = list;
            const rows = list.map(s => {
                const scope = `${s.academicYear || ""} ${s.className || ""}${s.section ? ("-" + s.section) : ""}${s.termId ? (" T" + s.termId) : ""}`.trim();
                return `
          <tr data-id="${s.schemeId}">
            <td>${s.schemeId}</td>
            <td>${s.schemeCode}</td>
            <td>${s.schemeName}</td>
            <td>${scope || "-"}</td>
            <td>${s.mode}</td>
            <td>${fmt(s.value)}</td>
            <td>${s.capAmount != null ? fmt(s.capAmount) : "-"}</td>
            <td>${s.discountHeadId}</td>
            <td>${s.isActive ? "Yes" : "No"}</td>
            <td>
              <div class="btn-group btn-group-sm">
                <button class="btn btn-outline-primary btn-ds-edit">Edit</button>
              </div>
            </td>
          </tr>
        `;
            }).join("");
            $tb.html(rows);
        }
        function loadSchemes() {
            clearAlert("#rdMsg");
            const q = {
                academicYear: $("#dsYear").val().trim() || null,
                className: $("#dsClass").val().trim() || null,
                section: $("#dsSection").val().trim() || null,
                termId: $("#dsTerm").val() ? parseInt($("#dsTerm").val(), 10) : null,
                isActive: $("#dsActive").val() || null
            };
            $("#dsTable tbody").html(`<tr><td colspan="10" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Fee/GetFeeDiscountSchemes",
                type: "GET",
                dataType: "json",
                data: q
            })
                .done(renderSchemes)
                .fail(xhr => showAlert("#rdMsg", "danger", parseError(xhr)));
        }
        function openSchemeModal(schemeId) {
            clearAlert("#dsFormMsg");
            $("#dsModalLabel").text(schemeId ? "Edit Discount Scheme" : "New Discount Scheme");
            $("#dsId").val(schemeId || 0);
            $("#dsCode,#dsName,#dsAy,#dsClassVal,#dsSectionVal,#dsTermVal,#dsValue,#dsCap,#dsHead").val("");
            $("#dsMode").val("Percent");
            $("#dsActiveVal").val("true");

            if (schemeId && schemeId > 0) {
                const s = schemesCache.find(x => x.schemeId === schemeId);
                if (s) {
                    $("#dsCode").val(s.schemeCode || "");
                    $("#dsName").val(s.schemeName || "");
                    $("#dsActiveVal").val(s.isActive ? "true" : "false");
                    $("#dsAy").val(s.academicYear || "");
                    $("#dsClassVal").val(s.className || "");
                    $("#dsSectionVal").val(s.section || "");
                    $("#dsTermVal").val(s.termId ?? "");
                    $("#dsMode").val(s.mode || "Percent");
                    $("#dsValue").val(s.value ?? "");
                    $("#dsCap").val(s.capAmount ?? "");
                    $("#dsHead").val(s.discountHeadId ?? "");
                }
            }
            dsModal.show();
        }
        function saveScheme() {
            clearAlert("#dsFormMsg");
            const dto = {
                schemeId: parseInt($("#dsId").val(), 10) || 0,
                schemeCode: $("#dsCode").val().trim(),
                schemeName: $("#dsName").val().trim(),
                academicYear: $("#dsAy").val().trim() || null,
                className: $("#dsClassVal").val().trim() || null,
                section: $("#dsSectionVal").val().trim() || null,
                termId: $("#dsTermVal").val() ? parseInt($("#dsTermVal").val(), 10) : null,
                mode: $("#dsMode").val(),
                value: $("#dsValue").val() ? parseFloat($("#dsValue").val()) : 0,
                capAmount: $("#dsCap").val() ? parseFloat($("#dsCap").val()) : null,
                discountHeadId: $("#dsHead").val() ? parseInt($("#dsHead").val(), 10) : 0,
                isActive: $("#dsActiveVal").val() === "true"
            };
            if (!dto.schemeCode || !dto.schemeName || !dto.mode || !dto.discountHeadId) {
                showAlert("#dsFormMsg", "warning", "Scheme Code, Name, Mode and Discount Head are required.");
                return;
            }
            const $btn = $("#btnSaveScheme").prop("disabled", true).text("Saving...");
            $.ajax({
                url: "/Fee/UpsertFeeDiscountScheme",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(dto)
            })
                .done(res => {
                    if (res?.success) {
                        dsModal.hide();
                        showAlert("#rdMsg", "success", "Saved.");
                        loadSchemes();
                    } else {
                        showAlert("#dsFormMsg", "warning", "Save failed.");
                    }
                })
                .fail(xhr => showAlert("#dsFormMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Save"));
        }

        $("#btnLoadSchemes, #btnSearchSchemes").on("click", loadSchemes);
        $("#btnNewScheme").on("click", () => openSchemeModal(0));
        $(document).on("click", ".btn-ds-edit", function () { const id = parseInt($(this).closest("tr").data("id"), 10); if (id) openSchemeModal(id); });
        $("#btnSaveScheme").on("click", saveScheme);

        // Scholarships
        function renderScholarships(list) {
            const $tb = $("#schTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="10" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            scholarshipsCache = list;
            const rows = list.map(s => `
        <tr data-id="${s.scholarshipId}">
          <td>${s.scholarshipId}</td>
          <td>${s.studentId}</td>
          <td>${s.academicYear || ""}</td>
          <td>${s.termId ?? ""}</td>
          <td>${s.mode}</td>
          <td>${fmt(s.value)}</td>
          <td>${s.capAmount != null ? fmt(s.capAmount) : "-"}</td>
          <td>${s.scholarshipHeadId}</td>
          <td>${s.isActive ? "Yes" : "No"}</td>
          <td>
            <div class="btn-group btn-group-sm">
              <button class="btn btn-outline-primary btn-sch-edit">Edit</button>
            </div>
          </td>
        </tr>
      `).join("");
            $tb.html(rows);
        }
        function loadScholarships() {
            clearAlert("#rdMsg");
            const q = {
                studentId: $("#schStudentId").val() ? parseInt($("#schStudentId").val(), 10) : null,
                academicYear: $("#schYear").val().trim() || null,
                termId: $("#schTerm").val() ? parseInt($("#schTerm").val(), 10) : null,
                isActive: $("#schActive").val() || null
            };
            $("#schTable tbody").html(`<tr><td colspan="10" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Fee/GetStudentScholarships",
                type: "GET",
                dataType: "json",
                data: q
            })
                .done(renderScholarships)
                .fail(xhr => showAlert("#rdMsg", "danger", parseError(xhr)));
        }
        function openScholarshipModal(id) {
            clearAlert("#schFormMsg");
            $("#schModalLabel").text(id ? "Edit Scholarship" : "New Scholarship");
            $("#schId").val(id || 0);
            $("#schStudent,#schAy,#schTermVal,#schMode,#schValue,#schCap,#schHead,#schScheme").val("");
            $("#schMode").val("Percent");
            $("#schActiveVal").val("true");

            if (id && id > 0) {
                const s = scholarshipsCache.find(x => x.scholarshipId === id);
                if (s) {
                    $("#schStudent").val(s.studentId || "");
                    $("#schAy").val(s.academicYear || "");
                    $("#schTermVal").val(s.termId ?? "");
                    $("#schMode").val(s.mode || "Percent");
                    $("#schValue").val(s.value ?? "");
                    $("#schCap").val(s.capAmount ?? "");
                    $("#schHead").val(s.scholarshipHeadId ?? "");
                    $("#schScheme").val(s.schemeId ?? "");
                    $("#schActiveVal").val(s.isActive ? "true" : "false");
                }
            }
            schModal.show();
        }
        function saveScholarship() {
            clearAlert("#schFormMsg");
            const dto = {
                scholarshipId: parseInt($("#schId").val(), 10) || 0,
                studentId: $("#schStudent").val() ? parseInt($("#schStudent").val(), 10) : 0,
                academicYear: $("#schAy").val().trim(),
                termId: $("#schTermVal").val() ? parseInt($("#schTermVal").val(), 10) : null,
                schemeId: $("#schScheme").val() ? parseInt($("#schScheme").val(), 10) : null,
                mode: $("#schMode").val(),
                value: $("#schValue").val() ? parseFloat($("#schValue").val()) : 0,
                capAmount: $("#schCap").val() ? parseFloat($("#schCap").val()) : null,
                scholarshipHeadId: $("#schHead").val() ? parseInt($("#schHead").val(), 10) : 0,
                isActive: $("#schActiveVal").val() === "true"
            };
            if (!dto.studentId || !dto.academicYear || !dto.mode || !dto.scholarshipHeadId) {
                showAlert("#schFormMsg", "warning", "Student, AY, Mode and Head are required.");
                return;
            }
            const $btn = $("#btnSaveScholarship").prop("disabled", true).text("Saving...");
            $.ajax({
                url: "/Fee/UpsertStudentScholarship",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(dto)
            })
                .done(res => {
                    if (res?.success) {
                        schModal.hide();
                        showAlert("#rdMsg", "success", "Saved.");
                        loadScholarships();
                    } else {
                        showAlert("#schFormMsg", "warning", "Save failed.");
                    }
                })
                .fail(xhr => showAlert("#schFormMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Save"));
        }

        $("#btnLoadScholarships, #btnSearchScholarships").on("click", loadScholarships);
        $("#btnNewScholarship").on("click", () => openScholarshipModal(0));
        $(document).on("click", ".btn-sch-edit", function () { const id = parseInt($(this).closest("tr").data("id"), 10); if (id) openScholarshipModal(id); });
        $("#btnSaveScholarship").on("click", saveScholarship);

        // =========================================================================
        // ADJUSTMENTS
        // =========================================================================
        function renderAdjustments(list) {
            const $tb = $("#adjTable tbody");
            if (!Array.isArray(list) || list.length === 0) {
                $tb.html(`<tr><td colspan="9" class="p-3 text-center text-muted">No data</td></tr>`);
                return;
            }
            const rows = list.map(a => `
        <tr>
          <td>${a.adjustmentId}</td>
          <td>${toLocalDateTime(a.entryDate || a.createdAtUtc)}</td>
          <td>${a.studentId}</td>
          <td>${a.academicYear || ""}</td>
          <td>${a.termId ?? ""}</td>
          <td>${a.type || ""}</td>
          <td>${a.headId ?? ""}</td>
          <td>${fmt(a.amount)}</td>
          <td>${a.narration || ""}</td>
        </tr>
      `).join("");
            $tb.html(rows);
        }

        $("#btnSaveAdjustment").on("click", function () {
            clearAlert("#adjMsg");
            const dto = {
                studentId: $("#adjStudentId").val() ? parseInt($("#adjStudentId").val(), 10) : 0,
                academicYear: $("#adjYear").val().trim(),
                termId: $("#adjTerm").val() ? parseInt($("#adjTerm").val(), 10) : null,
                headId: $("#adjHeadId").val() ? parseInt($("#adjHeadId").val(), 10) : null,
                type: $("#adjType").val(),
                amount: $("#adjAmount").val() ? parseFloat($("#adjAmount").val()) : 0,
                narration: $("#adjNarration").val().trim() || null,
                entryDate: new Date().toISOString()
            };
            if (!dto.studentId || !dto.academicYear || !dto.type || !dto.amount) {
                showAlert("#adjMsg", "warning", "Student, AY, Type and Amount are required.");
                return;
            }
            const $btn = $(this).prop("disabled", true).text("Inserting...");
            $.ajax({
                url: "/Fee/InsertStudentFeeAdjustment",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(dto)
            })
                .done(res => {
                    if (res?.success) {
                        showAlert("#adjMsg", "success", "Inserted. ID = " + res.id);
                        $("#btnLoadAdjustments").trigger("click");
                    } else {
                        showAlert("#adjMsg", "warning", "Insert failed.");
                    }
                })
                .fail(xhr => showAlert("#adjMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Insert"));
        });

        $("#btnLoadAdjustments").on("click", function () {
            clearAlert("#adjMsg");
            const q = {
                studentId: $("#ajStudentId").val() ? parseInt($("#ajStudentId").val(), 10) : null,
                academicYear: $("#ajYear").val().trim() || null,
                termId: $("#ajTerm").val() ? parseInt($("#ajTerm").val(), 10) : null,
                type: $("#ajType").val().trim() || null
            };
            $("#adjTable tbody").html(`<tr><td colspan="9" class="p-3 text-center text-muted">Loading...</td></tr>`);
            $.ajax({
                url: "/Fee/GetStudentFeeAdjustments",
                type: "GET",
                dataType: "json",
                data: q
            })
                .done(renderAdjustments)
                .fail(xhr => showAlert("#adjMsg", "danger", parseError(xhr)));
        });

        // =========================================================================
        // ONLINE PAYMENT
        // =========================================================================
        function opRecalcTotal() {
            let total = 0;
            $("#opItemsTable tbody tr").each(function () {
                const amt = $(this).find(".op-amount").val();
                if (amt) total += parseFloat(amt) || 0;
            });
            $("#opTotal").text(fmt(total));
        }
        function opRenderEmpty() {
            $("#opItemsTable tbody").html(`<tr><td colspan="4" class="p-3 text-center text-muted">No rows</td></tr>`);
            $("#opTotal").text("0.00");
        }
        function opAddRow(headId, amount) {
            const headOpts = (feeHeadsCache || []).map(h => `<option value="${h.headId}" ${h.headId === headId ? 'selected' : ''}>${h.headCode} - ${h.headName}</option>`).join("");
            const row = `
        <tr>
          <td></td>
          <td>
            <select class="form-select form-select-sm op-head">
              <option value="">Select</option>
              ${headOpts}
            </select>
          </td>
          <td><input type="number" step="0.01" class="form-control form-control-sm op-amount" value="${amount ?? ""}" /></td>
          <td><button class="btn btn-sm btn-outline-danger btn-op-del-row">Remove</button></td>
        </tr>
      `;
            const $tb = $("#opItemsTable tbody");
            if ($tb.find("tr td").length === 1) $tb.empty();
            $tb.append(row);
            opRecalcTotal();
        }

        $("#btnOpAddRow").on("click", () => opAddRow(null, null));
        $(document).on("click", ".btn-op-del-row", function () { $(this).closest("tr").remove(); const $tb = $("#opItemsTable tbody"); if ($tb.find("tr").length === 0) opRenderEmpty(); opRecalcTotal(); });
        $(document).on("input", ".op-amount", opRecalcTotal);

        $("#btnInitiateOnline").on("click", function () {
            clearAlert("#opMsg");
            const dto = {
                studentId: $("#opStudentId").val() ? parseInt($("#opStudentId").val(), 10) : 0,
                academicYear: $("#opYear").val().trim(),
                termId: $("#opTerm").val() ? parseInt($("#opTerm").val(), 10) : 0,
                currency: "INR",
                returnUrl: $("#opReturnUrl").val().trim() || null,
                callbackUrl: $("#opCallbackUrl").val().trim() || null,
                items: []
            };
            $("#opItemsTable tbody tr").each(function () {
                const headId = parseInt($(this).find(".op-head").val(), 10);
                const amount = $(this).find(".op-amount").val() ? parseFloat($(this).find(".op-amount").val()) : 0;
                if (headId && amount > 0) dto.items.push({ headId, amount });
            });
            if (!dto.studentId || !dto.academicYear || !dto.termId || dto.items.length === 0) {
                showAlert("#opMsg", "warning", "Student, AY, Term and at least one item are required.");
                return;
            }
            const $btn = $(this).prop("disabled", true).text("Initiating...");
            $.ajax({
                url: "/Fee/InitiateOnlinePayment",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=UTF-8",
                data: JSON.stringify(dto)
            })
                .done(res => {
                    if (res?.success) {
                        showAlert("#opMsg", "success", "Order created: " + res.OrderNo);
                        $("#opPaymentLink").removeClass("d-none").attr("href", res.PaymentUrl).text("Open Payment (" + res.GatewayName + ")");
                        $("#opOrderNo").val(res.OrderNo);
                    } else {
                        showAlert("#opMsg", "warning", "Initiation failed.");
                    }
                })
                .fail(xhr => showAlert("#opMsg", "danger", parseError(xhr)))
                .always(() => $btn.prop("disabled", false).text("Initiate"));
        });

        $("#btnCheckOrder").on("click", function () {
            clearAlert("#opMsg");
            const orderNo = $("#opOrderNo").val().trim();
            if (!orderNo) { showAlert("#opMsg", "warning", "Enter an Order No."); return; }
            $("#opOrderStatusBox").removeClass("d-none"); $("#opOrderStatus").text("Checking...");
            $.ajax({
                url: "/Fee/GetPaymentOrderStatus",
                type: "GET",
                dataType: "json",
                data: { orderNo }
            })
                .done(res => {
                    $("#opOrderStatus").text(res.status || res.Status || "-");
                })
                .fail(xhr => {
                    $("#opOrderStatus").text("Error");
                    showAlert("#opMsg", "danger", parseError(xhr));
                });
        });

        // ---------------- Initial Loads (Part 2) ----------------
        $("#clDate").val(todayStr());
        opRenderEmpty();
    });
})();