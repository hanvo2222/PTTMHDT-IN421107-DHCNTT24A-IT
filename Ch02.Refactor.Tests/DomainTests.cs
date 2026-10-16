using EduHub.Ch02.Domain;
using EduHub.Ch02.Logging;
using EduHub.Ch02.Notifications;
using EduHub.Ch02.Repositories;
using EduHub.Ch02.Services;

namespace Ch02.Refactor.Tests;

// Mẫu AAA: Arrange (chuẩn bị) - Act (thực hiện) - Assert (kiểm tra).
// Mỗi test chỉ kiểm tra MỘT hành vi. Đặt tên: TênMethod_TìnhHuống_KếtQuảMongĐợi.
public class DomainTests
{
    // Test mẫu của starter, đã đổi từ ClubManager sang ClubService sau khi refactor.
    [Fact]
    public void CreateClub_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var service = new ClubService(
            new InMemoryClubRepository(), new InMemoryMemberRepository(),
            new ConsoleLogger(), new NotificationServiceFactory());

        // Act
        var act = () => service.CreateClub("", "mô tả", "p@gmail.com");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    // ───────── Club ─────────

    [Fact]
    public void Club_Create_WithValidName_ReturnsClub()
    {
        // Arrange
        const string name = "CLB Lập trình";
        const string description = "Học C# cùng nhau";

        // Act
        var club = Club.Create(name, description);

        // Assert
        Assert.Equal(name, club.Name);
        Assert.Equal(description, club.Description);
        Assert.NotEqual(Guid.Empty, club.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Club_Create_WithInvalidName_ThrowsArgumentException(string invalidName)
    {
        // Arrange (tên rỗng hoặc toàn khoảng trắng được truyền qua InlineData)

        // Act
        var act = () => Club.Create(invalidName, "mô tả");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Club_Create_WithNameLongerThan100_ThrowsArgumentException()
    {
        // Arrange
        var tooLongName = new string('a', Club.MaxNameLength + 1);

        // Act
        var act = () => Club.Create(tooLongName, "mô tả");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Club_HasRoom_AtMaxMembers_ReturnsFalse()
    {
        // Arrange
        var club = Club.Create("CLB Lập trình", "mô tả");

        // Act
        var hasRoom = club.HasRoom(Club.MaxMembers);   // đúng 50 thành viên

        // Assert
        Assert.False(hasRoom);
    }

    [Fact]
    public void Club_HasRoom_JustBelowMax_ReturnsTrue()
    {
        // Arrange
        var club = Club.Create("CLB Lập trình", "mô tả");

        // Act
        var hasRoom = club.HasRoom(Club.MaxMembers - 1);   // 49 thành viên

        // Assert
        Assert.True(hasRoom);
    }

    // ───────── Event ─────────

    [Fact]
    public void Event_Register_NewMember_ReturnsTrue()
    {
        // Arrange
        var ev = NewEvent();
        var memberId = Guid.NewGuid();

        // Act
        var result = ev.Register(memberId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Event_Register_SameMemberTwice_ReturnsFalse()
    {
        // Arrange
        var ev = NewEvent();
        var memberId = Guid.NewGuid();
        ev.Register(memberId);

        // Act
        var secondResult = ev.Register(memberId);

        // Assert
        Assert.False(secondResult);
    }

    [Fact]
    public void Event_CheckIn_UnregisteredMember_ThrowsInvalidOperationException()
    {
        // Arrange
        var ev = NewEvent();
        var memberId = Guid.NewGuid();   // chưa đăng ký

        // Act
        var act = () => ev.CheckIn(memberId);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Event_CheckIn_RegisteredMember_RecordsCheckIn()
    {
        // Arrange
        var ev = NewEvent();
        var memberId = Guid.NewGuid();
        ev.Register(memberId);

        // Act
        ev.CheckIn(memberId);

        // Assert
        Assert.Contains(memberId, ev.CheckedInMemberIds);
    }

    [Fact]
    public void Event_AttendanceRate_NoRegistrations_ReturnsZero()
    {
        // Arrange
        var ev = NewEvent();

        // Act
        var rate = ev.AttendanceRate;

        // Assert
        Assert.Equal(0, rate);
    }

    private static Event NewEvent() => new()
    {
        Id = Guid.NewGuid(),
        ClubId = Guid.NewGuid(),
        Title = "Workshop SOLID",
        Date = DateTime.Today
    };
}
