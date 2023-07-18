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
builder.Services.AddSingleton<CategoryCore>();
builder.Services.AddSingleton<CategoryService>();
builder.Services.AddSingleton<MenuService>();
var menuService = builder.Services.BuildServiceProvider().GetRequiredService<MenuService>();

var menu = new CustomMenu()
    .Add("Create activity", menuService.NewCreationMenu<Activity>(null).Show)
    .Add("View activities", parent =>
    {
        Console.WriteLine("Loading activities...");
        menuService.NewEditionTableMenu<Activity>().SetParent(parent).Show();
    })
    .Add("View categories", parent =>
    {
        Console.WriteLine("Loading categories...");
        menuService.NewEditionTableMenu<Category>().SetParent(parent).Show();
    })
    .Add("Exit", () => Environment.Exit(0))
    .Configure(menuConfig =>
    {
        menuConfig.Title = "[Main menu]";
        menuConfig.WriteHeaderAction = Console.WriteLine;
    });
menu.Show();