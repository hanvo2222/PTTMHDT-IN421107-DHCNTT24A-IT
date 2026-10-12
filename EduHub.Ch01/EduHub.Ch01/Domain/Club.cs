namespace EduHub.Domain.Entities;

/// <summary>Câu lạc bộ. Danh sách thành viên chỉ được sửa qua AddMember/RemoveMember.</summary>
public class Club
{
    private readonly List<Member> _members = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Chỉ cho xem, không cho Add/Remove trực tiếp từ bên ngoài
    public IReadOnlyList<Member> Members => _members;

    private Club(string name, string description)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        CreatedAt = DateTime.Now;
    }

    public static Club Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên CLB trống");

        return new Club(name.Trim(), description?.Trim() ?? string.Empty);
    }

    /// <summary>Thêm thành viên (US-01): chặn null và chặn trùng.</summary>
    public void AddMember(Member member)
    {
        ArgumentNullException.ThrowIfNull(member);
        if (_members.Any(m => m.Id == member.Id))
            throw new InvalidOperationException($"{member} đã là thành viên");
        _members.Add(member);
    }

    /// <summary>Cho thành viên rời CLB (US-08).</summary>
    public void RemoveMember(Member member)
    {
        ArgumentNullException.ThrowIfNull(member);
        var existing = _members.FirstOrDefault(m => m.Id == member.Id);
        if (existing is null)
            throw new InvalidOperationException($"{member} không phải thành viên");
        _members.Remove(existing);
    }

    public int MemberCount() => _members.Count;

    public override string ToString() => Name;
}