using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using App.Services;
using App.Models;
using System;

namespace App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GoalController : ControllerBase
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
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Goal goal)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var newGoal = await _goalService.CreateGoalAsync(goal);
            var newProgress = await _progressService.CreateProgressAsync(newGoal.Id);
            
            return CreatedAtAction(nameof(Get), new { id = newGoal.Id }, newGoal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating goal: {ex}");
            return StatusCode(500, "An error occurred while creating the goal.");
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var goal = await _goalService.GetGoalByIdAsync(id);
        if (goal == null)
        {
            return NotFound();
        }
        return Ok(goal);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var goals = await _goalService.GetAllGoalsAsync();
            return Ok(goals);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all goals");
            return StatusCode(500, "An error occurred while retrieving goals.");
        }
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Goal goal)
    {
        if (id != goal.Id)
        {
            return BadRequest("Goal ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _goalService.UpdateGoalAsync(goal);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update goal");
            if (!await _goalService.GoalExistsAsync(goal.Id))
            {
                return NotFound();
            }
            return StatusCode(500, "An error occurred while updating the goal.");
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _goalService.DeleteGoalAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception raised while deleting goal");
            return StatusCode(500, "An error occurred while deleting the goal.");
        }
    }
}
