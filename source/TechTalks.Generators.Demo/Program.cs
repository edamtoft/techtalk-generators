using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TechTalks.Generators.Demo.Mediator;

var services = new ServiceCollection();

services.AddSingleton<SampleHandlers>();
services.AddMediatR(Assembly.GetExecutingAssembly());

var provider = services.BuildServiceProvider();

var mediator = provider.GetRequiredService<IMediator>();

await mediator.Send(new CreateUserRequest("Erik Damtoft", "edamtoft@gmail.com"));

await mediator.Send(new UpdateUserRequest("edamtoft@gmail.com", NewName: "Eric Damtoft"));

var res = await mediator.Send(new GetUserByEmailRequest("edamtoft@gmail.com"));

if (res.Found)
{
  Console.WriteLine($"Found!: {res.Name}");
}
else
{
  Console.WriteLine("User Not Found");
}

await mediator.Send(new DeleteUserRequest("edamtoft@gmail.com"));