using ConsoleTools;

namespace ProtoProxima.ConsoleUI;

public abstract class CustomMenu : ConsoleMenu
{
    protected CustomMenu()
    {
        Configure(config =>
        {
            config.Selector = "--> ";
            config.EnableBreadcrumb = true;
        });
    }

    public CustomMenu SetUp()
    {
        AddRange(GetItems());
        AddRange(GetButtons());
        Add((char)ConsoleKey.Backspace, "Back", CloseMenu);
        return this;
    }
    public CustomMenu SetParent(ConsoleMenu menu)
    {
        parent = menu;
        return this;
    }
    
    protected void RefreshItems()
    {
        ClearItems();
        AddRange(GetItems());
    }
    
    protected abstract IEnumerable<ItemBody> GetItems();
    
    protected abstract IEnumerable<ButtonBody> GetButtons();
    
}