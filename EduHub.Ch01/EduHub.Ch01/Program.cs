using System.Text;
using EduHub.Domain.Entities;

Console.OutputEncoding = Encoding.UTF8;

// ============ Dữ liệu và hàm tiện ích (chưa có database nên giữ trong List) ============
var members = new List<Member>();
var registrations = new List<Registration>();
var checkIns = new List<CheckIn>();

void Title(string text)
{
    Console.WriteLine();
    Console.WriteLine($"=== {text} ===");
}

// Số chỗ đã dùng = số phiếu chưa bị huỷ của sự kiện
int UsedSeats(Event ev) =>
    registrations.Count(r => r.EventId == ev.Id && r.Status != RegistrationStatus.Cancelled);

bool IsCheckedIn(Registration r) => checkIns.Any(c => c.RegistrationId == r.Id);

// US-02: còn chỗ thì tạo phiếu Pending, hết chỗ thì báo
Registration? Register(Member m, Event ev)
{
    if (!ev.HasCapacity(UsedSeats(ev)))
    {
        Console.WriteLine($"  {m.Name}: HẾT CHỖ ({ev.Title})");
        return null;
    }
    var reg = Registration.Create(m.Id, ev.Id);
    registrations.Add(reg);
    Console.WriteLine($"  {m.Name}: đăng ký thành công, trạng thái {reg.Status}");
    return reg;
}

// Thử một thao tác vi phạm luật: phải bị từ chối bằng exception
void TryViolation(string label, Action action)
{
    try
    {
        action();
        Console.WriteLine($"  [SAI] {label}: lẽ ra phải bị từ chối!");
    }
    catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
    {
        Console.WriteLine($"  [OK] {label} -> bị từ chối: {ex.Message}");
    }
}

// US-05: báo cáo tham dự
void PrintReport(Event ev)
{
    var regs = registrations.Where(r => r.EventId == ev.Id).ToList();
    int checkedIn = regs.Count(IsCheckedIn);

    Console.WriteLine($"Sự kiện : {ev.Title} ({ev.Date:dd/MM/yyyy}, {(ev.IsOnline ? "online" : "trực tiếp")})");
    Console.WriteLine($"Đăng ký : {UsedSeats(ev)}/{ev.Capacity}");
    Console.WriteLine($"Check-in: {checkedIn}");
    foreach (var r in regs)
    {
        var name = members.First(m => m.Id == r.MemberId).Name;
        var state = IsCheckedIn(r) ? "đã check-in" : "chưa check-in";
        Console.WriteLine($"  - {name,-16} {r.Status,-10} {state}");
    }
}

// ============ Luồng nghiệp vụ ============
Title("1. Tạo CLB");
var club = Club.Create("CLB Lập trình", "Sinh hoạt về lập trình và công nghệ");
Console.WriteLine($"Đã tạo: {club}");

Title("2. Thêm 3 thành viên");
members.Add(Member.Create("Nguyễn Văn An", "an@dthu.edu.vn", "DH24001"));
members.Add(Member.Create("Trần Thị Bình", "binh@dthu.edu.vn", "DH24002"));
members.Add(Member.Create("Lê Minh Châu", "chau@dthu.edu.vn", "DH24003"));
foreach (var m in members) club.AddMember(m);
Console.WriteLine($"CLB có {club.MemberCount()} thành viên");

Title("3. Tạo sự kiện");
var ev = Event.Create(club.Id, "Workshop C# cơ bản", DateTime.Now.AddDays(7), capacity: 3);
Console.WriteLine($"Đã tạo: {ev} - sức chứa {ev.Capacity}");

Title("4. Đăng ký (Pending)");
foreach (var m in members) Register(m, ev);

Title("5. Xác nhận phiếu (Confirmed)");
registrations[0].Confirm();
registrations[1].Confirm();
Console.WriteLine($"  Phiếu 1: {registrations[0].Status}");
Console.WriteLine($"  Phiếu 2: {registrations[1].Status}");
Console.WriteLine($"  Phiếu 3: {registrations[2].Status} (chưa xác nhận)");

Title("6. Check-in qua QR");
var checkIn = CheckIn.Create(registrations[0], CheckInMethod.QR);
checkIns.Add(checkIn);
Console.WriteLine($"  {checkIn.Summary()}");

Title("7. Báo cáo tham dự");
PrintReport(ev);

// ============ Bước 8: thử vi phạm luật nghiệp vụ ============
Title("8. Thử vi phạm luật nghiệp vụ");

TryViolation("Thêm thành viên đã có trong CLB",
    () => club.AddMember(members[0]));

TryViolation("Tạo thành viên với email sai",
    () => Member.Create("Test", "abc", "DH24999"));

TryViolation("Tạo sự kiện có sức chứa = 0",
    () => Event.Create(club.Id, "Sự kiện lỗi", DateTime.Now.AddDays(1), capacity: 0));

var member4 = Member.Create("Phạm Quốc Dũng", "dung@dthu.edu.vn", "DH24004");
members.Add(member4);
Register(member4, ev);   // sự kiện đã đủ 3/3 nên báo hết chỗ

TryViolation("Check-in bằng phiếu còn Pending",
    () => CheckIn.Create(registrations[2]));

TryViolation("Thành viên chưa tham gia CLB mà rời CLB",
    () => club.RemoveMember(member4));

// US-07: sinh viên huỷ phiếu (sự kiện chưa diễn ra) rồi thử xác nhận lại
if (!ev.HasEnded(DateTime.Now))
{
    registrations[1].Cancel();
    Console.WriteLine($"  Phiếu 2 đã huỷ, số chỗ đã dùng còn {UsedSeats(ev)}/{ev.Capacity}");
}
TryViolation("Xác nhận lại phiếu đã huỷ",
    () => registrations[1].Confirm());

// US-10: không huỷ được sự kiện đã diễn ra
var oldEvent = Event.Create(club.Id, "Seminar tuần trước", DateTime.Now.AddDays(-7), capacity: 20);
TryViolation("Huỷ sự kiện đã diễn ra",
    () => oldEvent.Cancel());

// ============ Bước 9 (thêm): Admin ngừng / kích hoạt lại thành viên ============
Title("9. Admin quản lý thành viên");
members[2].Deactivate();
Console.WriteLine($"  Đã ngừng {members[2]}");
Console.WriteLine("  Thành viên đang Active: " +
    string.Join(", ", members.Where(m => m.IsActive()).Select(m => m.Name)));
members[2].Activate();
Console.WriteLine($"  Đã kích hoạt lại {members[2]}, IsActive() = {members[2].IsActive()}");
TryViolation("Kích hoạt thành viên đang Active",
    () => members[2].Activate());

Console.WriteLine();
Console.WriteLine("Hoàn tất demo EduHub.");