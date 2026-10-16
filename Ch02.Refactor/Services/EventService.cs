using EduHub.Ch02.Domain;
using EduHub.Ch02.Observers;
using EduHub.Ch02.Repositories;

namespace EduHub.Ch02.Services;

public class EventService
{
    private readonly IClubRepository _clubs;
    private readonly IEventRepository _events;
    private readonly List<IEventObserver> _observers = new();

    public EventService(IClubRepository clubs, IEventRepository events)
    {
        _clubs = clubs;
        _events = events;
    }

    public void Subscribe(IEventObserver observer) => _observers.Add(observer);

    public Event CreateEvent(Guid clubId, string title, DateTime date)
    {
        var club = _clubs.GetById(clubId)
            ?? throw new InvalidOperationException("Club not found");

        var ev = new Event { Id = Guid.NewGuid(), ClubId = clubId, Title = title, Date = date };
        _events.Add(ev);

        foreach (var observer in _observers)      // không tự log hay gửi mail nữa
            observer.OnEventCreated(ev, club);

        return ev;
    }
}
