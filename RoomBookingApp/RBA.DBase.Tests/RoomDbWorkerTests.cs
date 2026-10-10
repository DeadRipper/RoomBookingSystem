using Microsoft.EntityFrameworkCore;
using Moq;
using RBA.DBase.DBRelations;
using RBA.DBase.Managers.DbWorker;
using RBA.Models.Models.RoomBookingModels;

namespace RBA.DBase.Tests
{
    public class Tests
    {
        private DbContextOptions<AppDbContext> _contextOptions;

        [SetUp]
        public void Setup()
        {
            _contextOptions = new DbContextOptionsBuilder<AppDbContext>().
                UseInMemoryDatabase("TestDatabase").Options;
        }

        [Test]
        public async Task Check_success_booking_room()
        {
            using (var context = new AppDbContext(_contextOptions))
            {
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
                context.Rooms.Add(requestedRoom);
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(_contextOptions))
            {
                var room = await context.Rooms.FindAsync(1);
                Assert.That(room != null);
                Assert.That(room.Name == "Test Room");
            }
        }
    }
}