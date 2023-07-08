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

    public async Task Delete(Activity element)
    {
        if (element.Id == null) 
            throw new Exception("Activity has no Id, can't delete.");
        
        await _activityService.DeleteAsync(element.Id);
    }

    public async Task<List<Activity>> GetList(FilterDefinition<Activity> filter)
    {
        return await _activityService.GetListAsync(filter); 
    }
}