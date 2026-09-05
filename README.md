# Apex Predator Circuit (RaceDay)

A full-stack web-based event management system for the South African road running, walking, and cycling community. Built as part of the PROG6212 Programming 2B Portfolio of Evidence.

## System Description

RaceDay allows Event Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enter events by selecting a category, view their own enrolments, and track their personal performance history.

## User Roles

- **Organiser**: Can create, edit, and delete events, manage event categories, capture participant results, and view all event enrolments.
- **Participant**: Can create an account, browse events, enter an event by selecting a category, view their own enrolments, and track their personal results.

## Repository Structure

```
ApexCircuit/
├── .github/workflows/ci.yml    # CI/CD pipeline
├── docs/
│   ├── ERD_Apex.png            # Entity Relationship Diagram
│   ├── ApexCircuit.sql         # Database creation script
│   ├── API_Endpoint_Plan.md    # API endpoint specification
│   └── SECTION B.pdf           # ERD + Data Dictionary
├── SECTION C.pdf               # SQL implementation screenshots
├── README.md                   # This file
└── Links.txt                   # Submission links
```

## CI/CD Status

![Apex CI](https://github.com/YOURUSERNAME/ApexCircuit/actions/workflows/ci.yml/badge.svg)



## YouTube Video

[Watch the video presentation here]

## Database

- **Database Name**: `ApexCircuitDB`
- **Tables**: Roles, Users, Events, Categories, Enrolments, Results
- **SQL Script**: Located in `docs/ApexCircuit.sql`

## How to Run the SQL Script

1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server instance
3. Open `docs/ApexCircuit.sql`
4. Press F5 to execute
5. Verify `ApexCircuitDB` is created with all 6 tables and sample data

## Technologies

- SQL Server (Database)
- GitHub Actions (CI/CD)
- C# / ASP.NET Core (Part 2 - API)
- MVC / Razor Pages (Part 3 - Web App)
- Azure Blob Storage (Part 3)
- Docker (Part 3)

## Author

[tebogo mashego] - PROG6212 Programming 2B
