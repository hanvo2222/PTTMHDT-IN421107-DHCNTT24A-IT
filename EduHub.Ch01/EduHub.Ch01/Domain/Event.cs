namespace EduHub.Domain.Entities;

/// <summary>Trạng thái của sự kiện.</summary>
public enum EventStatus
{
    Scheduled,
    Cancelled
}

/// <summary>Sự kiện do một CLB tổ chức. ClubId là khoá ngoại tới Club.</summary>
public class Event
{
    public Guid Id { get; private set; }
    public Guid ClubId { get; private set; }          // FK -> Club.Id
    public string Title { get; private set; }
    public DateTime Date { get; private set; }
    public int Capacity { get; private set; }
    public bool IsOnline { get; private set; }
    public EventStatus Status { get; private set; }

    private Event(Guid clubId, string title, DateTime date, int capacity, bool isOnline)
    {
        Id = Guid.NewGuid();
        ClubId = clubId;
        Title = title;
        Date = date;
        Capacity = capacity;
        IsOnline = isOnline;
        Status = EventStatus.Scheduled;
    }

    public static Event Create(Guid clubId, string title,
        DateTime date, int capacity, bool isOnline = false)
    {
        if (clubId == Guid.Empty)
            throw new ArgumentException("Thiếu ClubId");
        if (string.IsNullOrWhiteSpace(title))              // US-04: tiêu đề không được trống
            throw new ArgumentException("Tiêu đề trống");
        if (capacity <= 0)                                 // US-04: sức chứa phải lớn hơn 0
            throw new ArgumentOutOfRangeException(nameof(capacity), "Sức chứa phải lớn hơn 0");

        return new Event(clubId, title.Trim(), date, capacity, isOnline);
    }

    /// <summary>Còn chỗ nếu số phiếu đã dùng chưa đạt sức chứa (US-02).</summary>
    public bool HasCapacity(int registeredCount) => registeredCount < Capacity;

    /// <summary>Sự kiện đã diễn ra chưa?</summary>
    public bool HasEnded(DateTime now) => Date < now;

    /// <summary>Huỷ sự kiện (US-10): chỉ huỷ được khi chưa diễn ra và chưa bị huỷ.</summary>
    public void Cancel()
    {
        if (Status == EventStatus.Cancelled)
            throw new InvalidOperationException("Sự kiện đã bị huỷ");
        if (HasEnded(DateTime.Now))
            throw new InvalidOperationException("Sự kiện đã diễn ra, không thể huỷ");
        Status = EventStatus.Cancelled;
    }

    public override string ToString() => Title;
}