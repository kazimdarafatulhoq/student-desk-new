using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Forms
{
    // Base Form declared here too so Design-time / partial merge never falls back to object.
    public partial class frmReceiptPreview : Form
    {
        private IContainer components = null;
        private Panel pnlShell;
        private Panel pnlScrollHost;
        private Panel pnlReceiptHost;
        private Panel pnlFooter;
        private Label lblCaption;
        private Button btnClose;
        private Button btnPrint;

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
            this.pnlScrollHost = new System.Windows.Forms.Panel();
            this.pnlReceiptHost = new System.Windows.Forms.Panel();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.lblCaption = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.pnlShell.SuspendLayout();
            this.pnlScrollHost.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlShell
            //
            this.pnlShell.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.pnlShell.Controls.Add(this.pnlScrollHost);
            this.pnlShell.Controls.Add(this.pnlFooter);
            this.pnlShell.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlShell.Name = "pnlShell";
            this.pnlShell.Padding = new System.Windows.Forms.Padding(24);
            //
            // pnlScrollHost
            //
            this.pnlScrollHost.AutoScroll = true;
            this.pnlScrollHost.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.pnlScrollHost.Controls.Add(this.pnlReceiptHost);
            this.pnlScrollHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlScrollHost.Name = "pnlScrollHost";
            this.pnlScrollHost.Padding = new System.Windows.Forms.Padding(8);
            //
            // pnlReceiptHost
            //
            this.pnlReceiptHost.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.pnlReceiptHost.Location = new System.Drawing.Point(8, 8);
            this.pnlReceiptHost.Name = "pnlReceiptHost";
            this.pnlReceiptHost.Size = new System.Drawing.Size(720, 600);
            //
            // pnlFooter
            //
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.pnlFooter.Controls.Add(this.lblCaption);
            this.pnlFooter.Controls.Add(this.btnClose);
            this.pnlFooter.Controls.Add(this.btnPrint);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Height = 64;
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            //
            // lblCaption
            //
            this.lblCaption.AutoSize = true;
            this.lblCaption.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblCaption.Location = new System.Drawing.Point(20, 22);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Text = "Money Receipt Preview";
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.btnClose.Location = new System.Drawing.Point(490, 14);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 36);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // btnPrint
            //
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnPrint.FlatAppearance.BorderSize = 0;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(600, 14);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(140, 36);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "Print Copy";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            //
            // frmReceiptPreview
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.ClientSize = new System.Drawing.Size(780, 720);
            this.Controls.Add(this.pnlShell);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmReceiptPreview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Official Money Receipt";
            this.pnlShell.ResumeLayout(false);
            this.pnlScrollHost.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
