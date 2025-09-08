using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.Core.Interfaces;
using PropertyManagementSystem.Core.Models;
using PropertyManagementSystem.Web.Models;

namespace PropertyManagementSystem.Web.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly ILogger<PropertiesController> _logger;

        public PropertiesController(IPropertyService propertyService, ILogger<PropertiesController> logger)
        {
            _propertyService = propertyService;
            _logger = logger;
        }

        // GET: Properties (READ - 讀取所有物件)
        public async Task<IActionResult> Index(string? city, string? district, int? typeId, 
            int? statusId, decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                _logger.LogInformation("正在執行 READ 操作 - 獲取物件列表");
                
                var properties = await _propertyService.SearchPropertiesAsync(city, district, typeId, statusId, minPrice, maxPrice);
                
                // 準備下拉選單資料
                ViewBag.Cities = new[] { "台北市", "新北市", "桃園市", "台中市", "台南市", "高雄市" };
                ViewBag.PropertyTypes = new[] { 
                    new { Id = 1, Name = "公寓" }, 
                    new { Id = 2, Name = "透天" }, 
                    new { Id = 3, Name = "電梯大樓" } 
                };
                ViewBag.PropertyStatuses = new[] { 
                    new { Id = 1, Name = "待售" }, 
                    new { Id = 2, Name = "已售" }, 
                    new { Id = 3, Name = "出租" } 
                };
                
                // 搜尋統計
                ViewBag.SearchParams = new {
                    City = city,
                    District = district,
                    TypeId = typeId,
                    StatusId = statusId,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                };
                
                _logger.LogInformation("READ 操作完成 - 找到 {Count} 個物件", properties.Count());
                
                return View(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "READ 操作失敗");
                TempData["Error"] = "讀取物件資料時發生錯誤，請稍後再試";
                return View(new List<Property>());
            }
        }

        // GET: Properties/Details/5 (READ - 讀取單一物件詳情)
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                _logger.LogInformation("正在執行 READ 操作 - 獲取物件詳情，ID: {Id}", id);
                
                var property = await _propertyService.GetPropertyByIdAsync(id);
                if (property == null)
                {
                    _logger.LogWarning("READ 操作 - 物件不存在，ID: {Id}", id);
                    TempData["Error"] = "找不到指定的物件";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation("READ 操作完成 - 成功獲取物件詳情，ID: {Id}", id);
                return View(property);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "READ 操作失敗，ID: {Id}", id);
                TempData["Error"] = "讀取物件詳情時發生錯誤";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Properties/Create (CREATE - 顯示新增表單)
        public IActionResult Create()
        {
            _logger.LogInformation("顯示 CREATE 表單");
            
            LoadViewBagData();
            ViewBag.OperationType = "CREATE";
            return View();
        }

        // POST: Properties/Create (CREATE - 執行新增操作)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Property property, string? imageUrl)
        {
            try
            {
                _logger.LogInformation("正在執行 CREATE 操作 - 新增物件");
                
                if (ModelState.IsValid)
                {
                    property.CreatedAt = DateTime.UtcNow;
                    property.UpdatedAt = DateTime.UtcNow;
                    
                    // 處理圖片URL
                    if (!string.IsNullOrWhiteSpace(imageUrl))
                    {
                        property.Images = new List<PropertyImage>
                        {
                            new PropertyImage
                            {
                                ImageUrl = imageUrl.Trim(),
                                AltText = property.Title,
                                SortOrder = 0,
                                CreatedAt = DateTime.UtcNow
                            }
                        };
                    }
                    
                    var result = await _propertyService.CreatePropertyAsync(property);
                    
                    _logger.LogInformation("CREATE 操作完成 - 新增物件成功，ID: {Id}", result.Id);
                    TempData["Success"] = $"物件「{property.Title}」新增成功！";
                    
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning("CREATE 操作失敗 - 表單驗證錯誤");
                TempData["Error"] = "請檢查輸入資料是否正確";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CREATE 操作失敗");
                TempData["Error"] = "新增物件時發生錯誤，請稍後再試";
            }

            LoadViewBagData();
            ViewBag.OperationType = "CREATE";
            return View(property);
        }

        // GET: Properties/Edit/5 (UPDATE - 顯示編輯表單)
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                _logger.LogInformation("顯示 UPDATE 表單，ID: {Id}", id);
                
                var property = await _propertyService.GetPropertyByIdAsync(id);
                if (property == null)
                {
                    _logger.LogWarning("UPDATE 操作 - 物件不存在，ID: {Id}", id);
                    TempData["Error"] = "找不到指定的物件";
                    return RedirectToAction(nameof(Index));
                }

                LoadViewBagData();
                ViewBag.OperationType = "UPDATE";
                return View(property);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "顯示 UPDATE 表單失敗，ID: {Id}", id);
                TempData["Error"] = "載入編輯表單時發生錯誤";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Properties/Edit/5 (UPDATE - 執行更新操作)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Property property, string? imageUrl)
        {
            if (id != property.Id)
            {
                _logger.LogWarning("UPDATE 操作失敗 - ID 不匹配，路由ID: {RouteId}, 物件ID: {PropertyId}", id, property.Id);
                TempData["Error"] = "物件 ID 不匹配";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _logger.LogInformation("正在執行 UPDATE 操作，ID: {Id}", id);
                
                if (ModelState.IsValid)
                {
                    property.UpdatedAt = DateTime.UtcNow;
                    
                    // 處理圖片URL
                    if (!string.IsNullOrWhiteSpace(imageUrl))
                    {
                        // 清除現有圖片並新增新的圖片
                        property.Images = new List<PropertyImage>
                        {
                            new PropertyImage
                            {
                                PropertyId = property.Id,
                                ImageUrl = imageUrl.Trim(),
                                AltText = property.Title,
                                SortOrder = 0,
                                CreatedAt = DateTime.UtcNow
                            }
                        };
                    }
                    else
                    {
                        // 如果沒有提供圖片URL，保持現有圖片
                        property.Images = new List<PropertyImage>();
                    }
                    
                    await _propertyService.UpdatePropertyAsync(property);
                    
                    _logger.LogInformation("UPDATE 操作完成 - 更新物件成功，ID: {Id}", id);
                    TempData["Success"] = $"物件「{property.Title}」更新成功！";
                    
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning("UPDATE 操作失敗 - 表單驗證錯誤，ID: {Id}", id);
                TempData["Error"] = "請檢查輸入資料是否正確";
            }
            catch (InvalidOperationException)
            {
                _logger.LogWarning("UPDATE 操作失敗 - 物件不存在，ID: {Id}", id);
                TempData["Error"] = "找不到指定的物件";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UPDATE 操作失敗，ID: {Id}", id);
                TempData["Error"] = "更新物件時發生錯誤，請稍後再試";
            }

            LoadViewBagData();
            ViewBag.OperationType = "UPDATE";
            return View(property);
        }

        // POST: Properties/Delete (DELETE - 執行刪除操作)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("正在執行 DELETE 操作，ID: {Id}", id);
                
                var property = await _propertyService.GetPropertyByIdAsync(id);
                if (property == null)
                {
                    _logger.LogWarning("DELETE 操作失敗 - 物件不存在，ID: {Id}", id);
                    TempData["Error"] = "找不到指定的物件";
                    return RedirectToAction(nameof(Index));
                }

                var success = await _propertyService.DeletePropertyAsync(id);
                
                if (success)
                {
                    _logger.LogInformation("DELETE 操作完成 - 刪除物件成功，ID: {Id}", id);
                    TempData["Success"] = $"物件「{property.Title}」已成功刪除！";
                }
                else
                {
                    _logger.LogWarning("DELETE 操作失敗 - 刪除操作未成功，ID: {Id}", id);
                    TempData["Error"] = "刪除物件失敗，請稍後再試";
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DELETE 操作失敗，ID: {Id}", id);
                TempData["Error"] = "刪除物件時發生錯誤，請稍後再試";
                return RedirectToAction(nameof(Index));
            }
        }

        private void LoadViewBagData()
        {
            ViewBag.PropertyTypes = new[] { 
                new { Id = 1, Name = "公寓" }, 
                new { Id = 2, Name = "透天" }, 
                new { Id = 3, Name = "電梯大樓" } 
            };
            ViewBag.PropertyStatuses = new[] { 
                new { Id = 1, Name = "待售" }, 
                new { Id = 2, Name = "已售" }, 
                new { Id = 3, Name = "出租" } 
            };
        }
    }
}
