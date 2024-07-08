using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using App.Services;
using App.Models;
using System;

namespace App.Services;

public class GoalController : Controller
{
    private readonly ILogger<GoalController> _logger;
    private readonly IGoalService _goalService;
    
    public GoalController(ILogger<GoalController> logger, IGoalService goalService)
    {
        _logger = logger;
        _goalService = goalService;
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
                await _goalService.CreateGoalAsync(goal);
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
}
