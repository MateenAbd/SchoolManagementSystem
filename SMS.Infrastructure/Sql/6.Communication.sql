CREATE TABLE dbo.Conversations
(
    ConversationId INT IDENTITY(1,1) PRIMARY KEY,
    ConversationType NVARCHAR(20) NOT NULL DEFAULT 'Direct',
    CreatedByUserId INT NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsActive BIT NOT NULL DEFAULT 1
);
CREATE INDEX IX_Conversations_Type ON dbo.Conversations(ConversationType);
GO


CREATE TABLE dbo.ConversationParticipants
(
    ConversationId INT NOT NULL,
    UserId INT NOT NULL,
    JoinedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    LastReadMessageId INT NULL,
    LastSeenUtc DATETIME2 NULL,
    IsMuted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ConversationParticipants PRIMARY KEY (ConversationId, UserId),
    CONSTRAINT FK_CP_Conversations FOREIGN KEY (ConversationId) REFERENCES dbo.Conversations(ConversationId) ON DELETE CASCADE,
    CONSTRAINT FK_CP_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId)
);
CREATE INDEX IX_CP_User ON dbo.ConversationParticipants(UserId);
GO

CREATE TABLE dbo.Messages
(
    MessageId INT IDENTITY(1,1) PRIMARY KEY,
    ConversationId INT NOT NULL,
    SenderUserId INT NOT NULL,
    ContentType NVARCHAR(20) NOT NULL DEFAULT 'text',
    Body NVARCHAR(MAX) NOT NULL,
    SentAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    EditedAtUtc DATETIME2 NULL,
    DeletedAtUtc DATETIME2 NULL,
    CONSTRAINT FK_Messages_Conversations FOREIGN KEY (ConversationId) REFERENCES dbo.Conversations(ConversationId) ON DELETE CASCADE,
    CONSTRAINT FK_Messages_Users FOREIGN KEY (SenderUserId) REFERENCES dbo.Users(UserId)
);
CREATE INDEX IX_Messages_Conv ON dbo.Messages(ConversationId, MessageId DESC);
GO

CREATE TABLE dbo.MessageReceipts
(
    MessageId INT NOT NULL,
    UserId INT NOT NULL,
    ReceiptType NVARCHAR(10) NOT NULL, -- Delivered/Read
    ReceiptAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_MessageReceipts PRIMARY KEY (MessageId, UserId, ReceiptType),
    CONSTRAINT FK_MR_Messages FOREIGN KEY (MessageId) REFERENCES dbo.Messages(MessageId) ON DELETE CASCADE,
    CONSTRAINT FK_MR_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId)
);
CREATE INDEX IX_MR_User ON dbo.MessageReceipts(UserId, MessageId);
GO

CREATE TABLE dbo.UserPresence
(
    UserId INT PRIMARY KEY,
    IsOnline BIT NOT NULL,
    LastSeenUtc DATETIME2 NULL,
    LastActiveUtc DATETIME2 NULL,
    CONSTRAINT FK_Presence_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId)
);
GO

CREATE OR ALTER PROCEDURE CreateOrGetDirectConversation
    @UserA INT,
    @UserB INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @UserA = @UserB THROW 70001, 'Cannot create a direct conversation with self', 1;

    DECLARE @MinUser INT = CASE WHEN @UserA < @UserB THEN @UserA ELSE @UserB END;
    DECLARE @MaxUser INT = CASE WHEN @UserA < @UserB THEN @UserB ELSE @UserA END;

    DECLARE @ConvId INT = (
        SELECT TOP 1 c.ConversationId
        FROM dbo.Conversations c
        INNER JOIN dbo.ConversationParticipants p1 ON p1.ConversationId = c.ConversationId AND p1.UserId = @MinUser
        INNER JOIN dbo.ConversationParticipants p2 ON p2.ConversationId = c.ConversationId AND p2.UserId = @MaxUser
        WHERE c.ConversationType = 'Direct' AND c.IsActive = 1
        ORDER BY c.ConversationId DESC
    );

    IF @ConvId IS NULL
    BEGIN
        INSERT INTO dbo.Conversations (ConversationType, CreatedByUserId)
        VALUES ('Direct', @UserA);
        SET @ConvId = SCOPE_IDENTITY();

        INSERT INTO dbo.ConversationParticipants (ConversationId, UserId) VALUES (@ConvId, @MinUser);
        INSERT INTO dbo.ConversationParticipants (ConversationId, UserId) VALUES (@ConvId, @MaxUser);
    END

    RETURN @ConvId;
END
GO

