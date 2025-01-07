using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.ViewModels.StudentSchedule
{
    public class ScheduledStudentDeleteViewModel : CommonFields
    {
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public int? CourseSectionId { get; set; }
        public List<int>? StudentIds { get; set; }
        public List<int>? StaffIds { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
