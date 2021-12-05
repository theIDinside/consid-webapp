using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc.Repository.Interfaces;
using webapp.mvc.DataAccessLayer;
using webapp.mvc.Models;
using webapp.mvc.Models.ViewModels;

namespace webapp.mvc.Repository;

public class CategoryRepository : ConsidRepository<Category>, ICategoryRepository {
    public CategoryRepository(ApplicationDbContext contextReference) : base(contextReference) {

    }

    public IQueryable<Category> GetAllFilterBy(string? filter) {
        var items = from i in ctx.categoryItems select i;
        items = String.IsNullOrWhiteSpace(filter) ? items : items.Where(item => item.CategoryName.Contains(filter));
        return items;
    }
}