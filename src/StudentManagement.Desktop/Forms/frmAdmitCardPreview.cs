using StudentManagement.UserObjects.Objects;
using StudentManagement.Desktop.Helpers;
using StudentManagement.Reporting.Generators;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Forms
{
    [DesignerCategory("Form")]
    public partial class frmAdmitCardPreview : Form
    {
        private static readonly Color Navy = Color.FromArgb(15, 23, 42);
        private static readonly Color AccentBlue = Color.FromArgb(37, 99, 235);
        private static readonly Color Muted = Color.FromArgb(100, 116, 139);
        private static readonly Color BoxBg = Color.FromArgb(241, 245, 249);
        private static readonly Color Line = Color.FromArgb(203, 213, 225);

        private readonly IReadOnlyList<AdmitCardDto> _cards;
        private readonly string _institution;
        private readonly string _campus;
        private string? _savedPdfPath;

        public frmAdmitCardPreview()
        {
            InitializeComponent();
            _cards = Array.Empty<AdmitCardDto>();
            _institution = AppSession.InstitutionName;
            _campus = AppSession.InstitutionCampus;
        }

        public frmAdmitCardPreview(IReadOnlyList<AdmitCardDto> cards, string? institutionName = null, string? campusAddress = null)
        {
            InitializeComponent();
            _cards = cards ?? Array.Empty<AdmitCardDto>();
            _institution = string.IsNullOrWhiteSpace(institutionName) ? AppSession.InstitutionName : institutionName!;
            _campus = string.IsNullOrWhiteSpace(campusAddress) ? AppSession.InstitutionCampus : campusAddress!;
            Load += (_, _) => BuildPreview();
            Resize += (_, _) => LayoutFooterButtons();
        }

        private void LayoutFooterButtons()
        {
            btnPrint.Left = pnlFooter.Width - btnPrint.Width - 20;
            btnClose.Left = btnPrint.Left - btnClose.Width - 12;
        }

        private void BuildPreview()
        {
            flpCards.Controls.Clear();
            lblCaption.Text = _cards.Count <= 1
                ? "Single Admit Card Preview"
                : $"Bulk Admit Card Preview ({_cards.Count} cards)";

            foreach (var card in _cards)
            {
                var visual = CreateCardPanel(card);
                flpCards.Controls.Add(visual);
            }

            LayoutFooterButtons();
        }

        private Panel CreateCardPanel(AdmitCardDto card)
        {
            var cardPanel = new Panel
            {
                Width = 720,
                Height = 520,
                BackColor = Color.White,
                Margin = new Padding(16, 8, 16, 16),
                Padding = new Padding(24)
            };
            cardPanel.Paint += (s, e) =>
            {
                using var pen = new Pen(Line, 1);
                e.Graphics.DrawRectangle(pen, 0, 0, cardPanel.Width - 1, cardPanel.Height - 1);
            };

            int y = 20;
            var lblSchool = MakeLabel(_institution.ToUpperInvariant(), 14, FontStyle.Bold, Navy, true);
            lblSchool.Location = new Point(24, y);
            lblSchool.Width = 672;
            cardPanel.Controls.Add(lblSchool);
            y += 28;

            var lblCampus = MakeLabel(_campus, 9, FontStyle.Regular, Muted, true);
            lblCampus.Location = new Point(24, y);
            lblCampus.Width = 672;
            cardPanel.Controls.Add(lblCampus);
            y += 28;

            var banner = new Panel
            {
                BackColor = Navy,
                Location = new Point(120, y),
                Size = new Size(480, 34)
            };
            var lblBanner = MakeLabel($"{(card.TermName ?? "").ToUpperInvariant()} ADMIT CARD", 10, FontStyle.Bold, Color.White, true);
            lblBanner.Dock = DockStyle.Fill;
            lblBanner.TextAlign = ContentAlignment.MiddleCenter;
            banner.Controls.Add(lblBanner);
            cardPanel.Controls.Add(banner);
            y += 48;

            var rule = new Panel
            {
                BackColor = Navy,
                Location = new Point(24, y),
                Size = new Size(672, 2)
            };
            cardPanel.Controls.Add(rule);
            y += 16;

            // Student info box
            var infoBox = new Panel
            {
                BackColor = BoxBg,
                Location = new Point(24, y),
                Size = new Size(672, 130)
            };
            var photo = new Panel
            {
                BackColor = Color.White,
                Location = new Point(12, 12),
                Size = new Size(90, 106),
                BorderStyle = BorderStyle.FixedSingle
            };
            if (!string.IsNullOrWhiteSpace(card.PhotoPath) && File.Exists(card.PhotoPath))
            {
                try
                {
                    var pic = new PictureBox
                    {
                        Dock = DockStyle.Fill,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = Image.FromFile(card.PhotoPath)
                    };
                    photo.Controls.Add(pic);
                }
                catch
                {
                    AddPhotoPlaceholder(photo);
                }
            }
            else
            {
                AddPhotoPlaceholder(photo);
            }
            infoBox.Controls.Add(photo);

            int iy = 12;
            AddInfoRow(infoBox, "Student Name:", card.FullName, Navy, 118, ref iy);
            AddInfoRow(infoBox, "Registration No:", card.RegistrationNo, AccentBlue, 118, ref iy);
            AddInfoRow(infoBox, "Class & Section:", FormatClassSection(card), Navy, 118, ref iy);
            AddInfoRow(infoBox, "Roll No:", card.RollNumber, Navy, 118, ref iy);
            AddInfoRow(infoBox, "Venue:", card.Venue, Navy, 118, ref iy);
            cardPanel.Controls.Add(infoBox);
            y += 146;

            // Instructions
            var instBox = new Panel
            {
                BackColor = BoxBg,
                Location = new Point(24, y),
                Size = new Size(672, 100)
            };
            var lblInstTitle = MakeLabel("Instructions to Candidates:", 10, FontStyle.Bold, Navy, false);
            lblInstTitle.Location = new Point(12, 10);
            instBox.Controls.Add(lblInstTitle);
            var instructions =
                "1. Candidates must bring this Admit Card and Student ID to the examination hall.\n" +
                "2. Programmable calculators, mobile phones, and electronic devices are strictly prohibited.\n" +
                "3. " + card.FeeClearanceNote;
            var lblInst = MakeLabel(instructions, 8, FontStyle.Regular, Muted, false);
            lblInst.Location = new Point(12, 34);
            lblInst.Size = new Size(640, 56);
            instBox.Controls.Add(lblInst);
            cardPanel.Controls.Add(instBox);
            y += 120;

            // Signatures
            var sigTeacher = MakeLabel("_______________________\nClass Teacher", 9, FontStyle.Regular, Muted, true);
            sigTeacher.Location = new Point(80, y);
            sigTeacher.Size = new Size(200, 40);
            cardPanel.Controls.Add(sigTeacher);

            var sigPrincipal = MakeLabel("_______________________\nPrincipal & Controller", 9, FontStyle.Bold, Navy, true);
            sigPrincipal.Location = new Point(420, y);
            sigPrincipal.Size = new Size(220, 40);
            cardPanel.Controls.Add(sigPrincipal);
            y += 50;

            var footer = MakeLabel($"Admit No: {card.AdmitCardNo}   ·   Issued: {card.IssuedAt:dd-MMM-yyyy}", 8, FontStyle.Regular, Muted, true);
            footer.Location = new Point(24, y);
            footer.Width = 672;
            cardPanel.Controls.Add(footer);

            return cardPanel;
        }

        private static void AddPhotoPlaceholder(Panel photo)
        {
            var lbl = MakeLabel("PHOTO", 9, FontStyle.Regular, Muted, true);
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            photo.Controls.Add(lbl);
        }

        private static void AddInfoRow(Panel parent, string label, string value, Color valueColor, int left, ref int y)
        {
            var lbl = MakeLabel(label, 9, FontStyle.Regular, Muted, false);
            lbl.Location = new Point(left, y);
            lbl.AutoSize = true;
            parent.Controls.Add(lbl);

            var val = MakeLabel(value, 10, FontStyle.Bold, valueColor, false);
            val.Location = new Point(left + 120, y - 1);
            val.AutoSize = true;
            parent.Controls.Add(val);
            y += 22;
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

        private static string FormatClassSection(AdmitCardDto card)
        {
            var section = string.IsNullOrWhiteSpace(card.SectionName) ? "" : $" - Section {card.SectionName}";
            return $"Class {card.ClassName}{section}";
        }

        private void btnClose_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnPrint_Click(object? sender, EventArgs e)
        {
            if (_cards.Count == 0)
                return;

            try
            {
                btnPrint.Enabled = false;
                _savedPdfPath = AdmitCardPdfGenerator.GenerateBatch(
                    _cards,
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
                    $"Official admit card PDF saved:\n{_savedPdfPath}",
                    "Print Admit Card",
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

        public string? SavedPdfPath => _savedPdfPath;
    }
}
