using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using App.Services;
using App.Models;
using System;

namespace App.Controllers;

public class GoalController : Controller
{
    private readonly ILogger<GoalController> _logger;
    private readonly IGoalService _goalService;
     private readonly IProgressService _progressService;
    
    public GoalController(ILogger<GoalController> logger, IGoalService goalService, IProgressService progressService)
    {
        _logger = logger;
        _goalService = goalService;
        _progressService = progressService;
    }
    
    public async Task<IActionResult> Index()
    {
        try
        {
            var goals = await _goalService.GetAllGoalsAsync();
            if (!goals.Any())
            {
                return View(null);
            }

            var goalsWithProgress = new List<(Goal Goal, Progress LastProgress)>();

            foreach (var goal in goals)
            {
                var lastProgress = await _progressService.GetLastProgressInstance(goal.Id);
                goalsWithProgress.Add((goal, lastProgress));
            }

            return View(goalsWithProgress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching goals");
            return View(null);
        }
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] Goal goal)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var newGoal = await _goalService.CreateGoalAsync(goal);
                var newProgress = await _progressService.CreateProgressAsync(newGoal.Id);
                
                TempData["SuccessMessage"] = "Goal added";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating goal: {ex}");
                ModelState.AddModelError("", "Unable to create Goal");
            }
        }
        return View(goal);
    }
    
    public async Task<IActionResult> Update(string id)
    {
        var goal = await _goalService.GetGoalByIdAsync(id);
        if (goal == null)
        {
            return NotFound();
        }
        return View(goal);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(string id, [Bind("Id,Name,Description")] Goal goal)
    {
        if (id != goal.Id)
        {
            return NotFound();
        }
        else if (ModelState.IsValid)
        {
            try
            {
              await  _goalService.UpdateGoalAsync(goal);
              return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update goal");
                // If instance of goal does not exist in DB
                if (!await _goalService.GoalExistsAsync(goal.Id))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", "Unable to update instance of goal");
                }
            }
        }
        return View(goal);
    }
    
    public async Task<IActionResult> Delete(string id)
    {   
        var goal = await _goalService.GetGoalByIdAsync(id);
        if (goal == null)
        {
            return NotFound();
        }
        return View(goal);
    }
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        try
        {
            await _goalService.DeleteGoalAsync(id);
            TempData["SuccessMessage"] = "Goal deleted";
            return RedirectToAction("Index", "Goal");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception raised while deleting goal");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the goal.");
        }
    }
}
