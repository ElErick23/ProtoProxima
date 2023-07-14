using Microsoft.Extensions.Options;
using ProtoProxima.MongoDB.Models;

namespace ProtoProxima.MongoDB.Services;

public class CategoryService : MongoDbService<Category>
{
    public CategoryService(IOptions<MongoDBSettings> mongoDbSettings) : base(mongoDbSettings, "Categories")
    {
    }
}