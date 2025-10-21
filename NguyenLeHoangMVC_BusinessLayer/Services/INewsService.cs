// BusinessLayer/Services/INewsService.cs
using NguyenLeHoangMVC_DataLayer.Models;

namespace NguyenLeHoangMVC_BusinessLayer.Services;

public interface INewsService {
    Task<IEnumerable<NewsArticle>> GetActiveNewsAsync();  // Query news active (NewsStatus=true), dùng LINQ.
    Task<IEnumerable<NewsArticle>> GetAllNewsAsync();  // CHANGE: Thêm cho CRUD (tất cả news, Staff xem).
    Task<NewsArticle> GetByIdAsync(string id);  // CHANGE: Load by PK cho Edit.
    Task<NewsArticle> CreateNewsAsync(NewsArticle model);  // CHANGE: Insert mới (validation trong Controller).
    Task<NewsArticle> UpdateNewsAsync(NewsArticle model);  // CHANGE: Update (set ModifiedDate).
    Task DeleteNewsAsync(string id);  // CHANGE: Delete (check FK nếu cần).
    Task<IEnumerable<Category>> GetActiveCategoriesAsync();  // CHANGE: Load dropdown Category active.
}