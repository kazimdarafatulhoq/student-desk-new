using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using StudentManagement.BusinessLayer.Offline;
using StudentManagement.DataAccessLayer;
using StudentManagement.DataAccessLayer.Data_Access_Methods;
using StudentManagement.UserObjects.Enums;
using StudentManagement.UserObjects.Objects;

namespace StudentManagement.BusinessLayer
{
    public class FeeManager
    {
        public FeeCollectionResult Collect(FeeCollectionRequest request)
        {
            if (request.StudentId <= 0)
                throw new ArgumentException("StudentDto is required.");

            var otherFees = (request.OtherFees ?? new List<NamedFeeAmount>())
                .Where(f => f != null && f.Amount > 0 && !string.IsNullOrWhiteSpace(f.Name))
                .ToList();
            var otherTotal = otherFees.Sum(f => f.Amount);

            if (request.FeePeriods.Count == 0 && otherTotal <= 0)
                throw new ArgumentException("Select at least one tuition month or enter an optional fee amount.");

            var netPayable = request.TuitionAmount + request.FineAmount - request.WaiverAmount + otherTotal;
            if (netPayable <= 0)
                throw new ArgumentException("Net payable amount must be greater than zero.");

            var student = new StudentManager().GetById(request.StudentId)
                          ?? throw new InvalidOperationException("Student not found.");

            var periods = request.FeePeriods.Count > 0
                ? string.Join(", ", request.FeePeriods)
                : "Other Charges";

            var otherNote = otherFees.Count > 0
                ? " | " + string.Join(", ", otherFees.Select(f => $"{f.Name}: ৳{f.Amount:N2}"))
                : string.Empty;
            var remarks = string.IsNullOrWhiteSpace(request.Remarks)
                ? otherNote.TrimStart(' ', '|')
                : request.Remarks.Trim() + otherNote;

            string receiptNo;
            long collectionId;

            if (DbConfig.OfflineMode)
            {
                var store = OfflineStore.Instance;
                receiptNo = $"RCP-{DateTime.Now:yyyyMM}-{store.Payments.Count + 1:D5}";
                collectionId = store.NextPaymentId++;

                if (request.TuitionAmount > 0)
                {
                    store.Ledger.Add(new LedgerEntryDto
                    {
                        LedgerId = store.NextLedgerId++,
                        StudentId = request.StudentId,
                        TransactionDate = DateTime.Now,
                        VoucherNo = $"INV-{receiptNo}",
                        Particulars = $"Tuition Fee ({periods})",
                        FeePeriod = periods,
                        DebitAmount = request.TuitionAmount + request.FineAmount,
                        Status = LedgerStatus.Open,
                        EntryType = LedgerEntryType.Invoice
                    });
                }

                store.Ledger.Add(new LedgerEntryDto
                {
                    LedgerId = store.NextLedgerId++,
                    StudentId = request.StudentId,
                    TransactionDate = DateTime.Now,
                    VoucherNo = receiptNo,
                    Particulars = $"Fee Collection via {request.PaymentMethod}",
                    FeePeriod = periods,
                    CreditAmount = netPayable,
                    Status = LedgerStatus.Paid,
                    EntryType = LedgerEntryType.Payment
                });

                if (request.WaiverAmount > 0)
                {
                    store.Ledger.Add(new LedgerEntryDto
                    {
                        LedgerId = store.NextLedgerId++,
                        StudentId = request.StudentId,
                        TransactionDate = DateTime.Now,
                        VoucherNo = $"WVR-{receiptNo}",
                        Particulars = "Waiver / Discount",
                        FeePeriod = periods,
                        CreditAmount = request.WaiverAmount,
                        Status = LedgerStatus.Waived,
                        EntryType = LedgerEntryType.Waiver
                    });
                }

                student.NetDue = Math.Max(0, student.NetDue - netPayable - request.WaiverAmount);
            }
            else
            {
                var tbl = new tblFee();
                if (request.TuitionAmount > 0 && request.FeePeriods.Count > 0)
                {
                    tbl.PostTuitionInvoice(request.StudentId, periods, request.TuitionAmount,
                        "Tuition Fee", request.FineAmount, request.CollectedByUserId);
                }

                foreach (var fee in otherFees)
                {
                    tbl.PostTuitionInvoice(request.StudentId, periods, fee.Amount,
                        fee.Name.Trim(), 0, request.CollectedByUserId);
                }

                var payment = tbl.RecordFeePayment(
                    request.StudentId, netPayable, request.PaymentMethod, periods,
                    request.FineAmount, request.WaiverAmount, request.TransactionRef,
                    string.IsNullOrWhiteSpace(remarks) ? null : remarks,
                    request.CollectedByUserId);
                receiptNo = payment.ReceiptNo;
                collectionId = payment.CollectionId;
            }

            return new FeeCollectionResult
            {
                CollectionId = collectionId,
                ReceiptNo = receiptNo!,
                AmountPaid = netPayable,
                PaymentDate = DateTime.Now,
                StudentName = student.FullName,
                RegistrationNo = student.RegistrationNo,
                FeePeriods = periods,
                PaymentMethod = request.PaymentMethod,
                PaymentMethodDisplay = FormatPaymentMethod(request.PaymentMethod),
                LineItems = BuildReceiptLines(request, otherFees)
            };
        }

