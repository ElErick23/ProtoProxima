using ProtoProxima.Core.Services;

namespace ProtoProxima.ConsoleUI.ModeledMenus;

public class CreationMenu<T> : ModeledMenu<T>
{
    public CreationMenu(
        ICore<T> core,
        T? element,
        string[] args,
        int level = 0
    ) : base(core, element, args, level)
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
            new CreationMenu<T>(core, element, args, level).Show();
        });
        
        Add('B', "Back", m => m.CloseMenu());
        
        Configure(config => config.Title = $"[Creating {typeof(T).Name}]");
    }
}