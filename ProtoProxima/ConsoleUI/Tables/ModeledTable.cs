using BetterConsoles.Tables;
using BetterConsoles.Tables.Configuration;
using MongoDB.Bson.Serialization.Attributes;

namespace ProtoProxima.ConsoleUI.Tables;

public class ModeledTable<T> : Table
{
    private readonly string[] _rowStrings;
    
    public ModeledTable(List<T> data) : base(TableConfig.Unicode(), GetPropertyNames())
    {
        foreach (var element in data)
            AddRow(GetPropertyValues(element));
        _rowStrings = ToString().Split("\n");
    }

    public string Header => $"{"",-8}{_rowStrings[0]}{"",-8}{_rowStrings[1]}{"\n",-9}{_rowStrings[2]}";

    public string[] Options => _rowStrings[3..^2];
    
    public string Footer => $"{"\n",-9}{_rowStrings[^2]}";

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
    
    private static string DisplayableValue(object? value)
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