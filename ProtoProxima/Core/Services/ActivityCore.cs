using MongoDB.Driver;
using ProtoProxima.MongoDB.Models;
using ProtoProxima.MongoDB.Services;

namespace ProtoProxima.Core.Services;

public class ActivityCore : ICore<Activity>
{
    private readonly ActivityService _activityService;

    public ActivityCore(ActivityService activityService)
    {
        _activityService = activityService;
    }

    public async Task Save(Activity activity)
    {
        await _activityService.CreateAsync(activity);
    }

    public async Task Update(Activity element)
    {
        if (element.Id == null)
            throw new Exception("Activity has no Id, can't update.");

        await _activityService.UpdateAsync(element.Id, element);
    }

    public async Task UpdateCategory(Category category)
    {
        var filter = Builders<Activity>.Filter.Where(a => a.Category.Id == category.Id);
        var update = Builders<Activity>.Update.Set(a => a.Category, category);
        await _activityService.UpdateManyAsync(filter, update);
    }

    public async Task<bool> Delete(Activity element)
    {
        if (element.Id == null)
            throw new Exception("Activity has no Id, can't delete.");

        Console.WriteLine("Are you sure? (y/n)");
        var key = Console.ReadLine()?.ToLower();
        if (key != "y") return false;

        Console.WriteLine("Deleting...");
        await _activityService.DeleteAsync(element.Id);
        Console.WriteLine("Deleted! Press any key to continue...");
        Console.ReadKey();
        return true;
    }

    public async Task<List<Activity>> GetList(FilterDefinition<Activity> filter)
    {
        return await _activityService.GetListAsync(filter);
    }
}