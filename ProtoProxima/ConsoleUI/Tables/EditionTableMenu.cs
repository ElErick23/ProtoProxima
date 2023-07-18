using ConsoleTools;
using ProtoProxima.ConsoleUI.Services;
using ProtoProxima.Core.Services;

namespace ProtoProxima.ConsoleUI.Tables;

public class EditionTableMenu<T> : TableMenu<T>
{
    public EditionTableMenu(ICore<T> core, MenuService menuService) : base(core, menuService)
    {
    }
    
    protected override Action<ConsoleMenu> GetAction(T element)
    {
        return menu =>
        {
            MenuService.NewEditionMenu(element).SetParent(this).Show();
            CloseMenu();
            new EditionTableMenu<T>(Core, MenuService).SetParent(parent!).Show();
        };
    }

    protected override void AddButtons()
    {
        Add('S', "Set Done", m =>
        {
            var element = GetCurrentElement();
            element!.GetType().GetProperty("Done")!.SetValue(element, true);
            Core.Update(element).Wait();
            RefreshItems();
        }, ConsoleColor.Cyan);
        
        Add('D', "Delete", m =>
        {
            var element = GetCurrentElement();
            if (!Core.Delete(element).Result) return;
            
            RefreshItems();
        }, ConsoleColor.Red);
    }
}