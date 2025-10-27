using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrimeCase.Api.Models
{
    // ---------------------------
    // Enums
    // ---------------------------
    public enum Role { Admin, Investigator, Officer } // User roles in the system
    public enum Clearance { low, medium, high, critical }
    public enum CaseStatus { pending, ongoing, closed }
    public enum ReportStatus { pending, en_route, on_scene, under_investigation, resolved }
    public enum ParticipantRole { suspect, victim, witness }
    public enum EvidenceType { text, image }


}