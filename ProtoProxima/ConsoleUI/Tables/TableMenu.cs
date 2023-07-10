using MongoDB.Driver;
using ProtoProxima.ConsoleUI.Services;
using ProtoProxima.Core.Services;

namespace ProtoProxima.ConsoleUI.Tables;

public class TableMenu<T> : CustomMenu
{
    public TableMenu(
        ICore<T> core,
        MenuService menuService,
        string[] args,
        int level = 0)
        : base(args, level)
    {
        var data = core.GetList(Builders<T>.Filter.Empty).Result;
        var table = new ModeledTable<T>().Populate(data);
        var (header, tableRows, footer) = table.GetTableParts();

        for (var i = 0; i < data.Count; i++)
        {
            var row = tableRows[i];
            var element = data[i];
            Add(row, () =>
            {
                menuService.NewEditionMenu(element, args, level + 1).SetParent(this).Show();
                CloseMenu();
                new TableMenu<T>(core, menuService, args, level).SetParent(parent!).Show();
            });
        }

        //TODO: Add pagination
        //TODO: Add search
        //TODO: Add sorting
        //TODO: Add filtering
        //TODO: Set done just for activities
        Add('S', "Set Done", m =>
        {
            var element = data[CurrentItem.Index];
            element!.GetType().GetProperty("Done")!.SetValue(element, true);
            core.Update(element).Wait();
            m.CloseMenu();
            new TableMenu<T>(core, menuService, args, level).SetParent(parent!).Show();
        }, ConsoleColor.Cyan);
        
        Add('D', "Delete", m =>
        {
            var element = data[CurrentItem.Index];
            if (!core.Delete(element).Result) return;
            
            CloseMenu();
            menuService.NewTableMenu<T>(args, level).SetParent(parent!).Show();
        }, ConsoleColor.Red);
        
        Add('B', "Back", m => m.CloseMenu());

        Configure(config =>
        {
            config.Title = $"[Select {typeof(T).Name}]";
            config.WriteHeaderAction = () =>
            {
                if (data.Count > 0)
                {
                    Console.WriteLine();
                    Console.Write(header);
                }
                else
                    Console.WriteLine("No items found.");
            };
            config.WriteItemAction = item =>
            {
                Console.Write(item.Index < data.Count ? $"[{item.Index}] {item.Name}" : $"{item.Name}");
                if (item.Index != data.Count - 1) return;

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(footer);
            };
        });
    }
}