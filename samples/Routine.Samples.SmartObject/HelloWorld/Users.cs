namespace Routine.Samples.SmartObject.HelloWorld;

public class Users : IFinder
{
    internal static readonly List<User> users = new();

    public void Add(string name) => users.Add(new User(name));
    public User Find(string name) => users.FirstOrDefault(u => u.Name == name);

    object IFinder.Find(string name) => Find(name);
}
