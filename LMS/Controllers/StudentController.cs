using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : CommonController
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var classes = from enroll in db.Enrolled
                          where enroll.Student.UId == uid
                          select new
                          {
                              subject = enroll.Class.Course.Department.Subject,
                              number = enroll.Class.Course.Number,
                              name = enroll.Class.Course.Name,
                              season = enroll.Class.Semester,
                              year = enroll.Class.Year,
                              grade = enroll.Grade
                          };
            return Json(classes.ToArray());

        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            var assignQuery = from asg in db.Assignments
                              where asg.Ac.Class.Semester == season &&
                              asg.Ac.Class.Year == year &&
                              asg.Ac.Class.Course.Number == num && 
                              asg.Ac.Class.Course.Department.Subject == subject
                              join sub in db.Submissions
                              on new { first = asg.AssignmentId, second = uid } equals new { first = sub.AssignmentId, second = sub.Student.UId }
                              into assignments
                              from assignment in assignments.DefaultIfEmpty()
                              select new
                              {
                                aname = asg.Name,
                                cname = asg.Ac.Name,
                                due = asg.Due,
                                score = (ushort?)assignment.Score

                              };
            return Json(assignQuery.ToArray());
        }

        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// Does *not* automatically reject late submissions.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}.</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            var student = from stu in db.Students
                          where stu.UId == uid
                          select stu;

            var assignment = from asg in db.Assignments
                             where asg.Ac.Class.Course.Department.Subject == subject &&
                             asg.Ac.Class.Course.Number == num &&
                             asg.Ac.Class.Semester == season &&
                             asg.Ac.Class.Year == year &&
                             asg.Ac.Name == category &&
                             asg.Name == asgname 
                             select asg;

            // if student or assignment does not exist, then return false.
            if(student.Count() <= 0 || assignment.Count() <= 0)
            {
                return Json(new { success = false });
            }

            // check if there is an assignment or not.
            var submission = from sub in db.Submissions
                             where sub.Assignment.Ac.Class.Course.Department.Subject == subject && 
                             sub.Assignment.Ac.Class.Course.Number == num && 
                             sub.Assignment.Ac.Class.Semester == season && 
                             sub.Assignment.Ac.Class.Year == year && 
                             sub.Assignment.Ac.Name == category && 
                             sub.Assignment.Name == asgname && 
                             sub.Student.UId == uid
                             select sub;

            // replace it
            if (submission.Count() > 0)
            {
                Submissions currentSub = submission.First();
                currentSub.Contents = contents;
                currentSub.Time = DateTime.Now;
                db.SaveChanges();
            }
            else
            {
                Submissions sub = new Submissions
                {
                    Time = DateTime.Now,
                    Score = 0,
                    Contents = contents,
                    StudentId = student.First().StudentId,
                    AssignmentId = assignment.First().AssignmentId
                };
                db.Submissions.Add(sub);
                db.SaveChanges();
            }  
            return Json(new { success = true });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false},
        /// false if the student is already enrolled in the Class.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {

            var Student = from student in db.Students
                          where student.UId == uid
                          select student;

            var Class = from _class in db.Classes
                        where _class.Course.Department.Subject == subject &&
                        _class.Course.Number == num && 
                        _class.Semester == season && 
                        _class.Year == year
                        select _class;

            // if student or class is empty
            if(Student.Count() <=0 || Class.Count() <= 0)
            {
                return Json(new { success = false });
            }

            // Check if database has the data or not.
            var Enroll = from enroll in db.Enrolled
                           where enroll.Student.UId == uid && 
                           enroll.ClassId == Class.First().ClassId
                           select enroll;

            // If student is already in the class
            if(Enroll.Count() > 0)
            {
                return Json(new { success = false });
            }
            else { 
                Enrolled en = new Enrolled{
                    StudentId = Student.First().StudentId,
                    ClassId = Class.First().ClassId,
                    Grade = "--"
                };

                db.Enrolled.Add(en);
                db.SaveChanges();
                return Json(new { success = true });
            }
        }


        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student does not have any grades, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            var GPAGrade = from enrollment in db.Enrolled
                          where enrollment.Student.UId == uid
                          select enrollment.Grade;

            if (GPAGrade.Count() > 0)
            {
                double FinalGPA = 0.0;
                double temp = 0.0;
                int count = 0;
                foreach(string letter in GPAGrade)
                {
                    temp = GPAScale(letter);
                    if(temp == -1.0)
                    {
                        continue;
                    }
                    else
                    {
                        FinalGPA += temp;
                        count++;
                    }
                }
                return Json(new { gpa = (double)(FinalGPA / count) });
            }
            else
            {
                return Json(new { gpa = 0.0 });
            }
        }


        /// <summary>
        /// Return the GPA that corresponding to the letter.
        /// </summary>
        /// <param name="GPA"></param>
        /// <returns></returns>
        private double GPAScale(string GPA)
        {
            switch (GPA)
            {
                case "A+":
                case "A": return 4.0;
                case "A-": return 3.7;
                case "B+": return 3.3;
                case "B": return 3.0;
                case "B-": return 2.7;
                case "C+": return 2.3;
                case "C": return 2.0;
                case "C-": return 1.7;
                case "D+": return 1.3;
                case "D": return 1.0;
                case "--": return -1.0;
                default: return 0.0;
            }
        }

        /*******End code to modify********/

    }
}