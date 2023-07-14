using System.Reflection;
using MongoDB.Driver;
using ProtoProxima.MongoDB.Models;
using ProtoProxima.MongoDB.Services;

namespace ProtoProxima.Core.Services;

public class CategoryCore : ICore<Category>
{
    private readonly CategoryService _categoryService;

    public CategoryCore(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public Task Save(Category element)
    {
        throw new NotImplementedException();
    }

    public Task Update(Category element)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(Category element)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Category>> GetList(FilterDefinition<Category> filter)
    {
        return await _categoryService.GetListAsync(filter);
    }

    public string? GetProperty(PropertyInfo propertyInfo)
    {
        throw new NotImplementedException();
    }
}