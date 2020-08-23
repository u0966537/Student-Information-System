using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class CommonController : Controller
    {

        /*******Begin code to modify********/

        // TODO: Uncomment and change 'X' after you have scaffoled


        protected Team49LMSContext db;

        public CommonController()
        {
            db = new Team49LMSContext();
        }


        /*
         * WARNING: This is the quick and easy way to make the controller
         *          use a different LibraryContext - good enough for our purposes.
         *          The "right" way is through Dependency Injection via the constructor 
         *          (look this up if interested).
        */
        public void UseLMSContext(Team49LMSContext ctx)
        {
          db = ctx;
        }

        protected override void Dispose(bool disposing)
        {
          if (disposing)
          {
            db.Dispose();
          }
          base.Dispose(disposing);
        }
        
        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            var departments = from department in db.Departments
                              select new
                              {
                                  name = department.Name,
                                  subject = department.Subject
                              };
            return Json(departments.ToArray());
        }



        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 5530)
        ///            "cname": The course name (e.g. "Database Systems")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {
            var catalog = from department in db.Departments
                          select new
                          {
                              subject = department.Subject,
                              dname = department.Name,
                              courses = from c in department.Courses
                                        select new
                                        {
                                            number = c.Number,
                                            cname = c.Name
                                        }

                          };

            return Json(catalog.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject">The subject abbreviation, as in "CS"</param>
        /// <param name="number">The course number, as in 5530</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {
            var classes = from _class in db.Classes
                    where _class.Course.Number == number &&
                          _class.Course.Department.Subject == subject
                    select new
                    {
                        season = _class.Semester,
                        year = _class.Year,
                        location = _class.Location,
                        start = _class.StartTime,
                        end = _class.EndTime,
                        fname = _class.Professor.FirstName,
                        lname = _class.Professor.LastName
                    };

            return Json(classes.ToArray());
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {
            var AssignmentContents = from asg in db.Assignments
                    where asg.Ac.Class.Course.Department.Subject == subject &&
                          asg.Ac.Class.Course.Number == num &&
                          asg.Ac.Class.Semester == season &&
                          asg.Ac.Class.Year == year &&
                          asg.Ac.Name == category &&
                          asg.Name == asgname
                    select asg.Contents;

            if (AssignmentContents.Count() > 0)
            {
                return Content(AssignmentContents.First());
            }

            return Content("");
        }


        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// Returns the empty string ("") if there is no submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {
            /*
            var submission = from enroll in db.Enrolled
                             where enroll.Student.UId == uid
                             join _class in db.Classes
                             on enroll.ClassId equals _class.ClassId
                             where _class.Course.Department.Subject == subject &&
                             _class.Course.Number == num && _class.Semester == season &&
                             _class.Year == year
                             join ac in db.AssignmentCategories
                             on _class.ClassId equals ac.ClassId
                             where ac.Name == category
                             join asg in db.Assignments
                             on ac.Acid equals asg.Acid
                             where asg.Name == asgname
                             join sub in db.Submissions
                             on asg.AssignmentId equals sub.AssignmentId
                             select sub.Contents ;
                             */

            var submission = from sub in db.Submissions
                    where sub.Assignment.Ac.Class.Course.Department.Subject == subject &&
                          sub.Assignment.Ac.Class.Course.Number == num &&
                          sub.Assignment.Ac.Class.Semester == season &&
                          sub.Assignment.Ac.Class.Year == year &&
                          sub.Assignment.Ac.Name == category &&
                          sub.Assignment.Name == asgname &&
                          sub.Student.UId == uid
                    select sub.Contents;


            if(submission.Count() > 0)
            {
                return Content(submission.First());
            }
            return Content("");
        }


        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>
        /// The user JSON object 
        /// or an object containing {success: false} if the user doesn't exist
        /// </returns>
        public IActionResult GetUser(string uid)
        {
            // loop it up inside of student table.
            var StudentUser = from student in db.Students
                              where student.UId == uid
                              select new
                              {
                                  fname = student.FirstName,
                                  lname = student.LastName,
                                  uid = student.UId,
                                  department = student.Department.Name
                              };

            if(StudentUser.Count() > 0)
            {
                return Json(StudentUser.First());
            }

            // look it up inside of professor table.
            var ProfessorUser = from professor in db.Professors
                              where professor.UId == uid
                              select new
                              {
                                  fname = professor.FirstName,
                                  lname = professor.LastName,
                                  uid = professor.UId,
                                  department = professor.Department.Name
                              };

            if(ProfessorUser.Count() > 0)
            {
                return Json(ProfessorUser.First());
            }

            // look it up inside of admin table.
            var AdminUser = from admin in db.Administrators
                                where admin.UId == uid
                                select new
                                {
                                    fname = admin.FirstName,
                                    lname = admin.LastName,
                                    uid = admin.UId,
                                };

            if (AdminUser.Count() > 0)
            {
                return Json(AdminUser.First());
            }

            // an object containing {success: false} if the user doesn't exist
            return Json(new { success = false });
        }
        /*******End code to modify********/

    }
}