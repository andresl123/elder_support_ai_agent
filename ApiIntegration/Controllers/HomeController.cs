using Microsoft.AspNetCore.Mvc;
using ApiIntegration.Services;

namespace ApiIntegration.Controllers;

public class HomeController : Controller
{
    private readonly FastApiService _fastApiService;

    public HomeController(FastApiService fastApiService)
    {
        _fastApiService = fastApiService;
    }

    [HttpGet]
    public IActionResult Index(string? sessionId)
    {
        ViewBag.SessionId = sessionId;
        return View(new List<string>());
    }

    [HttpPost]
    public async Task<IActionResult> CreateSessionFromIndex()
    {
        var sessionId = await _fastApiService.CreateSessionAsync();
        return RedirectToAction("Index", new { sessionId });
    }

    [HttpPost]
    public async Task<IActionResult> CallAgent(string sessionId)
    {
        var messages = await _fastApiService.GetFastApiResponseAsync(sessionId);
        ViewBag.SessionId = sessionId;
        return View("Index", messages);
    }
}