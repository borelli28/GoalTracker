using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using App.Services;
using App.Models;
using System;

namespace App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProgressController : ControllerBase
{
    private readonly ILogger<ProgressController> _logger;
    private readonly IProgressService _progressService;
    
    public ProgressController(ILogger<ProgressController> logger, IProgressService progressService)
    {
        _logger = logger;
        _progressService = progressService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Progress progress)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var newProgress = await _progressService.CreateProgressAsync(progress.GoalId);
            return CreatedAtAction(nameof(Get), new { id = newProgress.Id }, newProgress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating progress: {ex}");
            return StatusCode(500, "An error occurred while creating the progress.");
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var progress = await _progressService.GetProgressByIdAsync(id);
        if (progress == null)
        {
            return NotFound();
        }
        return Ok(progress);
    }
    
    [HttpGet("goal/{goalId}")]
    public async Task<IActionResult> GetProgressForGoal(string goalId, [FromQuery] DateTime startDate)
    {
        var progress = await _progressService.GetProgressesForGoalAsync(goalId, startDate);
        return Ok(progress);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Progress progress)
    {
        if (id != progress.Id)
        {
            return BadRequest("Progress ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _progressService.UpdateProgressAsync(progress);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update progress");
            return StatusCode(500, "An error occurred while updating the progress.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _progressService.DeleteProgressAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception raised while deleting progress");
            return StatusCode(500, "An error occurred while deleting the progress.");
        }
    }
}
