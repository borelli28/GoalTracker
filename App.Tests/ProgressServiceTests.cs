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
        
        [Test]
        public async Task CreateProgressAsync_ShouldCreateAndReturnProgress()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "New Goal" };
            await _goalService.CreateGoalAsync(goal);
            
            var progress = new Progress { Id = "1" };
            
            // Act
            var result = await _progressService.CreateProgressAsync(progress);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(progress.Id, result.Id);
            Assert.AreEqual(progress.GoalId, result.GoalId);
        }
        
        [Test]
        public async Task GetProgressByIdAsync_ShouldReturnProgress()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "New Goal" };
            await _goalService.CreateGoalAsync(goal);
            
            var progress = new Progress { GoalId = goal.Id };
            await _progressService.CreateProgressAsync(progress);
            
            // Act
            var result = await _progressService.GetProgressByIdAsync(progress.Id);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(progress.Id, result.Id);
        }
        
        [Test]
        public async Task UpdateProgressAsync_ShouldUpdateProgress()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "New Goal" };
            await _goalService.CreateGoalAsync(goal);
            
            var progress = new Progress { GoalId = goal.Id };
            await _progressService.CreateProgressAsync(progress);
            
            // Act
            progress.Completed = true;
            await _progressService.UpdateProgressAsync(progress);
            var result = await _progressService.GetProgressByIdAsync(progress.Id);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.Completed);
        }
        
        [Test]
        public async Task DeleteProgressAsync_ShouldRemoveProgress()
        {
            // Arrange
            var goal = new Goal { /* Initialize with required properties */ };
            await _goalService.CreateGoalAsync(goal);
    
            var progress = new Progress { GoalId = goal.Id };
            await _progressService.CreateProgressAsync(progress);
            
            // Act
            await _progressService.DeleteProgressAsync(progress.Id);
            
            // Assert
            var deletedProgress = await _progressService.GetProgressByIdAsync(progress.Id);
            Assert.IsNull(deletedProgress);
        }
        
        [Test]
        public async Task GetAllProgressAsync_ShouldReturnAllProgress()
        {
            // Arrange
            var goal = new Goal { /* Initialize with required properties */ };
            await _goalService.CreateGoalAsync(goal);
    
            var progress1 = new Progress { GoalId = goal.Id };
            var progress2 = new Progress { GoalId = goal.Id };
            await _progressService.CreateProgressAsync(progress1);
            await _progressService.CreateProgressAsync(progress2);
            
            // Act
            var allProgressInstances = await _progressService.GetAllProgressAsync();
            
            // Assert
            Assert.AreEqual(2, result.Count());
        }
    }
}