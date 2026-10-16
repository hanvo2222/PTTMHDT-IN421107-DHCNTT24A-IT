using EduHub.Ch02.Domain;
using EduHub.Ch02.Logging;
using EduHub.Ch02.Notifications;
using EduHub.Ch02.Repositories;

namespace EduHub.Ch02.Services;

/// <summary>Nghiệp vụ CLB và thành viên. Mọi phụ thuộc nhận qua constructor (DIP).</summary>
public class ClubService
{
    private readonly IClubRepository _clubs;
    private readonly IMemberRepository _members;
    private readonly IAppLogger _logger;
    private readonly INotificationServiceFactory _notifications;

    public ClubService(IClubRepository clubs, IMemberRepository members,
                       IAppLogger logger, INotificationServiceFactory notifications)
    {
        _clubs = clubs;
        _members = members;
        _logger = logger;
        _notifications = notifications;
    }

    public void CreateClub(string name, string description, string presidentEmail)
    {
        var club = Club.Create(name, description);          // validate nằm trong Domain
        _clubs.Add(club);
        _logger.Info($"Club created: {name}");
        _notifications.Create(NotificationChannel.Email)
                      .Send(presidentEmail, $"Club '{name}' has been created");
    }

    public void AddMemberToClub(Guid clubId, string memberName, string email)
    {
        var club = _clubs.GetById(clubId)
            ?? throw new InvalidOperationException("Club not found");

        if (!club.HasRoom(_members.GetByClub(clubId).Count))
            throw new InvalidOperationException($"Club is full (max {Club.MaxMembers} members)");

        var member = new Member { Id = Guid.NewGuid(), Name = memberName, Email = email, ClubId = clubId };
        _members.Add(member);

        _logger.Info($"Member {memberName} joined {club.Name}");
        _notifications.Create(NotificationChannel.Email).Send(email, $"Welcome to {club.Name}");
        _notifications.Create(NotificationChannel.Slack).Send("#clubs", $"{memberName} joined {club.Name}");
    }

    public IReadOnlyList<Club> GetAllClubs() => _clubs.GetAll();
}
