using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            /*
            var students = from _class in db.Classes
                           where _class.Course.Department.Subject == subject && _class.Course.Number == num &&
                                 _class.Semester == season && _class.Year == year
                           join enroll in db.Enrolled
                           on _class.ClassId equals enroll.ClassId
                           select new
                           {
                               fname = enroll.Student.FirstName,
                               lname = enroll.Student.LastName,
                               uid = enroll.Student.UId,
                               dob = enroll.Student.Dob,
                               grade = enroll.Grade
                           };
                           */
            var students = from enroll in db.Enrolled
                           where enroll.Class.Course.Department.Subject == subject &&
                                 enroll.Class.Course.Number == num &&
                                 enroll.Class.Semester == season &&
                                 enroll.Class.Year == year
                           select new
                           {
                               fname = enroll.Student.FirstName,
                               lname = enroll.Student.LastName,
                               uid = enroll.Student.UId,
                               dob = enroll.Student.Dob,
                               grade = enroll.Grade
                           };
            return Json(students.ToArray());
        }

        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            // null to return assignments from all categories.
            if(category == null || category.Length <= 0)
            {
                // return assignments from all categories.
                var assignments = from asg in db.Assignments
                                  where asg.Ac.Class.Course.Department.Subject == subject &&
                                  asg.Ac.Class.Course.Number == num && 
                                  asg.Ac.Class.Semester == season &&
                                  asg.Ac.Class.Year == year
                                  select new
                                  {
                                      aname = asg.Name,
                                      cname = asg.Ac.Name,
                                      due = asg.Due,
                                      submissions = asg.Submissions.Count()
                                  };
                 return Json(assignments.ToArray());
            }
            else
            {
                // return assignments from the categories.
                var assignments = from asg in db.Assignments
                                  where asg.Ac.Class.Course.Department.Subject == subject &&
                                  asg.Ac.Class.Course.Number == num && 
                                  asg.Ac.Class.Semester == season &&
                                  asg.Ac.Class.Year == year && 
                                  asg.Ac.Name == category
                                  select new
                                  {
                                      aname = asg.Name,
                                      cname = asg.Ac.Name,
                                      due = asg.Due,
                                      submissions = asg.Submissions.Count()
                                  };
                return Json(assignments.ToArray());

            }
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var Acs = from ac in db.AssignmentCategories
                      where ac.Class.Course.Department.Subject == subject &&
                      ac.Class.Course.Number == num && 
                      ac.Class.Semester == season &&
                      ac.Class.Year == year
                      select new
                      {
                          name = ac.Name,
                          weight = ac.Weight
                      };
            return Json(Acs.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false},
        ///	false if an assignment category with the same name already exists in the same class.</returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            if(category == null || category.Length <= 0 || catweight <= 0)
            {
                return Json(new { success = false });
            }

            var Category = from ac in db.AssignmentCategories
                           where ac.Class.Course.Department.Subject == subject &&
                           ac.Class.Course.Number == num &&
                           ac.Class.Semester == season &&
                           ac.Class.Year == year &&
                           ac.Name == category 
                           select ac;

            // false if an assignment category with the same name already exists in the same class.
            if (Category.Count() > 0)
            {
                return Json(new { success = false });
            }
            else
            {
                var Class = from _class in db.Classes
                            where _class.Course.Department.Subject == subject &&
                                  _class.Course.Number == num && 
                                  _class.Semester == season && 
                                  _class.Year == year
                            select _class;

                AssignmentCategories ac = new AssignmentCategories
                {
                    Weight = (ushort)catweight,
                    Name = category,
                    ClassId = Class.First().ClassId
                };

                db.AssignmentCategories.Add(ac);
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false,
        /// false if an assignment with the same name already exists in the same assignment category.</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            /*
            var assignment = from _class in db.Classes
                             where _class.Course.Department.Subject == subject &&
                             _class.Course.Number == num && _class.Semester == season &&
                             _class.Year == year
                             join ac in db.AssignmentCategories
                             on _class.ClassId equals ac.ClassId
                             where ac.Name == category
                             join asg in db.Assignments
                             on ac.Name equals asg.Ac.Name
                             where asg.Name == asgname
                             select asg;
                             */

            var assignment = from asg in db.Assignments
                             where asg.Ac.Class.Course.Department.Subject == subject &&
                             asg.Ac.Class.Course.Number == num &&
                             asg.Ac.Class.Semester == season &&
                             asg.Ac.Class.Year == year &&
                             asg.Ac.Name == category &&
                             asg.Name == asgname
                             select asg;

            // If it has the assignment, then return, else create.
            if(assignment.Count() > 0)
            {
                return Json(new { success = false });
            }
            else
            {
                // find AssignmentCategorie ID
                var acID = (from ac in db.AssignmentCategories
                            where ac.Class.Course.Department.Subject == subject &&
                            ac.Class.Course.Number == num && 
                            ac.Class.Semester == season &&
                            ac.Class.Year == year && 
                            ac.Name == category
                            select ac).First().Acid;

                Assignments asg = new Assignments
                {
                    Name = asgname,
                    MaxPoints = (ushort)asgpoints,
                    Contents = asgcontents,
                    Due = asgdue,
                    Acid = acID
                };

                db.Assignments.Add(asg);
                db.SaveChanges();

                var studentEnrolled = from enroll in db.Enrolled
                                      where enroll.Class.Course.Department.Subject == subject &&
                                      enroll.Class.Course.Number == num &&
                                      enroll.Class.Semester == season &&
                                      enroll.Class.Year == year
                                      select enroll;

                foreach(var student in studentEnrolled)
                {
                    // recalculate GPA.
                    double TotalWeight = 0;
                    double TotalPoint = 0;
                    var grades = from sub in db.Submissions
                                 where sub.Assignment.Ac.Class.Course.Department.Subject == subject &&
                                 sub.Assignment.Ac.Class.Course.Number == num &&
                                 sub.Assignment.Ac.Class.Semester == season &&
                                 sub.Assignment.Ac.Class.Year == year &&
                                 sub.StudentId == student.StudentId
                                 select new
                                 {
                                     score = sub.Score,
                                     point = sub.Assignment.MaxPoints,
                                     weight = sub.Assignment.Ac.Weight
                                 };

                    double percentage = 0;
                    foreach (var temp in grades)
                    {
                        // the percentage score of an assignment.
                        percentage = ((double)temp.score / (double)temp.point) * 100;
                        // Calculate total weighted points.
                        TotalPoint = TotalPoint + (percentage * temp.weight);
                        TotalWeight += temp.weight;
                    }
                    string LetterGrade = "--";
                    if (TotalWeight != 0)
                    {
                        double grade = TotalPoint / TotalWeight;
                        LetterGrade = GetLetterGrade(grade);
                    }
                    // find the enrolled class.
                    var EnrolledClassGrade = from enroll in db.Enrolled
                                             where enroll.Class.Course.Department.Subject == subject &&
                                             enroll.Class.Course.Number == num &&
                                             enroll.Class.Semester == season &&
                                             enroll.Class.Year == year && 
                                             enroll.StudentId == student.StudentId
                                             select enroll;

                    EnrolledClassGrade.First().Grade = LetterGrade;
                    db.Enrolled.UpdateRange(EnrolledClassGrade);
                    db.SaveChanges();
                }
                return Json(new { success = true });
            }
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var submissions = from sub in db.Submissions
                              where sub.Assignment.Ac.Class.Course.Department.Subject == subject &&
                              sub.Assignment.Ac.Class.Course.Number == num &&
                              sub.Assignment.Ac.Class.Semester == season &&
                              sub.Assignment.Ac.Class.Year == year &&
                              sub.Assignment.Ac.Name == category &&
                              sub.Assignment.Name == asgname
                              select new
                              {
                                  fname = sub.Student.FirstName,
                                  lname = sub.Student.LastName,
                                  uid = sub.Student.UId,
                                  time = sub.Time,
                                  score = sub.Score
                              };
            return Json(submissions.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            var submissions = from sub in db.Submissions
                              where sub.Assignment.Ac.Class.Course.Department.Subject == subject &&
                              sub.Assignment.Ac.Class.Course.Number == num &&
                              sub.Assignment.Ac.Class.Semester == season &&
                              sub.Assignment.Ac.Class.Year == year &&
                              sub.Assignment.Ac.Name == category &&
                              sub.Assignment.Name ==asgname &&
                              sub.Student.UId == uid
                              select sub;

            if(submissions.Count() > 0)
            {
                submissions.First().Score = (ushort)score;
                db.SaveChanges();

                // recalculate GPA.
                double TotalWeight = 0;
                double TotalPoint = 0;
                var grades = from sub in db.Submissions
                             where sub.Assignment.Ac.Class.Course.Department.Subject == subject &&
                             sub.Assignment.Ac.Class.Course.Number == num &&
                             sub.Assignment.Ac.Class.Semester == season &&
                             sub.Assignment.Ac.Class.Year == year &&
                             sub.Student.UId == uid
                             select new
                             {
                                 score = sub.Score,
                                 point = sub.Assignment.MaxPoints,
                                 weight = sub.Assignment.Ac.Weight
                             };
                                            

                double percentage = 0;
                foreach (var temp in grades)
                {
                    // the percentage score of an assignment.
                    percentage = ((double)temp.score / (double)temp.point) * 100;
                    // Calculate total weighted points.
                    TotalPoint = TotalPoint + (percentage * temp.weight);
                    TotalWeight += temp.weight;
                }
                string LetterGrade = "--";
                if (TotalWeight != 0)
                {
                    double grade = TotalPoint / TotalWeight;
                    LetterGrade = GetLetterGrade(grade);
                }
                // find the enrolled class.
                var EnrolledClassGrade = from enroll in db.Enrolled
                                         where enroll.Class.Course.Department.Subject == subject &&
                                         enroll.Class.Course.Number == num &&
                                         enroll.Class.Semester == season &&
                                         enroll.Class.Year == year &&
                                         enroll.Student.UId == uid
                                         select enroll;

                EnrolledClassGrade.First().Grade = LetterGrade;
                db.Enrolled.UpdateRange(EnrolledClassGrade);
                db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        private string GetLetterGrade(double score)
        {
            if (score >= 93.0)
            {
                return "A";
            }
            else if (score >= 90.0)
            {
                return "A-";
            }
            else if (score >= 87.0)
            {
                return "B+";
            }
            else if (score >= 83.0)
            {
                return "B";
            }
            else if (score >= 80.0 )
            {
                return "B-";
            }
            else if (score >= 77.0)
            {
                return "C+";
            }
            else if (score >= 73.0)
            {
                return "C";
            }
            else if (score >= 70.0)
            {
                return "C-";
            }
            else if (score >= 67.0)
            {
                return "D+";
            }
            else if (score >= 65.0)
            {
                return "D";
            }
            else
            {
                return "F";
            }

        }



        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var classes = from _class in db.Classes
                          where _class.Professor.UId == uid
                          select new
                          {
                              subject = _class.Course.Department.Subject,
                              number = _class.Course.Number,
                              name = _class.Course.Name,
                              season = _class.Semester,
                              year = _class.Year
                          };
            return Json(classes.ToArray());
        }
        /*******End code to modify********/

    }
}