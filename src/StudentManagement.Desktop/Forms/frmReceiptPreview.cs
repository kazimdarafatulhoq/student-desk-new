using StudentManagement.UserObjects.Objects;
using StudentManagement.Desktop.Helpers;
using StudentManagement.Reporting.Generators;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Forms
{
    /// <summary>
    /// On-screen money receipt preview (admit-card style): review line items, then Print Copy.
    /// </summary>
    [DesignerCategory("Form")]
    public partial class frmReceiptPreview : Form
    {
        private static readonly Color Navy = Color.FromArgb(15, 23, 42);
        private static readonly Color AccentBlue = Color.FromArgb(37, 99, 235);
        private static readonly Color Muted = Color.FromArgb(100, 116, 139);
        private static readonly Color BoxBg = Color.FromArgb(241, 245, 249);
        private static readonly Color Line = Color.FromArgb(203, 213, 225);
        private static readonly Color HeaderBar = Color.FromArgb(51, 65, 85);
        private static readonly Color CreditBg = Color.FromArgb(220, 252, 231);
        private static readonly Color CreditFg = Color.FromArgb(22, 101, 52);

        private readonly FeeCollectionResult _receipt;
        private readonly string _institution;
        private readonly string _campus;
        private string? _savedPdfPath;

        public frmReceiptPreview()
        {
            InitializeComponent();
            
            if (DesignTime.IsActive)
                return;
            _receipt = new FeeCollectionResult();
                        _institution = AppSession.InstitutionName;
                        _campus = AppSession.InstitutionCampus;
        }

        public frmReceiptPreview(FeeCollectionResult receipt, string? institutionName = null, string? campusAddress = null)
        {
            InitializeComponent();
            _receipt = receipt ?? new FeeCollectionResult();
            _institution = string.IsNullOrWhiteSpace(institutionName) ? AppSession.InstitutionName : institutionName!;
            _campus = string.IsNullOrWhiteSpace(campusAddress) ? AppSession.InstitutionCampus : campusAddress!;
            Load += (_, _) => BuildPreview();
            Resize += (_, _) => LayoutFooterButtons();
        }

        public string? SavedPdfPath => _savedPdfPath;

        private void LayoutFooterButtons()
        {
            btnPrint.Left = pnlFooter.Width - btnPrint.Width - 20;
            btnClose.Left = btnPrint.Left - btnClose.Width - 12;
        }

        private void BuildPreview()
        {
            pnlReceiptHost.Controls.Clear();
            lblCaption.Text = "Money Receipt Preview — review charges, then Print Copy";

            var card = CreateReceiptCard();
            pnlReceiptHost.Controls.Add(card);
            pnlReceiptHost.Width = Math.Max(720, pnlScrollHost.ClientSize.Width - 24);
            pnlReceiptHost.Height = card.Bottom + 24;
            card.Left = Math.Max(8, (pnlReceiptHost.Width - card.Width) / 2);

            LayoutFooterButtons();
        }

        private Panel CreateReceiptCard()
        {
            var lines = (_receipt.LineItems ?? new System.Collections.Generic.List<ReceiptLineItemDto>())
                .Where(l => l != null && l.Amount > 0 && !string.IsNullOrWhiteSpace(l.Description))
                .ToList();

            int rowH = 36;
            int tableBodyH = Math.Max(rowH, lines.Count * rowH);
            int cardH = 280 + 40 + tableBodyH + 56 + 24;

            var card = new Panel
            {
                Width = 560,
                Height = cardH,
                BackColor = Color.White,
                Margin = new Padding(16),
                Padding = new Padding(28)
            };
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(Line, 1);
                e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };

            int y = 24;
            int contentW = 504;
            int left = 28;

            var lblSchool = MakeLabel(_institution.ToUpperInvariant(), 13, FontStyle.Bold, Navy, true);
            lblSchool.Location = new Point(left, y);
            lblSchool.Size = new Size(contentW, 24);
            card.Controls.Add(lblSchool);
            y += 26;

            var campusText = string.IsNullOrWhiteSpace(_campus)
                ? "Dhanmondi Campus, Dhaka-1205, Bangladesh"
                : (_campus.IndexOf("Bangladesh", StringComparison.OrdinalIgnoreCase) >= 0
                    ? _campus
                    : _campus + ", Bangladesh");
            var lblCampus = MakeLabel(campusText, 9, FontStyle.Regular, Muted, true);
            lblCampus.Location = new Point(left, y);
            lblCampus.Size = new Size(contentW, 18);
            card.Controls.Add(lblCampus);
            y += 28;

            var lblTitle = MakeLabel("OFFICIAL MONEY RECEIPT", 12, FontStyle.Bold, AccentBlue, true);
            lblTitle.Location = new Point(left, y);
            lblTitle.Size = new Size(contentW, 22);
            card.Controls.Add(lblTitle);
            y += 32;

            // Info box
            var infoBox = new Panel
            {
                BackColor = BoxBg,
                Location = new Point(left, y),
                Size = new Size(contentW, 118)
            };
            infoBox.Paint += (s, e) =>
            {
                using var path = RoundRect(0, 0, infoBox.Width - 1, infoBox.Height - 1, 8);
                using var pen = new Pen(Line);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawPath(pen, path);
            };
            card.Controls.Add(infoBox);

            int iy = 12;
            AddInfoRow(infoBox, "Receipt No:", _receipt.ReceiptNo, Navy, 12, ref iy);
            AddInfoRow(infoBox, "Student Name:", _receipt.StudentName, Navy, 12, ref iy);
            AddInfoRow(infoBox, "Registration No:", _receipt.RegistrationNo, Navy, 12, ref iy);
            AddInfoRow(infoBox, "Date & Time:", _receipt.PaymentDate.ToString("M/d/yyyy, h:mm:ss tt"), Muted, 12, ref iy, boldValue: false);
            var method = string.IsNullOrWhiteSpace(_receipt.PaymentMethodDisplay)
                ? _receipt.PaymentMethod.ToString()
                : _receipt.PaymentMethodDisplay;
            AddInfoRow(infoBox, "Payment Method:", method, Muted, 12, ref iy, boldValue: false);
            y += infoBox.Height + 18;

            // Fee table
            var table = new Panel
            {
                Location = new Point(left, y),
                Size = new Size(contentW, 40 + tableBodyH + 44),
                BackColor = Color.White
            };
            table.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundRect(0, 0, table.Width - 1, table.Height - 1, 8);
                using var pen = new Pen(Line);
                e.Graphics.DrawPath(pen, path);
            };
            card.Controls.Add(table);

            var header = new Panel
            {
                BackColor = HeaderBar,
                Location = new Point(1, 1),
                Size = new Size(contentW - 2, 36)
            };
            var lblDescH = MakeLabel("DESCRIPTION", 8.5f, FontStyle.Bold, Color.White, false);
            lblDescH.Location = new Point(14, 9);
            lblDescH.Size = new Size(280, 18);
            header.Controls.Add(lblDescH);
            var lblAmtH = MakeLabel("AMOUNT (৳)", 8.5f, FontStyle.Bold, Color.White, false);
            lblAmtH.Location = new Point(contentW - 150, 9);
            lblAmtH.Size = new Size(120, 18);
            lblAmtH.TextAlign = ContentAlignment.MiddleRight;
            header.Controls.Add(lblAmtH);
            table.Controls.Add(header);

            int ty = 38;
            foreach (var item in lines)
            {
                var row = new Panel
                {
                    Location = new Point(1, ty),
                    Size = new Size(contentW - 2, rowH),
                    BackColor = item.IsCredit ? CreditBg : Color.White
                };
                var descColor = item.IsCredit ? CreditFg : Navy;
                var amtColor = item.IsCredit ? CreditFg : Navy;
                var lblD = MakeLabel(item.Description, 9.5f, item.IsCredit ? FontStyle.Bold : FontStyle.Regular, descColor, false);
                lblD.Location = new Point(14, 8);
                lblD.Size = new Size(320, 20);
                row.Controls.Add(lblD);

                var lblA = MakeLabel($"৳ {item.Amount:N0}", 9.5f, FontStyle.Bold, amtColor, false);
                lblA.Location = new Point(contentW - 150, 8);
                lblA.Size = new Size(120, 20);
                lblA.TextAlign = ContentAlignment.MiddleRight;
                row.Controls.Add(lblA);

                // separator
                row.Paint += (s, e) =>
                {
                    using var pen = new Pen(Line);
                    e.Graphics.DrawLine(pen, 0, row.Height - 1, row.Width, row.Height - 1);
                };
                table.Controls.Add(row);
                ty += rowH;
            }

            if (lines.Count == 0)
            {
                var empty = MakeLabel("No charge lines (all amounts were zero).", 9, FontStyle.Italic, Muted, true);
                empty.Location = new Point(1, ty);
                empty.Size = new Size(contentW - 2, rowH);
                table.Controls.Add(empty);
                ty += rowH;
            }

            var totalRow = new Panel
            {
                Location = new Point(1, ty),
                Size = new Size(contentW - 2, 42),
                BackColor = Color.White
            };
            var lblTotal = MakeLabel("Total Paid Amount", 10.5f, FontStyle.Bold, Navy, false);
            lblTotal.Location = new Point(14, 10);
            lblTotal.Size = new Size(280, 22);
            totalRow.Controls.Add(lblTotal);
            var lblTotalAmt = MakeLabel($"৳ {_receipt.AmountPaid:N0}", 11f, FontStyle.Bold, Navy, false);
            lblTotalAmt.Location = new Point(contentW - 150, 10);
            lblTotalAmt.Size = new Size(120, 22);
            lblTotalAmt.TextAlign = ContentAlignment.MiddleRight;
            totalRow.Controls.Add(lblTotalAmt);
            table.Controls.Add(totalRow);

            return card;
        }

        private static void AddInfoRow(Control parent, string label, string value, Color valueColor, int left, ref int y, bool boldValue = true)
        {
            var lbl = MakeLabel(label, 9, FontStyle.Regular, Muted, false);
            lbl.Location = new Point(left, y);
            lbl.Size = new Size(120, 18);
            parent.Controls.Add(lbl);

            var val = MakeLabel(value ?? "—", 9.5f, boldValue ? FontStyle.Bold : FontStyle.Regular, valueColor, false);
            val.Location = new Point(left + 124, y - 1);
            val.AutoSize = true;
            parent.Controls.Add(val);
            y += 20;
        }

        private static Label MakeLabel(string text, float size, FontStyle style, Color color, bool center)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", size, style),
                ForeColor = color,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = center ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft
            };
        }

        private static GraphicsPath RoundRect(int x, int y, int w, int h, int r)
        {
            var path = new GraphicsPath();
            int d = r * 2;
            path.AddArc(x, y, d, d, 180, 90);
            path.AddArc(x + w - d, y, d, d, 270, 90);
            path.AddArc(x + w - d, y + h - d, d, d, 0, 90);
            path.AddArc(x, y + h - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void btnClose_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnPrint_Click(object? sender, EventArgs e)
        {
            try
            {
                btnPrint.Enabled = false;
                _savedPdfPath = ReceiptSpooler.GeneratePdf(
                    _receipt,
                    _institution,
                    AppSession.ReportsPath,
                    _campus);

                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = _savedPdfPath,
                        UseShellExecute = true
                    });
                }
                catch
                {
                    // PDF saved even if shell open fails
                }

                MessageBox.Show(
                    $"Official money receipt PDF saved:\n{_savedPdfPath}",
                    "Print Copy",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Print Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnPrint.Enabled = true;
            }
        }
    }
}
