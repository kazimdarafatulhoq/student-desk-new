using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using StudentManagement.UserObjects.Objects;

namespace StudentManagement.Desktop.Forms
{
    partial class frmAdmitCardPreview
    {
        private IContainer components = null;
        private Panel pnlShell;
        private Panel pnlFooter;
        private FlowLayoutPanel flpCards;
        private Button btnClose;
        private Button btnPrint;
        private Label lblCaption;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlShell = new System.Windows.Forms.Panel();
            this.flpCards = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.lblCaption = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.pnlShell.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            // pnlShell
            this.pnlShell.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.pnlShell.Controls.Add(this.flpCards);
            this.pnlShell.Controls.Add(this.pnlFooter);
            this.pnlShell.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlShell.Name = "pnlShell";
            this.pnlShell.Padding = new System.Windows.Forms.Padding(24);
            // flpCards
            this.flpCards.AutoScroll = true;
            this.flpCards.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.flpCards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpCards.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpCards.Name = "flpCards";
            this.flpCards.Padding = new System.Windows.Forms.Padding(8);
            this.flpCards.WrapContents = false;
            // pnlFooter
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.pnlFooter.Controls.Add(this.lblCaption);
            this.pnlFooter.Controls.Add(this.btnClose);
            this.pnlFooter.Controls.Add(this.btnPrint);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Height = 64;
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            // lblCaption
            this.lblCaption.AutoSize = true;
            this.lblCaption.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblCaption.Location = new System.Drawing.Point(20, 22);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Text = "Admit Card Preview";
            // btnClose
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.btnClose.Location = new System.Drawing.Point(470, 14);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 36);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // btnPrint
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.btnPrint.FlatAppearance.BorderSize = 0;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(580, 14);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(220, 36);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "Print Official Admit Card";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.ClientSize = new System.Drawing.Size(820, 720);
            this.Controls.Add(this.pnlShell);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAdmitCardPreview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Admit Card Preview";
            this.pnlShell.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
