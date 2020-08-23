using System;
using System.Linq;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 5530)
        /// "name" - The course name (as in "Database Systems")
        /// </summary>
        /// <param name="subject">The department subject abbreviation (as in "CS")</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetCourses(string subject)
        {
            var Courses = from course in db.Courses
                          where course.Department.Subject == subject
                          select new
                          {
                              number = course.Number,
                              name = course.Name
                          };
             return Json(Courses.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetProfessors(string subject)
        {
            var Professor = from professor in db.Professors
                            where professor.Department.Subject == subject
                            select new
                            {
                                lname = professor.LastName,
                                fname = professor.FirstName,
                                uid = professor.UId
                            };
             return Json(Professor.ToArray());
        }

        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false},
        /// false if the Course already exists.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {
            // Base case:
            if (subject == null || subject.Length == 0 || name == null || name.Length == 0 || number <= 0 )
            {
                return Json(new { success = false });
            }

            // Check if database contains the course or not.
            var ExistingCourses = from course in db.Courses
                          where course.Number == number && course.Department.Subject == subject
                          select course;
            if (ExistingCourses.Count() > 0)
            {
                return Json(new { success = false });
            }

            // Find the departmentID that corresponding to the subject.
            var DepartmentID = from department in db.Departments where department.Subject == subject select department.DepartmentId;

            // if DepartmentID is empty
            if(DepartmentID.Count() <= 0)
            {
                return Json(new { success = false });
            }

            Courses Course = new Courses
            {
                Name = name,
                Number = (ushort)number,
                DepartmentId = DepartmentID.First()
            };

            db.Courses.Add(Course);
            db.SaveChanges();
            return Json(new { success = true });
        }

        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            // base case:
            if(year > 2100 || year < 1900)
            {
                return Json(new { success = false });
            }
            if (start == null || end == null || end.TimeOfDay <= start.TimeOfDay)
            {
                return Json(new { success = false });
            }
            if(instructor == null || instructor.Length == 0)
            {
                return Json(new { success = false });
            }

            // false if another class occupies the same location during any time 
            // within the start-end range in the same semester
            var ExistingClasses1 = from _class in db.Classes
                                  where _class.Year == year && _class.Semester == season && _class.Location == location
                                  && ((_class.StartTime <= end.TimeOfDay) && (start.TimeOfDay <= _class.EndTime))
                                  select _class;
            if(ExistingClasses1.Count() > 0)
            {
                return Json(new { success = false });
            }

            // or if there is already
            // a Class offering of the same Course in the same Semester.
            var ExistingClasses2 = from _class in db.Classes
                                   where _class.Course.Department.Subject == subject && _class.Course.Number == number
                                   && _class.Semester == season && _class.Year == year
                                   select _class;
            if (ExistingClasses2.Count() > 0)
            {
                return Json(new { success = false });
            }

            // get course id.
            var CourseID = from course in db.Courses
                         where course.Number == number && course.Department.Subject == subject 
                         select course.CourseId;

            // get professor id.
            var ProfessorID = from professor in db.Professors
                             where professor.UId == instructor
                             select professor.ProfessorId;

            // if course or professor is empty, then return false;
            if(CourseID.Count() <= 0 || ProfessorID.Count() <= 0)
            {
                return Json(new { success = false });
            }

            Classes Class = new Classes
            {
                Semester = season,
                Year = (ushort)year,
                Location = location,
                StartTime = start.TimeOfDay,
                EndTime = end.TimeOfDay,
                CourseId = CourseID.First(),
                ProfessorId = ProfessorID.First()
            };
            db.Classes.Add(Class);
            db.SaveChanges();
            return Json(new { success = true });
        }
        /*******End code to modify********/
    }
}