using Microsoft.EntityFrameworkCore;
using Moq;
using RBA.DBase.DBRelations;
using RBA.DBase.Managers;

namespace RBA.DBase.Tests
{
    public class Tests
    {
        AppDbContext _appDbContext;
        Mock<IDBWorker> _mockDBWorker;

        [SetUp]
        public void Setup()
        {
            _appDbContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options);
            _appDbContext.Rooms.Add(new RBA.Models.Models.RoomModel { Id = 1, RoomState = RBA.Models.States.RoomState.Available });
            _appDbContext.SaveChanges();
            _mockDBWorker = new Mock<IDBWorker>();
                {
                                    
                }
            ;
        }

        [Test]
        public void Test1()
        {
            
        }

        [TearDown]
        public void TearDown()
        {
            _appDbContext.Dispose();
        }
    }
}