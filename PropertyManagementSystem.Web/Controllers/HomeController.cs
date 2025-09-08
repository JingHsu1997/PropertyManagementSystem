using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.Core.Interfaces;
using PropertyManagementSystem.Web.Models;

namespace PropertyManagementSystem.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPropertyService _propertyService;

    public HomeController(ILogger<HomeController> logger, IPropertyService propertyService)
    {
        _logger = logger;
        _propertyService = propertyService;
    }

    public async Task<IActionResult> Index()
    {
        // 獲取最新的物件來展示在首頁
        var recentProperties = await _propertyService.GetAllPropertiesAsync();
        ViewBag.RecentProperties = recentProperties.Take(6); // 只顯示前6個物件
        
        // 統計資料
        ViewBag.TotalProperties = recentProperties.Count();
        ViewBag.ForSaleCount = recentProperties.Count(p => p.StatusId == 1);
        ViewBag.ForRentCount = recentProperties.Count(p => p.StatusId == 3);
        ViewBag.SoldCount = recentProperties.Count(p => p.StatusId == 2);
        
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