        public LedgerSummaryDto GetSummary(int studentId)
        {
            if (DbConfig.OfflineMode)
            {
                var student = OfflineStore.Instance.FindStudent(studentId);
                var entries = OfflineStore.Instance.Ledger.Where(l => l.StudentId == studentId).ToList();
                var debit = entries.Sum(e => e.DebitAmount);
                var credit = entries.Sum(e => e.CreditAmount);
                var bal = debit - credit;
                return new LedgerSummaryDto
                {
                    StudentId = studentId,
                    RegistrationNo = student?.RegistrationNo ?? "",
                    FullName = student?.FullName ?? "",
                    TotalInvoiced = debit,
                    TotalPaid = credit,
                    AdvanceBalance = bal < 0 ? Math.Abs(bal) : 0,
                    NetDue = bal > 0 ? bal : 0
                };
            }

            var ds = new tblFee().GetStudentLedger(studentId);
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                var student = new StudentManager().GetById(studentId);
                return new LedgerSummaryDto
                {
                    StudentId = studentId,
                    RegistrationNo = student?.RegistrationNo ?? "",
                    FullName = student?.FullName ?? ""
                };
            }

            var header = ds.Tables[0].Rows[0];
            return new LedgerSummaryDto
            {
                StudentId = studentId,
                RegistrationNo = header.Table.Columns.Contains("RegistrationNo")
                    ? Convert.ToString(header["RegistrationNo"]) ?? "" : "",
                FullName = header.Table.Columns.Contains("FullName")
                    ? Convert.ToString(header["FullName"]) ?? "" : "",
                TotalInvoiced = header.Table.Columns.Contains("TotalDebit") && header["TotalDebit"] != DBNull.Value
                    ? Convert.ToDecimal(header["TotalDebit"])
                    : header.Table.Columns.Contains("TotalInvoiced") && header["TotalInvoiced"] != DBNull.Value
                        ? Convert.ToDecimal(header["TotalInvoiced"]) : 0,
                TotalPaid = header.Table.Columns.Contains("TotalCredit") && header["TotalCredit"] != DBNull.Value
                    ? Convert.ToDecimal(header["TotalCredit"])
                    : header.Table.Columns.Contains("TotalPaid") && header["TotalPaid"] != DBNull.Value
                        ? Convert.ToDecimal(header["TotalPaid"]) : 0,
                NetDue = header.Table.Columns.Contains("NetDue") && header["NetDue"] != DBNull.Value
                    ? Convert.ToDecimal(header["NetDue"]) : 0,
                AdvanceBalance = header.Table.Columns.Contains("AdvanceBalance") && header["AdvanceBalance"] != DBNull.Value
                    ? Convert.ToDecimal(header["AdvanceBalance"]) : 0
            };
        }

        public List<LedgerEntryDto> GetLedgerDetailed(int studentId)
        {
            if (DbConfig.OfflineMode)
            {
                decimal running = 0;
                var list = new List<LedgerEntryDto>();
                foreach (var e in OfflineStore.Instance.Ledger.Where(l => l.StudentId == studentId)
                             .OrderBy(l => l.TransactionDate).ThenBy(l => l.LedgerId))
                {
                    running += e.DebitAmount - e.CreditAmount;
                    list.Add(new LedgerEntryDto
                    {
                        LedgerId = e.LedgerId,
                        TransactionDate = e.TransactionDate,
                        VoucherNo = e.VoucherNo,
                        Particulars = e.Particulars,
                        FeePeriod = e.FeePeriod,
                        DebitAmount = e.DebitAmount,
                        CreditAmount = e.CreditAmount,
                        RunningBalance = running,
                        Status = e.Status,
                        EntryType = e.EntryType
                    });
                }
                return list;
            }

            var ds = new tblFee().GetStudentLedger(studentId);
            if (ds.Tables.Count < 2) return new List<LedgerEntryDto>();

            return ds.Tables[1].AsEnumerable().Select(r => new LedgerEntryDto
            {
                LedgerId = Convert.ToInt64(r["LedgerId"]),
                TransactionDate = Convert.ToDateTime(r["TransactionDate"]),
                VoucherNo = Convert.ToString(r["VoucherNo"]) ?? "",
                Particulars = Convert.ToString(r["Particulars"]) ?? "",
                FeePeriod = r.Table.Columns.Contains("FeePeriod") ? Convert.ToString(r["FeePeriod"]) : null,
                DebitAmount = Convert.ToDecimal(r["DebitAmount"]),
                CreditAmount = Convert.ToDecimal(r["CreditAmount"]),
                RunningBalance = r.Table.Columns.Contains("RunningBalance") && r["RunningBalance"] != DBNull.Value
                    ? Convert.ToDecimal(r["RunningBalance"]) : 0,
                Status = r.Table.Columns.Contains("Status") && r["Status"] != DBNull.Value
                    ? (LedgerStatus)Convert.ToInt32(r["Status"]) : LedgerStatus.Open,
                EntryType = r.Table.Columns.Contains("EntryType") && r["EntryType"] != DBNull.Value
                    ? (LedgerEntryType)Convert.ToInt32(r["EntryType"]) : LedgerEntryType.Invoice
            }).ToList();
        }

