using EduHub.Ch02.Logging;
using EduHub.Ch02.Reports;
using EduHub.Ch02.Repositories;

namespace EduHub.Ch02.Services;

public class ReportService
{
    private readonly IClubRepository _clubs;
    private readonly IMemberRepository _members;
    private readonly IAppLogger _logger;

    public ReportService(IClubRepository clubs, IMemberRepository members, IAppLogger logger)
    {
        _clubs = clubs;
        _members = members;
        _logger = logger;
    }

    public void Generate(IReportStrategy strategy)
    {
        var rows = _clubs.GetAll()
            .Select(c => new ClubReportRow(c.Name, _members.GetByClub(c.Id).Count))
            .ToList();

        strategy.Write(rows);

        foreach (var row in rows)
            _logger.Info($"Report generated for {row.ClubName}");
    }
}
