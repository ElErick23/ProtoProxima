using ConsoleTools;
using ProtoProxima.ConsoleUI.Services;
using ProtoProxima.Core.Services;

namespace ProtoProxima.ConsoleUI.Tables;

public class EditionTableMenu<T> : TableMenu<T>
{
    public EditionTableMenu(
        ICore<T> core, 
        MenuService menuService, 
        string[] args, 
        int level = 0
        ) : base(core, menuService, args, level)
    {
    }
    
    protected override Action<ConsoleMenu> GetAction(T element, string[] args)
    {
        return menu =>
        {
            MenuService.NewEditionMenu(element, args).SetParent(this).Show();
            CloseMenu();
            new EditionTableMenu<T>(Core, MenuService, args).SetParent(parent!).Show();
        };
    }

    protected override void AddButtons(string[] args)
    {
        Add('S', "Set Done", m =>
        {
            var element = GetCurrentElement();
            element!.GetType().GetProperty("Done")!.SetValue(element, true);
            Core.Update(element).Wait();
            m.CloseMenu();
            new EditionTableMenu<T>(Core, MenuService, args).SetParent(parent!).Show();
        }, ConsoleColor.Cyan);
        
        Add('D', "Delete", m =>
        {
            var element = GetCurrentElement();
            if (!Core.Delete(element).Result) return;
            
            CloseMenu();
            MenuService.NewEditionTableMenu<T>(args).SetParent(parent!).Show();
        }, ConsoleColor.Red);
    }
}