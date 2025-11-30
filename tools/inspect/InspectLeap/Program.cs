using System;
using Asterism.Time;

class Program
{
    static void Main()
    {
        Console.WriteLine($"StrictMode={LeapSeconds.StrictMode}");
        Console.WriteLine($"LastSupportedInstantUtc={LeapSeconds.LastSupportedInstantUtc:o}");
        Console.WriteLine($"StalenessHorizonYears={LeapSeconds.StalenessHorizonYears}");

        var d1 = new DateTime(2019,1,1,0,0,0, DateTimeKind.Utc);
        Console.WriteLine($"IsStale(2019-01-01)={LeapSeconds.IsStale(d1)}");

        var before = new DateTime(2016,12,31,23,59,59, DateTimeKind.Utc);
        var after = new DateTime(2017,1,1,0,0,0, DateTimeKind.Utc);
        Console.WriteLine($"Offset before 2016-12-31 23:59:59 = {LeapSeconds.SecondsBetweenUtcAndTai(before)}");
        Console.WriteLine($"Offset after 2017-01-01 00:00:00 = {LeapSeconds.SecondsBetweenUtcAndTai(after)}");
    }
}
