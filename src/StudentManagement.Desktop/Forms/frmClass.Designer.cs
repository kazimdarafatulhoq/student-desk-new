using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Forms
{
    partial class frmClass
    {
        private IContainer components = null;
        private Label lblTitle;
        private Panel pnlEditor;
        private Label lblClassName;
        private TextBox txtClassName;
        private Label lblClassCode;
        private TextBox txtClassCode;
        private Label lblSortOrder;
        private TextBox txtSortOrder;
        private CheckBox chkActive;
        private Label lblEditHint;
        private Button btnSave;
        private Button btnNew;
        private DataGridView dgvClasses;
        private DataGridViewTextBoxColumn colClassId;
        private DataGridViewTextBoxColumn colClassName;
        private DataGridViewTextBoxColumn colClassCode;
        private DataGridViewTextBoxColumn colSort;
        private DataGridViewTextBoxColumn colStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.lblTitle = new Label();
            this.pnlEditor = new Panel();
            this.lblClassName = new Label();
            this.txtClassName = new TextBox();
            this.lblClassCode = new Label();
            this.txtClassCode = new TextBox();
            this.lblSortOrder = new Label();
            this.txtSortOrder = new TextBox();
            this.chkActive = new CheckBox();
            this.lblEditHint = new Label();
            this.btnSave = new Button();
            this.btnNew = new Button();
            this.dgvClasses = new DataGridView();
            this.colClassId = new DataGridViewTextBoxColumn();
            this.colClassName = new DataGridViewTextBoxColumn();
            this.colClassCode = new DataGridViewTextBoxColumn();
            this.colSort = new DataGridViewTextBoxColumn();
            this.colStatus = new DataGridViewTextBoxColumn();
            this.pnlEditor.SuspendLayout();
            ((ISupportInitialize)(this.dgvClasses)).BeginInit();
            this.SuspendLayout();

            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Location = new Point(16, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "Class Setup";

            this.pnlEditor.BackColor = Color.FromArgb(13, 20, 48);
            this.pnlEditor.Location = new Point(16, 55);
            this.pnlEditor.Name = "pnlEditor";
            this.pnlEditor.Size = new Size(960, 110);
            this.pnlEditor.Tag = "card";
            this.pnlEditor.Controls.Add(this.lblClassName);
            this.pnlEditor.Controls.Add(this.txtClassName);
            this.pnlEditor.Controls.Add(this.lblClassCode);
            this.pnlEditor.Controls.Add(this.txtClassCode);
            this.pnlEditor.Controls.Add(this.lblSortOrder);
            this.pnlEditor.Controls.Add(this.txtSortOrder);
            this.pnlEditor.Controls.Add(this.chkActive);
            this.pnlEditor.Controls.Add(this.lblEditHint);
            this.pnlEditor.Controls.Add(this.btnSave);
            this.pnlEditor.Controls.Add(this.btnNew);

            this.lblClassName.AutoSize = true;
            this.lblClassName.ForeColor = Color.FromArgb(138, 147, 166);
            this.lblClassName.Location = new Point(16, 14);
            this.lblClassName.Text = "Class Name";
            this.txtClassName.BackColor = Color.FromArgb(17, 24, 39);
            this.txtClassName.BorderStyle = BorderStyle.FixedSingle;
            this.txtClassName.ForeColor = Color.White;
            this.txtClassName.Location = new Point(16, 34);
            this.txtClassName.Size = new Size(220, 23);

            this.lblClassCode.AutoSize = true;
            this.lblClassCode.ForeColor = Color.FromArgb(138, 147, 166);
            this.lblClassCode.Location = new Point(250, 14);
            this.lblClassCode.Text = "Class Code";
            this.txtClassCode.BackColor = Color.FromArgb(17, 24, 39);
            this.txtClassCode.BorderStyle = BorderStyle.FixedSingle;
            this.txtClassCode.ForeColor = Color.White;
            this.txtClassCode.Location = new Point(250, 34);
            this.txtClassCode.Size = new Size(120, 23);

            this.lblSortOrder.AutoSize = true;
            this.lblSortOrder.ForeColor = Color.FromArgb(138, 147, 166);
            this.lblSortOrder.Location = new Point(390, 14);
            this.lblSortOrder.Text = "Sort Order";
            this.txtSortOrder.BackColor = Color.FromArgb(17, 24, 39);
            this.txtSortOrder.BorderStyle = BorderStyle.FixedSingle;
            this.txtSortOrder.ForeColor = Color.White;
            this.txtSortOrder.Location = new Point(390, 34);
            this.txtSortOrder.Size = new Size(80, 23);
            this.txtSortOrder.Text = "0";

            this.chkActive.AutoSize = true;
            this.chkActive.ForeColor = Color.White;
            this.chkActive.Location = new Point(490, 36);
            this.chkActive.Text = "Active";
            this.chkActive.Checked = true;

            this.lblEditHint.AutoSize = true;
            this.lblEditHint.ForeColor = Color.FromArgb(138, 147, 166);
            this.lblEditHint.Location = new Point(16, 72);
            this.lblEditHint.Tag = "muted";
            this.lblEditHint.Text = "New class";

            this.btnSave.Location = new Point(700, 30);
            this.btnSave.Size = new Size(120, 36);
            this.btnSave.Text = "Add Class";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnNew.Location = new Point(830, 30);
            this.btnNew.Size = new Size(100, 36);
            this.btnNew.Tag = "ghost";
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            this.dgvClasses.AllowUserToAddRows = false;
            this.dgvClasses.AllowUserToDeleteRows = false;
            this.dgvClasses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvClasses.BackgroundColor = Color.FromArgb(13, 20, 48);
            this.dgvClasses.BorderStyle = BorderStyle.None;
            this.dgvClasses.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClasses.Columns.AddRange(new DataGridViewColumn[] {
                this.colClassId, this.colClassName, this.colClassCode, this.colSort, this.colStatus });
            this.dgvClasses.Location = new Point(16, 180);
            this.dgvClasses.MultiSelect = false;
            this.dgvClasses.Name = "dgvClasses";
            this.dgvClasses.ReadOnly = true;
            this.dgvClasses.RowHeadersVisible = false;
            this.dgvClasses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvClasses.Size = new Size(960, 420);
            this.dgvClasses.CellClick += new DataGridViewCellEventHandler(this.dgvClasses_CellClick);

            this.colClassId.HeaderText = "Id";
            this.colClassId.Name = "colClassId";
            this.colClassId.FillWeight = 40F;
            this.colClassName.HeaderText = "Class Name";
            this.colClassName.Name = "colClassName";
            this.colClassName.FillWeight = 160F;
            this.colClassCode.HeaderText = "Code";
            this.colClassCode.Name = "colClassCode";
            this.colClassCode.FillWeight = 80F;
            this.colSort.HeaderText = "Sort";
            this.colSort.Name = "colSort";
            this.colSort.FillWeight = 50F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.FillWeight = 70F;

            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(9, 12, 23);
            this.ClientSize = new Size(1000, 620);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pnlEditor);
            this.Controls.Add(this.dgvClasses);
            this.Name = "frmClass";
            this.Text = "Class Setup";
            this.pnlEditor.ResumeLayout(false);
            this.pnlEditor.PerformLayout();
            ((ISupportInitialize)(this.dgvClasses)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