CREATE OR ALTER PROCEDURE InsertMessage
    @ConversationId INT,
    @SenderUserId INT,
    @ContentType NVARCHAR(20),
    @Body NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.ConversationParticipants WHERE ConversationId=@ConversationId AND UserId=@SenderUserId)
        THROW 70011, 'Sender not a participant', 1;

    INSERT INTO dbo.Messages (ConversationId, SenderUserId, ContentType, Body)
    VALUES (@ConversationId, @SenderUserId, @ContentType, @Body);

    DECLARE @MessageId INT = SCOPE_IDENTITY();

    INSERT INTO dbo.MessageReceipts (MessageId, UserId, ReceiptType)
    SELECT @MessageId, p.UserId, 'Delivered'
    FROM dbo.ConversationParticipants p
    WHERE p.ConversationId = @ConversationId AND p.UserId <> @SenderUserId;

    RETURN @MessageId;
END
GO

CREATE OR ALTER PROCEDURE InsertOrUpdateReceipt
    @MessageId INT,
    @UserId INT,
    @ReceiptType NVARCHAR(10) -- Delivered/Read
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.ConversationParticipants cp
                   INNER JOIN dbo.Messages m ON m.ConversationId = cp.ConversationId
                   WHERE cp.UserId = @UserId AND m.MessageId=@MessageId)
        THROW 70021, 'User not participant', 1;

    MERGE dbo.MessageReceipts AS tgt
    USING (SELECT @MessageId AS MessageId, @UserId AS UserId, @ReceiptType AS ReceiptType) AS src
    ON (tgt.MessageId = src.MessageId AND tgt.UserId = src.UserId AND tgt.ReceiptType = src.ReceiptType)
    WHEN MATCHED THEN UPDATE SET ReceiptAtUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN INSERT (MessageId, UserId, ReceiptType) VALUES (src.MessageId, src.UserId, src.ReceiptType);

    RETURN 1;
END
GO

CREATE OR ALTER PROCEDURE UpdateParticipantLastRead
    @ConversationId INT,
    @UserId INT,
    @LastReadMessageId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ConversationParticipants
    SET LastReadMessageId = @LastReadMessageId, LastSeenUtc = SYSUTCDATETIME()
    WHERE ConversationId=@ConversationId AND UserId=@UserId;

    RETURN @@ROWCOUNT;
END
GO

CREATE OR ALTER PROCEDURE SetUserPresence
    @UserId INT,
    @IsOnline BIT,
    @AtUtc DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    MERGE dbo.UserPresence AS tgt
    USING (SELECT @UserId AS UserId) AS src
    ON (tgt.UserId = src.UserId)
    WHEN MATCHED THEN
        UPDATE SET IsOnline = @IsOnline,
                   LastActiveUtc = @AtUtc,
                   LastSeenUtc = CASE WHEN @IsOnline = 0 THEN @AtUtc ELSE tgt.LastSeenUtc END
    WHEN NOT MATCHED THEN
        INSERT (UserId, IsOnline, LastSeenUtc, LastActiveUtc)
        VALUES (@UserId, @IsOnline, CASE WHEN @IsOnline=0 THEN @AtUtc ELSE NULL END, @AtUtc);

    RETURN 1;
END
GO

CREATE OR ALTER PROCEDURE GetUserPresence
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM dbo.UserPresence WHERE UserId=@UserId;
END
GO

CREATE OR ALTER PROCEDURE IsParticipant
    @ConversationId INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.ConversationParticipants WHERE ConversationId=@ConversationId AND UserId=@UserId)
        RETURN 1;
    RETURN 0;
END
GO

CREATE OR ALTER PROCEDURE GetUserConversationIds
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ConversationId FROM dbo.ConversationParticipants WHERE UserId=@UserId AND EXISTS (SELECT 1 FROM dbo.Conversations c WHERE c.ConversationId = ConversationId AND c.IsActive=1);
END
GO

CREATE OR ALTER PROCEDURE GetMessages
    @ConversationId INT,
    @UserId INT,
    @PageSize INT = 50,
    @BeforeMessageId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.ConversationParticipants WHERE ConversationId=@ConversationId AND UserId=@UserId)
        THROW 70031, 'Forbidden', 1;

    SELECT TOP(@PageSize) m.*
    FROM dbo.Messages m
    WHERE m.ConversationId=@ConversationId
      AND (@BeforeMessageId IS NULL OR m.MessageId < @BeforeMessageId)
    ORDER BY m.MessageId DESC;
END
GO

CREATE OR ALTER PROCEDURE GetConversationsForUser
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.*
    FROM dbo.Conversations c
    INNER JOIN dbo.ConversationParticipants p ON p.ConversationId = c.ConversationId AND p.UserId = @UserId
    WHERE c.IsActive = 1
    ORDER BY c.ConversationId DESC;
END
GO