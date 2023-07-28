using ConsoleTools;
using ProtoProxima.ConsoleUI.Services;
using ProtoProxima.Core.Services;

namespace ProtoProxima.ConsoleUI.ModeledMenus;

public class CreationMenu<T> : ModeledMenu<T>
{
    public CreationMenu(ICore<T> core, MenuService menuService, T? element) : base(core, menuService, element)
    {
        Configure(config => config.Title = $"[Creating {typeof(T).Name}]");
    }

    protected override IEnumerable<ButtonBody> GetButtons()
    {
        return new List<ButtonBody>
        {
            new()
            {
                Key = 'S',
                Name = "Save",
                Action = () =>
                {
                    Console.WriteLine("Saving...");
                    Core.Save(Element).Wait();
                    Element = Activator.CreateInstance<T>();
                    Console.WriteLine("Saved! Press any key to continue...");
                    Console.ReadKey();
                    CloseMenu();
                },
                ForegroundColor = ConsoleColor.Green
            },
            new()
            {
                Key = 'C',
                Name = "Clear",
                Action = () =>
                {
                    Console.WriteLine("Clearing...");
                    Element = Activator.CreateInstance<T>();
                    RefreshItems();
                }
            }
        };
    }
}