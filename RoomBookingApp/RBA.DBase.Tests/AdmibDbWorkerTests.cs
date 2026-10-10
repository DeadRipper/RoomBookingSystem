using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RBA.DBase.DBRelations;
using RBA.DBase.Workers.AdminWorkers;
using RBA.DBase.Workers.RoomWorkers;
using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.Admin.NewRoom;
using RBA.Models.Request.RoomBooking.BookRoom;
using RBA.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Tests
{
    internal class AdmibDbWorkerTests
    {
        private DbContextOptions<AppDbContext> _contextOptions;
        private Mock<ILogger<AdminDbWorker>> loggerMock;

        [SetUp]
        public async Task Setup()
        {
            _contextOptions = new DbContextOptionsBuilder<AppDbContext>().
               UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using var context = new AppDbContext(_contextOptions);
            
            var amenty1 = new AmenityModel { Id = 1, Name = "Projector" };
            context.Amenities.Add(amenty1);

            await context.SaveChangesAsync();

            loggerMock = new Mock<ILogger<AdminDbWorker>>();
        }

        [Test, TestCaseSource(nameof(ValidInsertNewRoomRequests))]
        public async Task Check_new_insert_correct_create_new_room(NewRoomRequest request)
        {
            using var context = new AppDbContext(_contextOptions);
            var result = await new AdminDbWorker(context, loggerMock.Object).InsertNewRoom(request);
            Assert.That(result, Is.Not.Null);
        }

        [Test, TestCaseSource(nameof(InvalidInsertNewRoomRequests))]
        public async Task Check_new_insert_incorrect_amenty_not_create_new_room(NewRoomRequest request)
        {
            using var context = new AppDbContext(_contextOptions);
            Assert.Catch(() => new AdminDbWorker(context, loggerMock.Object).InsertNewRoom(request).GetAwaiter().GetResult(), It.IsAny<string>());
        }

        public static IEnumerable<NewRoomRequest> ValidInsertNewRoomRequests()
        {
            yield return new NewRoomRequest { Name = "Test Room", Floor = 1, Capacity = 10, Amenities = new AmenityModel { Id = 1 }, Image = "test.jpg" };
        }

        public static IEnumerable<NewRoomRequest> InvalidInsertNewRoomRequests()
        {
            yield return new NewRoomRequest { Name = "Test Room", Floor = 1, Capacity = 10, Amenities = new AmenityModel { Id = 2 }, Image = "test.jpg" };
        }
    }
}