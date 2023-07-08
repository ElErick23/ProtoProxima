using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoProxima.ConsoleUI;
using ProtoProxima.ConsoleUI.Services;
using ProtoProxima.Core.Services;
using ProtoProxima.MongoDB;
using ProtoProxima.MongoDB.Models;
using ProtoProxima.MongoDB.Services;

var builder = Host.CreateApplicationBuilder(args);

var config = builder.Configuration.GetSection("MongoDB");
builder.Services.Configure<MongoDBSettings>(config);
builder.Services.AddSingleton<ActivityCore>();
builder.Services.AddSingleton<ActivityService>();
builder.Services.AddSingleton<MenuService>();
var menuService = builder.Services.BuildServiceProvider().GetRequiredService<MenuService>();

var menu = new CustomMenu(args, level: 0)
    .Add("Create activity", menuService.NewCreationMenu<Activity>(null, args, 1).Show)
    .Add("View activities", parent =>
    {
        Console.WriteLine("Loading activities...");
        menuService.NewTableMenu<Activity>(args, 1).SetParent(parent).Show();
    })
    .Add("Exit", () => Environment.Exit(0))
    .Configure(menuConfig =>
    {
        menuConfig.Title = "[Main menu]";
        menuConfig.WriteHeaderAction = Console.WriteLine;
    });
menu.Show();