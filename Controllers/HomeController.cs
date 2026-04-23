using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProjectLMS.Services.Interfaces;
using System.Collections.Generic;
using ProjectLMS.Models.ViewModels;
using System.Linq;

namespace ProjectLMS.Controllers;

/// <summary>
/// Controller for the home page and general navigation.
/// </summary>
[AllowAnonymous]
public class HomeController : Controller
{
    private readonly IStatisticsService _statisticsService;

    public HomeController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    /// <summary>
    /// GET: /
    /// Displays the home page with system overview.
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// GET: /Home/About
    /// Displays information about the LMS system.
    /// </summary>
    public IActionResult About()
    {
        ViewData["Message"] = "Learning Management System (LMS) built with ASP.NET Core MVC";

        return View();
    }

    /// <summary>
    /// GET: /Home/Contact
    /// Displays contact information.
    /// </summary>
    public IActionResult Contact()
    {
        ViewData["Message"] = "Contact information for the LMS administrators.";

        return View();
    }

    /// <summary>
    /// GET: /Home/Privacy
    /// Displays privacy policy.
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// GET: /Home/Error
    /// Displays error page.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
