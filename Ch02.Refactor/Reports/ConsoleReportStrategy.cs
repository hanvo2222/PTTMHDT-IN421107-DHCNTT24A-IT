namespace EduHub.Ch02.Reports;

public class ConsoleReportStrategy : IReportStrategy
{
    public void Write(IReadOnlyList<ClubReportRow> rows)
    {
        foreach (var row in rows)
            Console.WriteLine($"Club: {row.ClubName} | Members: {row.MemberCount}");
    }
}
