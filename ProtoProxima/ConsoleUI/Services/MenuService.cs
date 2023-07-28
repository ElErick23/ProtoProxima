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

    public CreationMenu<T> NewCreationMenu<T>(T? element)
    {
        return CastMenu(new CreationMenu<T>(GetCore<T>(), this, element));
    }

    public EditionMenu<T> NewEditionMenu<T>(T? element)
    {
        return CastMenu(new EditionMenu<T>(GetCore<T>(), this, element));
    }

    public SelectionTableMenu<T> NewDefaultTableMenu<T>()
    {
        return CastMenu(new SelectionTableMenu<T>(GetCore<T>(), this));
    }

    public EditionTableMenu<T> NewEditionTableMenu<T>()
    {
        return CastMenu(new EditionTableMenu<T>(GetCore<T>(), this));
    }

    private static T CastMenu<T>(T menu) where T : CustomMenu => (T)menu.SetUp();

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