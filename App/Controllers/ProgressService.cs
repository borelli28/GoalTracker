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
    Task<List<Progress>> GetProgressesForGoalAsync(string goalId, DateTime startDate);
    Task CreateProgressInstancesForDateRange(DateTime startDate, DateTime endDate);
}

public class ProgressService : IProgressService
{
    private readonly AppDbContext _context;
    
    public ProgressService(AppDbContext context)
    {
        _context = context;
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
        var existingProgress = await GetProgressByIdAsync(progress.Id);
        if (existingProgress == null)
        {
            throw new ArgumentException("Progress not found", nameof(progress));
        }
        existingProgress.Completed = progress.Completed;
        existingProgress.Notes = progress.Notes;
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
    
    public async Task<List<Progress>> GetProgressesForGoalAsync(string goalId, DateTime startDate)
    {
        var endDate = DateTime.UtcNow;
        var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                 .Select(offset => startDate.AddDays(offset))
                                 .ToList();
    
        var existingProgress = await _context.Progresses
            .Where(p => p.GoalId == goalId && p.Date >= startDate && p.Date <= endDate)
            .ToListAsync();
    
        var result = allDates.Select(date => 
            existingProgress.FirstOrDefault(p => p.Date.Date == date.Date) ?? 
            new Progress 
            { 
                GoalId = goalId, 
                Date = date, 
                Completed = false 
            }
        ).ToList();
    
        return result;
    }
    
    public async Task CreateProgressInstancesForDateRange(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
        {
            throw new ArgumentException("End date must be after start date", nameof(endDate));
        }
    
        var goals = await _context.Goals.ToListAsync();
        var dateRange = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                  .Select(offset => startDate.AddDays(offset));

        foreach (var goal in goals)
        {
            foreach (var date in dateRange)
            {
                if (!await _context.Progresses.AnyAsync(p => p.GoalId == goal.Id && p.Date == date))
                {
                    _context.Progresses.Add(new Progress
                    {
                        GoalId = goal.Id,
                        Date = date,
                        Completed = false,
                        Notes = string.Empty
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
    }
}
