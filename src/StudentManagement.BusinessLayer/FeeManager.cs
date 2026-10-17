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
                ? FormatFeePeriodRange(request.FeePeriods)
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
                var today = DateTime.Today;
                var monthStart = new DateTime(today.Year, today.Month, 1);
                var payments = OfflineStore.Instance.Payments;
                var todayPayments = payments.Where(p => p.PaymentDate.Date == today).ToList();
                var monthPayments = payments.Where(p => p.PaymentDate.Date >= monthStart).ToList();
                var tuitionInv = OfflineStore.Instance.Ledger
                    .Where(l => l.DebitAmount > 0 && (l.Particulars ?? "").IndexOf("Tuition", StringComparison.OrdinalIgnoreCase) >= 0)
                    .Sum(l => l.DebitAmount);
                var ictInv = OfflineStore.Instance.Ledger
                    .Where(l => l.DebitAmount > 0 && (l.Particulars ?? "").IndexOf("ICT", StringComparison.OrdinalIgnoreCase) >= 0)
                    .Sum(l => l.DebitAmount);
                var examInv = OfflineStore.Instance.Ledger
                    .Where(l => l.DebitAmount > 0 && (l.Particulars ?? "").IndexOf("Exam", StringComparison.OrdinalIgnoreCase) >= 0)
                    .Sum(l => l.DebitAmount);

                return new DashboardStats
                {
                    ActiveStudents = students.Count,
                    ClearStudents = clear,
                    StudentsWithDues = withDues,
                    TotalReceivables = students.Where(s => s.NetDue > 0).Sum(s => s.NetDue),
                    FeeCollectionRatePct = students.Count == 0 ? 0 : (int)Math.Round(clear * 100.0 / students.Count),
                    CollectionsToday = todayPayments.Sum(p => p.AmountPaid),
                    ReceiptsToday = todayPayments.Count,
                    CollectionsThisMonth = monthPayments.Sum(p => p.AmountPaid),
                    ClearedForExam = clear,
                    AdmitCardsIssued = 0,
                    TuitionInvoiced = tuitionInv,
                    IctInvoiced = ictInv,
                    ExamInvoiced = examInv
                };
            }

            var dt = new tblFee().GetDashboardStats();
            if (dt.Rows.Count == 0) return new DashboardStats();
            var r = dt.Rows[0];
            return new DashboardStats
            {
                ActiveStudents = ColInt(r, "ActiveStudents"),
                ClearStudents = ColInt(r, "ClearStudents"),
                StudentsWithDues = ColInt(r, "StudentsWithDues"),
                TotalReceivables = ColDec(r, "TotalReceivables"),
                FeeCollectionRatePct = ColInt(r, "FeeCollectionRatePct"),
                CollectionsToday = ColDec(r, "CollectionsToday"),
                ReceiptsToday = ColInt(r, "ReceiptsToday"),
                CollectionsThisMonth = ColDec(r, "CollectionsThisMonth"),
                ClearedForExam = ColInt(r, "ClearedForExam"),
                AdmitCardsIssued = ColInt(r, "AdmitCardsIssued"),
                TuitionInvoiced = ColDec(r, "TuitionInvoiced"),
                IctInvoiced = ColDec(r, "IctInvoiced"),
                ExamInvoiced = ColDec(r, "ExamInvoiced")
            };
        }

        private static int ColInt(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? Convert.ToInt32(r[col]) : 0;

        private static decimal ColDec(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? Convert.ToDecimal(r[col]) : 0m;

        public static List<ReceiptLineItemDto> BuildReceiptLines(FeeCollectionRequest request, List<NamedFeeAmount>? otherFees = null)
        {
            var lines = new List<ReceiptLineItemDto>();
            otherFees ??= (request.OtherFees ?? new List<NamedFeeAmount>())
                .Where(f => f != null && f.Amount > 0 && !string.IsNullOrWhiteSpace(f.Name)).ToList();

            if (request.TuitionAmount > 0 && request.FeePeriods.Count > 0)
            {
                // One line for all selected months: "Tuition Fee (January - March 2026)" + total.
                lines.Add(new ReceiptLineItemDto
                {
                    Description = $"Tuition Fee ({FormatFeePeriodRange(request.FeePeriods)})",
                    Amount = request.TuitionAmount
                });
            }

            foreach (var fee in otherFees)
            {
                var name = fee.Name.Trim();
                // Transport already totals selected months; annotate period when months exist.
                if (name.Equals("Monthly Transport Fee", StringComparison.OrdinalIgnoreCase)
                    && request.FeePeriods.Count > 0)
                {
                    name = $"Monthly Transport Fee ({FormatFeePeriodRange(request.FeePeriods)})";
                }
                lines.Add(new ReceiptLineItemDto { Description = name, Amount = fee.Amount });
            }

            if (request.FineAmount > 0)
                lines.Add(new ReceiptLineItemDto { Description = "Late Fine / Penalty", Amount = request.FineAmount });

            if (request.WaiverAmount > 0)
                lines.Add(new ReceiptLineItemDto { Description = "Waiver / Discount", Amount = request.WaiverAmount, IsCredit = true });

            return lines;
        }

        public static string FormatPaymentMethod(PaymentMethod method)
        {
            try
            {
                var match = new AcademicManager().GetPaymentMethods()
                    .FirstOrDefault(p => p.PaymentMethodId == (int)method);
                if (match != null && !string.IsNullOrWhiteSpace(match.Name))
                    return match.Name;
            }
            catch
            {
                // Fall back to enum label if lookup table is unavailable.
            }

            return EnumDisplay.PaymentMethodLabel(method);
        }

        public static string FormatFeePeriod(string period)
        {
            if (string.IsNullOrWhiteSpace(period)) return period;
            if (DateTime.TryParseExact(period.Trim(), "MMM-yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var dt))
                return dt.ToString("MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
            return period.Trim();
        }

        /// <summary>
        /// Collapses selected months into one label, e.g. "January - March 2026".
        /// </summary>
        public static string FormatFeePeriodRange(IEnumerable<string> periods)
        {
            var months = (periods ?? Enumerable.Empty<string>())
                .Select(p =>
                {
                    if (DateTime.TryParseExact(
                            (p ?? string.Empty).Trim(),
                            "MMM-yyyy",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out var dt))
                        return (DateTime?)dt;
                    return null;
                })
                .Where(d => d.HasValue)
                .Select(d => d!.Value)
                .OrderBy(d => d)
                .ToList();

            if (months.Count == 0)
                return string.Join(", ", periods ?? Enumerable.Empty<string>());
            if (months.Count == 1)
                return months[0].ToString("MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture);

            var first = months[0];
            var last = months[months.Count - 1];
            var culture = System.Globalization.CultureInfo.InvariantCulture;

            if (first.Year == last.Year)
                return $"{first.ToString("MMMM", culture)} - {last.ToString("MMMM yyyy", culture)}";

            return $"{first.ToString("MMMM yyyy", culture)} - {last.ToString("MMMM yyyy", culture)}";
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

        /// <summary>
        /// Transport bills from the calendar month AFTER activation (not the activation month).
        /// </summary>
        public static bool TryParseFeePeriod(string period, out DateTime monthStart)
        {
            monthStart = default;
            if (string.IsNullOrWhiteSpace(period))
                return false;
            return DateTime.TryParseExact(
                period.Trim(),
                "MMM-yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out monthStart);
        }

        public static DateTime FirstTransportBillMonth(DateTime activatedOn)
            => new DateTime(activatedOn.Year, activatedOn.Month, 1).AddMonths(1);

        public static int CountBillableTransportMonths(
            IEnumerable<string> feePeriods,
            bool transportActive,
            DateTime? transportActivatedOn)
        {
            if (!transportActive || !transportActivatedOn.HasValue)
                return 0;

            var firstBillable = FirstTransportBillMonth(transportActivatedOn.Value);
            var count = 0;
            foreach (var period in feePeriods)
            {
                if (TryParseFeePeriod(period, out var monthStart) && monthStart >= firstBillable)
                    count++;
            }
            return count;
        }

        public static decimal CalculateAutoTransportFee(
            StudentDto student,
            IEnumerable<string> feePeriods)
        {
            if (student == null || !student.TransportActive || student.MonthlyTransportFee <= 0)
                return 0;
            var months = CountBillableTransportMonths(
                feePeriods, student.TransportActive, student.TransportActivatedOn);
            return student.MonthlyTransportFee * months;
        }

        public System.Threading.Tasks.Task<FeeCollectionResult> CollectAsync(FeeCollectionRequest request, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(Collect(request));
        public System.Threading.Tasks.Task<LedgerSummaryDto> GetSummaryAsync(int studentId, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetSummary(studentId));
        public System.Threading.Tasks.Task<System.Collections.Generic.List<LedgerEntryDto>> GetLedgerDetailedAsync(int studentId, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetLedgerDetailed(studentId));
        public System.Threading.Tasks.Task<DashboardStats> GetDashboardStatsAsync(System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetDashboardStats());

    }
}
