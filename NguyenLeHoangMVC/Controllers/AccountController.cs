using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using NguyenLeHoangMVC_BusinessLayer.Services;
using NguyenLeHoangMVC_DataLayer.Models;

namespace NguyenLeHoangMVC.Controllers;

public class AccountController : Controller {
    private readonly IAdminService _adminService;

    public AccountController(IAdminService adminService) {
        _adminService = adminService;
    }

    public IActionResult Login() {
        // role check, if logged in then redirect to Home
        if (HttpContext.Session.GetString("UserEmail") != null) {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    public IActionResult Logout() {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password) {
        // data validation
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) {
            TempData["Error"] = "Email and password cannot be empty!";
            return View();
        }

        var account = await _adminService.AuthenticateAsync(email, password);
        if (account != null) {
            HttpContext.Session.SetString("UserEmail", email);
            int role = account.AccountRole ?? 0;
            HttpContext.Session.SetInt32("UserRole", role);
            return RedirectToAction("Index", "Home");
        }

        TempData["Error"] = "Invalid email or password!";
        return View();
    }

    public async Task<IActionResult> AccountManagement(string searchString) {
        // role check, only role=0 can access
        if (HttpContext.Session.GetInt32("UserRole") != 0) {
            TempData["Error"] = "You do not have permission to access this page!";
            return RedirectToAction("Index", "Home");
        }

        var allAccounts = await _adminService.GetAllAccountsAsync();
        if (!string.IsNullOrEmpty(searchString)) {
            var words = searchString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            allAccounts = allAccounts.Where(a => words.Any(word => (a.AccountName?.Contains(word, StringComparison.OrdinalIgnoreCase) == true) || (a.AccountEmail?.Contains(word, StringComparison.OrdinalIgnoreCase) == true)));
        }

        allAccounts = allAccounts.OrderBy(a => a.AccountId);
        ViewBag.RoleNames = new Dictionary<int, string> { { 0, "Admin" }, { 1, "Staff" }, { 2, "Lecturer" } };
        return View(allAccounts);
    }

    [HttpGet]
    public IActionResult Create() {
        // role check, only role=0 can access
        if (HttpContext.Session.GetInt32("UserRole") != 0) {
            return Unauthorized();
        }

        return View("CreateAccount");
    }

    [HttpPost]
    public async Task<IActionResult> Create(SystemAccount model) {
        // role check, only role=0 can access
        if (HttpContext.Session.GetInt32("UserRole") != 0) {
            return Unauthorized();
        }

        // data validation
        if (!ModelState.IsValid) {
            return Json(new { success = false, error = "Validation failed. Please check the form." });
        }

        await _adminService.CreateAccountAsync(model);
        return Json(new { success = true });
    }


    [HttpGet]
    public async Task<IActionResult> Update(short id) {
        // role check, only role=0 can access
        if (HttpContext.Session.GetInt32("UserRole") != 0) {
            return Unauthorized();
        }

        var account = await _adminService.GetAccountByIdAsync(id);
        if (account == null) {
            return NotFound();
        }

        var currentEmail = HttpContext.Session.GetString("UserEmail");
        if (account.AccountEmail == currentEmail) {
            TempData["Error"] = "You cannot edit your own account!";
            return RedirectToAction("AccountManagement");
        }

        return View("UpdateAccount", account);
    }

    [HttpPost]
    public async Task<IActionResult> Update(short id, SystemAccount model) {
        // role check, only role=0 can access
        if (HttpContext.Session.GetInt32("UserRole") != 0) {
            return Unauthorized();
        }

        // data validation
        if (id != model.AccountId || !ModelState.IsValid) {
            return Json(new { success = false, error = "Validation failed. Please check the form." });
        }

        var currentEmail = HttpContext.Session.GetString("UserEmail");
        if (model.AccountEmail == currentEmail) {
            TempData["Error"] = "Admin cannot edit their own account!";
            return Json(new { success = false, error = "Admin cannot edit their own account!" });
        }

        await _adminService.UpdateAccountAsync(model);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(short id) {
        // role check, only role=0 can access
        if (HttpContext.Session.GetInt32("UserRole") != 0) {
            return Unauthorized();
        }

        await _adminService.DeleteAccountAsync(id);
        TempData["Success"] = "Account deleted successfully!";
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetAccountJson(short id) {
        var account = await _adminService.GetAccountByIdAsync(id);
        if (account == null) {
            return NotFound();
        }

        return Json(new {
            accountId = account.AccountId,
            accountName = account.AccountName,
            accountEmail = account.AccountEmail,
            accountPassword = account.AccountPassword,
            accountRole = account.AccountRole
        });
    }
}