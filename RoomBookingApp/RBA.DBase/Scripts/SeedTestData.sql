/*
    SeedTestData.sql
    -----------------
    Generates test data for the RoomBooking database (SQL Server / T-SQL).

    Tables populated, matching RBA.Models.Models + EF Core migrations
    (see RBA.DBase/Migrations/AppDbContextModelSnapshot.cs):

        AmenityModel   (Id, Name)                              ~20 rows
        Users          (Id, UserName)                          ~250 rows
        Rooms          (Id, Name, Floor, Capacity, AmenitiesId, Image, RoomState)  ~100 rows
        Reservations   (Id, Date, UsersId)                     ~630 rows
        ------------------------------------------------------------
        Total                                                  ~1000 rows

    RoomState enum (RBA.Models.States.RoomState): 0 = Available, 1 = Occupied, 2 = Maintenance

    Notes:
        - Run against a database that already has the schema applied
          (dotnet ef database update / migrations already run).
        - Script is idempotent-ish: it clears existing rows from these four
          tables before inserting (respecting FK order), and reseeds identities.
        - Uses set-based generation (tally numbers + CROSS APPLY) instead of
          1000 literal INSERT statements, so row counts are easy to tune below.
*/

USE RoomBooking;
GO

SET NOCOUNT ON;

-------------------------------------------------------------------------------
-- 0. Config: tweak row counts here if needed
-------------------------------------------------------------------------------
DECLARE @AmenityCount   INT = 20;
DECLARE @UserCount      INT = 250;
DECLARE @RoomCount      INT = 100;
DECLARE @ReservationCount INT = 630;

-------------------------------------------------------------------------------
-- 1. Clean existing data (children first, respecting FKs)
-------------------------------------------------------------------------------
DELETE FROM dbo.Reservations;
DBCC CHECKIDENT ('dbo.Reservations', RESEED, 0);

DELETE FROM dbo.Rooms;
DBCC CHECKIDENT ('dbo.Rooms', RESEED, 0);

DELETE FROM dbo.Users;
DBCC CHECKIDENT ('dbo.Users', RESEED, 0);

DELETE FROM dbo.AmenityModel;
DBCC CHECKIDENT ('dbo.AmenityModel', RESEED, 0);

-------------------------------------------------------------------------------
-- 2. Tally table helper (numbers 1..N), used to drive all set-based generation
-------------------------------------------------------------------------------
IF OBJECT_ID('tempdb..#Tally') IS NOT NULL DROP TABLE #Tally;

;WITH
    E1(N) AS (SELECT 1 FROM (VALUES (1),(1),(1),(1),(1),(1),(1),(1),(1),(1)) AS X(N)),
    E2(N) AS (SELECT 1 FROM E1 a CROSS JOIN E1 b),
    E4(N) AS (SELECT 1 FROM E2 a CROSS JOIN E2 b),
    Tally(N) AS (SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) FROM E4 a CROSS JOIN E1 b)
SELECT TOP (2000) N
INTO #Tally
FROM Tally
ORDER BY N;

-------------------------------------------------------------------------------
-- 3. AmenityModel — fixed, realistic amenity catalog
-------------------------------------------------------------------------------
DECLARE @Amenities TABLE (Name NVARCHAR(200));
INSERT INTO @Amenities (Name) VALUES
    (N'Wi-Fi'), (N'Projector'), (N'Whiteboard'), (N'Video Conferencing'),
    (N'Air Conditioning'), (N'Coffee Machine'), (N'Sound System'), (N'TV Screen'),
    (N'Flip Chart'), (N'Conference Phone'), (N'Catering Service'), (N'Parking'),
    (N'Wheelchair Access'), (N'Natural Light'), (N'Standing Desk'), (N'Smart Board'),
    (N'Printer'), (N'Soundproofing'), (N'Kitchenette'), (N'Lounge Area');

INSERT INTO dbo.AmenityModel (Name)
SELECT TOP (@AmenityCount) Name
FROM @Amenities
ORDER BY (SELECT NULL);

-------------------------------------------------------------------------------
-- 4. Users — generated from first/last name pools, unique via row number
-------------------------------------------------------------------------------
DECLARE @FirstNames TABLE (Id INT IDENTITY(1,1), Name NVARCHAR(100));
INSERT INTO @FirstNames (Name) VALUES
    (N'James'), (N'Mary'), (N'John'), (N'Patricia'), (N'Robert'), (N'Jennifer'),
    (N'Michael'), (N'Linda'), (N'William'), (N'Elizabeth'), (N'David'), (N'Barbara'),
    (N'Richard'), (N'Susan'), (N'Joseph'), (N'Jessica'), (N'Thomas'), (N'Sarah'),
    (N'Charles'), (N'Karen'), (N'Daniel'), (N'Nancy'), (N'Matthew'), (N'Lisa'),
    (N'Anthony'), (N'Betty'), (N'Mark'), (N'Margaret'), (N'Paul'), (N'Sandra');

