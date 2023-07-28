using ConsoleTools;
using MongoDB.Driver;
using ProtoProxima.ConsoleUI.Services;
using ProtoProxima.Core.Services;

namespace ProtoProxima.ConsoleUI.Tables;

public class SelectionTableMenu<T> : TableMenu<T>
{
    private int _selectedElementIndex = -1;

    public SelectionTableMenu(ICore<T> core, MenuService menuService) : base(core, menuService)
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
            // var element = Data[i];
            items.Add(new ItemBody
            {
                Name = row,
                Action = () =>
                {
                    _selectedElementIndex = CurrentItem.Index;
                    CloseMenu();
                }
            });
        }

        return items;
    }

    public T? GetSelectedElement()
    {
        return _selectedElementIndex != -1 ? Data[_selectedElementIndex] : default;
    }
}