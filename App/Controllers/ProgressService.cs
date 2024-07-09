using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Services;
using App.Models;
using App.Data;

namespace App.Services;

public interface IProgressService
{
    Task<Progress> CreateProgressAsync(Progress progress);
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
    
    public async Task<Progress> CreateProgressAsync(Progress progress)
    {
        _context.Add(progress);
        await _context.SaveChangesAsync();
        return progress;
    }
}