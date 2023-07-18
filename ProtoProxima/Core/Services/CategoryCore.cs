using MongoDB.Driver;
using ProtoProxima.MongoDB.Models;
using ProtoProxima.MongoDB.Services;

namespace ProtoProxima.Core.Services;

public class CategoryCore : ICore<Category>
{
    private readonly CategoryService _categoryService;
    private readonly ActivityCore _activityCore;

    public CategoryCore(CategoryService categoryService, ActivityCore activityCore)
    {
        _categoryService = categoryService;
        _activityCore = activityCore;
    }

    public async Task Save(Category element)
    {
        await _categoryService.CreateAsync(element);
    }

    public async Task Update(Category element)
    {
        if (element.Id == null)
            throw new Exception("Category has no Id, can't update.");

        await _categoryService.UpdateAsync(element.Id, element);
        await _activityCore.UpdateCategory(element);
    }

    public async Task<bool> Delete(Category element)
    {
        if (element.Id == null)
            throw new Exception("Category has no Id, can't delete.");

        Console.WriteLine("Are you sure? (y/n)");
        var key = Console.ReadLine()?.ToLower();
        if (key != "y") return false;

        Console.WriteLine("Deleting...");
        await _categoryService.DeleteAsync(element.Id);
        Console.WriteLine("Deleted! Press any key to continue...");
        Console.ReadKey();
        return true;
    }

    public async Task<List<Category>> GetList(FilterDefinition<Category> filter)
    {
        return await _categoryService.GetListAsync(filter);
    }
}