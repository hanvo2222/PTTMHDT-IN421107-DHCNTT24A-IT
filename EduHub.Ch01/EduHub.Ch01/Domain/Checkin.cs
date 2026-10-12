namespace EduHub.Domain.Entities;

/// <summary>Các cách check-in hợp lệ (dùng hằng số thay vì chuỗi rải rác).</summary>
public static class CheckInMethod
{
    public const string Manual = "Manual";
    public const string QR = "QR";

    public static bool IsValid(string? method) =>
        method == Manual || method == QR;
}

/// <summary>Bản ghi điểm danh. RegistrationId là khoá ngoại tới Registration.</summary>
public class CheckIn
{
    public Guid Id { get; private set; }
    public Guid RegistrationId { get; private set; }   // FK -> Registration.Id
    public DateTime CheckedInAt { get; private set; }
    public string Method { get; private set; }

    private CheckIn(Guid registrationId, string method)
    {
        Id = Guid.NewGuid();
        RegistrationId = registrationId;
        CheckedInAt = DateTime.Now;
        Method = method;
    }

    /// <summary>Tạo bản ghi check-in (US-03): chỉ phiếu Confirmed mới check-in được.</summary>
    public static CheckIn Create(Registration registration,
        string method = CheckInMethod.Manual)
    {
        ArgumentNullException.ThrowIfNull(registration);
        if (registration.Status != RegistrationStatus.Confirmed)
            throw new InvalidOperationException("Chỉ phiếu Confirmed mới check-in");
        if (!CheckInMethod.IsValid(method))
            throw new ArgumentException("Phương thức check-in không hợp lệ", nameof(method));

        return new CheckIn(registration.Id, method);
    }

    public string Summary() =>
        $"Check-in {Method} lúc {CheckedInAt:HH:mm dd/MM/yyyy} (phiếu {RegistrationId})";
}