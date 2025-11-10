Crime Management System – ASP.NET Core Web API
Backend API for managing crime cases, reports, evidence, and users for District Core.
Built as part of the BE Rihal Codestacker Challenge 2025 (Backend) using ASP.NET Core, Entity Framework Core, and JWT Authentication.

🌍 Background
District Core is under siege — rising crime and failing trust have left the community on edge.
To turn things around, officials launched a Crime Management System, enabling real-time crime reporting and faster police response.

Now, the officials need your expertise to develop a robust backend API to efficiently manage growing crime data, ensure security, and improve response times.

💡 Problem Statement
Develop a backend API system for a Crime Case Management Platform.
This system enables registered users — Admins, Investigators, and Officers — to create, update, and monitor criminal cases, while citizens can report crimes publicly.
The API ensures data integrity, security, and scalability with a seamless role-based experience.

🧩 System Architecture
Controller	Description
AuthController	Handles registration, login, and JWT token authentication.
UserController	Admin-only user management (create, update, delete, assign roles & clearance).
CrimeReportsController	Public crime reporting and tracking endpoints for citizens.
CasesController	Create, update, list, and view detailed crime cases linked to reports.
CaseAssigneesController	Assign and manage officers and investigators on each case.
ParticipantsController	Manage suspects, victims, and witnesses within a case.
EvidenceController	Create, update, and delete (soft/hard) evidence records with audit logs.
CaseCommentsController	Internal discussion/comments between investigators and officers.
CitizenSubscriptionsController	Manage citizen email subscriptions to city alerts.
AlertsController	Send email alerts and safety announcements to the public.
👥 User Roles and Permissions
Role	Permissions
Admin	Full access to manage all users, cases, and reports. Can assign roles and clearance levels.
Investigator	Create, update, and close cases. Assign officers and manage evidence, suspects, victims, and witnesses.
Officer	View assigned cases, upload evidence, and update progress. Cannot delete or edit others’ entries.
Citizen	Public access — can report crimes and track their status using the report ID.
🔐 Clearance Rule: Officers can only be assigned to cases with equal or lower authorization levels (Low, Medium, High, Critical).

🧭 System Flow
🧑‍🤝‍🧑 1️⃣ Citizen Flow (Public Access)
Action	Endpoint	Authentication
Report a crime	POST /api/CrimeReports	❌ Public
Track report status	GET /api/CrimeReports/TrackCrimeReport/{reportId}	❌ Public
Subscribe to alerts	POST /api/CitizenSubscriptions/subscribe	❌ Public
Unsubscribe from alerts	POST /api/CitizenSubscriptions/unsubscribe?email=...	❌ Public
Process:

Citizen submits a crime report.
System generates a reportId for tracking.
Admins and Investigators are notified automatically.
Citizen can track progress anytime using the report ID.
🕵️‍♀️ 2️⃣ Investigator Flow
Action	Endpoint	Role
Create case from report	POST /api/Cases	Investigator
Assign officers to case	POST /api/CaseAssignees/AssignUser	Investigator
Add participants	POST /api/Participants/CreateParticipant	Investigator
Add or remove evidence	POST /api/Evidence/AddTextEvidence, DELETE /api/Evidence/SoftDeleteEvidence/{id}	Investigator
Comment on a case	POST /api/CaseComments/AddComment	Investigator
Process:

Investigator logs in via /api/Auth/login.
Creates a new case linked to existing crime reports.
Assigns officers according to clearance level.
Adds suspects, victims, witnesses, and evidence.
Posts comments and updates visible to the case team.
👮 3️⃣ Officer Flow
Action	Endpoint	Role
View assigned cases	GET /api/Cases	Officer
Upload image evidence	POST /api/Evidence/AddImageEvidence	Officer
Comment on a case	POST /api/CaseComments/AddComment	Officer
Process:

Officer logs in.
Views all assigned cases.
Uploads image or text evidence.
Adds comments to update investigators.
Cannot delete or modify existing records.
👨‍💼 4️⃣ Admin Flow
Action	Endpoint	Role
Manage users	POST /api/User, PUT /api/User/UpdateUserByID	Admin
Assign roles and clearance	PUT /api/User/AssignRole	Admin
Delete user	DELETE /api/User/DeleteUser/{id}	Admin
Send community alert	POST /api/Alerts/CommunityAlert	Admin
Process:

Admin logs in via /api/Auth/login.
Manages all system users, roles, and permissions.
Reviews all crime reports and active cases.
Sends safety alerts to subscribed citizens.
⚙️ System Flow Diagram
 ┌────────────────────────────┐
 │        CITIZEN (Public)   │
 │────────────────────────────│
 │ • Report a crime           │
 │ • Track report status      │
 │ • Subscribe to alerts      │
 └──────────────┬─────────────┘
                │
                ▼
       ┌────────────────────────┐
       │  ADMIN / INVESTIGATOR  │
       │────────────────────────│
       │ • Create & manage cases│
       │ • Assign officers      │
       │ • Add participants     │
       │ • Update case status   │
       └────────────┬───────────┘
                    │
                    ▼
           ┌─────────────────┐
           │     OFFICER     │
           │─────────────────│
           │ • View assigned │
           │   cases         │
           │ • Upload        │
           │   evidence      │
           │ • Add comments  │
           └───────┬─────────┘
                   │
                   ▼
          ┌──────────────────────┐
          │   EVIDENCE MODULE    │
          │──────────────────────│
          │ • Upload / Delete    │
          │ • Audit logging      │
          └────────┬─────────────┘
                   │
                   ▼
        ┌──────────────────────────┐
        │ EMAIL NOTIFICATION SYSTEM│
        │──────────────────────────│
        │ • New crime alerts       │
        │ • Case updates           │
        │ • Community broadcasts   │
        └──────────┬───────────────┘
                   │
                   ▼
         ┌────────────────────────┐
         │  CITIZENS (Subscribers)│
         │────────────────────────│
         │ Receive safety alerts  │
         │ via email notifications│
         └────────────────────────┘
✉️ Email Notification System
The system automatically sends emails to keep both citizens and officials informed about important events.

Event	Trigger
New Crime Report	Notifies Admins and Investigators when a report is submitted.
Case Update	Alerts Citizens and Assigned Officers when a case is updated or closed.
Community Alert	Enables Admins to broadcast city-wide safety messages.
💬 Case Commenting Rules
Rule	Description
Length	Comments must be between 5–150 characters.
Allowed Characters	Letters, numbers, and basic punctuation (. , ! ? ' -).
Disallowed	HTML tags, code snippets, or special characters.
Rate Limit	Max 5 comments per minute per user (to prevent spam).
🧱 Tech Stack Summary
ASP.NET Core 8 (Web API)
Entity Framework Core (Code-First)
SQL Server Database
JWT Authentication & Role-based Authorization
Email Notifications via SMTP
Swagger UI for testing and documentation
🏁 License
Developed as part of the BE Rihal Codestacker Challenge 2025 — Backend Evaluation Project.
