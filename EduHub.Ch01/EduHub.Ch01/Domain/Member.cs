namespace EduHub.Domain.Entities;

/// <summary>Trạng thái của thành viên.</summary>
public enum MemberStatus
{
    Active,
    Inactive
}

/// <summary>Thành viên (sinh viên) của hệ thống EduHub.</summary>
public class Member
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string StudentCode { get; private set; }
    public DateTime JoinedAt { get; private set; }
    public MemberStatus Status { get; private set; }

    // Constructor private: chỉ cho tạo đối tượng qua Create()
    private Member(string name, string email, string studentCode)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        StudentCode = studentCode;
        JoinedAt = DateTime.Now;
        Status = MemberStatus.Active;
    }

    /// <summary>Factory: kiểm tra dữ liệu rồi mới tạo Member.</summary>
    public static Member Create(string name, string email, string studentCode)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên trống");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Email sai");
        if (string.IsNullOrWhiteSpace(studentCode))
            throw new ArgumentException("Mã số sinh viên trống");

        return new Member(
            name.Trim(),
            email.Trim().ToLowerInvariant(),
            studentCode.Trim());
    }

    public bool IsActive() => Status == MemberStatus.Active;

    /// <summary>Ngừng thành viên (US-06). Lịch sử đăng ký không bị xoá.</summary>
    public void Deactivate()
    {
        if (!IsActive())
            throw new InvalidOperationException($"{this} đã bị ngừng từ trước");
        Status = MemberStatus.Inactive;
    }

    /// <summary>Kích hoạt lại thành viên (US-11).</summary>
    public void Activate()
    {
        if (IsActive())
            throw new InvalidOperationException($"{this} đã hoạt động");
        Status = MemberStatus.Active;
    }

    public override string ToString() => $"{Name} ({StudentCode})";
}