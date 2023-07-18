using ConsoleTools;
using ProtoProxima.ConsoleUI.ModeledMenus;
using ProtoProxima.ConsoleUI.Tables;
using ProtoProxima.Core.Services;
using ProtoProxima.MongoDB.Models;

namespace ProtoProxima.ConsoleUI.Services;

public class MenuService
{
    private readonly ActivityCore _activityCore;
    private readonly CategoryCore _categoryCore;

    public MenuService(ActivityCore activityCore, CategoryCore categoryCore)
    {
        _activityCore = activityCore;
        _categoryCore = categoryCore;
    }

    public CreationMenu<T> NewCreationMenu<T>(T? element) => new(GetCore<T>(), this, element);

    public EditionMenu<T> NewEditionMenu<T>(T? element) => new(GetCore<T>(), this, element);

    public DefaultTableMenu<T> NewDefaultTableMenu<T>() => new(GetCore<T>(), this);
    
    public EditionTableMenu<T> NewEditionTableMenu<T>() => new(GetCore<T>(), this);

    private ICore<T> GetCore<T>()
    {
        var type = typeof(T);
        if (type == typeof(Activity))
            return (ICore<T>)_activityCore;
        if (type == typeof(Category))
            return (ICore<T>)_categoryCore;

        throw new Exception($"No core found for type {type.Name}");
    }
}