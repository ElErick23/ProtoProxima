using ProtoProxima.ConsoleUI.Services;
using ProtoProxima.Core.Services;

namespace ProtoProxima.ConsoleUI.ModeledMenus;

public class CreationMenu<T> : ModeledMenu<T>
{
    public CreationMenu(ICore<T> core, MenuService menuService, T? element) : base(core, menuService, element)
    {
        Add('S', "Save", menu =>
        {
            Console.WriteLine("Saving...");
            core.Save(Element).Wait();
            Element = Activator.CreateInstance<T>();
            Console.WriteLine("Saved! Press any key to continue...");
            Console.ReadKey();
            CloseMenu();
        }, ConsoleColor.Green);

        Add('C', "Clear", menu =>
        {
            Console.WriteLine("Clearing...");
            Element = Activator.CreateInstance<T>();
            CloseMenu();
            new CreationMenu<T>(core, menuService, element)
                .SetParent(parent!)
                .Show();
        });
        
        Add('B', "Back", m => m.CloseMenu());

        Configure(config => config.Title = $"[Creating {typeof(T).Name}]");
    }
}