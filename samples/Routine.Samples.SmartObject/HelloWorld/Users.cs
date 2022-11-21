namespace Routine.Samples.SmartObject.HelloWorld;

public class Users : IFinder
{
    internal static readonly List<User> _users = new();

    public void Add(string name) => _users.Add(new User(name));
    public User Find(string name) => _users.FirstOrDefault(u => u.Name == name);

    object IFinder.Find(string name) => Find(name);
}
