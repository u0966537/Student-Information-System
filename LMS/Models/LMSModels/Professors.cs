using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professors
    {
        public Professors()
        {
            Classes = new HashSet<Classes>();
        }

        public uint ProfessorId { get; set; }
        public string UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }
        public uint DepartmentId { get; set; }

        public virtual Departments Department { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
