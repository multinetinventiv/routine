using System.Collections.Generic;

namespace Routine.Test.Performance.Domain;

public class BusinessPerformance
{
    public int Id { get; set; }
    public List<BusinessPerformanceSub> Items { get; set; }
    public BusinessPerformance() { Items = new List<BusinessPerformanceSub>(); }

    public BusinessPerformanceSub GetSub(int index) { return Items[index]; }
    public int Create(List<BusinessPerformanceInput> input) { return 0; }
}
