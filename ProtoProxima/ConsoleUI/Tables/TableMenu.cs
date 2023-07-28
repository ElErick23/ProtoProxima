using ConsoleTools;
using MongoDB.Driver;
using ProtoProxima.ConsoleUI.Services;
using ProtoProxima.Core.Services;

namespace ProtoProxima.ConsoleUI.Tables;

public abstract class TableMenu<T> : CustomMenu
{
    protected readonly ICore<T> Core;
    protected readonly MenuService MenuService;
    protected List<T> Data;
    protected ModeledTable<T> Table;

    protected TableMenu(ICore<T> core, MenuService menuService)
    {
        Core = core;
        MenuService = menuService;

        //TODO: Add pagination
        //TODO: Add search
        //TODO: Add sorting
        //TODO: Add filtering
        Configure(config =>
        {
            config.Title = $"[Select {typeof(T).Name}]";
            config.WriteHeaderAction = () =>
            {
                if (Data.Count > 0)
                {
                    Console.WriteLine();
                    Console.Write(Table.Header);
                }
                else
                    Console.WriteLine("No items found.");
            };
            config.WriteItemAction = item =>
            {
                Console.Write(item.Index < Data.Count ? $"[{item.Index}] {item.Name}" : $"{item.Name}");
                if (item.Index != Data.Count - 1) return;

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(Table.Footer);
            };
        });
    }

    protected override IEnumerable<ButtonBody> GetButtons() =>
        new List<ButtonBody>
        {
            new()
            {
                Key = 'C',
                Name = "Create",
                Action = () =>
                {
                    MenuService.NewCreationMenu<T>(default).SetParent(this).Show();
                    RefreshItems();
                },
                ForegroundColor = ConsoleColor.Green
            }
        };

    protected T GetCurrentElement()
    {
        return Data[CurrentItem.Index];
    }
}