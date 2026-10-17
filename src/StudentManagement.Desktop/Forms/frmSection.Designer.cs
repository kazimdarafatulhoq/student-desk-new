using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Forms
{
    partial class frmSection
    {
        private IContainer components = null;
        private Label lblTitle;
        private Panel pnlEditor;
        private Label lblClass;
        private ComboBox cboClass;
        private Label lblSectionName;
        private TextBox txtSectionName;
        private Label lblCapacity;
        private TextBox txtCapacity;
        private CheckBox chkActive;
        private Label lblEditHint;
        private Button btnSave;
        private Button btnNew;
        private DataGridView dgvSections;
        private DataGridViewTextBoxColumn colSectionId;
        private DataGridViewTextBoxColumn colClassId;
        private DataGridViewTextBoxColumn colClassName;
        private DataGridViewTextBoxColumn colSectionName;
        private DataGridViewTextBoxColumn colCapacity;
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
            this.lblClass = new Label();
            this.cboClass = new ComboBox();
            this.lblSectionName = new Label();
            this.txtSectionName = new TextBox();
            this.lblCapacity = new Label();
            this.txtCapacity = new TextBox();
            this.chkActive = new CheckBox();
            this.lblEditHint = new Label();
            this.btnSave = new Button();
            this.btnNew = new Button();
            this.dgvSections = new DataGridView();
            this.colSectionId = new DataGridViewTextBoxColumn();
            this.colClassId = new DataGridViewTextBoxColumn();
            this.colClassName = new DataGridViewTextBoxColumn();
            this.colSectionName = new DataGridViewTextBoxColumn();
            this.colCapacity = new DataGridViewTextBoxColumn();
            this.colStatus = new DataGridViewTextBoxColumn();
            this.pnlEditor.SuspendLayout();
            ((ISupportInitialize)(this.dgvSections)).BeginInit();
            this.SuspendLayout();

            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Location = new Point(16, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "Section Setup";

            this.pnlEditor.BackColor = Color.FromArgb(13, 20, 48);
            this.pnlEditor.Location = new Point(16, 55);
            this.pnlEditor.Name = "pnlEditor";
            this.pnlEditor.Size = new Size(960, 110);
            this.pnlEditor.Tag = "card";
            this.pnlEditor.Controls.Add(this.lblClass);
            this.pnlEditor.Controls.Add(this.cboClass);
            this.pnlEditor.Controls.Add(this.lblSectionName);
            this.pnlEditor.Controls.Add(this.txtSectionName);
            this.pnlEditor.Controls.Add(this.lblCapacity);
            this.pnlEditor.Controls.Add(this.txtCapacity);
            this.pnlEditor.Controls.Add(this.chkActive);
            this.pnlEditor.Controls.Add(this.lblEditHint);
            this.pnlEditor.Controls.Add(this.btnSave);
            this.pnlEditor.Controls.Add(this.btnNew);

            this.lblClass.AutoSize = true;
            this.lblClass.ForeColor = Color.FromArgb(138, 147, 166);
            this.lblClass.Location = new Point(16, 14);
            this.lblClass.Text = "Class";
            this.cboClass.BackColor = Color.FromArgb(17, 24, 39);
            this.cboClass.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboClass.FlatStyle = FlatStyle.Flat;
            this.cboClass.ForeColor = Color.White;
            this.cboClass.Location = new Point(16, 34);
            this.cboClass.Size = new Size(220, 23);

            this.lblSectionName.AutoSize = true;
            this.lblSectionName.ForeColor = Color.FromArgb(138, 147, 166);
            this.lblSectionName.Location = new Point(250, 14);
            this.lblSectionName.Text = "Section Name";
            this.txtSectionName.BackColor = Color.FromArgb(17, 24, 39);
            this.txtSectionName.BorderStyle = BorderStyle.FixedSingle;
            this.txtSectionName.ForeColor = Color.White;
            this.txtSectionName.Location = new Point(250, 34);
            this.txtSectionName.Size = new Size(140, 23);

            this.lblCapacity.AutoSize = true;
            this.lblCapacity.ForeColor = Color.FromArgb(138, 147, 166);
            this.lblCapacity.Location = new Point(410, 14);
            this.lblCapacity.Text = "Capacity";
            this.txtCapacity.BackColor = Color.FromArgb(17, 24, 39);
            this.txtCapacity.BorderStyle = BorderStyle.FixedSingle;
            this.txtCapacity.ForeColor = Color.White;
            this.txtCapacity.Location = new Point(410, 34);
            this.txtCapacity.Size = new Size(80, 23);
            this.txtCapacity.Text = "40";

            this.chkActive.AutoSize = true;
            this.chkActive.ForeColor = Color.White;
            this.chkActive.Location = new Point(510, 36);
            this.chkActive.Text = "Active";
            this.chkActive.Checked = true;

            this.lblEditHint.AutoSize = true;
            this.lblEditHint.ForeColor = Color.FromArgb(138, 147, 166);
            this.lblEditHint.Location = new Point(16, 72);
            this.lblEditHint.Tag = "muted";
            this.lblEditHint.Text = "New section";

            this.btnSave.Location = new Point(700, 30);
            this.btnSave.Size = new Size(120, 36);
            this.btnSave.Text = "Add Section";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnNew.Location = new Point(830, 30);
            this.btnNew.Size = new Size(100, 36);
            this.btnNew.Tag = "ghost";
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            this.dgvSections.AllowUserToAddRows = false;
            this.dgvSections.AllowUserToDeleteRows = false;
            this.dgvSections.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSections.BackgroundColor = Color.FromArgb(13, 20, 48);
            this.dgvSections.BorderStyle = BorderStyle.None;
            this.dgvSections.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSections.Columns.AddRange(new DataGridViewColumn[] {
                this.colSectionId, this.colClassId, this.colClassName, this.colSectionName, this.colCapacity, this.colStatus });
            this.dgvSections.Location = new Point(16, 180);
            this.dgvSections.MultiSelect = false;
            this.dgvSections.Name = "dgvSections";
            this.dgvSections.ReadOnly = true;
            this.dgvSections.RowHeadersVisible = false;
            this.dgvSections.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvSections.Size = new Size(960, 420);
            this.dgvSections.CellClick += new DataGridViewCellEventHandler(this.dgvSections_CellClick);

            this.colSectionId.HeaderText = "Id";
            this.colSectionId.Name = "colSectionId";
            this.colSectionId.FillWeight = 40F;
            this.colClassId.HeaderText = "ClassId";
            this.colClassId.Name = "colClassId";
            this.colClassId.Visible = false;
            this.colClassName.HeaderText = "Class";
            this.colClassName.Name = "colClassName";
            this.colClassName.FillWeight = 140F;
            this.colSectionName.HeaderText = "Section";
            this.colSectionName.Name = "colSectionName";
            this.colSectionName.FillWeight = 100F;
            this.colCapacity.HeaderText = "Capacity";
            this.colCapacity.Name = "colCapacity";
            this.colCapacity.FillWeight = 60F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.FillWeight = 70F;

            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(9, 12, 23);
            this.ClientSize = new Size(1000, 620);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pnlEditor);
            this.Controls.Add(this.dgvSections);
            this.Name = "frmSection";
            this.Text = "Section Setup";
            this.pnlEditor.ResumeLayout(false);
            this.pnlEditor.PerformLayout();
            ((ISupportInitialize)(this.dgvSections)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
