using ConsoleTools;
using MongoDB.Driver;
using ProtoProxima.ConsoleUI.Services;
using ProtoProxima.Core.Services;

namespace ProtoProxima.ConsoleUI.Tables;

public abstract class TableMenu<T> : CustomMenu
{
    protected readonly ICore<T> Core;
    protected readonly MenuService MenuService;
    protected readonly List<T> Data;
    
    protected TableMenu(
        ICore<T> core,
        MenuService menuService,
        string[] args,
        int level = 0)
        : base(args, level)
    {
        Core = core;
        MenuService = menuService;
        Data = core.GetList(Builders<T>.Filter.Empty).Result;
        
        var table = new ModeledTable<T>().Populate(Data);
        var (header, tableRows, footer) = table.GetTableParts();
        
        for (var i = 0; i < Data.Count; i++)
        {
            var row = tableRows[i];
            var element = Data[i];
            Add(row, GetAction(element, args));
        }

        //TODO: Add pagination
        //TODO: Add search
        //TODO: Add sorting
        //TODO: Add filtering
        //TODO: Set done just for activities
        AddButtons(args);

        Add('B', "Back", m => m.CloseMenu());

        Configure(config =>
        {
            config.Title = $"[Select {typeof(T).Name}]";
            config.WriteHeaderAction = () =>
            {
                if (Data.Count > 0)
                {
                    Console.WriteLine();
                    Console.Write(header);
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
                Console.Write(footer);
            };
        });
    }

    protected abstract Action<ConsoleMenu> GetAction(T element, string[] args);

    protected abstract void AddButtons(string[] args);

    public T GetCurrentElement()
    {
        return Data[CurrentItem.Index];
    }
}