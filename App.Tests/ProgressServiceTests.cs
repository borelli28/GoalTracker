using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using App.Services;
using System.Linq;
using App.Models;
using App.Data;

namespace App.UnitTests.Services
{
    [TestFixture]
    public class ProgressServiceTests
    {
        private AppDbContext _context;
        private IGoalService _goalService;
        private IProgressService _progressService;
    
        [Setup]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestProgressServiceDB")
                .Options;
                
            _context = new AppDbContext(options);
            _goalService = new IGoalService(_context);
            _progressService = new IProgressService(_context);
        }
        
        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
