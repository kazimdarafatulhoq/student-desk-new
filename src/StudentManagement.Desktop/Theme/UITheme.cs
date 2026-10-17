using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Theme
{
    /// <summary>
    /// Fouzdarhat Safiria Dakhil Madrasah theme — near-black navbar/header, slate body canvas.
    /// Do NOT reference this class from InitializeComponent (breaks the VS WinForms designer).
    /// Call UITheme.ApplyForm(this) from the form constructor after InitializeComponent.
    /// </summary>
    public static class UITheme
    {
        // Shell surfaces (navbar / header / body) — aligned to IDEAL SMS BD mockups.
        public static readonly Color Canvas = Color.FromArgb(17, 24, 39);      // #111827 body
        public static readonly Color Sidebar = Color.FromArgb(11, 14, 20);     // #0B0E14 navbar
        public static readonly Color Card = Color.FromArgb(13, 20, 48);
        public static readonly Color InputBack = Color.FromArgb(17, 24, 39);
        public static readonly Color Border = Color.FromArgb(36, 48, 73);
        public static readonly Color GridAlt = Color.FromArgb(11, 18, 40);
        public static readonly Color HeaderBack = Color.FromArgb(11, 14, 20);  // #0B0E14 header (matches navbar)

        public static readonly Color Primary = Color.FromArgb(37, 99, 235);
        public static readonly Color PrimaryHover = Color.FromArgb(29, 78, 216);
        public static readonly Color PrimaryIndigo = Color.FromArgb(79, 70, 229);
        public static readonly Color AccentPurple = Color.FromArgb(139, 92, 246);
        public static readonly Color AccentCyan = Color.FromArgb(59, 130, 246);
        public static readonly Color Selection = Color.FromArgb(29, 78, 216);

        public static readonly Color Success = Color.FromArgb(16, 185, 129);
        public static readonly Color Online = Color.FromArgb(34, 197, 94);
        public static readonly Color Warning = Color.FromArgb(250, 204, 21);
        public static readonly Color Partial = Color.FromArgb(249, 160, 63);
        public static readonly Color Danger = Color.FromArgb(244, 63, 94);

        public static readonly Color TextPrimary = Color.White;
        public static readonly Color TextMuted = Color.FromArgb(138, 147, 166);
        public static readonly Color TextDim = Color.FromArgb(160, 174, 192);

        public static readonly Font FontBody = new Font("Segoe UI", 9F, FontStyle.Regular);
        public static readonly Font FontSubtitle = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static readonly Font FontHeader = new Font("Segoe UI", 14F, FontStyle.Bold);
        public static readonly Font FontTitle = new Font("Segoe UI", 18F, FontStyle.Bold);
        public static readonly Font FontNav = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static readonly Font FontSection = new Font("Segoe UI", 9F, FontStyle.Bold);
        public static readonly Font FontMetric = new Font("Segoe UI", 22F, FontStyle.Bold);

        public static void ApplyForm(Form form)
        {
            if (form == null)
                return;

            form.BackColor = Canvas;
            form.ForeColor = TextPrimary;
            form.Font = FontBody;
            foreach (Control c in form.Controls)
                ApplyControl(c);
        }

        public static void ApplyControl(Control control)
        {
            if (control == null)
                return;

            switch (control)
            {
                case FlowLayoutPanel flow:
                    if (flow.Tag != null && flow.Tag.ToString() == "sidebar")
                        flow.BackColor = Sidebar;
                    break;
                case Panel panel:
                    ApplyPanel(panel);
                    break;
                case Label label:
                    ApplyLabel(label);
                    break;
                case TextBox textBox:
                    StyleTextBox(textBox);
                    break;
                case ComboBox combo:
                    StyleComboBox(combo);
                    break;
                case DateTimePicker dtp:
                    dtp.CalendarMonthBackground = Card;
                    dtp.CalendarForeColor = TextPrimary;
                    dtp.CalendarTitleBackColor = Primary;
                    dtp.CalendarTitleForeColor = Color.White;
                    dtp.Font = FontBody;
                    dtp.Format = DateTimePickerFormat.Custom;
                    dtp.CustomFormat = "dd-MMM-yyyy";
                    break;
                case CheckBox check:
                    check.ForeColor = TextPrimary;
                    check.FlatStyle = FlatStyle.Flat;
                    break;
                case Button button:
                    StyleButton(button);
                    break;
                case DataGridView grid:
                    StyleGrid(grid);
                    break;
                case GroupBox group:
                    group.ForeColor = Primary;
                    group.BackColor = Card;
                    group.Font = FontSection;
                    break;
                case StatusStrip strip:
                    strip.BackColor = HeaderBack;
                    strip.ForeColor = TextMuted;
                    break;
                case ProgressBar bar:
                    bar.BackColor = InputBack;
                    break;
            }

            foreach (Control child in control.Controls)
                ApplyControl(child);
        }

        private static void ApplyPanel(Panel panel)
        {
            string? tag = panel.Tag != null ? panel.Tag.ToString() : string.Empty;
            if (tag == "card")
                panel.BackColor = Card;
            else if (tag == "sidebar")
                panel.BackColor = Sidebar;
            else if (tag == "header")
                panel.BackColor = HeaderBack;
            else
                panel.BackColor = panel.Parent != null ? panel.Parent.BackColor : Canvas;
        }

        private static void ApplyLabel(Label label)
        {
            string? tag = label.Tag != null ? label.Tag.ToString() : string.Empty;
            if (string.IsNullOrEmpty(tag))
                return;

            if (tag.IndexOf("muted", StringComparison.OrdinalIgnoreCase) >= 0 ||
                tag.IndexOf("dim", StringComparison.OrdinalIgnoreCase) >= 0)
                label.ForeColor = TextMuted;
            else if (tag.IndexOf("section", StringComparison.OrdinalIgnoreCase) >= 0)
                label.ForeColor = Primary;
            else if (tag.IndexOf("success", StringComparison.OrdinalIgnoreCase) >= 0)
                label.ForeColor = Success;
            else if (tag.IndexOf("danger", StringComparison.OrdinalIgnoreCase) >= 0)
                label.ForeColor = Danger;
            else if (tag.IndexOf("warning", StringComparison.OrdinalIgnoreCase) >= 0)
                label.ForeColor = Warning;
            else if (tag.IndexOf("primary", StringComparison.OrdinalIgnoreCase) >= 0)
                label.ForeColor = Primary;
            else if (tag.IndexOf("cyan", StringComparison.OrdinalIgnoreCase) >= 0)
                label.ForeColor = AccentCyan;
            else if (tag.IndexOf("purple", StringComparison.OrdinalIgnoreCase) >= 0)
                label.ForeColor = AccentPurple;
            else
                label.ForeColor = TextPrimary;

            if (tag.IndexOf("header", StringComparison.OrdinalIgnoreCase) >= 0)
                label.Font = FontHeader;
            else if (tag.IndexOf("subtitle", StringComparison.OrdinalIgnoreCase) >= 0)
                label.Font = FontSubtitle;
            else if (tag.IndexOf("metric", StringComparison.OrdinalIgnoreCase) >= 0)
                label.Font = FontMetric;
            else if (tag.IndexOf("section", StringComparison.OrdinalIgnoreCase) >= 0)
                label.Font = FontSection;
        }

        public static void StyleTextBox(TextBox textBox)
        {
            textBox.BackColor = InputBack;
            textBox.ForeColor = TextPrimary;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = FontBody;
        }

        public static void StyleComboBox(ComboBox combo)
        {
            combo.BackColor = InputBack;
            combo.ForeColor = TextPrimary;
            combo.FlatStyle = FlatStyle.Flat;
            combo.Font = FontBody;
        }

        public static void StyleButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;

            string? tag = button.Tag != null ? button.Tag.ToString() : string.Empty;
            bool isNav = tag == "nav" || tag == "nav-active" || tag == "nav-danger";

            if (isNav)
            {
                // Regular weight + taller row so descenders (g/y/p) are not clipped.
                button.Font = FontNav;
                button.Height = 50;
                button.TextImageRelation = TextImageRelation.ImageBeforeText;
                button.UseCompatibleTextRendering = true;
            }
            else
            {
                button.Font = FontSubtitle;
                if (button.Height < 36)
                    button.Height = 36;
            }

            if (tag == "danger" || tag == "nav-danger")
            {
                button.BackColor = Danger;
                button.ForeColor = Color.White;
                if (tag == "nav-danger")
                {
                    button.TextAlign = ContentAlignment.MiddleLeft;
                    button.ImageAlign = ContentAlignment.MiddleLeft;
                    button.Padding = new Padding(12, 10, 8, 10);
                    button.FlatAppearance.MouseOverBackColor = Color.FromArgb(225, 29, 72);
                }
            }
            else if (tag == "success")
            {
                button.BackColor = Success;
                button.ForeColor = Color.White;
            }
            else if (tag == "ghost")
            {
                button.BackColor = Card;
                button.ForeColor = TextPrimary;
                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.BorderColor = Border;
            }
            else if (tag == "indigo")
            {
                button.BackColor = PrimaryIndigo;
                button.ForeColor = Color.White;
            }
            else if (tag == "nav")
            {
                button.BackColor = Color.Transparent;
                button.ForeColor = TextMuted;
                button.TextAlign = ContentAlignment.MiddleLeft;
                button.ImageAlign = ContentAlignment.MiddleLeft;
                button.FlatAppearance.MouseOverBackColor = Card;
                button.Padding = new Padding(12, 10, 8, 10);
            }
            else if (tag == "nav-active")
            {
                button.BackColor = Primary;
                button.ForeColor = Color.White;
                button.TextAlign = ContentAlignment.MiddleLeft;
                button.ImageAlign = ContentAlignment.MiddleLeft;
                button.FlatAppearance.MouseOverBackColor = PrimaryHover;
                button.Padding = new Padding(12, 10, 8, 10);
            }
            else
            {
                button.BackColor = Primary;
                button.ForeColor = Color.White;
                button.FlatAppearance.MouseOverBackColor = PrimaryHover;
            }
        }

        public static void SetNavActive(Button button, bool active)
        {
            if (button == null)
                return;
            string? current = button.Tag != null ? button.Tag.ToString() : string.Empty;
            if (current == "nav-danger" || current == "danger")
                return;
            button.Tag = active ? "nav-active" : "nav";
            StyleButton(button);
        }

        public static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Card;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Border;
            grid.RowHeadersVisible = false;
            grid.EnableHeadersVisualStyles = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeight = 40;
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = HeaderBack,
                ForeColor = TextMuted,
                Font = FontSection,
                SelectionBackColor = HeaderBack,
                SelectionForeColor = TextMuted,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(8, 4, 8, 4)
            };
            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Card,
                ForeColor = TextPrimary,
                Font = FontBody,
                SelectionBackColor = Selection,
                SelectionForeColor = Color.White,
                Padding = new Padding(6, 2, 6, 2)
            };
            grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = GridAlt,
                ForeColor = TextPrimary,
                SelectionBackColor = Selection,
                SelectionForeColor = Color.White
            };
            grid.RowTemplate.Height = 36;
        }

        public static Color StatusColor(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return TextMuted;
            string s = status.Trim().ToLowerInvariant();
            if (s == "paid" || s == "active" || s == "cleared")
                return Success;
            if (s == "partial")
                return Partial;
            if (s == "unpaid" || s == "due" || s == "overdue")
                return Danger;
            return TextMuted;
        }

        public static void PaintCardBorder(PaintEventArgs e, Control card, Color accent)
        {
            if (e == null || card == null)
                return;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (Pen border = new Pen(Border))
                e.Graphics.DrawRectangle(border, 0, 0, card.Width - 1, card.Height - 1);
            using (Pen accentPen = new Pen(accent, 3))
                e.Graphics.DrawLine(accentPen, 0, 8, 0, card.Height - 8);
        }
    }
}
