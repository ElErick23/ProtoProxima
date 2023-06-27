using System.Reflection;
using ProtoProxima.Services;

namespace ProtoProxima.ConsoleUI.ModeledMenus;

public class CreationMenu<T> : ModeledMenu<T>
{
    private CreationMenu(
        T? element,
        Func<PropertyInfo, ModeledMenu<T>, Action> propAction,
        List<CustomItem> menuActions,
        string[] args,
        int level = 0
    ) : base(element, propAction, menuActions, args, level)
    {
        Configure(config => config.Title = $"[Creating {typeof(T).Name}]");
    }

    public static CreationMenu<T> Build(
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
                
                Console.WriteLine($"Set {prop.Name} ({propTypeName}):");
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
            new("Save", menu => () =>
            {
                Console.WriteLine("Saving...");
                service.CreateAsync(menu.Element).Wait();
                Console.WriteLine("Saved! Press any key to continue...");
                Console.ReadKey();
                menu.CloseMenu();
            }, itemConfig =>
            {
                itemConfig.ItemForegroundColor = ConsoleColor.Green;
                itemConfig.ItemBackgroundColor = ConsoleColor.Black;
                itemConfig.SelectedItemForegroundColor = ConsoleColor.Black;
                itemConfig.SelectedItemBackgroundColor = ConsoleColor.Green;
            }),
            new("Clear", menu => () =>
            {
                Console.WriteLine("Clearing...");
                menu.Element = Activator.CreateInstance<T>();
                menu.CloseMenu();
                Build(menu.Element, service, args, level).Show();
            }, itemConfig =>
            {
                itemConfig.ItemForegroundColor = ConsoleColor.DarkCyan;
                itemConfig.ItemBackgroundColor = ConsoleColor.Black;
                itemConfig.SelectedItemForegroundColor = ConsoleColor.Black;
                itemConfig.SelectedItemBackgroundColor = ConsoleColor.DarkCyan;
            })
        };

        return new CreationMenu<T>(element, PropsAction, actionItems, args, level);
    }
}