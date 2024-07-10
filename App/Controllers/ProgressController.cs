using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using App.Services;
using App.Models;
using System;

namespace App.Controllers;

public class ProgressController : Controller
{
    private readonly ILogger<ProgressController> _logger;
    private readonly IProgressService _progressService;
    
    public ProgressController(ILogger<ProgressController> logger, IProgressService progressService)
    {
        _logger = logger;
        _progressService = progressService;
    }
    
    public async Task<IActionResult> Update(string id)
    {
        var progress = await _progressService.GetProgressByIdAsync(id);
        if (progress == null)
        {
            return NotFound();
        }
        return View(progress);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(string id, [Bind("Id,Completed,Notes")] Progress progress)
    {
        if (id != progress.Id)
        {
            return NotFound();
        }
        else if (ModelState.IsValid)
        {
            try
            {
              await  _progressService.UpdateProgressAsync(progress);
              return RedirectToAction("Update", "Progress");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update progress");
                // If instance of goal does not exist in DB
                if (!await _progressService.ProgressExistsAsync(progress.Id))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", "Unable to update instance of progress");
                }
            }
        }
        return View(progress);
    }
}
