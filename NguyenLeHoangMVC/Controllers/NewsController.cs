// Controllers/NewsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using NguyenLeHoangMVC_BusinessLayer.Services;  // CHANGE: Import cho INewsService (DI cho CRUD).
using NguyenLeHoangMVC_DataLayer.Models;  // CHANGE: Using Models OK từ trước.

namespace NguyenLeHoangMVC.Controllers;

public class NewsController : Controller {
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService) {
        _newsService = newsService;  // DI từ BusinessLayer.
    }

    // CHANGE: Action NewsManagement: List all news (Staff role=1), với search theo Title (LINQ Where). Role check session. Tương tự Index nhưng view riêng.
    public async Task<IActionResult> NewsManagement(string searchString) {
        if (HttpContext.Session.GetInt32("UserRole") != 1) {  // CHANGE: Chỉ Staff (role=1) được xem management.
            TempData["Error"] = "Bạn không có quyền truy cập trang này!";
            return RedirectToAction("Index", "Home");
        }
        var allNews = await _newsService.GetAllNewsAsync();  // CHANGE: Query all news.
        if (!string.IsNullOrEmpty(searchString)) {
            allNews = allNews.Where(n => n.NewsTitle.Contains(searchString));  // CHANGE: LINQ search theo Title.
        }
        allNews = allNews.OrderByDescending(n => n.CreatedDate);  // CHANGE: Sort descending CreatedDate (theo đề báo cáo).
        return View("NewsManagement", allNews);  // CHANGE: Truyền model sang View NewsManagement.cshtml.
    }

    // CHANGE: Action Create: Hiển thị form popup (sau implement Modal), validation, insert qua Service.
    [HttpGet]
    public async Task<IActionResult> Create() {  // CHANGE: Async để load categories.
        if (HttpContext.Session.GetInt32("UserRole") != 1) {
            return Unauthorized();
        }
        var categories = await _newsService.GetActiveCategoriesAsync();  // CHANGE: Load active categories cho dropdown.
        ViewBag.Categories = categories;  // CHANGE: Truyền ViewBag cho <select> in View.
        return View();  // View CreateNews.cshtml với form (NewsTitle required, etc.).
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewsArticle model) {  // CHANGE: Model binding từ form, validation.
        if (HttpContext.Session.GetInt32("UserRole") != 1) {
            return Unauthorized();
        }
        if (!ModelState.IsValid) {  // CHANGE: Validation theo đề (required fields).
            var categories = await _newsService.GetActiveCategoriesAsync();  // CHANGE: Reload categories nếu invalid (giữ dropdown).
            ViewBag.Categories = categories;
            return View(model);
        }
        model.NewsStatus = true;  // CHANGE: Default active.
        model.CreatedById = (short) (HttpContext.Session.GetInt32("UserRole") ?? 1);  // Giả sử CreatedById từ session (sau lấy AccountId).
        await _newsService.CreateNewsAsync(model);  // Giả sử thêm method CreateNewsAsync trong INewsService.
        TempData["Success"] = "Tạo news thành công!";
        return RedirectToAction("Index");
    }

    // CHANGE: Action Edit: Load news by ID, form popup, update qua Service.
    [HttpGet]
    public async Task<IActionResult> Edit(string id) {  // PK NewsArticleId string.
        if (HttpContext.Session.GetInt32("UserRole") != 1) {
            return Unauthorized();
        }
        var news = await _newsService.GetByIdAsync(id);
        if (news == null) {
            return NotFound();
        }
        var categories = await _newsService.GetActiveCategoriesAsync();  // CHANGE: Load categories cho dropdown.
        ViewBag.Categories = categories;
        return View(news);  // View UpdateNews.cshtml.
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, NewsArticle model) {
        if (HttpContext.Session.GetInt32("UserRole") != 1) {
            return Unauthorized();
        }
        if (id != model.NewsArticleId || !ModelState.IsValid) {
            var categories = await _newsService.GetActiveCategoriesAsync();  // CHANGE: Reload nếu invalid.
            ViewBag.Categories = categories;
            return View(model);
        }
        model.UpdatedById = (short) (HttpContext.Session.GetInt32("UserRole") ?? 1);  // Giả sử UpdatedById từ session.
        model.ModifiedDate = DateTime.Now;  // CHANGE: Update date.
        await _newsService.UpdateNewsAsync(model);
        TempData["Success"] = "Cập nhật news thành công!";
        return RedirectToAction("Index");
    }

    // CHANGE: Action Delete: Confirm dialog (JS sau), delete qua Service.
    [HttpPost]
    public async Task<IActionResult> Delete(string id) {
        if (HttpContext.Session.GetInt32("UserRole") != 1) {
            return Unauthorized();
        }
        await _newsService.DeleteNewsAsync(id);  // Giả sử thêm method DeleteNewsAsync.
        TempData["Success"] = "Xóa news thành công!";
        return RedirectToAction("Index");
    }
}