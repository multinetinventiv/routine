namespace Routine.Samples.Basic.HelloWorld;

public class HelloWorldService
{
    public string GetMessage(string name) => $"Hello {name}!";

    public async Task<string> GetMessageAsync(string name, int delay)
    {
        await Task.Delay(delay);

        return GetMessage(name);
    }
}
