namespace Routine.Samples.SmartObject.HelloWorld;

public class Users : IFinder
{
    internal static readonly List<User> List = new();

    public void Add(string name) => List.Add(new User(name));
    public User Find(string name) => List.FirstOrDefault(u => u.Name == name);

    object IFinder.Find(string name) => Find(name);
}
