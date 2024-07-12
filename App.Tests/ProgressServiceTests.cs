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

            // Act
            var result = await _progressService.CreateProgressAsync(goal.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.GoalId, Is.EqualTo(goal.Id));
            Assert.That(result.Completed, Is.False);
            Assert.That(result.Notes, Is.Empty);
        }
        
        [Test]
        public async Task CreateProgressAsync_WithExistingProgress_ShouldCreateAndReturnProgress()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "New Goal" };
            await _goalService.CreateGoalAsync(goal);
            
            var progress = new Progress { GoalId = goal.Id, Notes = "Custom note" };
            
            // Act
            var result = await _progressService.CreateProgressAsync(goal.Id, progress);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(progress.Id));
            Assert.That(result.GoalId, Is.EqualTo(goal.Id));
            Assert.That(result.Notes, Is.EqualTo("Custom note"));
        }
        
        [Test]
        public async Task GetProgressByIdAsync_ShouldReturnProgress()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "New Goal" };
            await _goalService.CreateGoalAsync(goal);
            
            var progress = new Progress { GoalId = goal.Id };
            await _progressService.CreateProgressAsync(goal.Id, progress);
            
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
            await _progressService.CreateProgressAsync(goal.Id, progress);
            
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
            await _progressService.CreateProgressAsync(goal.Id, progress);
            
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
            await _progressService.CreateProgressAsync(goal.Id, progress1);
            await _progressService.CreateProgressAsync(goal.Id, progress2);
            
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
            await _progressService.CreateProgressAsync(goal.Id, progress);
    
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
        
        [Test]
        public async Task GetLastProgressInstance_ShouldReturnMostRecentProgress()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "Test Goal" };
            await _goalService.CreateGoalAsync(goal);
        
            var progress1 = new Progress { GoalId = goal.Id, Date = DateTime.UtcNow.AddDays(-2) };
            var progress2 = new Progress { GoalId = goal.Id, Date = DateTime.UtcNow.AddDays(-1) };
            var progress3 = new Progress { GoalId = goal.Id, Date = DateTime.UtcNow };
        
            await _progressService.CreateProgressAsync(goal.Id, progress1);
            await _progressService.CreateProgressAsync(goal.Id, progress2);
            await _progressService.CreateProgressAsync(goal.Id, progress3);
        
            // Act
            var result = await _progressService.GetLastProgressInstance(goal.Id);
        
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(progress3.Id));
            Assert.That(result.Date, Is.EqualTo(progress3.Date));
        }
        
        [Test]
        public async Task GetProgressesForGoalAsync_ShouldReturnProgressesWithinDateRange()
        {
            // Arrange
            var goalId = "goal1";
            var startDate = DateTime.UtcNow.AddDays(-10);

            var progresses = new List<Progress>
            {
                new Progress { Id = "1", GoalId = goalId, Date = DateTime.UtcNow.AddDays(-15), Completed = false },
                new Progress { Id = "2", GoalId = goalId, Date = DateTime.UtcNow.AddDays(-5), Completed = true },
                new Progress { Id = "3", GoalId = goalId, Date = DateTime.UtcNow.AddDays(-1), Completed = false },
                new Progress { Id = "4", GoalId = "goal2", Date = DateTime.UtcNow.AddDays(-5), Completed = true }
            };

            await _context.Progresses.AddRangeAsync(progresses);
            await _context.SaveChangesAsync();

            // Act
            var result = await _progressService.GetProgressesForGoalAsync(goalId, startDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(p => p.GoalId == goalId));
            Assert.IsTrue(result.All(p => p.Date >= startDate));
        }

        [Test]
        public async Task GetProgressesForGoalAsync_ShouldReturnEmptyListWhenNoProgresses()
        {
            // Arrange
            var goalId = "goal1";
            var startDate = DateTime.UtcNow.AddDays(-10);

            // Act
            var result = await _progressService.GetProgressesForGoalAsync(goalId, startDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task GetProgressesForGoalAsync_ShouldReturnEmptyListWhenNoProgressesInDateRange()
        {
            // Arrange
            var goalId = "goal1";
            var startDate = DateTime.UtcNow.AddDays(-5);

            var progresses = new List<Progress>
            {
                new Progress { Id = "1", GoalId = goalId, Date = DateTime.UtcNow.AddDays(-15), Completed = false },
                new Progress { Id = "2", GoalId = goalId, Date = DateTime.UtcNow.AddDays(-10), Completed = true },
            };

            await _context.Progresses.AddRangeAsync(progresses);
            await _context.SaveChangesAsync();

            // Act
            var result = await _progressService.GetProgressesForGoalAsync(goalId, startDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
