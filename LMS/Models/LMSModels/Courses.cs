using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Courses
    {
        public Courses()
        {
            Classes = new HashSet<Classes>();
        }

        public uint CourseId { get; set; }
        public string Name { get; set; }
        public uint DepartmentId { get; set; }
        public ushort Number { get; set; }
        public virtual Departments Department { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
