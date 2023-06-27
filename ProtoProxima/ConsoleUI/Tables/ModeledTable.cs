using BetterConsoles.Tables;
using BetterConsoles.Tables.Configuration;
using MongoDB.Bson.Serialization.Attributes;

namespace ProtoProxima.ConsoleUI.Tables;

public class ModeledTable<T> : Table
{
    public ModeledTable() : base(TableConfig.Unicode(), GetPropertyNames())
    {
    }

    public ModeledTable<T> Populate(List<T> data)
    {
        foreach (var element in data)
            AddRow(GetPropertyValues(element));
        return this;
    }

    public (string, string[], string) GetTableParts()
    {
        var rowStrings = ToString().Split("\n");
        var header = "".PadRight(8) + rowStrings[0] + "".PadRight(8) + rowStrings[1] + "\n".PadRight(9) + rowStrings[2]; 
        var options = rowStrings[3..^2];
        var footer = "\n".PadRight(9) + rowStrings[^2];
        return (header, options, footer);
    }

    private static string[] GetPropertyNames()
    {
        //Return all the property names that are not the Id
        return (from prop in typeof(T).GetProperties()
            where prop.CustomAttributes.All(a => a.AttributeType != typeof(BsonIdAttribute))
            select prop.Name).ToArray();
    }

    private static string[] GetPropertyValues(T instance)
    {
        //Return all the values of the properties that are not the Id
        return (from prop in typeof(T).GetProperties()
            where prop.CustomAttributes.All(a => a.AttributeType != typeof(BsonIdAttribute))
            select DisplayableValue(prop.GetValue(instance))).ToArray();
    }
    
    public static string DisplayableValue(object? value)
    {
        if (value == null)
            return "---";
        
        var type = value.GetType();
        if (type == typeof(DateTime))
            return ((DateTime) value).ToString("dd/MMM/yyyy HH:mm");
        if (type == typeof(TimeSpan))
            return IParser.Humanize((TimeSpan) value);
        if (type == typeof(bool))
            return value is true ? "✅" : "  ";
        
        return value.ToString() ?? "---";
    }
}