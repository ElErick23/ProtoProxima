using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ProtoProxima.MongoDB.Services;

public abstract class MongoDbService<T>
{
    private readonly IMongoCollection<T> _modelCollection;

    public MongoDbService(IOptions<MongoDBSettings> mongoDbSettings, string collectionName)
    {
        var client = new MongoClient(mongoDbSettings.Value.ConnectionURI);
        var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _modelCollection = database.GetCollection<T>(collectionName);
    }

    public async Task<T> GetAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        return await _modelCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<T>> GetListAsync(FilterDefinition<T> filter)
    {
        return await _modelCollection.Find(filter).ToListAsync();
    }

    public async Task CreateAsync(T model)
    {
        await _modelCollection.InsertOneAsync(model);
    }

    public async Task UpdateAsync(string id, T model)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        await _modelCollection.ReplaceOneAsync(filter, model);
    }

    public async Task UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
        await _modelCollection.UpdateManyAsync(filter, update);
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        await _modelCollection.DeleteOneAsync(filter);
    }
}