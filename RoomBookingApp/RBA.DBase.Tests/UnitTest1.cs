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