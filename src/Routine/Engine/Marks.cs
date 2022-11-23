namespace Routine.Engine;

public class Marks
{
    private readonly HashSet<string> _marks;

    public Marks() : this(new string[] { }) { }
    public Marks(IEnumerable<string> list)
    {
        _marks = new();

        foreach (var mark in list)
        {
            _marks.Add(mark);
        }
    }

    public HashSet<string> Set => _marks;

    public bool Has(string mark) => _marks.Contains(mark);

    public void Join(IEnumerable<string> list)
    {
        foreach (var mark in list)
        {
            if (!_marks.Contains(mark))
            {
                _marks.Add(mark);
            }
        }
    }
}
