using ConsoleTables;
using MediatR;
using Sharprompt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalks.Generators.Demo.Application.Requests;

namespace TechTalks.Generators.Demo.Presentation
{
  internal class UserCliApp
  {
    private readonly IMediator _mediator;

    public UserCliApp(IMediator mediator) => _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task Run()
    {
      while (true)
      {
        var selection = Prompt.Select("Select Action", new[]
        {
          "Create User",
          "List Users",
          "Update User",
          "Delete User",
          "Exit",
        });

        switch (selection)
        {
          case "Create User":
            await CreateUser();
            break;
          case "List Users":
            await ListUsers();
            break;
          case "Update User":
            await UpdateUser();
            break;
          case "Delete User":
            await DeleteUser();
            break;
          case "Exit":
            return;
        }
      }
    }

    private async Task DeleteUser()
    {
      var email = Prompt.Input<string>("Enter email of user to delete", validators: new[] { Validators.Required() });
      var (found, user) = await _mediator.Send(new GetUserByEmailRequest(email));
      if (!found)
      {
        Console.WriteLine("User not found");
        return;
      }
      await _mediator.Send(new DeleteUserRequest(user!.Id));
    }

    private async Task UpdateUser()
    {
      var email = Prompt.Input<string>("Enter email of user to update", validators: new[] { Validators.Required() });
      var (found, user) = await _mediator.Send(new GetUserByEmailRequest(email));
      if (!found)
      {
        Console.WriteLine("User not found");
        return;
      }

      var newName = Prompt.Input<string>("Enter new name", defaultValue: user!.Name);
      var newEmail = Prompt.Input<string>("Enter new email", defaultValue: user!.Email);

      await _mediator.Send(new UpdateUserRequest(user!.Id, newName, newEmail));
    }

    private async Task ListUsers()
    {
      var result = await _mediator.Send(new ListUsersRequest());

      ConsoleTable.From(result.Value)
        .Write(Format.Default);
    }

    private async Task CreateUser()
    {
      var name = Prompt.Input<string>("Enter name", validators: new[] { Validators.Required() });
      var email = Prompt.Input<string>("Enter email", validators: new[] { Validators.Required() });

      var response = await _mediator.Send(new CreateUserRequest(name, email));

      Console.Write($"User created: {response.Value}");
    }
  }
}
