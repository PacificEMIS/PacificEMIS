﻿using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StudentAttendances
{
    public class StudentAttendanceCommentsAddViewModel : CommonFields
    {        
        public StudentAttendanceComments? studentAttendanceComments { get; set; }
        public int? StaffId { get; set; }
    }
}
