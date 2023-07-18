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
    private ModeledTable<T> _table;
    
    protected TableMenu(ICore<T> core, MenuService menuService)
    {
        Core = core;
        MenuService = menuService;
        CreateItems();

        //TODO: Add pagination
        //TODO: Add search
        //TODO: Add sorting
        //TODO: Add filtering
        //TODO: Set done just for activities
        Add('C', "Create", m =>
        {
            MenuService.NewCreationMenu<T>(default).SetParent(this).Show();
            RefreshItems();
        });

        AddButtons();

        Add('B', "Back", m => m.CloseMenu());

        Configure(config =>
        {
            config.Title = $"[Select {typeof(T).Name}]";
            config.WriteHeaderAction = () =>
            {
                if (Data.Count > 0)
                {
                    Console.WriteLine();
                    Console.Write(_table.Header);
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
                Console.Write(_table.Footer);
            };
        });
    }

    private void CreateItems()
    {
        Data = Core.GetList(Builders<T>.Filter.Empty).Result;
        _table = new ModeledTable<T>(Data);
        for (var i = 0; i < Data.Count; i++)
        {
            var row = _table.Options[i];
            var element = Data[i];
            Add(row, GetAction(element));
        }
    }

    protected void RefreshItems()
    {
        ClearItems();
        CreateItems();
    }

    protected abstract Action<ConsoleMenu> GetAction(T element);

    protected abstract void AddButtons();

    protected T GetCurrentElement()
    {
        return Data[CurrentItem.Index];
    }
}