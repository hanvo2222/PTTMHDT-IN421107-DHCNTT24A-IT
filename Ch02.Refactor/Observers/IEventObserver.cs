using EduHub.Ch02.Domain;

namespace EduHub.Ch02.Observers;

/// <summary>OBSERVER: EventService chỉ báo "có sự kiện mới", không biết ai nhận.</summary>
public interface IEventObserver
{
    void OnEventCreated(Event ev, Club club);
}
