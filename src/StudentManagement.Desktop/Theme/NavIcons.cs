using System.Drawing;
using System.Drawing.Drawing2D;

namespace StudentManagement.Desktop.Theme
{
    /// <summary>
    /// Lightweight line icons for the main-menu sidebar (no external image assets).
    /// </summary>
    public static class NavIcons
    {
        public const int Size = 20;

        /// <summary>Transparent gap to the right of the glyph so text is not flush against the icon.</summary>
        public const int RightPadding = 8;

        public static Image Toggle(Color color) => Draw(color, g =>
        {
            using (Pen p = Pen(color, 2f))
            {
                g.DrawLine(p, 3, 6, 17, 6);
                g.DrawLine(p, 3, 10, 17, 10);
                g.DrawLine(p, 3, 14, 17, 14);
            }
        });

        public static Image Dashboard(Color color) => Draw(color, g =>
        {
            using (Pen p = Pen(color, 1.6f))
            {
                g.DrawRectangle(p, 3, 3, 6, 6);
                g.DrawRectangle(p, 11, 3, 6, 6);
                g.DrawRectangle(p, 3, 11, 6, 6);
                g.DrawRectangle(p, 11, 11, 6, 6);
            }
        });

        public static Image Admission(Color color) => Draw(color, g =>
        {
            using (Pen p = Pen(color, 1.6f))
            {
                g.DrawEllipse(p, 7, 3, 6, 6);
                g.DrawArc(p, 4, 10, 12, 10, 200, 140);
                g.DrawLine(p, 15, 12, 15, 17);
                g.DrawLine(p, 13, 14.5f, 17, 14.5f);
            }
        });

        public static Image Search(Color color) => Draw(color, g =>
        {
            using (Pen p = Pen(color, 1.8f))
            {
                g.DrawEllipse(p, 3, 3, 10, 10);
                g.DrawLine(p, 12, 12, 17, 17);
            }
        });

        public static Image Fee(Color color) => Draw(color, g =>
        {
            using (Pen p = Pen(color, 1.6f))
            {
                g.DrawRectangle(p, 3, 5, 14, 11);
                g.DrawLine(p, 3, 9, 17, 9);
                g.DrawEllipse(p, 8, 11, 4, 3);
            }
        });

        public static Image Ledger(Color color) => Draw(color, g =>
        {
            using (Pen p = Pen(color, 1.6f))
            {
                g.DrawRectangle(p, 4, 3, 12, 14);
                g.DrawLine(p, 7, 7, 13, 7);
                g.DrawLine(p, 7, 10, 13, 10);
                g.DrawLine(p, 7, 13, 11, 13);
            }
        });

        public static Image Exam(Color color) => Draw(color, g =>
        {
            using (Pen p = Pen(color, 1.8f))
            {
                g.DrawRectangle(p, 4, 3, 12, 14);
                g.DrawLine(p, 7, 11, 9, 13);
                g.DrawLine(p, 9, 13, 14, 7);
            }
        });

        public static Image AdmitCard(Color color) => Draw(color, g =>
        {
            using (Pen p = Pen(color, 1.6f))
            {
                g.DrawRectangle(p, 2, 5, 16, 11);
                g.DrawLine(p, 2, 9, 18, 9);
                g.DrawRectangle(p, 5, 11, 4, 3);
            }
        });

        public static Image Users(Color color) => Draw(color, g =>
        {
            using (Pen p = Pen(color, 1.6f))
            {
                g.DrawEllipse(p, 7, 2, 6, 6);
                g.DrawArc(p, 4, 9, 12, 10, 200, 140);
                g.DrawEllipse(p, 2, 5, 4, 4);
                g.DrawEllipse(p, 14, 5, 4, 4);
            }
        });

        public static Image ClassSetup(Color color) => Draw(color, g =>
        {
            using (Pen p = Pen(color, 1.6f))
            {
                g.DrawRectangle(p, 3, 4, 14, 12);
                g.DrawLine(p, 3, 8, 17, 8);
                g.DrawLine(p, 10, 8, 10, 16);
            }
        });

        public static Image SectionSetup(Color color) => Draw(color, g =>
        {
            using (Pen p = Pen(color, 1.6f))
            {
                g.DrawRectangle(p, 2, 4, 7, 12);
                g.DrawRectangle(p, 11, 4, 7, 12);
                g.DrawLine(p, 4, 8, 7, 8);
                g.DrawLine(p, 13, 8, 16, 8);
            }
        });

        public static Image Logout(Color color) => Draw(color, g =>
        {
            using (Pen p = Pen(color, 1.8f))
            {
                g.DrawArc(p, 3, 3, 14, 14, 40, 280);
                g.DrawLine(p, 10, 2, 10, 10);
            }
        });

        private static Image Draw(Color color, System.Action<Graphics> paint)
        {
            // Wider than the glyph so ImageBeforeText leaves a gap before the label.
            Bitmap bmp = new Bitmap(Size + RightPadding, Size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.Clear(Color.Transparent);
                paint(g);
            }
            return bmp;
        }

        private static Pen Pen(Color color, float width)
        {
            return new Pen(color, width)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };
        }
    }
}
