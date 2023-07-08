using ProtoProxima.Core.Services;
namespace ProtoProxima.ConsoleUI.ModeledMenus;

public class EditionMenu<T> : ModeledMenu<T>
{
    public EditionMenu(
        ICore<T> core,
        T? element,
        string[] args,
        int level = 0
    ) : base(core, element, args, level)
    {
        AddCustomItem("Update", menu => () =>
        {
            Console.WriteLine("Updating...");
            core.Update(Element).Wait();
            Console.WriteLine("Updated! Press any key to continue...");
            Console.ReadKey();
            CloseMenu();
        }, itemConfig =>
        {
            itemConfig.ItemForegroundColor = ConsoleColor.Magenta;
            itemConfig.ItemBackgroundColor = ConsoleColor.Black;
            itemConfig.SelectedItemForegroundColor = ConsoleColor.Black;
            itemConfig.SelectedItemBackgroundColor = ConsoleColor.Magenta;
        });

        AddCustomItem("Delete", menu => () =>
        {
            Console.WriteLine("Are you sure? (y/n)");
            var key = Console.ReadLine();
            if (key != "y") return;

            Console.WriteLine("Deleting...");
            core.Delete(Element).Wait();
            Console.WriteLine("Deleted! Press any key to continue...");
            Console.ReadKey();
            CloseMenu();
        }, itemConfig =>
        {
            itemConfig.ItemForegroundColor = ConsoleColor.Red;
            itemConfig.ItemBackgroundColor = ConsoleColor.Black;
            itemConfig.SelectedItemForegroundColor = ConsoleColor.Black;
            itemConfig.SelectedItemBackgroundColor = ConsoleColor.Red;
        });
        
        Add("Back", Close);
        
        Configure(config => config.Title = $"[Editing {typeof(T).Name}]");
    }

}