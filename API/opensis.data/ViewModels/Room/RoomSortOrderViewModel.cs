using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.ViewModels.Room
{
    public class RoomSortOrderViewModel : CommonFields
    {
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        //public int? RoomId { get; set; }
        public int? PreviousSortOrder { get; set; }
        public int? CurrentSortOrder { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
