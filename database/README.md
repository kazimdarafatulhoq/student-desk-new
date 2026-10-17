# Database scripts — StudentManagementSDB

SQL Server 2022 scripts for Ideal High School Student Management.

## Run order (SSMS as `sa` / Administrator)

1. [`CreateDatabaseAndGrantAccess.sql`](CreateDatabaseAndGrantAccess.sql) — creates empty `StudentManagementSDB` and grants login `[anik]`.
2. [`StudentManagementSDB.sql`](StudentManagementSDB.sql) — tables, indexes, views, stored procedures, demo seed.
3. [`StoredProcedures_StudentSave.sql`](StoredProcedures_StudentSave.sql) — admit / update / status SPs used by Save & Edit Profile (run after schema, or again after upgrades).

`StudentManagementDB.sql` is a stub that points here (old name). Do not use it for schema.

## Connection string

Must match the desktop app:

```
Data Source=NGBC-IT-168\MSSQLSERVER1;Initial Catalog=StudentManagementSDB;User Id=anik;Password=***;TrustServerCertificate=True;MultipleActiveResultSets=True;
```

Set `OfflineMode` to `false` in `App.config` / `appsettings.json` when SQL Server is available.

## Objects created

Schema mirrors the WinForms modules: Admission, Fee Collection Counter (month matrix + optional fees), Student Ledger, Exam Clearance, Admit Card, Dashboard, and User Management.

### Tables

| Table | Purpose |
|-------|---------|
| `InstitutionSettings` | Ideal High School & College / campus / session (matches AppSession) |
| `Users` | App logins / RBAC |
| `Classes` / `Sections` | Academic structure (Class 6–10, A/B) |
| `Students` | Admission form fields |
| `FeeCategories` | Tuition + Fee Collection optional fees + ICT (dashboard) |
| `StudentFeeStructures` | Per-student fee amounts |
| `StudentLedger` | Debit/credit accounting |
| `FeeInvoices` / `FeePayments` | Billing & POS receipts |
| `ExamTerms` / `ExamClearances` / `AdmitCards` | Exam flow |
| `NumberSequences` | Atomic REG / RCP / INV counters |

### Fee categories (designer labels)

Tuition Fee · Registration Fee · New Admission / Re-admission · Monthly Transport Fee · Examination Fee (Term / Annual) · ICT & Computer Lab Fees · Transcript / Testimonial / Certificate · Transfer Certificate / Letter · Hostel Food Charges · Miscellaneous

### Views

- `vw_StudentLedgerDetailed` — ledger lines with running balance
- `vw_StudentBalanceSummary` — net due per student
- `vw_ActiveStudents` — active enrollment join

### Stored procedures

| Procedure | Use |
|-----------|-----|
| `usp_GetNextRegistrationNo` | `REG-YYYY-####` |
| `usp_GetNextReceiptNo` | `RCP-yyyyMM-#####` |
| `usp_GetNextInvoiceNo` | `INV-yyyyMM-#####` |
| `usp_GetNextAdmitCardNo` | Admit card + barcode |
| `usp_GetStudentBalance` | Net due header |
| `usp_GetStudentLedger` | Header + detail |
| `usp_GetDashboardStats` | Dashboard KPIs |
| `usp_SearchStudents` | Search / filter |
| `usp_GetOutstandingDues` | Receivables list |
| `usp_GetClassSectionLookup` | Class/section picker |
| `usp_PostTuitionInvoice` | Post invoice + ledger (**Fee Collection** save) |
| `usp_RecordFeePayment` | Post payment (+ optional waiver) (**Fee Collection** save) |
| `usp_AdmitStudent` | Admit student + opening tuition invoice (**Admission** save); collects transport waitlist |
| `usp_UpdateStudent` | Edit student profile (**Edit Profile** save); activate Gari seat |
| `usp_SetStudentStatus` | Mark inactive / change status |

**Transport (Gari):** admit with `WantTransport` (waitlist). Activate later via Edit (`TransportActive`). Fee Collection auto-adds `Monthly Transport Fee` for selected months **after** the activation month.

Existing databases: run `Alter_Students_Transport.sql`, then re-run `StoredProcedures_StudentSave.sql`.

The desktop app persists **admission, profile edit, status change, and fee collection** through these stored procedures (not EF entity inserts for those paths).

### Quick checks after install

```sql
USE StudentManagementSDB;
EXEC dbo.usp_GetDashboardStats;
EXEC dbo.usp_SearchStudents @Term = N'Ayesha';
EXEC dbo.usp_GetStudentLedger @RegistrationNo = N'REG-2026-0001';
```

### Default users (passwords fixed on first app start)

| Username | Password | Role |
|----------|----------|------|
| `admin` | `Admin@123` | Super Admin |
| `accounts` | `Accounts@123` | Accounts Manager |
| `admission` | `Admission@123` | Admission Officer |
| `teacher` | `Teacher@123` | Teacher |

SQL seed password hashes are placeholders. `DatabaseBootstrapper` re-hashes them when the desktop app starts against this database.
