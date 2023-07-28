using ConsoleTools;
using ProtoProxima.ConsoleUI.Services;
using ProtoProxima.Core.Services;

namespace ProtoProxima.ConsoleUI.ModeledMenus;

public class EditionMenu<T> : ModeledMenu<T>
{
    public EditionMenu(ICore<T> core, MenuService menuService, T? element) : base(core, menuService, element)
    {
        Configure(config => config.Title = $"[Editing {typeof(T).Name}]");
    }

    protected override IEnumerable<ButtonBody> GetButtons()
    {
        return new List<ButtonBody>
        {
            new()
            {
                Key = 'U',
                Name = "Update",
                Action = () =>
                {
                    Console.WriteLine("Updating...");
                    Core.Update(Element).Wait();
                    Console.WriteLine("Updated! Press any key to continue...");
                    Console.ReadKey();
                    CloseMenu();
                },
                ForegroundColor = ConsoleColor.Magenta
            },
            new()
            {
                Key = 'D',
                Name = "Delete",
                Action = () =>
                {
                    if (Core.Delete(Element).Result)
                        CloseMenu();
                },
                ForegroundColor = ConsoleColor.Red
            }
        };
    }
}