using EduHub.Ch02.Logging;
using EduHub.Ch02.Notifications;
using EduHub.Ch02.Observers;
using EduHub.Ch02.Reports;
using EduHub.Ch02.Repositories;
using EduHub.Ch02.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Composition root: nơi DUY NHẤT dùng `new` cho các class cụ thể.
// Đổi ConsoleLogger sang FileLogger("club-log.txt") chỉ cần sửa đúng dòng này.
IAppLogger logger = new ConsoleLogger();

IClubRepository clubs = new LoggingClubRepository(new InMemoryClubRepository(), logger);   // Decorator
IMemberRepository members = new InMemoryMemberRepository();
IEventRepository events = new InMemoryEventRepository();
INotificationServiceFactory notifications = new NotificationServiceFactory();               // Factory

var clubService = new ClubService(clubs, members, logger, notifications);
var eventService = new EventService(clubs, events);
eventService.Subscribe(new LogNotifier(logger));                       // Observer
eventService.Subscribe(new EmailNotifier(members, notifications));     // Observer
var reportService = new ReportService(clubs, members, logger);

clubService.CreateClub("CLB Lập trình", "Học C# cùng nhau", "president@gmail.com");
var club = clubService.GetAllClubs()[0];
clubService.AddMemberToClub(club.Id, "An", "an@yahoo.com");
eventService.CreateEvent(club.Id, "Workshop SOLID", DateTime.Today.AddDays(7));

reportService.Generate(new ConsoleReportStrategy());                   // Strategy
reportService.Generate(new CsvReportStrategy("club-report.csv"));
