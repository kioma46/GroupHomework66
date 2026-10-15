using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GroupHomework66.Models;
using Microsoft.AspNetCore.Authorization;

namespace GroupHomework66.Controllers;

public class HomeController : Controller
{
    [Authorize]
    public IActionResult Index()
    {
        if (User.IsInRole("Company"))
            return RedirectToAction("Index", "Company");

        if (User.IsInRole("Applicant"))
            return RedirectToAction("List", "Vacancy");
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}