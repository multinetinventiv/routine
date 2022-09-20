namespace Routine.Engine;

public class Marks
{
    private readonly HashSet<string> marks;

    public Marks() : this(new string[] { }) { }
    public Marks(IEnumerable<string> list)
    {
        marks = new();

        foreach (var mark in list)
        {
            marks.Add(mark);
        }
    }

    public HashSet<string> Set => marks;

    public bool Has(string mark) => marks.Contains(mark);

    public void Join(IEnumerable<string> list)
    {
        foreach (var mark in list)
        {
            if (!marks.Contains(mark))
            {
                marks.Add(mark);
            }
        }
    }
}
