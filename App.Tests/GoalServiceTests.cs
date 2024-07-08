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
            _goalService = new GoalService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        
        [Test]
        public async Task CreateGoalAsync_ShouldAddGoalAndReturnIt()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "New Goal" };

            // Act
            var result = await _goalService.CreateGoalAsync(goal);

            // Assert
            Assert.That(result, Is.EqualTo(goal));
            Assert.That(_context.Goals.Count(), Is.EqualTo(1));
        }
        
        [Test]
        public async Task GetGoalByIdAsync_ShouldReturnGoal()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "Test Goal" };
            await _context.Goals.AddAsync(goal);
            await _context.SaveChangesAsync();

            // Act
            var result = await _goalService.GetGoalByIdAsync("1");

            // Assert
            Assert.That(result, Is.EqualTo(goal));
        }
        
        [Test]
        public async Task UpdateGoalAsync_ShouldUpdateGoal()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "Original Goal" };
            await _context.Goals.AddAsync(goal);
            await _context.SaveChangesAsync();

            goal.Name = "Updated Goal";

            // Act
            await _goalService.UpdateGoalAsync(goal);

            // Assert
            var updatedGoal = await _context.Goals.FindAsync("1");
            Assert.That(updatedGoal.Name, Is.EqualTo("Updated Goal"));
        }
        
        [Test]
        public async Task DeleteGoalAsync_ShouldRemoveGoal()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "Goal to Delete" };
            await _context.Goals.AddAsync(goal);
            await _context.SaveChangesAsync();

            // Act
            await _goalService.DeleteGoalAsync("1");

            // Assert
            var deletedGoal = await _context.Goals.FindAsync("1");
            Assert.That(deletedGoal, Is.Null);
        }
        
        [Test]
        public async Task GetAllGoalsAsync_ShouldReturnAllGoals()
        {
            // Arrange
            await _context.Goals.AddRangeAsync(
                new Goal { Id = "1", Name = "Goal 1" },
                new Goal { Id = "2", Name = "Goal 2" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _goalService.GetAllGoalsAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }
    }
}
