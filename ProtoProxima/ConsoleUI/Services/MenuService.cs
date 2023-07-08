using ProtoProxima.ConsoleUI.ModeledMenus;
using ProtoProxima.ConsoleUI.Tables;
using ProtoProxima.Core.Services;
using ProtoProxima.MongoDB.Models;

namespace ProtoProxima.ConsoleUI.Services;

public class MenuService
{
    private readonly ActivityCore _activityCore;

    public MenuService(ActivityCore activityCore)
    {
        _activityCore = activityCore;
    }

    public CreationMenu<T> NewCreationMenu<T>(
        T? element,
        string[] args,
        int level = 0
    ) => new(GetCore<T>(), element, args, level);

    public EditionMenu<T> NewEditionMenu<T>(
        T? element,
        string[] args,
        int level = 0
    ) => new(GetCore<T>(), element, args, level);

    public TableMenu<T> NewTableMenu<T>(
        string[] args,
        int level = 0
    ) => new(GetCore<T>(), this, args, level);

    private ICore<T> GetCore<T>()
    {
        var type = typeof(T);
        if (type == typeof(Activity))
            return (ICore<T>)_activityCore;

        throw new Exception($"No core found for type {type.Name}");
    }
}