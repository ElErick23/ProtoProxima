using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoProxima.ConsoleUI;
using ProtoProxima.ConsoleUI.ModeledMenus;
using ProtoProxima.ConsoleUI.Tables;
using ProtoProxima.Models;
using ProtoProxima.Services;

var builder = Host.CreateApplicationBuilder(args);

var config = builder.Configuration.GetSection("MongoDB");
builder.Services.Configure<MongoDBSettings>(config);
builder.Services.AddSingleton<ActivityService>();
var activityService = builder.Services.BuildServiceProvider().GetRequiredService<ActivityService>();

var menu = new CustomMenu(args, level: 0)
    .Add("Create activity", parent =>
    {
        CreationMenu<Activity>.Build(null, activityService, args, 1).SetParent(parent).Show();
    })
    .Add("View activities", parent =>
    {
        Console.WriteLine("Loading activities...");
        new TableMenu<Activity>(activityService, args, level: 1).SetParent(parent).Show();
    })
    .Add("Exit", () => Environment.Exit(0))
    .Configure(menuConfig =>
    {
        menuConfig.Title = "[Main menu]";
        menuConfig.Selector = "--> ";
        menuConfig.EnableWriteTitle = true;
        menuConfig.WriteHeaderAction = () => Console.WriteLine("");
    });
menu.Show();