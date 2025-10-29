namespace BERihalCodestackerChallenge2025.Services
{
    public class UnitServices : IUnitServices
    {
        public IUserService Users { get; } // User service for managing users
        public ICaseService Cases { get; } // Case service for managing cases
        public IReportService Reports { get; } // Report service for managing reports
        public IEvidenceService Evidence { get; } // Evidence service for managing evidences
        public IAuditLogService Audit { get; } // Audit log service for managing audit logs


        public UnitServices( // Constructor accepting all service dependencies
            IUserService users, // User service
            ICaseService cases, // Case service
            IReportService reports, // Report service
            IEvidenceService evidence, // Evidence service
            IAuditLogService audit // Audit log service
            )
        {
            Users = users; // User service assignment
            Cases = cases; // Case service assignment
            Reports = reports; // Report service assignment
            Evidence = evidence; // Evidence service assignment
            Audit = audit; // Audit log service assignment
        }
    }
}

