/*
    SeedAdmins.sql
    --------------
    Populates dbo.AdminInfo and dbo.Admins (SQL Server / T-SQL), matching the
    EF Core migration 20261002155335_admin:

        AdminInfo (Id identity PK, Username, Password, Email, Roles)   all nvarchar(max) NOT NULL
        Admins    (Id identity PK, AdminInfoId FK -> AdminInfo.Id, ON DELETE CASCADE)

    Notes:
        - Roles is an EF "primitive collection" (List<string>) stored as a JSON array,
          e.g. ["SuperAdmin","Editor"].
        - POST /api/Admin/login authenticates with Admins.Id (the "Id" field of the
          request) and AdminInfo.Password, compared as plain text. So in the app,
          log in with the Admins.Id printed at the end of this script.
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
-------------------------------------------------------------------------------
INSERT INTO dbo.AdminInfo (Username, Password, Email, Roles)
VALUES
    (N'admin',      N'admin123',   N'admin@roombooking.local',      N'["SuperAdmin"]'),
    (N'dmakarenko', N'Passw0rd!1', N'dmakarenko@roombooking.local', N'["Admin","RoomManager"]'),
    (N'olena',      N'Passw0rd!2', N'olena@roombooking.local',      N'["Admin"]'),
    (N'manager',    N'Passw0rd!3', N'manager@roombooking.local',    N'["RoomManager"]'),
    (N'auditor',    N'Passw0rd!4', N'auditor@roombooking.local',    N'["ReadOnly"]');

-------------------------------------------------------------------------------
-- 3. Admins: one row per AdminInfo, linked through AdminInfoId
-------------------------------------------------------------------------------
INSERT INTO dbo.Admins (AdminInfoId)
SELECT Id
FROM dbo.AdminInfo
ORDER BY Id;

-------------------------------------------------------------------------------
-- 4. Verify: Admins.Id is the login id used by the API
-------------------------------------------------------------------------------
SELECT a.Id AS LoginId, i.Username, i.Password, i.Email, i.Roles
FROM dbo.Admins a
JOIN dbo.AdminInfo i ON i.Id = a.AdminInfoId
ORDER BY a.Id;
GO