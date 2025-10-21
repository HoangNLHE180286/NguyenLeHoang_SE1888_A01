// BusinessLayer/Services/IAdminService.cs
using NguyenLeHoangMVC_DataLayer.Models;

namespace NguyenLeHoangMVC_BusinessLayer.Services;  // CHANGE: Update namespace to match project name.

public interface IAdminService {
    Task<SystemAccount> AuthenticateAsync(string email, string password);  // So sánh với config hoặc DB.
    int GetAdminRole();  // Lấy role=0 từ appsettings (inject IConfiguration).
    Task<IEnumerable<SystemAccount>> GetAllAccountsAsync();  // Query all accounts từ Repository.
    // CHANGE: Thêm CRUD methods cho Account (tương tự NewsService, theo đề CRUD + search).
    Task<SystemAccount> GetAccountByIdAsync(short id);  // Load by PK cho Edit.
    Task<SystemAccount> CreateAccountAsync(SystemAccount model);  // Insert mới (set default role).
    Task<SystemAccount> UpdateAccountAsync(SystemAccount model);  // Update.
    Task DeleteAccountAsync(short id);  // Delete (check FK nếu cần).
}