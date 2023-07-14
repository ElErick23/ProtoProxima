﻿using System.Reflection;
using MongoDB.Driver;
using ProtoProxima.MongoDB.Models;
using ProtoProxima.MongoDB.Services;

namespace ProtoProxima.Core.Services;

public class ActivityCore : ICore<Activity>
{
    private readonly ActivityService _activityService;

    public ActivityCore(ActivityService activityService)
    {
        _activityService = activityService;
    }

    public async Task Save(Activity activity)
    {
        await _activityService.CreateAsync(activity);
    }

    public async Task Update(Activity element)
    {
        if (element.Id == null)
            throw new Exception("Activity has no Id, can't update.");

        await _activityService.UpdateAsync(element.Id, element);
    }

    public async Task<bool> Delete(Activity element)
    {
        if (element.Id == null)
            throw new Exception("Activity has no Id, can't delete.");

        Console.WriteLine("Are you sure? (y/n)");
        var key = Console.ReadLine()?.ToLower();
        if (key != "y") return false;

        Console.WriteLine("Deleting...");
        await _activityService.DeleteAsync(element.Id);
        Console.WriteLine("Deleted! Press any key to continue...");
        Console.ReadKey();
        return true;
    }

    public async Task<List<Activity>> GetList(FilterDefinition<Activity> filter)
    {
        return await _activityService.GetListAsync(filter);
    }

    public string? GetProperty(PropertyInfo propertyInfo)
    {
        if (propertyInfo.PropertyType == typeof(Category))
        {
            Console.WriteLine("nO .|.");
            return null;
        }
        else
        {
            var propTypeName = propertyInfo.PropertyType.Name;
            if (propertyInfo.PropertyType.IsGenericType &&
                propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                propTypeName = propertyInfo.PropertyType.GetGenericArguments()[0].Name;

            Console.WriteLine($"Set {propertyInfo.Name} ({propTypeName}):");
            var value = Console.ReadLine();
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}