using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using App.Models;
using App.Data;

namespace App.Services;

public interface IGoalService
{
    Task<Goal> CreateGoalAsync(Goal goal);
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
}
