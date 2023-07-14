using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProtoProxima.MongoDB.Models;

namespace ProtoProxima.MongoDB.Services;

public class ActivityService : MongoDbService<Activity>
{
    public ActivityService(IOptions<MongoDBSettings> mongoDbSettings) : base(mongoDbSettings, "Activities")
    {
    }

    public new async Task<List<Activity>> GetListAsync(FilterDefinition<Activity> filter)
    {
        var list = await base.GetListAsync(filter);
        list.ForEach(ac => ac.Due = ac.Due.ToLocalTime());
        return list;
    }
}