        public DashboardStats GetDashboardStats()
        {
            if (DbConfig.OfflineMode)
            {
                var students = OfflineStore.Instance.Students.Where(s => s.Status == StudentStatus.Active).ToList();
                var clear = students.Count(s => s.NetDue <= 0);
                var withDues = students.Count(s => s.NetDue > 0);
                return new DashboardStats
                {
                    ActiveStudents = students.Count,
                    ClearStudents = clear,
                    StudentsWithDues = withDues,
                    TotalReceivables = students.Sum(s => s.NetDue),
                    FeeCollectionRatePct = students.Count == 0 ? 0 : (int)Math.Round(clear * 100.0 / students.Count),
                    CollectionsToday = 0,
                    CollectionsThisMonth = 0
                };
            }

            var dt = new tblFee().GetDashboardStats();
            if (dt.Rows.Count == 0) return new DashboardStats();
            var r = dt.Rows[0];
            return new DashboardStats
            {
                ActiveStudents = Convert.ToInt32(r["ActiveStudents"]),
                ClearStudents = Convert.ToInt32(r["ClearStudents"]),
                StudentsWithDues = Convert.ToInt32(r["StudentsWithDues"]),
                TotalReceivables = Convert.ToDecimal(r["TotalReceivables"]),
                FeeCollectionRatePct = Convert.ToInt32(r["FeeCollectionRatePct"]),
                CollectionsToday = Convert.ToDecimal(r["CollectionsToday"]),
                CollectionsThisMonth = Convert.ToDecimal(r["CollectionsThisMonth"])
            };
        }

        public static List<ReceiptLineItemDto> BuildReceiptLines(FeeCollectionRequest request, List<NamedFeeAmount>? otherFees = null)
        {
            var lines = new List<ReceiptLineItemDto>();
            otherFees ??= (request.OtherFees ?? new List<NamedFeeAmount>())
                .Where(f => f != null && f.Amount > 0 && !string.IsNullOrWhiteSpace(f.Name)).ToList();

            if (request.TuitionAmount > 0 && request.FeePeriods.Count > 0)
            {
                var unit = Math.Round(request.TuitionAmount / request.FeePeriods.Count, 2);
                var remainder = request.TuitionAmount - (unit * request.FeePeriods.Count);
                for (var i = 0; i < request.FeePeriods.Count; i++)
                {
                    var amount = unit + (i == request.FeePeriods.Count - 1 ? remainder : 0);
                    if (amount <= 0) continue;
                    lines.Add(new ReceiptLineItemDto
                    {
                        Description = $"Tuition Fee ({FormatFeePeriod(request.FeePeriods[i])})",
                        Amount = amount
                    });
                }
            }

            foreach (var fee in otherFees)
                lines.Add(new ReceiptLineItemDto { Description = fee.Name.Trim(), Amount = fee.Amount });

            if (request.FineAmount > 0)
                lines.Add(new ReceiptLineItemDto { Description = "Late Fine / Penalty", Amount = request.FineAmount });

            if (request.WaiverAmount > 0)
                lines.Add(new ReceiptLineItemDto { Description = "Waiver / Discount", Amount = request.WaiverAmount, IsCredit = true });

            return lines;
        }

        public static string FormatPaymentMethod(PaymentMethod method) => EnumDisplay.PaymentMethodLabel(method);

        public static string FormatFeePeriod(string period)
        {
            if (string.IsNullOrWhiteSpace(period)) return period;
            if (DateTime.TryParseExact(period.Trim(), "MMM-yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var dt))
                return dt.ToString("MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
            return period.Trim();
        }

        public static decimal CalculateNetPayable(decimal tuition, decimal fine, decimal waiver, decimal otherFees = 0)
            => Math.Max(0, tuition + fine + otherFees - waiver);

        public static List<string> BuildMonthMatrix(int year, int startMonth = 1)
        {
            var months = new List<string>();
            for (var m = startMonth; m <= 12; m++)
                months.Add(new DateTime(year, m, 1).ToString("MMM-yyyy"));
            return months;
        }

        public System.Threading.Tasks.Task<FeeCollectionResult> CollectAsync(FeeCollectionRequest request, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(Collect(request));
        public System.Threading.Tasks.Task<LedgerSummaryDto> GetSummaryAsync(int studentId, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetSummary(studentId));
        public System.Threading.Tasks.Task<System.Collections.Generic.List<LedgerEntryDto>> GetLedgerDetailedAsync(int studentId, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetLedgerDetailed(studentId));

    }
}
