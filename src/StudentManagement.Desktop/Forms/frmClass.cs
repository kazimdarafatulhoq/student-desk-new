using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using StudentManagement.BusinessLayer;
using StudentManagement.Desktop.Helpers;
using StudentManagement.Desktop.Theme;
using StudentManagement.UserObjects.Objects;

namespace StudentManagement.Desktop.Forms
{
    [DesignerCategory("Form")]
    public partial class frmClass : Form
    {
        private AcademicManager? _academic;
        private AcademicManager Academic => _academic ??= new AcademicManager();
        private int _editClassId;

        public frmClass()
        {
            InitializeComponent();
            if (DesignTime.IsActive)
                return;
            UITheme.ApplyForm(this);
            Load += async (_, _) => await RefreshGridAsync();
        }

        private async Task RefreshGridAsync()
        {
            if (DesignTime.IsActive) return;
            var list = await Academic.GetClassesAdminAsync();
            dgvClasses.Rows.Clear();
            foreach (var c in list)
            {
                dgvClasses.Rows.Add(
                    c.ClassId,
                    c.ClassName,
                    c.ClassCode ?? "",
                    c.SortOrder,
                    c.IsActive ? "Active" : "Inactive");
            }
        }

        private void ClearEditor()
        {
            _editClassId = 0;
            txtClassName.Clear();
            txtClassCode.Clear();
            txtSortOrder.Text = "0";
            chkActive.Checked = true;
            btnSave.Text = "Add Class";
            lblEditHint.Text = "New class";
        }

        private void dgvClasses_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvClasses.Rows[e.RowIndex];
            _editClassId = Convert.ToInt32(row.Cells[0].Value);
            txtClassName.Text = Convert.ToString(row.Cells[1].Value) ?? "";
            txtClassCode.Text = Convert.ToString(row.Cells[2].Value) ?? "";
            txtSortOrder.Text = Convert.ToString(row.Cells[3].Value) ?? "0";
            chkActive.Checked = string.Equals(Convert.ToString(row.Cells[4].Value), "Active", StringComparison.OrdinalIgnoreCase);
            btnSave.Text = "Save Changes";
            lblEditHint.Text = "Editing Class #" + _editClassId;
        }

        private async void btnSave_Click(object? sender, EventArgs e)
        {
            if (DesignTime.IsActive) return;
            try
            {
                if (!int.TryParse(txtSortOrder.Text.Trim(), out var sort))
                    sort = 0;

                await Academic.SaveClassAsync(new ClassDto
                {
                    ClassId = _editClassId,
                    ClassName = txtClassName.Text.Trim(),
                    ClassCode = txtClassCode.Text.Trim(),
                    SortOrder = sort,
                    IsActive = chkActive.Checked
                }, AppSession.Current?.Username);

                ClearEditor();
                await RefreshGridAsync();
                MessageBox.Show("Class saved.", "Classes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNew_Click(object? sender, EventArgs e) => ClearEditor();
    }
}
