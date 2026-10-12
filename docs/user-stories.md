# User Stories – EduHub

### US-07 · Sinh viên: hủy đăng ký một sự kiện
Là sinh viên, tôi muốn hủy đăng ký khi bận đột xuất, để nhường chỗ cho bạn khác.

1. Given phiếu đang Pending hoặc Confirmed và sự kiện chưa diễn ra, When tôi hủy, Then phiếu chuyển sang Cancelled và số chỗ trống tăng thêm 1.
2. Given sự kiện đã diễn ra, When tôi hủy, Then bị từ chối.

### US-08 · Sinh viên: rời khỏi một CLB
Là sinh viên, tôi muốn rời CLB mình không còn tham gia, để không nhận thông báo và danh sách thành viên chính xác.

1. Given tôi đang là thành viên của CLB, When tôi rời CLB, Then tên tôi không còn trong club.Members.
2. Given tôi chưa là thành viên của CLB, When tôi rời CLB, Then báo lỗi "không phải thành viên".

### US-09 · Ban CLB: xác nhận phiếu đăng ký
Là Ban CLB, tôi muốn duyệt các phiếu đang chờ, để chỉ những người đủ điều kiện mới được check-in.

1. Given phiếu đang Pending, When tôi xác nhận, Then phiếu chuyển sang Confirmed.
2. Given phiếu đã Cancelled, When tôi xác nhận, Then bị từ chối.

### US-10 · Ban CLB: hủy một sự kiện
Là Ban CLB, tôi muốn hủy sự kiện khi có sự cố, để người đăng ký không đến vô ích.

1. Given sự kiện chưa diễn ra, When tôi hủy sự kiện, Then sự kiện chuyển sang Cancelled và mọi phiếu của sự kiện đó cũng thành Cancelled.
2. Given sự kiện đã diễn ra, When tôi hủy sự kiện, Then bị từ chối.

### US-11 · Admin: kích hoạt lại một thành viên
Là Admin, tôi muốn mở lại tài khoản thành viên đã bị ngừng, để họ tiếp tục tham gia khi vấn đề đã được giải quyết.

1. Given thành viên đang ngừng (Inactive), When tôi kích hoạt, Then IsActive() = true.
2. Given thành viên đang Active, When tôi kích hoạt, Then không thay đổi gì và báo "đã hoạt động".

### US-12 · Admin: xem danh sách thành viên theo trạng thái
Là Admin, tôi muốn lọc thành viên theo Active / Inactive, để nắm nhanh ai đang tham gia hệ thống.

1. Given hệ thống có thành viên, When tôi chọn lọc "Active", Then chỉ hiện thành viên có IsActive() = true.
2. Given không có thành viên nào khớp bộ lọc, When tôi lọc, Then hiện danh sách trống kèm thông báo, không báo lỗi.
