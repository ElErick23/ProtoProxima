using MongoDB.Driver;
using ProtoProxima.ConsoleUI.ModeledMenus;
using ProtoProxima.Services;

namespace ProtoProxima.ConsoleUI.Tables;

public class TableMenu<T> : CustomMenu
{
    public TableMenu(MongoDbService<T> service, string[] args, int level = 0) : base(args, level)
    {
        var data = service.GetListAsync(Builders<T>.Filter.Empty).Result;
        var table = new ModeledTable<T>().Populate(data);
        var (header, tableRows, footer) = table.GetTableParts();

        for (var i = 0; i < data.Count; i++)
        {
            var row = tableRows[i];
            var element = data[i];
            Add(row, () =>
            {
                EditionMenu<T>.Build(element, service, args, level + 1).SetParent(this).Show();
                CloseMenu();
                new TableMenu<T>(service, args, level).SetParent(parent!).Show();
            });
        }
        Add("Back", Close);

        Configure(config =>
        {
            config.Title = $"[Select {typeof(T).Name}]";
            config.Selector = "--> ";
            config.EnableBreadcrumb = true;
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
                Console.Write(item.Index >= data.Count ? $"{item.Name}" : $"[{item.Index}] {item.Name}");
                if (item.Index != data.Count - 1) return;

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(footer);
            };
        });
    }


}