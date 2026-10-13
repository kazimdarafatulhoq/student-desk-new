# Student Management & Financial Accounting

WinForms desktop app for Ideal High School — admissions, fee collection, ledger, exam clearance, and admit cards.

Architecture matches a classic **DyeERP-style** layout (Form → Manager → tbl → Stored Procedure).

## Solution structure

| Project | Like DyeERP | Responsibility |
|---------|-------------|----------------|
| `StudentManagement.Desktop` | `DyeERP.Bangladesh` | WinForms UI (`frm…`) |
| `StudentManagement.BusinessLayer` | `DyeERP.BusinessLayer` | `StudentManager`, `FeeManager`, `AuthManager`, `ExamManager` |
| `StudentManagement.DataAccessLayer` | `DyeERP.DataAccessLayer` | `Execute`, `tblStudent`, `tblFee`, `tblUser`, `tblExam`, `tblAcademic` |
| `StudentManagement.UserObjects` | `DyeERP.UserObjects` | POCOs / DTOs + enums |
| `StudentManagement.Reporting` | (reports) | QuestPDF receipts / admit cards / ledger CSV |

### Call flow (same idea as DyeERP)

```
Form  →  new StudentManager()  →  new tblStudent()  →  Execute SP  →  DataTable  →  List<StudentDto>
```

Example:

```csharp
StudentManager objStudentManager = new StudentManager();
List<StudentDto> lst = objStudentManager.SearchStudents(filter);
```

No DI container. Forms create managers with `new`.

## Prerequisites

- Windows 10/11
- [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
- SQL Server 2022
- Visual Studio 2019/2022 (or `dotnet` CLI)

## Database setup

1. Run [`database/CreateDatabaseAndGrantAccess.sql`](database/CreateDatabaseAndGrantAccess.sql)
2. Run [`database/StudentManagementSDB.sql`](database/StudentManagementSDB.sql)
3. Run [`database/StoredProcedures_StudentSave.sql`](database/StoredProcedures_StudentSave.sql)
4. Set connection string in `src/StudentManagement.Desktop/App.config`
5. Set `OfflineMode` to `false` when SQL Server is available

Saves go through stored procedures: `usp_AdmitStudent`, `usp_UpdateStudent`, `usp_SetStudentStatus`, `usp_PostTuitionInvoice`, `usp_RecordFeePayment`.

```
Data Source=NGBC-IT-168\MSSQLSERVER1;Initial Catalog=StudentManagementSDB;User Id=anik;Password=***;TrustServerCertificate=True;MultipleActiveResultSets=True;
```

### Default login

| Username | Password | Role |
|----------|----------|------|
| `admin` | `Admin@123` | Super Admin |
| `accounts` | `Accounts@123` | Accounts Manager |
| `admission` | `Admission@123` | Admission Officer |
| `teacher` | `Teacher@123` | Teacher |

## Run

```bash
dotnet restore StudentManagement.sln
dotnet build StudentManagement.sln -c Release
dotnet run --project src/StudentManagement.Desktop -c Release
```

Or open `StudentManagement.sln` and set **StudentManagement.Desktop** as startup.

`OfflineMode=true` in App.config runs with in-memory demo data (no SQL Server).

## Modules

- Student Admission / Edit Profile / Mark Inactive  
- Student Directory (search + class filter)  
- Fee Collection Counter + receipt preview/PDF  
- Student Ledger  
- Exam Clearance Verification  
- Admit Card single / bulk print  
- User Access & Security  
