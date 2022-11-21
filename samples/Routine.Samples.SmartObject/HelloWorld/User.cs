namespace Routine.Samples.SmartObject.HelloWorld;

public class User
{
    public string Name { get; }

    public User(string name)
    {
        Name = name;
    }

    public string GetMessage() => $"Hello {Name}!";
    public void Delete() => Users._users.Remove(this);
}
