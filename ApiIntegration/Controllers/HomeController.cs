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

    public async Task<IActionResult> Index()
    {
        var messages = await _fastApiService.GetFastApiResponseAsync();
        return View(messages); // pass List<string> directly to view
    }
}
