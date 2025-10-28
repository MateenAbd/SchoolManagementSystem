USE SMSDb;
SET XACT_ABORT ON;

BEGIN TRAN;

DECLARE @UserId INT;
DECLARE @Email NVARCHAR(256) = N'mateen@gmail.com';
DECLARE @UserName NVARCHAR(100) = N'mateen';
DECLARE @Phone NVARCHAR(30) = N'9999999999';
DECLARE @PwdHash NVARCHAR(500) = N'PBKDF2$100000$k3zR92C5AD/flvJeynOnmQ==$4ZAxn1qwovbVP51JpqEi4RO7HmHf6l9ND5o18x3Ogoc='; --Password: mateen

-- Create (or reuse) user
IF EXISTS (SELECT 1 FROM dbo.Users WHERE NormalizedEmail = UPPER(LTRIM(RTRIM(@Email))))
    SELECT @UserId = UserId FROM dbo.Users WHERE NormalizedEmail = UPPER(LTRIM(RTRIM(@Email)));
ELSE
    EXEC @UserId = dbo.CreateUser
        @UserName    = @UserName,
        @Email       = @Email,
        @PhoneNumber = @Phone,
        @PasswordHash= @PwdHash,
        @IsActive    = 1;

-- Optional: refresh password hash + security stamp
IF @UserId IS NOT NULL
    EXEC dbo.SetUserPasswordHash @UserId = @UserId, @PasswordHash = @PwdHash;

-- Assign Admin role (creates role if missing and maps it)
EXEC dbo.AssignRoleToUser @UserId = @UserId, @RoleName = N'Admin';

COMMIT TRAN;
