using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using StudentManagement.BusinessLayer;
using StudentManagement.Desktop.Helpers;
using StudentManagement.Desktop.Theme;
using StudentManagement.UserObjects.Objects;

namespace StudentManagement.Desktop.Forms
{
    [DesignerCategory("Form")]
    public partial class frmSection : Form
    {
        private AcademicManager? _academic;
        private AcademicManager Academic => _academic ??= new AcademicManager();
        private int _editSectionId;

        public frmSection()
        {
            InitializeComponent();
            if (DesignTime.IsActive)
                return;
            UITheme.ApplyForm(this);
            Load += async (_, _) =>
            {
                await LoadClassesAsync();
                await RefreshGridAsync();
            };
        }

        private async Task LoadClassesAsync()
        {
            var classes = await Academic.GetClassesAdminAsync();
            cboClass.DisplayMember = nameof(ClassDto.ClassName);
            cboClass.ValueMember = nameof(ClassDto.ClassId);
            cboClass.DataSource = classes.Where(c => c.IsActive).ToList();
        }

        private async Task RefreshGridAsync()
        {
            if (DesignTime.IsActive) return;
            var list = await Academic.GetSectionsAdminAsync();
            dgvSections.Rows.Clear();
            foreach (var s in list)
            {
                dgvSections.Rows.Add(
                    s.SectionId,
                    s.ClassId,
                    s.ClassName,
                    s.SectionName,
                    s.Capacity,
                    s.IsActive ? "Active" : "Inactive");
            }
        }

        private void ClearEditor()
        {
            _editSectionId = 0;
            if (cboClass.Items.Count > 0)
                cboClass.SelectedIndex = 0;
            txtSectionName.Clear();
            txtCapacity.Text = "40";
            chkActive.Checked = true;
            btnSave.Text = "Add Section";
            lblEditHint.Text = "New section";
        }

        private void dgvSections_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvSections.Rows[e.RowIndex];
            _editSectionId = Convert.ToInt32(row.Cells[0].Value);
            var classId = Convert.ToInt32(row.Cells[1].Value);
            cboClass.SelectedValue = classId;
            txtSectionName.Text = Convert.ToString(row.Cells[3].Value) ?? "";
            txtCapacity.Text = Convert.ToString(row.Cells[4].Value) ?? "40";
            chkActive.Checked = string.Equals(Convert.ToString(row.Cells[5].Value), "Active", StringComparison.OrdinalIgnoreCase);
            btnSave.Text = "Save Changes";
            lblEditHint.Text = "Editing Section #" + _editSectionId;
        }

        private async void btnSave_Click(object? sender, EventArgs e)
        {
            if (DesignTime.IsActive) return;
            try
            {
                if (cboClass.SelectedValue == null)
                {
                    MessageBox.Show("Select a class.", "Sections", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtCapacity.Text.Trim(), out var capacity) || capacity < 0)
                    capacity = 40;

                await Academic.SaveSectionAsync(new SectionDto
                {
                    SectionId = _editSectionId,
                    ClassId = Convert.ToInt32(cboClass.SelectedValue),
                    SectionName = txtSectionName.Text.Trim(),
                    Capacity = capacity,
                    IsActive = chkActive.Checked
                }, AppSession.Current?.Username);

                ClearEditor();
                await RefreshGridAsync();
                MessageBox.Show("Section saved.", "Sections", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNew_Click(object? sender, EventArgs e) => ClearEditor();
    }
}
