using TechTalks.Generators.Demo.Domain;

namespace TechTalks.Generators.Demo.Application
{
  public sealed class UserHandlers
  {
    private readonly List<User> _users = new();

    [Handler]
    internal Guid CreateUser(string name, string email)
    {
      var userId = Guid.NewGuid();
      _users.Add(new User(userId, name, email));
      return userId;
    }

    [Handler]
    internal bool DeleteUser(Guid userId)
    {
      var deleted = _users.RemoveAll(user => user.Id == userId);
      return deleted > 0;
    }

    [Handler]
    internal (bool Found, User? User) GetUserByEmail(string email)
    {
      var user = _users.FirstOrDefault(user => user.Email == email);
      if (user is null)
      {
        return (false, null);
      }
      return (true, user);
    }

    [Handler]
    internal bool UpdateUser(Guid userId, string newName, string newEmail)
    {
      var user = _users.FirstOrDefault(user => user.Id == userId);
      if (user is null)
      {
        return false;
      }
      _users.Remove(user);
      _users.Add(user with { Name = newName, Email = newEmail });
      return true;
    }

    [Handler]
    internal List<User> ListUsers()
    {
      return _users.ToList();
    }
  }
}
