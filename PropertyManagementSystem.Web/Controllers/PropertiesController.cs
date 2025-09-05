using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.Core.Interfaces;
using PropertyManagementSystem.Core.Models;
using PropertyManagementSystem.Web.Models;

namespace PropertyManagementSystem.Web.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        // GET: Properties
        public async Task<IActionResult> Index(string? city, string? district, int? typeId, 
            int? statusId, decimal? minPrice, decimal? maxPrice)
        {
            var properties = await _propertyService.SearchPropertiesAsync(city, district, typeId, statusId, minPrice, maxPrice);
            
            ViewBag.Cities = new[] { "台北市", "新北市", "桃園市", "台中市", "台南市", "高雄市" };
            ViewBag.PropertyTypes = new[] { new { Id = 1, Name = "公寓" }, new { Id = 2, Name = "透天" }, new { Id = 3, Name = "電梯大樓" } };
            ViewBag.PropertyStatuses = new[] { new { Id = 1, Name = "待售" }, new { Id = 2, Name = "已售" }, new { Id = 3, Name = "出租" } };
            
            return View(properties);
        }

        // GET: Properties/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }

        // GET: Properties/Create
        public IActionResult Create()
        {
            ViewBag.PropertyTypes = new[] { new { Id = 1, Name = "公寓" }, new { Id = 2, Name = "透天" }, new { Id = 3, Name = "電梯大樓" } };
            ViewBag.PropertyStatuses = new[] { new { Id = 1, Name = "待售" }, new { Id = 2, Name = "已售" }, new { Id = 3, Name = "出租" } };
            return View();
        }

        // POST: Properties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Property property)
        {
            if (ModelState.IsValid)
            {
                property.CreatedAt = DateTime.UtcNow;
                await _propertyService.CreatePropertyAsync(property);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.PropertyTypes = new[] { new { Id = 1, Name = "公寓" }, new { Id = 2, Name = "透天" }, new { Id = 3, Name = "電梯大樓" } };
            ViewBag.PropertyStatuses = new[] { new { Id = 1, Name = "待售" }, new { Id = 2, Name = "已售" }, new { Id = 3, Name = "出租" } };
            return View(property);
        }

        // GET: Properties/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            ViewBag.PropertyTypes = new[] { new { Id = 1, Name = "公寓" }, new { Id = 2, Name = "透天" }, new { Id = 3, Name = "電梯大樓" } };
            ViewBag.PropertyStatuses = new[] { new { Id = 1, Name = "待售" }, new { Id = 2, Name = "已售" }, new { Id = 3, Name = "出租" } };
            return View(property);
        }

        // POST: Properties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Property property)
        {
            if (id != property.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _propertyService.UpdatePropertyAsync(property);
                }
                catch (InvalidOperationException)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.PropertyTypes = new[] { new { Id = 1, Name = "公寓" }, new { Id = 2, Name = "透天" }, new { Id = 3, Name = "電梯大樓" } };
            ViewBag.PropertyStatuses = new[] { new { Id = 1, Name = "待售" }, new { Id = 2, Name = "已售" }, new { Id = 3, Name = "出租" } };
            return View(property);
        }

        // GET: Properties/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _propertyService.DeletePropertyAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
