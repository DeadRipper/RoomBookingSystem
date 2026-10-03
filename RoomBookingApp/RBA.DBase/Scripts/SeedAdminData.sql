/*
    SeedAdminData.sql
    -----------------
    Populates dbo.AdminInfo and dbo.Admins (SQL Server / T-SQL), matching the
    EF Core migrations 20261002155335_admin and 20261003094325_logout:

        AdminInfo (Id identity PK, Username, Password, Email, Roles   nvarchar(max) NOT NULL,
                   LoginDate, LogoutDate                              datetime2     NOT NULL,
                   CurrentlyIn                                        int           NOT NULL)
        Admins    (Id identity PK, AdminInfoId FK -> AdminInfo.Id, ON DELETE CASCADE)

    Notes:
        - Roles is an EF "primitive collection" (List<string>) stored as a JSON array,
          e.g. ["SuperAdmin","Editor"].
        - POST /api/Admin/login authenticates with AdminInfo.Username + AdminInfo.Password
          (compared as plain text) and, on success, sets CurrentlyIn = 1 and LoginDate = now.
          POST /api/Admin/logout sets CurrentlyIn = 0 and LogoutDate = now.
        - LoginDate / LogoutDate are NOT NULL, so every seeded admin needs a value.
          Seeded as "logged out": CurrentlyIn = 0 and LogoutDate later than LoginDate
          (a previous session), staggered over the last few days.
        - Passwords below are plain-text test values only. Do not use in production.
        - Re-runnable: clears both tables (children first) and reseeds identities.
*/

USE RoomBooking;
GO

SET NOCOUNT ON;

-------------------------------------------------------------------------------
-- 1. Clean existing data (Admins first, because of the FK)
-------------------------------------------------------------------------------
DELETE FROM dbo.Admins;
DBCC CHECKIDENT ('dbo.Admins', RESEED, 0);

DELETE FROM dbo.AdminInfo;
DBCC CHECKIDENT ('dbo.AdminInfo', RESEED, 0);

-------------------------------------------------------------------------------
-- 2. AdminInfo (Id is generated: 1..5 in the order below)
--    Previous session = LoginDate .. LogoutDate, ending N days/hours ago.
-------------------------------------------------------------------------------
INSERT INTO dbo.AdminInfo (Username, Password, Email, Roles, LoginDate, LogoutDate, CurrentlyIn)
VALUES
    (N'1',      N'1',   N'admin@roombooking.local',      N'["SuperAdmin"]',
        DATEADD(HOUR, -26, SYSDATETIME()), DATEADD(HOUR, -24, SYSDATETIME()), 0),
    (N'dmakarenko', N'Passw0rd!1', N'dmakarenko@roombooking.local', N'["Admin","RoomManager"]',
        DATEADD(HOUR, -52, SYSDATETIME()), DATEADD(HOUR, -50, SYSDATETIME()), 0),
    (N'olena',      N'Passw0rd!2', N'olena@roombooking.local',      N'["Admin"]',
        DATEADD(HOUR, -76, SYSDATETIME()), DATEADD(HOUR, -75, SYSDATETIME()), 0),
    (N'manager',    N'Passw0rd!3', N'manager@roombooking.local',    N'["RoomManager"]',
        DATEADD(HOUR, -100, SYSDATETIME()), DATEADD(HOUR, -98, SYSDATETIME()), 0),
    (N'auditor',    N'Passw0rd!4', N'auditor@roombooking.local',    N'["ReadOnly"]',
        DATEADD(HOUR, -124, SYSDATETIME()), DATEADD(HOUR, -123, SYSDATETIME()), 0);

-------------------------------------------------------------------------------
-- 3. Admins: one row per AdminInfo, linked through AdminInfoId
-------------------------------------------------------------------------------
INSERT INTO dbo.Admins (AdminInfoId)
SELECT Id
FROM dbo.AdminInfo
ORDER BY Id;

-------------------------------------------------------------------------------
-- 4. Verify
-------------------------------------------------------------------------------
SELECT a.Id AS AdminId, i.Username, i.Password, i.Email, i.Roles,
       i.LoginDate, i.LogoutDate, i.CurrentlyIn
FROM dbo.Admins a
JOIN dbo.AdminInfo i ON i.Id = a.AdminInfoId
ORDER BY a.Id;
GO