DECLARE @LastNames TABLE (Id INT IDENTITY(1,1), Name NVARCHAR(100));
INSERT INTO @LastNames (Name) VALUES
    (N'Smith'), (N'Johnson'), (N'Williams'), (N'Brown'), (N'Jones'), (N'Garcia'),
    (N'Miller'), (N'Davis'), (N'Rodriguez'), (N'Martinez'), (N'Hernandez'), (N'Lopez'),
    (N'Gonzalez'), (N'Wilson'), (N'Anderson'), (N'Thomas'), (N'Taylor'), (N'Moore'),
    (N'Jackson'), (N'Martin'), (N'Lee'), (N'Perez'), (N'Thompson'), (N'White'),
    (N'Harris'), (N'Sanchez'), (N'Clark'), (N'Ramirez'), (N'Lewis'), (N'Robinson');

DECLARE @FirstNameCount INT = (SELECT COUNT(*) FROM @FirstNames);
DECLARE @LastNameCount INT = (SELECT COUNT(*) FROM @LastNames);

INSERT INTO dbo.Users (UserName)
SELECT
    LOWER(fn.Name) + N'.' + LOWER(ln.Name) + CAST(t.N AS NVARCHAR(10))
FROM #Tally t
JOIN @FirstNames fn ON fn.Id = ((t.N - 1) % @FirstNameCount) + 1
JOIN @LastNames ln ON ln.Id = ((t.N * 7 - 1) % @LastNameCount) + 1
WHERE t.N <= @UserCount
ORDER BY t.N;

-------------------------------------------------------------------------------
-- 5. Rooms — random-ish but sensible Floor/Capacity/RoomState, valid AmenitiesId
-------------------------------------------------------------------------------
DECLARE @RoomAdjectives TABLE (Id INT IDENTITY(1,1), Word NVARCHAR(50));
INSERT INTO @RoomAdjectives (Word) VALUES
    (N'Summit'), (N'Horizon'), (N'Atlas'), (N'Nova'), (N'Vertex'), (N'Pioneer'),
    (N'Compass'), (N'Beacon'), (N'Zenith'), (N'Orbit'), (N'Meridian'), (N'Aurora'),
    (N'Harbor'), (N'Skyline'), (N'Junction'), (N'Cascade'), (N'Granite'), (N'Cobalt'),
    (N'Ember'), (N'Willow');

DECLARE @RoomAdjCount INT = (SELECT COUNT(*) FROM @RoomAdjectives);
DECLARE @MaxAmenityId INT = (SELECT MAX(Id) FROM dbo.AmenityModel);

INSERT INTO dbo.Rooms (Name, Floor, Capacity, AmenitiesId, Image, RoomState)
SELECT
    ra.Word + N' Room ' + CAST(t.N AS NVARCHAR(10))                       AS Name,
    ((t.N - 1) % 10) + 1                                                  AS Floor,
    (ABS(CHECKSUM(NEWID())) % 48) + 2                                     AS Capacity,       -- 2..49
    ((ABS(CHECKSUM(NEWID())) + t.N) % @MaxAmenityId) + 1                  AS AmenitiesId,
    N'https://picsum.photos/seed/room' + CAST(t.N AS NVARCHAR(10)) + N'/640/480' AS Image,
    ABS(CHECKSUM(NEWID())) % 3                                            AS RoomState       -- 0 Available / 1 Occupied / 2 Maintenance
FROM #Tally t
JOIN @RoomAdjectives ra ON ra.Id = ((t.N - 1) % @RoomAdjCount) + 1
WHERE t.N <= @RoomCount
ORDER BY t.N;

-------------------------------------------------------------------------------
-- 6. Reservations — valid UsersId, Date spread across +/- ~6 months from today
-------------------------------------------------------------------------------
DECLARE @MaxUserId INT = (SELECT MAX(Id) FROM dbo.Users);

INSERT INTO dbo.Reservations (Date, UsersId)
SELECT
    DATEADD(MINUTE, (ABS(CHECKSUM(NEWID())) % (60 * 24)),
        DATEADD(DAY, (ABS(CHECKSUM(NEWID())) % 365) - 180, CAST(GETUTCDATE() AS DATE))) AS Date,
    ((ABS(CHECKSUM(NEWID())) + t.N) % @MaxUserId) + 1                                    AS UsersId
FROM #Tally t
WHERE t.N <= @ReservationCount
ORDER BY t.N;

-------------------------------------------------------------------------------
-- 7. Cleanup + summary
-------------------------------------------------------------------------------
DROP TABLE #Tally;

SELECT 'AmenityModel' AS TableName, COUNT(*) AS RowsInserted FROM dbo.AmenityModel
UNION ALL SELECT 'Users', COUNT(*) FROM dbo.Users
UNION ALL SELECT 'Rooms', COUNT(*) FROM dbo.Rooms
UNION ALL SELECT 'Reservations', COUNT(*) FROM dbo.Reservations;
