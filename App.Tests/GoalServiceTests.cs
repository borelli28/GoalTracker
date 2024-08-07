using Microsoft.EntityFrameworkCore;
using App.Services;
using App.Models;
using App.Data;

namespace App.UnitTests.Services
{
    [TestFixture]
    public class GoalServiceTests
    {
        private AppDbContext _context = null!;
        private IGoalService _goalService = null!;

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
            var result = await _goalService.GetGoalByIdAsync(goal.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result?.Name, Is.EqualTo("Updated Goal"));
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
        
        [Test]
        public async Task GoalExistsAsync_ShouldReturnTrueForExistingGoal()
        {
            // Arrange
            var goal = new Goal { Id = "1", Name = "Existing Goal" };
            await _context.Goals.AddAsync(goal);
            await _context.SaveChangesAsync();

            // Act
            var result = await _goalService.GoalExistsAsync("1");

            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task GoalExistsAsync_ShouldReturnFalseForNonExistingGoal()
        {
            // Act
            var result = await _goalService.GoalExistsAsync("999");

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
