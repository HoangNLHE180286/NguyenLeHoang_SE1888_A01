using NguyenLeHoangMVC_DataLayer.Models;
using NguyenLeHoangMVC_DataLayer.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace NguyenLeHoangMVC_BusinessLayer.Services;

public class AdminService : IAdminService {
    private readonly IRepository<SystemAccount> _accountRepo;
    private readonly IConfiguration _config;

    public AdminService(IRepository<SystemAccount> accountRepo, IConfiguration config) {
        _accountRepo = accountRepo;
        _config = config;
    }

    public async Task<SystemAccount> AuthenticateAsync(string email, string password) {
        var adminEmail = _config["Admin:Email"];
        var adminPassword = _config["Admin:Password"];
        if (email == adminEmail && password == adminPassword) {
            // admin account existence check
            var adminAccounts = await _accountRepo.FindAsync(a => a.AccountEmail == email);
            var adminAccount = adminAccounts.FirstOrDefault();
            if (adminAccount == null) {
                adminAccount = new SystemAccount { AccountEmail = email, AccountPassword = password, AccountRole = GetAdminRole() };
                await _accountRepo.InsertAsync(adminAccount);
            }
            return adminAccount;
        }

        var accounts = await _accountRepo.FindAsync(a => a.AccountEmail == email && a.AccountPassword == password);
        return accounts.FirstOrDefault();
    }

    public int GetAdminRole() {
        return int.TryParse(_config["Admin:Role"], out int role) ? role : 0;
    }

    public async Task<IEnumerable<SystemAccount>> GetAllAccountsAsync() {
        return await _accountRepo.GetAllAsync();
    }

    public async Task<SystemAccount> GetAccountByIdAsync(short id) {
        return await _accountRepo.GetByIdAsync(id);
    }

    public async Task<SystemAccount> CreateAccountAsync(SystemAccount model) {
        // auto generate PK AccountID with maxId + 1
        var maxId = await _accountRepo.GetAllAsync();
        short nextId = maxId.Any() ? (short) (maxId.Max(a => a.AccountId) + 1) : (short) 1;
        model.AccountId = nextId;
        return await _accountRepo.InsertAsync(model);
    }

    public async Task<SystemAccount> UpdateAccountAsync(SystemAccount model) {
        return await _accountRepo.UpdateAsync(model);
    }

    public async Task DeleteAccountAsync(short id) {
        await _accountRepo.DeleteAsync(id);
    }
}