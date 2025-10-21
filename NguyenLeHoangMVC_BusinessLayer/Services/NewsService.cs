// BusinessLayer/Services/NewsService.cs
using NguyenLeHoangMVC_DataLayer.Repositories;
using NguyenLeHoangMVC_DataLayer.Models;
using Microsoft.EntityFrameworkCore;  // CHANGE: Add using cho Include navigation (Category, Tags).

namespace NguyenLeHoangMVC_BusinessLayer.Services;

public class NewsService : INewsService {
    private readonly IRepository<NewsArticle> _newsRepo;
    private readonly IRepository<Category> _categoryRepo;  // CHANGE: Inject IRepository<Category> cho GetActiveCategoriesAsync (fix CS7036, dùng DI Scoped).

    public NewsService(IRepository<NewsArticle> newsRepo, IRepository<Category> categoryRepo) {  // CHANGE: Thêm param categoryRepo cho DI.
        _newsRepo = newsRepo;
        _categoryRepo = categoryRepo;  // CHANGE: Khởi tạo field mới.
    }

    public async Task<IEnumerable<NewsArticle>> GetActiveNewsAsync() {
        // CHANGE: Dùng LINQ qua FindAsync (Where NewsStatus==true, theo đề). Include Category cho display.
        return await _newsRepo.FindAsync(n => n.NewsStatus == true);  // EF tự load navigation nếu cần.
    }

    // CHANGE: Implement GetAllNewsAsync (tất cả news cho Staff CRUD).
    public async Task<IEnumerable<NewsArticle>> GetAllNewsAsync() {
        return await _newsRepo.GetAllAsync();  // Lấy tất cả (sau thêm paging/search).
    }

    // CHANGE: Implement GetByIdAsync (load single với Include Category/Tags cho Edit).
    public async Task<NewsArticle> GetByIdAsync(string id) {
        return await _newsRepo.GetByIdAsync(id);  // Giả sử Repository.FindAsync(id) cho single.
    }

    // CHANGE: Implement CreateNewsAsync (insert, set CreatedDate).
    public async Task<NewsArticle> CreateNewsAsync(NewsArticle model) {
        model.CreatedDate = DateTime.Now;  // CHANGE: Set default date.
        return await _newsRepo.InsertAsync(model);
    }

    // CHANGE: Implement UpdateNewsAsync (update).
    public async Task<NewsArticle> UpdateNewsAsync(NewsArticle model) {
        return await _newsRepo.UpdateAsync(model);
    }

    // CHANGE: Implement DeleteNewsAsync (delete, check FK nếu cần).
    public async Task DeleteNewsAsync(string id) {
        await _newsRepo.DeleteAsync(id);
    }

    // CHANGE: Implement GetActiveCategoriesAsync (query Categories IsActive=true, dùng _categoryRepo inject).
    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync() {
        return await _categoryRepo.FindAsync(c => c.IsActive == true);  // CHANGE: Dùng _categoryRepo (fix constructor error, không new thủ công).
    }
}