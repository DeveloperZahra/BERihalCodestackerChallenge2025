namespace BERihalCodestackerChallenge2025.Services
{
    public interface IUnitServices
    {
        IAuditLogService Audit { get; }
        ICaseService Cases { get; }
        IEvidenceService Evidence { get; }
        IReportService Reports { get; }
        IUserService Users { get; }
    }
}