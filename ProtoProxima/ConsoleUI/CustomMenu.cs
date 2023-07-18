using ConsoleTools;

namespace ProtoProxima.ConsoleUI;

public class CustomMenu : ConsoleMenu
{

    public CustomMenu()
    {
        Configure(config =>
        {
            config.Selector = "--> ";
            config.EnableBreadcrumb = true;
        });

    }
    
    public CustomMenu SetParent(ConsoleMenu menu)
    {
        parent = menu;
        return this;
    }
    
    
    
}