using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SMS.Core.Entities;

namespace SMS.Application.Interfaces
{
    public interface ICommunicationRepository
    {
        // Conversations
        Task<int> CreateOrGetDirectConversationAsync(CancellationToken token, int userA, int userB);
        Task<IEnumerable<int>> GetUserConversationIdsAsync(CancellationToken token, int userId);
        Task<IEnumerable<Conversation>> GetConversationsForUserAsync(CancellationToken token, int userId);

        // Messages
        Task<int> InsertMessageAsync(CancellationToken token, Message msg);
        Task<IEnumerable<Message>> GetMessagesAsync(CancellationToken token, int conversationId, int userId, int pageSize, int? beforeMessageId);

        // Receipts & last read
        Task<int> InsertOrUpdateReceiptAsync(CancellationToken token, int messageId, int userId, string receiptType);
        Task<int> UpdateParticipantLastReadAsync(CancellationToken token, int conversationId, int userId, int lastReadMessageId);

        // Presence
        Task<int> SetUserPresenceAsync(CancellationToken token, int userId, bool isOnline, DateTime atUtc);
        Task<UserPresence?> GetUserPresenceAsync(CancellationToken token, int userId);

        // Security helpers
        Task<bool> IsParticipantAsync(CancellationToken token, int conversationId, int userId);
    }
}