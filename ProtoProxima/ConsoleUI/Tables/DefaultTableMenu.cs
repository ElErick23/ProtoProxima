using ConsoleTools;
using ProtoProxima.ConsoleUI.Services;
using ProtoProxima.Core.Services;

namespace ProtoProxima.ConsoleUI.Tables;

public class DefaultTableMenu<T> : TableMenu<T>
{
    private int _selectedElementIndex = -1;
    
    public DefaultTableMenu(ICore<T> core, MenuService menuService, string[] args, int level = 0) : base(core, menuService, args, level)
    {
    }

    protected override Action<ConsoleMenu> GetAction(T element, string[] args)
    {
        return menu =>
        {
            _selectedElementIndex = CurrentItem.Index;
            CloseMenu();
        };
    }

    protected override void AddButtons(string[] args)
    {
    }
    
    public T? GetSelectedElement()
    {
        
        return _selectedElementIndex != -1 ? Data[_selectedElementIndex] : default;
        
    }
}