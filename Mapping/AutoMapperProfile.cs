using AutoMapper;
using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using System.Runtime;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BERihalCodestackerChallenge2025.Mapping
{
    //AutoMapper Profile for mapping Models <-> DTOs
    public class AutoMapperProfile : Profile 
    {
        public AutoMapperProfile()
        {
            // ========== USERS ==========
            CreateMap<User, UserReadDto>();
            CreateMap<UserCreateUpdateDto, User>();


            // ========== CRIME REPORTS ==========
            CreateMap<CrimeReport, CrimeReportStatusDto>()
                .ForMember(dest => dest.ReportedBy, opt => opt.MapFrom(src =>
                    src.ReportedByUserId == null ? "Citizen" : src.ReportedByUser.FullName));

            CreateMap<CrimeReportCreateDto, CrimeReport>();

            // ========== CASES ==========
            // Create / Update mapping
            CreateMap<CaseCreateDto, Case>()
                .ForMember(dest => dest.CaseId, opt => opt.Ignore()) // ID handled by DB
                .ForMember(dest => dest.CaseNumber, opt => opt.Ignore()) // auto-generated
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "pending")) // default
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<CaseUpdateDto, Case>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            // Ignore nulls (only update sent fields)

            // List Item mapping (summary for list API)
            CreateMap<Case, CaseListItemDto>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByUser.FullName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                    src.Description.Length > 100
                        ? src.Description.Substring(0, src.Description.Substring(0, 100).LastIndexOf(' ')) + " ..."
                        : src.Description));

            //  Case Assignee mapping
            CreateMap<CaseAssignee, CaseAssigneeDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName));




            // ========== PARTICIPANTS ==========
            CreateMap<Participant, ParticipantDto>().ReverseMap();

            CreateMap<CaseParticipant, CaseParticipantReadDto>()
                .ForMember(dest => dest.ParticipantId, opt => opt.MapFrom(src => src.ParticipantId))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Participant.FullName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Participant.Phone))
                .ForMember(dest => dest.AddedBy, opt => opt.MapFrom(src =>
                    src.AddedByUser != null ? src.AddedByUser.FullName : "System"));

            // ========== EVIDENCE ==========
            CreateMap<Evidence, EvidenceReadDto>()
                .ForMember(dest => dest.AddedBy, opt => opt.MapFrom(src => src.AddedByUser.FullName));

            CreateMap<EvidenceCreateDto, Evidence>();

            CreateMap<EvidenceAuditLog, EvidenceAuditLogDto>()
                .ForMember(dest => dest.ActedBy, opt => opt.MapFrom(src => src.ActedByUser.FullName));

            // ========== AUTH ==========
            CreateMap<User, AuthUserDto>()
                .ForMember(dest => dest.Token, opt => opt.Ignore());
        }
    }
}
    
