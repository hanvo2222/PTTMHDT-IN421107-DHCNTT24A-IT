namespace EduHub.Ch02.Reports;

/// <summary>STRATEGY: mỗi định dạng báo cáo là một cài đặt riêng (OCP).</summary>
public interface IReportStrategy
{
    void Write(IReadOnlyList<ClubReportRow> rows);
}
