using System.Collections.Generic;
using System.Linq;

namespace Routine.Samples.SmartObject.HelloWorld;

public class User
{
    public string Name { get; }

    public User(string name)
    {
        Name = name;
    }

    public string GetMessage() => $"Hello {Name}!";
    public void Delete() => Users.users.Remove(this);
}

public class Users
{
    internal static readonly List<User> users = new();

    public void Add(string name) => users.Add(new User(name));
    public User Find(string name) => users.FirstOrDefault(u => u.Name == name);
}
