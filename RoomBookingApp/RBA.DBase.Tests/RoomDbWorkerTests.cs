using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RBA.DBase.DBRelations;
using RBA.DBase.Managers.DbWorker;
using RBA.DBase.Workers.RoomWorkers;
using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.RoomBooking.BookRoom;
using RBA.Models.States;

namespace RBA.DBase.Tests
{
    public class Tests
    {
        private DbContextOptions<AppDbContext> _contextOptions;
        private Mock<ILogger<RoomDbWorker>> loggerMock;

        [SetUp]
        public async Task Setup()
        {
            _contextOptions = new DbContextOptionsBuilder<AppDbContext>().
                UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using var context = new AppDbContext(_contextOptions);
            var requestedRoom = new RoomModel
            {
                Id = 1,
                Name = "Test Room",
                Capacity = 10,
                Image = string.Empty,
                Amenities = new AmenityModel()
                {
                    Id = 1,
                    Name = string.Empty
                },
                Floor = 1,
                ReservationsId = new List<int>(),
            };
            var userModel = new UserModel
            {
                Id = 1,
                UserName = "Test User",
                Email = "testuser@example.com",
                Password = "password"
            };
            context.Users.Add(userModel);
            context.Rooms.Add(requestedRoom);
            await context.SaveChangesAsync();

            loggerMock = new Mock<ILogger<RoomDbWorker>>();
        }

        [Test, TestCaseSource(nameof(ValidBookingRequests))]
        public async Task Check_success_booking_room(BookRoomRequest request)
        {
            using var context = new AppDbContext(_contextOptions);
            var result = await new RoomDbWorker(context, loggerMock.Object).BookingRoom(request);
            Assert.That(result, Is.EqualTo(BookState.Confirmed));
        }

        [Test, TestCaseSource(nameof(ValidBookingRequests))]
        public async Task Check_double_booking_returns_failed(BookRoomRequest request)
        {
            using var context = new AppDbContext(_contextOptions);
            await new RoomDbWorker(context, loggerMock.Object).BookingRoom(request);
            using var context2 = new AppDbContext(_contextOptions);
            var result = await new RoomDbWorker(context2, loggerMock.Object).BookingRoom(request);
            Assert.That(result, Is.EqualTo(BookState.Failed));
        }

        [Test, TestCaseSource(nameof(InvalidBookingRequests))]
        public async Task Check_invalid_booking_returns_failed(BookRoomRequest request)
        {
            using var context = new AppDbContext(_contextOptions);
            var result = await new RoomDbWorker(context, loggerMock.Object).BookingRoom(request);
            Assert.That(result, Is.EqualTo(BookState.Failed));
        }

        #region [Test request fixtures]
        public static IEnumerable<BookRoomRequest> ValidBookingRequests()
        {
            yield return new BookRoomRequest { RoomId = 1, UserId = 1, BookingDate = DateTime.Parse("2024-01-01T09:00:00"), MeetingTitle = "Meeting" };
        }

        public static IEnumerable<BookRoomRequest> InvalidBookingRequests()
        {
            yield return new BookRoomRequest { RoomId = 999, UserId = 1, BookingDate = DateTime.Parse("2024-01-01T09:00:00"), MeetingTitle = "Meeting" };
            yield return new BookRoomRequest { RoomId = 1, UserId = 999, BookingDate = DateTime.Parse("2024-01-01T09:00:00"), MeetingTitle = "Meeting" };
        }
        #endregion
    }
}