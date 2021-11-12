using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TechTalks.Generators.Demo.Mediator;

var services = new ServiceCollection();

services.AddSingleton<SampleHandlers>();
services.AddMediatR(Assembly.GetExecutingAssembly());

var provider = services.BuildServiceProvider();

var mediator = provider.GetRequiredService<IMediator>();

await mediator.Send(new CreateUserRequest("edamtoft", "edamtoft@gmail.com"));