using NUnit.Framework;
using App.Services;
using App.Models;
using App.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.UnitTests.Services
{
    [TestFixture]
    public class GoalServiceTests
    {
        private AppDbContext _context;
        private IGoalService _goalService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestGoalServiceDB")
                .Options;

            _context = new AppDbContext(options);
            _jobService = new JobService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
