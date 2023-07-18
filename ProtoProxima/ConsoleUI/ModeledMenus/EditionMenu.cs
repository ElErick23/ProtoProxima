using ProtoProxima.ConsoleUI.Services;
using ProtoProxima.Core.Services;
namespace ProtoProxima.ConsoleUI.ModeledMenus;

public class EditionMenu<T> : ModeledMenu<T>
{
    public EditionMenu(ICore<T> core, MenuService menuService, T? element) : base(core, menuService, element)
    {
        Add('U',"Update", menu =>
        {
            Console.WriteLine("Updating...");
            core.Update(Element).Wait();
            Console.WriteLine("Updated! Press any key to continue...");
            Console.ReadKey();
            CloseMenu();
        }, ConsoleColor.Magenta);

        Add('D', "Delete", menu =>
        {
            if (core.Delete(Element).Result)
                CloseMenu();
        }, ConsoleColor.Red);

        Add('B', "Back", m => m.CloseMenu());
        
        Configure(config => config.Title = $"[Editing {typeof(T).Name}]");
    }

}