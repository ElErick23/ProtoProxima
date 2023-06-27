using ConsoleTools;

namespace ProtoProxima.ConsoleUI;

public class CustomMenu : ConsoleMenu
{

    public CustomMenu(string[] args, int level = 0) : base(args, level)
    {
    }
    
    public CustomMenu SetParent(ConsoleMenu menu)
    {
        parent = menu;
        return this;
    }
    

    public CustomMenu Add(string name, Action action, Action<MenuItem>? itemConfig = null)
    {
        var menu = (CustomMenu) base.Add(name, action);
        itemConfig?.Invoke(menu.Items[^1]);
        return menu;
    }

    public CustomMenu Add(string name, Func<CancellationToken, Task> action, Action<MenuItem>? itemConfig = null)
    {
        var menu = (CustomMenu) base.Add(name, action);
        itemConfig?.Invoke(menu.Items[^1]);
        return menu;
    }

    public CustomMenu Add(string name, Action<ConsoleMenu> action, Action<MenuItem>? itemConfig = null)
    {
        var menu = (CustomMenu) base.Add(name, action);
        itemConfig?.Invoke(menu.Items[^1]);
        return menu;
    }

    public CustomMenu Add(string name, Func<ConsoleMenu, CancellationToken, Task> action, Action<MenuItem>? itemConfig = null)
    {
        var menu = (CustomMenu) base.Add(name, action);
        itemConfig?.Invoke(menu.Items[^1]);
        return menu;
    }
    
}