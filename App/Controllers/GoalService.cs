using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using App.Models;
using App.Data;

namespace App.Services;

public interface IGoalService
{
    Task<Goal> CreateGoalAsync(Goal goal);
    Task<Goal> GetGoalByIdAsync(string id);
    Task UpdateGoalAsync(Goal goal);
    Task DeleteGoalAsync(string id);
    Task<IEnumerable<Goal>> GetAllGoalsAsync();
    Task<bool> GoalExistsAsync(string id);
}

public class GoalService : IGoalService
{
    private readonly AppDbContext _context;

    public GoalService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Goal> CreateGoalAsync(Goal goal)
    {
        _context.Add(goal);
        await _context.SaveChangesAsync();
        return goal;
    }
    
    public async Task<Goal> GetGoalByIdAsync(string id)
    {
        return await _context.Goals.FirstOrDefaultAsync(m => m.Id == id);
    }
    
    public async Task UpdateGoalAsync(Goal goal)
    {
        _context.Update(goal);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteGoalAsync(string id)
    {
        var goal = await GetGoalByIdAsync(id);
        if (goal != null)
        {
            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<IEnumerable<Goal>> GetAllGoalsAsync()
    {
        return await _context.Goals.ToListAsync();
    }
    
    public async Task<bool> GoalExistsAsync(string id)
    {
        return await _context.Goals.AnyAsync(e => e.Id == id);
    }
}
