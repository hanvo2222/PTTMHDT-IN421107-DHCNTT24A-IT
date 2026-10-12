namespace EduHub.Domain.Entities;

/// <summary>Vòng đời phiếu: Pending -> Confirmed -> (check-in), hoặc Cancelled.</summary>
public enum RegistrationStatus
{
    Pending,
    Confirmed,
    Cancelled
}

/// <summary>Phiếu đăng ký. MemberId, EventId là khoá ngoại.</summary>
public class Registration
{
    public Guid Id { get; private set; }
    public Guid MemberId { get; private set; }         // FK -> Member.Id
    public Guid EventId { get; private set; }          // FK -> Event.Id
    public DateTime RegisteredAt { get; private set; }
    public RegistrationStatus Status { get; private set; }

    private Registration(Guid memberId, Guid eventId)
    {
        Id = Guid.NewGuid();
        MemberId = memberId;
        EventId = eventId;
        RegisteredAt = DateTime.Now;
        Status = RegistrationStatus.Pending;           // mới tạo = chờ xác nhận
    }

    public static Registration Create(Guid memberId, Guid eventId)
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException("Thiếu MemberId");
        if (eventId == Guid.Empty)
            throw new ArgumentException("Thiếu EventId");

        return new Registration(memberId, eventId);
    }

    /// <summary>Xác nhận phiếu (US-09): chỉ phiếu Pending mới được xác nhận.</summary>
    public void Confirm()
    {
        if (Status != RegistrationStatus.Pending)
            throw new InvalidOperationException(
                $"Không thể xác nhận phiếu đang ở trạng thái {Status}.");
        Status = RegistrationStatus.Confirmed;
    }

    /// <summary>Huỷ phiếu (US-07): cho phép khi Pending hoặc Confirmed.</summary>
    public void Cancel()
    {
        if (Status == RegistrationStatus.Cancelled)
            throw new InvalidOperationException("Phiếu đã bị huỷ");
        Status = RegistrationStatus.Cancelled;
    }
}