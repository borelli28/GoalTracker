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
    
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestProgressServiceDB")
                .Options;
                
            _context = new AppDbContext(options);
            _goalService = new GoalService(_context);
            _progressService = new ProgressService(_context, _goalService);
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
            
            var progress = new Progress { GoalId = goal.Id };
            
            // Act
            var result = await _progressService.CreateProgressAsync(progress);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(progress.Id));
            Assert.That(result.GoalId, Is.EqualTo(progress.GoalId));
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
            Assert.That(result.Id, Is.EqualTo(progress.Id));
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
            Assert.That(result.Completed, Is.EqualTo(true));
        }
        
        [Test]
        public async Task DeleteProgressAsync_ShouldRemoveProgress()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "New Goal" };
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
            var goal = new Goal { Id = "1", Name = "New Goal" };
            await _goalService.CreateGoalAsync(goal);
    
            var progress1 = new Progress { GoalId = goal.Id };
            var progress2 = new Progress { GoalId = goal.Id };
            await _progressService.CreateProgressAsync(progress1);
            await _progressService.CreateProgressAsync(progress2);
            
            // Act
            var allProgressInstances = await _progressService.GetAllProgressAsync();
            
            // Assert
            Assert.That(allProgressInstances.Count(), Is.EqualTo(2));
        }
        
        [Test]
        public async Task ProgressExistsAsync_ShouldReturnTrueForExistingProgress()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "New Goal" };
            await _goalService.CreateGoalAsync(goal);
    
            var progress = new Progress { GoalId = goal.Id };
            await _progressService.CreateProgressAsync(progress);
    
            // Act
            var result = await _progressService.ProgressExistsAsync(progress.Id);
    
            // Assert
            Assert.IsTrue(result);
        }
    
        [Test]
        public async Task ProgressExistsAsync_ShouldReturnFalseForNonExistingProgress()
        {
            // Act
            var result = await _progressService.ProgressExistsAsync("ThisIdDontExists");
    
            // Assert
            Assert.IsFalse(result);
        }
    }
}
