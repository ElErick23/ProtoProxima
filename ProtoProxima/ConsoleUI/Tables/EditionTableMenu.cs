using ConsoleTools;
using MongoDB.Driver;
using ProtoProxima.ConsoleUI.Services;
using ProtoProxima.Core.Services;
using ProtoProxima.MongoDB.Models;

namespace ProtoProxima.ConsoleUI.Tables;

public class EditionTableMenu<T> : TableMenu<T>
{
    public EditionTableMenu(ICore<T> core, MenuService menuService) : base(core, menuService)
    {
    }

    protected override IEnumerable<ItemBody> GetItems()
    {
        var items = new List<ItemBody>();
        Data = Core.GetList(Builders<T>.Filter.Empty).Result;
        Table = new ModeledTable<T>(Data);
        for (var i = 0; i < Data.Count; i++)
        {
            var row = Table.Options[i];
            var element = Data[i];
            items.Add(new ItemBody
            {
                Name = row,
                Action = () =>
                {
                    MenuService.NewEditionMenu(element).SetParent(this).Show();
                    RefreshItems();
                }
            });
        }

        return items;
    }

    protected override IEnumerable<ButtonBody> GetButtons()
    {
        var buttons = new List<ButtonBody>
        {
            new()
            {
                Key = 'D',
                Name = "Delete",
                Action = () =>
                {
                    var element = GetCurrentElement();
                    if (!Core.Delete(element).Result) return;

                    RefreshItems();
                },
                ForegroundColor = ConsoleColor.Red
            }
        };

        if (typeof(T) == typeof(Activity))
            buttons.Add(new ButtonBody
            {
                Key = 'M',
                Name = "Mark Done",
                Action = () =>
                {
                    var element = GetCurrentElement();
                    element!.GetType().GetProperty("Done")!.SetValue(element, true);
                    Core.Update(element).Wait();
                    RefreshItems();
                }
            });

        return base.GetButtons().Concat(buttons);
    }
}