using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using SMS.Application.Interfaces;
using SMS.Core.Entities;
using SMS.Core.Interfaces;

namespace SMS.Infrastructure.Repositories
{
    public class CommunicationRepository : ICommunicationRepository
    {
        private readonly IRepository _db;
        public CommunicationRepository(IRepository db) { _db = db; }

        public async Task<int> CreateOrGetDirectConversationAsync(CancellationToken token, int userA, int userB)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@UserA", ParameterValue = userA, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@UserB", ParameterValue = userB, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "CreateOrGetDirectConversation", p);
        }

        public Task<IEnumerable<int>> GetUserConversationIdsAsync(CancellationToken token, int userId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<int>(token, "GetUserConversationIds", p);
        }

        public Task<IEnumerable<Conversation>> GetConversationsForUserAsync(CancellationToken token, int userId)
        {
            var p = new List<ParametersCollection> { new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input } };
            return _db.ExecuteSpListAsync<Conversation>(token, "GetConversationsForUser", p);
        }

        public async Task<int> InsertMessageAsync(CancellationToken token, Message msg)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ConversationId", ParameterValue = msg.ConversationId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@SenderUserId", ParameterValue = msg.SenderUserId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ContentType", ParameterValue = msg.ContentType, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@Body", ParameterValue = msg.Body, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "InsertMessage", p);
        }

        public Task<IEnumerable<Message>> GetMessagesAsync(CancellationToken token, int conversationId, int userId, int pageSize, int? beforeMessageId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ConversationId", ParameterValue = conversationId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@PageSize", ParameterValue = pageSize, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@BeforeMessageId", ParameterValue = beforeMessageId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<Message>(token, "GetMessages", p);
        }

        public async Task<int> InsertOrUpdateReceiptAsync(CancellationToken token, int messageId, int userId, string receiptType)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@MessageId", ParameterValue = messageId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@ReceiptType", ParameterValue = receiptType, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "InsertOrUpdateReceipt", p);
        }

        public async Task<int> UpdateParticipantLastReadAsync(CancellationToken token, int conversationId, int userId, int lastReadMessageId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ConversationId", ParameterValue = conversationId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@LastReadMessageId", ParameterValue = lastReadMessageId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdateParticipantLastRead", p);
        }

        public async Task<int> SetUserPresenceAsync(CancellationToken token, int userId, bool isOnline, DateTime atUtc)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@IsOnline", ParameterValue = isOnline, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@AtUtc", ParameterValue = atUtc, ParameterType = DbType.DateTime2, ParameterDirection = ParameterDirection.Input }
            };
            return (int)await _db.ExecuteSpReturnValueAsync(token, "SetUserPresence", p);
        }

        public Task<UserPresence?> GetUserPresenceAsync(CancellationToken token, int userId)
        {
            var p = new List<ParametersCollection> { new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input } };
            return _db.ExecuteSpSingleAsync<UserPresence>(token, "GetUserPresence", p);
        }

        public Task<bool> IsParticipantAsync(CancellationToken token, int conversationId, int userId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@ConversationId", ParameterValue = conversationId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            // Proc returns 1 or 0
            return _db.ExecuteSpReturnValueAsync(token, "IsParticipant", p).ContinueWith(t => t.Result > 0);
        }
    }
}