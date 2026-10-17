using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using StudentManagement.BusinessLayer.Offline;
using StudentManagement.DataAccessLayer;
using StudentManagement.DataAccessLayer.Data_Access_Methods;
using StudentManagement.UserObjects.Objects;

namespace StudentManagement.BusinessLayer
{
    public class AcademicManager
    {
        public List<BloodGroupDto> GetBloodGroups()
        {
            if (DbConfig.OfflineMode)
                return OfflineStore.Instance.BloodGroups.Where(b => b.IsActive).OrderBy(b => b.SortOrder).ToList();

            return new tblAcademic().GetBloodGroups().AsEnumerable().Select(r => new BloodGroupDto
            {
                BloodGroupId = Convert.ToInt32(r["BloodGroupId"]),
                Name = Convert.ToString(r["Name"]) ?? string.Empty,
                Code = Convert.ToString(r["Code"]) ?? string.Empty,
                SortOrder = r.Table.Columns.Contains("SortOrder") && r["SortOrder"] != DBNull.Value
                    ? Convert.ToInt32(r["SortOrder"]) : 0,
                IsActive = !r.Table.Columns.Contains("IsActive") || r["IsActive"] == DBNull.Value || Convert.ToBoolean(r["IsActive"])
            }).ToList();
        }

        public List<PaymentMethodDto> GetPaymentMethods()
        {
            if (DbConfig.OfflineMode)
                return OfflineStore.Instance.PaymentMethods.Where(p => p.IsActive).OrderBy(p => p.SortOrder).ToList();

            return new tblAcademic().GetPaymentMethods().AsEnumerable().Select(r => new PaymentMethodDto
            {
                PaymentMethodId = Convert.ToInt32(r["PaymentMethodId"]),
                Name = Convert.ToString(r["Name"]) ?? string.Empty,
                SortOrder = r.Table.Columns.Contains("SortOrder") && r["SortOrder"] != DBNull.Value
                    ? Convert.ToInt32(r["SortOrder"]) : 0,
                IsActive = !r.Table.Columns.Contains("IsActive") || r["IsActive"] == DBNull.Value || Convert.ToBoolean(r["IsActive"])
            }).ToList();
        }

        public List<ClassDto> GetClassesAdmin()
        {
            if (DbConfig.OfflineMode)
                return OfflineStore.Instance.Classes.OrderBy(c => c.SortOrder).ThenBy(c => c.ClassName).ToList();

            return new tblAcademic().GetClassesAdmin().AsEnumerable().Select(MapClass).ToList();
        }

        public int SaveClass(ClassDto dto, string? updatedBy = null)
        {
            if (dto == null) throw new ArgumentException("Class is required.");
            if (string.IsNullOrWhiteSpace(dto.ClassName)) throw new ArgumentException("Class name is required.");

            if (DbConfig.OfflineMode)
            {
                var store = OfflineStore.Instance;
                if (dto.ClassId <= 0)
                {
                    dto.ClassId = store.Classes.Count == 0 ? 1 : store.Classes.Max(c => c.ClassId) + 1;
                    store.Classes.Add(new ClassDto
                    {
                        ClassId = dto.ClassId,
                        ClassName = dto.ClassName.Trim(),
                        ClassCode = dto.ClassCode?.Trim(),
                        SortOrder = dto.SortOrder,
                        IsActive = dto.IsActive
                    });
                    return dto.ClassId;
                }

                var existing = store.Classes.FirstOrDefault(c => c.ClassId == dto.ClassId)
                               ?? throw new InvalidOperationException("Class not found.");
                existing.ClassName = dto.ClassName.Trim();
                existing.ClassCode = dto.ClassCode?.Trim();
                existing.SortOrder = dto.SortOrder;
                existing.IsActive = dto.IsActive;
                return existing.ClassId;
            }

            return new tblAcademic().SaveClass(dto, updatedBy);
        }

