using System.Reflection;
using ConsoleTools;
using MongoDB.Bson.Serialization.Attributes;
using ProtoProxima.Models;

namespace ProtoProxima.ConsoleUI.ModeledMenus;

public class ModeledMenu<T> : CustomMenu
{
    public T Element;
    private readonly List<PropertyInfo> _props;

    protected struct CustomItem
    {
        public string Name { get; set; }
        public Func<ModeledMenu<T>, Action> Action { get; set; }
        public Action<MenuItem>? ItemConfig { get; set; }

        public CustomItem(string name, Func<ModeledMenu<T>, Action> action, Action<MenuItem>? itemConfig = null)
        {
            Name = name;
            Action = action;
            ItemConfig = itemConfig;
        }
    }

    protected ModeledMenu(
        T? element,
        Func<PropertyInfo, ModeledMenu<T>, Action> propsAction,
        List<CustomItem> actionItems,
        string[] args,
        int level = 0
    ) : base(args, level)
    {
        Element = element ?? Activator.CreateInstance<T>();
        _props = GetProperties();
        _props.ForEach(prop =>
            Add($"{prop.Name,-30}{DisplayableValue(prop.GetValue(Element))}", propsAction(prop, this)));
        //propAction returns in this moment the future action to be executed when the property is selected

        actionItems.ForEach(item => Add(item.Name, item.Action(this), item.ItemConfig));
        Add("Back", Close);
        Configure(config =>
        {
            config.WriteHeaderAction = Console.WriteLine;
            config.EnableBreadcrumb = true;
            config.Selector = "--> ";
            config.WriteItemAction = item =>
            {
                if (item.Index < _props.Count)
                    Console.Write("[{0}] {1}", item.Index, item.Name);
                if (item.Index == _props.Count - 1)
                    Console.WriteLine();
                if (item.Index >= _props.Count)
                    Console.Write(item.Name);
            };
        });
    }

    private static List<PropertyInfo> GetProperties()
    {
        var type = typeof(T);
        var properties = type.GetProperties().ToList();
        return properties.FindAll(prop =>
        {
            var attributes = prop.GetCustomAttributes().ToArray();
            if (attributes.Any(a => a is BsonIdAttribute))
                return false;
            
            return !attributes.Any(a =>
                a is HiddenAttribute hidden
                && hidden.MenuClasses.Contains(typeof(ModeledMenu<>))
            );
        });
    }

    protected static object? ParseValue(string value, Type type)
    {
        if (type == typeof(TimeSpan))
            return TimeSpan.TryParse(value, out var timeSpan) ? timeSpan : TimeSpan.Zero;
        if (type == typeof(TimeSpan?))
            return TimeSpan.TryParse(value, out var timeSpan) ? timeSpan : null;
        if (type == typeof(DateTime))
            return DateTime.TryParse(value, out var dateTime) ? dateTime : DateTime.Now;
        if (type == typeof(Category))
            return !string.IsNullOrWhiteSpace(value) ? new Category {Name = value} : null;
        if (type == typeof(bool))
            return value.ToLower() switch
            {
                "yes" => true,
                "no" => false,
                _ => bool.TryParse(value, out var boolean) ? boolean : null
            };

        return Convert.ChangeType(value, type);
    }

    public static string DisplayableValue(object? value)
    {
        if (value == null)
            return "---";

        var type = value.GetType();
        if (type == typeof(DateTime))
            return ((DateTime)value).ToString("dd/MMM/yyyy HH:mm");
        if (type == typeof(TimeSpan))
            return IParser.Humanize((TimeSpan) value);
        if (type == typeof(bool))
            return value is true ? "Yes" : "No";

        return value.ToString() ?? "---";
    }
}