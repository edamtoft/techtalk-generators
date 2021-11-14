using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechTalks.Generators.Demo.Mediator
{
  internal sealed class SampleHandlers
  {
    private readonly List<(string Name, string Email)> _users = new();

    [Handler]
    public bool CreateUser(string name, string email)
    {
      _users.Add((name, email));
      return true;
    }

    [Handler]
    public bool DeleteUser(string email)
    {
      var deleted = _users.RemoveAll(user => user.Email == email);
      return deleted > 0;
    }

    [Handler]
    public (bool Found, string? Name) GetUserByEmail(string email)
    {
      var user = _users.FirstOrDefault(user => user.Email == email);
      if (user == default)
      {
        return (false, null);
      }
      return (true, user.Name);
    }

    [Handler]
    public bool UpdateUser(string email, string newName)
    {
      var user = _users.FirstOrDefault(user => user.Email == email);
      if (user == default)
      {
        return false;
      }
      _users.Remove(user);
      _users.Add(user with { Name = newName });
      return true;
    }
  }
}