        public List<SectionDto> GetSectionsAdmin(int? classId = null)
        {
            if (DbConfig.OfflineMode)
            {
                return OfflineStore.Instance.Sections
                    .Where(s => !classId.HasValue || classId.Value <= 0 || s.ClassId == classId.Value)
                    .Select(s =>
                    {
                        s.ClassName = OfflineStore.Instance.Classes.FirstOrDefault(c => c.ClassId == s.ClassId)?.ClassName ?? s.ClassName;
                        return s;
                    })
                    .OrderBy(s => s.ClassName).ThenBy(s => s.SectionName)
                    .ToList();
            }

            return new tblAcademic().GetSectionsAdmin(classId).AsEnumerable().Select(r => new SectionDto
            {
                SectionId = Convert.ToInt32(r["SectionId"]),
                ClassId = Convert.ToInt32(r["ClassId"]),
                ClassName = r.Table.Columns.Contains("ClassName") ? Convert.ToString(r["ClassName"]) ?? "" : "",
                SectionName = Convert.ToString(r["SectionName"]) ?? string.Empty,
                Capacity = r.Table.Columns.Contains("Capacity") && r["Capacity"] != DBNull.Value
                    ? Convert.ToInt32(r["Capacity"]) : 40,
                IsActive = !r.Table.Columns.Contains("IsActive") || r["IsActive"] == DBNull.Value || Convert.ToBoolean(r["IsActive"])
            }).ToList();
        }

        public int SaveSection(SectionDto dto, string? updatedBy = null)
        {
            if (dto == null) throw new ArgumentException("Section is required.");
            if (dto.ClassId <= 0) throw new ArgumentException("Class is required.");
            if (string.IsNullOrWhiteSpace(dto.SectionName)) throw new ArgumentException("Section name is required.");

            if (DbConfig.OfflineMode)
            {
                var store = OfflineStore.Instance;
                if (dto.SectionId <= 0)
                {
                    if (store.Sections.Any(s => s.ClassId == dto.ClassId
                        && string.Equals(s.SectionName, dto.SectionName.Trim(), StringComparison.OrdinalIgnoreCase)))
                        throw new InvalidOperationException("Section already exists for this class.");

                    dto.SectionId = store.Sections.Count == 0 ? 1 : store.Sections.Max(s => s.SectionId) + 1;
                    store.Sections.Add(new SectionDto
                    {
                        SectionId = dto.SectionId,
                        ClassId = dto.ClassId,
                        ClassName = store.Classes.FirstOrDefault(c => c.ClassId == dto.ClassId)?.ClassName ?? "",
                        SectionName = dto.SectionName.Trim(),
                        Capacity = dto.Capacity <= 0 ? 40 : dto.Capacity,
                        IsActive = dto.IsActive
                    });
                    return dto.SectionId;
                }

                var existing = store.Sections.FirstOrDefault(s => s.SectionId == dto.SectionId)
                               ?? throw new InvalidOperationException("Section not found.");
                existing.ClassId = dto.ClassId;
                existing.ClassName = store.Classes.FirstOrDefault(c => c.ClassId == dto.ClassId)?.ClassName ?? existing.ClassName;
                existing.SectionName = dto.SectionName.Trim();
                existing.Capacity = dto.Capacity <= 0 ? 40 : dto.Capacity;
                existing.IsActive = dto.IsActive;
                return existing.SectionId;
            }

            return new tblAcademic().SaveSection(dto, updatedBy);
        }

        private static ClassDto MapClass(DataRow r) => new ClassDto
        {
            ClassId = Convert.ToInt32(r["ClassId"]),
            ClassName = Convert.ToString(r["ClassName"]) ?? string.Empty,
            ClassCode = r.Table.Columns.Contains("ClassCode") ? Convert.ToString(r["ClassCode"]) : null,
            SortOrder = r.Table.Columns.Contains("SortOrder") && r["SortOrder"] != DBNull.Value
                ? Convert.ToInt32(r["SortOrder"]) : 0,
            IsActive = !r.Table.Columns.Contains("IsActive") || r["IsActive"] == DBNull.Value || Convert.ToBoolean(r["IsActive"])
        };

        public System.Threading.Tasks.Task<List<BloodGroupDto>> GetBloodGroupsAsync(System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetBloodGroups());
        public System.Threading.Tasks.Task<List<PaymentMethodDto>> GetPaymentMethodsAsync(System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetPaymentMethods());
        public System.Threading.Tasks.Task<List<ClassDto>> GetClassesAdminAsync(System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetClassesAdmin());
        public System.Threading.Tasks.Task<int> SaveClassAsync(ClassDto dto, string? updatedBy = null, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(SaveClass(dto, updatedBy));
        public System.Threading.Tasks.Task<List<SectionDto>> GetSectionsAdminAsync(int? classId = null, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetSectionsAdmin(classId));
        public System.Threading.Tasks.Task<int> SaveSectionAsync(SectionDto dto, string? updatedBy = null, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(SaveSection(dto, updatedBy));
    }
}
