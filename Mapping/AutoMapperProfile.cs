using BERihalCodestackerChallenge2025.DTOs;
using System.Runtime;
using System.Security.Policy;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BERihalCodestackerChallenge2025.Mapping
{
    //AutoMapper Profile for mapping Models <-> DTOs
    public class MappingProfile : Profile
    {
        public MappingProfile()
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
            CreateMap<Case, CaseListItemDto>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByUser.FullName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                    src.Description.Length > 100
                    ? src.Description.Substring(0, src.Description.Substring(0, 100).LastIndexOf(' ')) + " ..."
                    : src.Description));

            CreateMap<Case, CaseDetailsDto>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByUser.FullName))
                .ForMember(dest => dest.CaseLevel, opt => opt.MapFrom(src => src.AuthorizationLevel));

            CreateMap<CaseCreateDto, Case>();
            CreateMap<CaseUpdateDto, Case>();

            // ========== ASSIGNEES ==========
            CreateMap<CaseAssignee, CaseAssigneeDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName));

            // ========== PARTICIPANTS ==========
            CreateMap<Participant, ParticipantCreateDto>().ReverseMap();

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
