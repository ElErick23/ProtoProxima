using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProtoProxima.ConsoleUI.ModeledMenus;

namespace ProtoProxima.MongoDB.Models;

public class Activity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public bool Done { get; set; }
    public string Name { get; set; } = null!;
    public Category Category { get; set; } = null!;
    
    [BsonIgnore]
    [Hidden(typeof(ModeledMenu<>))]
    public Status Status
    {
        get
        {
            var limitStart = Due - Duration;
            var now = DateTime.Now;
            if (Done)
                return Status.Done;
            if (now > Due)
                return Status.Delayed;
            if (now > limitStart)
                return Status.ImminentDelay;
            
            //Double the duration to be sure
            var urgentLimit = limitStart - Duration;
            if (now > urgentLimit) 
                return Status.Urgent;
            return now > urgentLimit - Duration ? Status.Upcoming : Status.Distant;
        }
    }

    public DateTime Due { get; set; } = DateTime.Now + TimeSpan.FromDays(1);
    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan? TimeBeforeNextDue { get; set; }
    [Detail] public string? Desc { get; set; } = null;
}

public enum Status
{
    Distant,
    Upcoming,
    Urgent,
    ImminentDelay,
    Delayed,
    Done
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DetailAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public class HiddenAttribute : Attribute
{
    public Type[] MenuClasses { get; set; }

    public HiddenAttribute(params Type[] menuClasses)
    {
        MenuClasses = menuClasses;
    }
}