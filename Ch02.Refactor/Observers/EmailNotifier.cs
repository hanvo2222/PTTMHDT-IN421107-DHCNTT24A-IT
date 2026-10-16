using EduHub.Ch02.Domain;
using EduHub.Ch02.Notifications;
using EduHub.Ch02.Repositories;

namespace EduHub.Ch02.Observers;

public class EmailNotifier : IEventObserver
{
    private readonly IMemberRepository _members;
    private readonly INotificationService _email;

    public EmailNotifier(IMemberRepository members, INotificationServiceFactory factory)
    {
        _members = members;
        _email = factory.Create(NotificationChannel.Email);
    }

    public void OnEventCreated(Event ev, Club club)
    {
        foreach (var member in _members.GetByClub(club.Id))
            _email.Send(member.Email, $"new event '{ev.Title}' at {club.Name}");
    }
}
