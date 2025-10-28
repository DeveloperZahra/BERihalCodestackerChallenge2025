using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Model
{
    // ---------------------------
    // Enums
    // ---------------------------
    public enum Role { Admin, Investigator, Officer } // User roles in the system
    public enum Clearance { low, medium, high, critical } // Clearance levels for accessing cases
    public enum CaseStatus { pending, ongoing, closed } // Status of a case
    public enum Status { pending, en_route, on_scene, under_investigation, resolved } // Status of a crime report
    public enum ParticipantRole { Suspect, Victim, Witness } // Roles of participants in a case
    public enum EvidenceType { text, image } // Types of evidence


}