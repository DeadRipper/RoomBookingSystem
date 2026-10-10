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

        [Test, TestCaseSource(nameof(GetTestBookingRequests))]
        public async Task Check_success_booking_room(BookRoomRequest request)
        {
            using var context = new AppDbContext(_contextOptions);
            var result = await new RoomDbWorker(context, loggerMock.Object).BookingRoom(request);
            Assert.That(result, Is.EqualTo(BookState.Confirmed));
        }

        public static IEnumerable<BookRoomRequest> GetTestBookingRequests()
        {
            return new List<BookRoomRequest>
            {
                new BookRoomRequest
                {
                    RoomId = 1,
                    BookingDate = DateTime.Parse("2024-01-01T09:00:00"),
                    MeetingTitle = "Test Meeting",
                    UserId = 1,
                    UserName = "Test User"
                }
            };
        }
    }
}