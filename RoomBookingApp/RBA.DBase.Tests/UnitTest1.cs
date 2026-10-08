using Microsoft.EntityFrameworkCore;
using Moq;
using RBA.DBase.DBRelations;
using RBA.DBase.Managers.DbWorker;

namespace RBA.DBase.Tests
{
    public class Tests
    {
        AppDbContext _appDbContext;
        Mock<IUserDBWorker> _mockDBWorker;

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