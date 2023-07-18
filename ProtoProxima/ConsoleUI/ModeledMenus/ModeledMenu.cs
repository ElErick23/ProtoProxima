using System.Reflection;
using MongoDB.Bson.Serialization.Attributes;
using ProtoProxima.ConsoleUI.Services;
using ProtoProxima.Core.Services;
using ProtoProxima.MongoDB.Models;

namespace ProtoProxima.ConsoleUI.ModeledMenus;

public abstract class ModeledMenu<T> : CustomMenu
{
    protected T Element;
    protected readonly ICore<T> Core;
    private readonly MenuService _menuService;
    private readonly List<PropertyInfo> _props;

    protected ModeledMenu(ICore<T> core, MenuService menuService, T? element)
    {
        Element = element ?? Activator.CreateInstance<T>();
        Core = core;
        _menuService = menuService;
        _props = GetProperties();
        _props.ForEach(prop => Add(prop.Name, () =>
        {
            var propType = prop.PropertyType;
            if (propType == typeof(Category))
            {
                var categoryMenu = menuService.NewDefaultTableMenu<Category>();
                categoryMenu.SetParent(this)
                    .Show();
                var category = categoryMenu.GetSelectedElement();
                if (category == null) return;
                prop.SetValue(Element, category);
            }
            else
            {
                var propTypeName = propType.Name;
                if (prop.PropertyType.IsGenericType &&
                    prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propTypeName = prop.PropertyType.GetGenericArguments()[0].Name;

                Console.WriteLine($"Set {prop.Name} ({propTypeName}):");
                var value = Console.ReadLine();
                if (string.IsNullOrEmpty(value)) return;
                prop.SetValue(Element, ParseValue(value, prop.PropertyType));
            }
        }));

        Configure(config =>
        {
            config.WriteHeaderAction = Console.WriteLine;
            config.WriteItemAction = item =>
            {
                var i = item.Index;
                if (i < _props.Count)
                    Console.Write("[{0}] {1}{2}", i, item.Name.PadRight(20),
                        DisplayableValue(_props[i].GetValue(Element)));
                if (i == _props.Count - 1)
                    Console.WriteLine();
                if (i >= _props.Count)
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
            return !string.IsNullOrWhiteSpace(value) ? new Category { Name = value } : null;
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
            return IParser.Humanize((TimeSpan)value);
        if (type == typeof(bool))
            return value is true ? "Yes" : "No";

        return value.ToString() ?? "---";
    }
}