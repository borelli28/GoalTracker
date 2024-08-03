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
        private IProgressService _progressService;
    
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestProgressServiceDB")
                .Options;
                
            _context = new AppDbContext(options);
            _progressService = new ProgressService(_context);
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
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();

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
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
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
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
            var progress = new Progress { GoalId = goal.Id };
            await _progressService.CreateProgressAsync(goal.Id, progress);

            // Act
            var result = await _progressService.GetProgressByIdAsync(progress.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result?.Id, Is.EqualTo(progress.Id));
        }

        [Test]
        public async Task UpdateProgressAsync_ShouldUpdateProgress()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "New Goal" };
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
            var progress = new Progress { GoalId = goal.Id };
            await _progressService.CreateProgressAsync(goal.Id, progress);

            // Act
            progress.Completed = true;
            await _progressService.UpdateProgressAsync(progress);
            var result = await _progressService.GetProgressByIdAsync(progress.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result?.Completed, Is.EqualTo(true));
        }

        [Test]
        public async Task DeleteProgressAsync_ShouldRemoveProgress()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "New Goal" };
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
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
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
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
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
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
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
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
            Assert.That(result?.Id, Is.EqualTo(progress3.Id));
            Assert.That(result?.Date, Is.EqualTo(progress3.Date));
        }

        [Test]
        public async Task GetProgressesForGoalAsync_ReturnsCorrectProgresses()
        {
            // Arrange
            var goalId = "goal1";
            var startDate = DateTime.UtcNow.AddDays(-5);
            var endDate = DateTime.UtcNow;
    
            var progresses = new List<Progress>
            {
                new Progress { GoalId = goalId, Date = startDate.AddDays(1), Completed = true },
                new Progress { GoalId = goalId, Date = startDate.AddDays(3), Completed = true }
            };
    
            await _context.Progresses.AddRangeAsync(progresses);
            await _context.SaveChangesAsync();
    
            // Act
            var result = await _progressService.GetProgressesForGoalAsync(goalId, startDate);
    
            // Assert
            Assert.That(result.Count, Is.EqualTo(6));
            Assert.IsTrue(result.Any(p => p.Date == startDate.AddDays(1) && p.Completed));
            Assert.IsTrue(result.Any(p => p.Date == startDate.AddDays(3) && p.Completed));
            Assert.IsTrue(result.All(p => p.GoalId == goalId));
        }

        [Test]
        public async Task CreateProgressInstancesForDateRange_ShouldCreateInstancesForAllGoalsInRange()
        {
            // Arrange
            var goal1 = new Goal { Id = "1", Name = "Goal 1" };
            var goal2 = new Goal { Id = "2", Name = "Goal 2" };
            _context.Goals.AddRange(goal1, goal2);
            await _context.SaveChangesAsync();
            var startDate = DateTime.UtcNow.Date;
            var endDate = startDate.AddDays(5);

            // Act
            await _progressService.CreateProgressInstancesForDateRange(startDate, endDate);

            // Assert
            var allProgresses = await _context.Progresses.ToListAsync();
            Assert.That(allProgresses.Count, Is.EqualTo(12)); // 2 goals * 6 days (inclusive)
            Assert.That(allProgresses.Count(p => p.GoalId == goal1.Id), Is.EqualTo(6));
            Assert.That(allProgresses.Count(p => p.GoalId == goal2.Id), Is.EqualTo(6));
            Assert.That(allProgresses.All(p => p.Date >= startDate && p.Date <= endDate));
        }

        [Test]
        public async Task CreateProgressInstancesForDateRange_ShouldNotCreateDuplicateInstances()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "Goal 1" };
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
            var startDate = DateTime.UtcNow.Date;
            var endDate = startDate.AddDays(5);

            // Create some initial progress instances
            await _progressService.CreateProgressInstancesForDateRange(startDate, endDate);

            // Act
            await _progressService.CreateProgressInstancesForDateRange(startDate, endDate);

            // Assert
            var allProgresses = await _context.Progresses.ToListAsync();
            Assert.That(allProgresses.Count, Is.EqualTo(6)); // Still only 6 instances (1 goal * 6 days)
        }

        [Test]
        public async Task CreateProgressInstancesForDateRange_ShouldCreateInstancesForFutureDates()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "Goal 1" };
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
            var startDate = DateTime.UtcNow.Date.AddDays(1);
            var endDate = startDate.AddDays(30);

            // Act
            await _progressService.CreateProgressInstancesForDateRange(startDate, endDate);

            // Assert
            var allProgresses = await _context.Progresses.ToListAsync();
            Assert.That(allProgresses.Count, Is.EqualTo(31)); // 1 goal * 31 days
            Assert.That(allProgresses.All(p => p.Date >= startDate && p.Date <= endDate));
        }

        [Test]
        public async Task CreateProgressInstancesForDateRange_ShouldHandleEmptyGoalList()
        {
            // Arrange
            var startDate = DateTime.UtcNow.Date;
            var endDate = startDate.AddDays(5);

            // Act
            await _progressService.CreateProgressInstancesForDateRange(startDate, endDate);

            // Assert
            var allProgresses = await _context.Progresses.ToListAsync();
            Assert.That(allProgresses.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task CreateProgressInstancesForDateRange_ShouldHandleInvalidDateRange()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "Goal 1" };
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
            var startDate = DateTime.UtcNow.Date;
            var endDate = startDate.AddDays(-5); // End date before start date

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _progressService.CreateProgressInstancesForDateRange(startDate, endDate));
        }
    }
}
