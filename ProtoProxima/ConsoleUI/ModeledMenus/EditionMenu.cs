using System.Reflection;
using ProtoProxima.Services;

namespace ProtoProxima.ConsoleUI.ModeledMenus;

public class EditionMenu<T> : ModeledMenu<T>
{
    protected EditionMenu(
        T? element,
        Func<PropertyInfo, ModeledMenu<T>, Action> propsAction,
        List<CustomItem> actionItems,
        string[] args,
        int level = 0
    ) : base(element, propsAction, actionItems, args, level)
    {
        Configure(config => config.Title = $"[Editing {typeof(T).Name}]");
    }

    public static EditionMenu<T> Build(
        T? element,
        MongoDbService<T> service,
        string[] args,
        int level = 0
    )
    {
        Action PropsAction(PropertyInfo prop, ModeledMenu<T> menu)
        {
            return () =>
            {
                var propTypeName = prop.PropertyType.Name;
                if (prop.PropertyType.IsGenericType &&
                    prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propTypeName = prop.PropertyType.GetGenericArguments()[0].Name;

                Console.WriteLine($"Update {prop.Name} ({propTypeName}):");
                var value = Console.ReadLine();
                if (string.IsNullOrEmpty(value)) return;

                prop.SetValue(menu.Element, ParseValue(value, prop.PropertyType));
                menu.CloseMenu();
                Build(menu.Element, service, args, level).Show();
                //element is the instance at is was created
                //menu.Element is the instance traveling all the iterations
            };
        }

        var actionItems = new List<CustomItem>
        {
            new("Update", menu => () =>
            {
                Console.WriteLine("Updating...");
                var id = typeof(T).GetProperties()[0].GetValue(menu.Element);
                service.UpdateAsync(id!.ToString()!, menu.Element).Wait();
                Console.WriteLine("Updated! Press any key to continue...");
                Console.ReadKey();
                menu.CloseMenu();
            }, itemConfig =>
            {
                itemConfig.ItemForegroundColor = ConsoleColor.Magenta;
                itemConfig.ItemBackgroundColor = ConsoleColor.Black;
                itemConfig.SelectedItemForegroundColor = ConsoleColor.Black;
                itemConfig.SelectedItemBackgroundColor = ConsoleColor.Magenta;
            }),
            new("Delete", menu => () =>
            {
                Console.WriteLine("Are you sure? (y/n)");
                var key = Console.ReadLine();
                if (key != "y") return;

                Console.WriteLine("Deleting...");
                var id = typeof(T).GetProperties()[0].GetValue(menu.Element);
                service.DeleteAsync(id!.ToString()!).Wait();
                Console.WriteLine("Deleted! Press any key to continue...");
                Console.ReadKey();
                menu.CloseMenu();
            }, itemConfig =>
            {
                itemConfig.ItemForegroundColor = ConsoleColor.Red;
                itemConfig.ItemBackgroundColor = ConsoleColor.Black;
                itemConfig.SelectedItemForegroundColor = ConsoleColor.Black;
                itemConfig.SelectedItemBackgroundColor = ConsoleColor.Red;
            })
        };

        return new EditionMenu<T>(element, PropsAction, actionItems, args, level);
    }
}