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
}
