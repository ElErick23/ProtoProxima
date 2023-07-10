
using MongoDB.Driver;

namespace ProtoProxima.Core.Services;

public interface ICore<T>
{ 
    Task Save(T element);
    
    Task Update(T element);

    Task<bool> Delete(T element);

    Task<List<T>> GetList(FilterDefinition<T> filter);
}