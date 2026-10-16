namespace EduHub.Ch02.Reports;

public class CsvReportStrategy : IReportStrategy
{
    private readonly string _path;
    public CsvReportStrategy(string path) => _path = path;

    public void Write(IReadOnlyList<ClubReportRow> rows)
    {
        var lines = new List<string> { "Club,Members" };
        lines.AddRange(rows.Select(r => $"\"{r.ClubName.Replace("\"", "\"\"")}\",{r.MemberCount}"));
        File.WriteAllLines(_path, lines);
    }
}
