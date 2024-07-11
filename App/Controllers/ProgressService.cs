using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Services;
using App.Models;
using App.Data;

namespace App.Services;

public interface IProgressService
{
    Task<Progress> CreateProgressAsync(string goalId, Progress progress = null);
    Task<Progress?> GetProgressByIdAsync(string id);
    Task UpdateProgressAsync(Progress progress);
    Task DeleteProgressAsync(string id);
    Task<IEnumerable<Progress>> GetAllProgressAsync();
    Task<bool> ProgressExistsAsync(string id);
    Task<Progress> GetLastProgressInstance(string goalId);
}

public class ProgressService : IProgressService
{
    private readonly AppDbContext _context;
    private readonly IGoalService _goalService;
    
    public ProgressService(AppDbContext context, IGoalService goalService)
    {
        _context = context;
        _goalService = goalService;
    }
    
    public async Task<Progress> CreateProgressAsync(string goalId, Progress progress = null)
    {
        if (progress == null)
        {
            progress = new Progress
            {
                GoalId = goalId
            };
        }
    
        _context.Add(progress);
        await _context.SaveChangesAsync();
        return progress;
    }
    
    public async Task<Progress?> GetProgressByIdAsync(string id)
    {
        return await _context.Progresses.FirstOrDefaultAsync(m => m.Id == id);
    }
    
    public async Task UpdateProgressAsync(Progress progress)
    {
        _context.Update(progress);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteProgressAsync(string id)
    {
        var progress = await GetProgressByIdAsync(id);
        if (progress != null)
        {
            _context.Progresses.Remove(progress);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<IEnumerable<Progress>> GetAllProgressAsync()
    {
        return await _context.Progresses.ToListAsync();
    }
    
    public async Task<bool> ProgressExistsAsync(string id)
    {
        return await _context.Progresses.AnyAsync(e => e.Id == id);
    }
    
    public async Task<Progress> GetLastProgressInstance(string goalId)
    {
        return await _context.Progresses
            .Where(p => p.GoalId == goalId)
            .OrderByDescending(p => p.Date)
            .FirstOrDefaultAsync();
    }
}
