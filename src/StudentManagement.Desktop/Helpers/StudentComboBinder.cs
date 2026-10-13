using StudentManagement.UserObjects.Objects;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Helpers
{
    /// <summary>Binds and filters student ComboBoxes (type-to-search + optional class filter).</summary>
    public static class StudentComboBinder
    {
        public sealed class StudentListItem
        {
            public int StudentId { get; set; }
            public int ClassId { get; set; }
            public string Display { get; set; } = string.Empty;
        }

        public static void Bind(
            ComboBox cbo,
            IEnumerable<StudentDto> students,
            string? filter = null,
            int? classId = null)
        {
            IEnumerable<StudentDto> query = students ?? Enumerable.Empty<StudentDto>();

            if (classId is > 0)
                query = query.Where(s => s.ClassId == classId.Value);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var term = filter.Trim();
                query = query.Where(s =>
                    (!string.IsNullOrEmpty(s.FullName) &&
                     s.FullName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(s.RegistrationNo) &&
                     s.RegistrationNo.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(s.GuardianPhone) &&
                     s.GuardianPhone.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(s.RollNumber) &&
                     s.RollNumber.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(s.ClassName) &&
                     s.ClassName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            var items = query
                .OrderBy(s => s.ClassName)
                .ThenBy(s => s.RollNumber)
                .ThenBy(s => s.FullName)
                .Select(s => new StudentListItem
                {
                    StudentId = s.StudentId,
                    ClassId = s.ClassId,
                    Display = string.IsNullOrWhiteSpace(s.ClassName)
                        ? $"{s.RegistrationNo} — {s.FullName}"
                        : $"{s.RegistrationNo} — {s.FullName}  ({s.ClassName} · Roll {s.RollNumber})"
                })
                .ToList();

            var previousId = TryGetSelectedInt(cbo);

            cbo.BeginUpdate();
            try
            {
                cbo.DataSource = null;
                cbo.DisplayMember = nameof(StudentListItem.Display);
                cbo.ValueMember = nameof(StudentListItem.StudentId);
                cbo.DataSource = items;

                if (previousId is int id && items.Any(i => i.StudentId == id))
                    cbo.SelectedValue = id;
                else if (items.Count > 0)
                    cbo.SelectedIndex = 0;
                else
                    cbo.SelectedIndex = -1;
            }
            finally
            {
                cbo.EndUpdate();
            }
        }

        public static int? TryGetSelectedInt(ComboBox? cbo)
        {
            if (cbo?.SelectedValue == null || cbo.SelectedValue == DBNull.Value)
                return null;

            try
            {
                var value = Convert.ToInt32(cbo.SelectedValue);
                return value > 0 ? value : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
