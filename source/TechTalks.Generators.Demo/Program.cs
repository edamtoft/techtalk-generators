using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TechTalks.Generators.Demo.Mediator.Application;
using TechTalks.Generators.Demo.Mediator.Presentation;

var app = new ServiceCollection()
  .AddSingleton<UserHandlers>()
  .AddMediatR(Assembly.GetExecutingAssembly())
  .AddTransient<UserCliApp>()
  .BuildServiceProvider()
  .GetRequiredService<UserCliApp>();

await app.Run();