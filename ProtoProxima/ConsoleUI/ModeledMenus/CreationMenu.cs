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
        AddCustomItem("Save", menu => () =>
        {
            Console.WriteLine("Saving...");
            core.Save(Element).Wait();
            Element = Activator.CreateInstance<T>();
            Console.WriteLine("Saved! Press any key to continue...");
            Console.ReadKey();
            CloseMenu();
        }, itemConfig =>
        {
            itemConfig.ItemForegroundColor = ConsoleColor.Green;
            itemConfig.ItemBackgroundColor = ConsoleColor.Black;
            itemConfig.SelectedItemForegroundColor = ConsoleColor.Black;
            itemConfig.SelectedItemBackgroundColor = ConsoleColor.Green;
        });
        
        AddCustomItem("Clear", menu => () =>
        {
            Console.WriteLine("Clearing...");
            Element = Activator.CreateInstance<T>();
            CloseMenu();
            new CreationMenu<T>(core, element, args, level).Show();
        }, itemConfig =>
        {
            itemConfig.ItemForegroundColor = ConsoleColor.DarkCyan;
            itemConfig.ItemBackgroundColor = ConsoleColor.Black;
            itemConfig.SelectedItemForegroundColor = ConsoleColor.Black;
            itemConfig.SelectedItemBackgroundColor = ConsoleColor.DarkCyan;
        });

        Add("Back", Close);
        
        Configure(config => config.Title = $"[Creating {typeof(T).Name}]");
    }
}