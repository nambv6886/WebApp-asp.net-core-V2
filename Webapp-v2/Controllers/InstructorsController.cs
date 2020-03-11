using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Webapp_v2.Data;
using Webapp_v2.Models;
using Webapp_v2.Models.SchoolViewModels;

namespace Webapp_v2.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly SchoolContext _context;

        public InstructorsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Instructors
        public async Task<IActionResult> Index(int? id, int? courseID)
        {
            var viewModel = new InstructorIndexData();
            viewModel.Instructors = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(i => i.Enrollments)
                            .ThenInclude(i => i.Student)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(i => i.Department)
                .AsNoTracking()
                .OrderBy(i => i.LastName)
                .ToListAsync();

            if(id != null)
            {
                ViewData["InstructorID"] = id.Value;

                Instructor instructor = viewModel.Instructors.Where(
                    i => i.ID == id.Value).Single();

                viewModel.Courses = instructor.CourseAssignments.Select(s => s.Course);
            }

            if(courseID != null)
            {
                ViewData["CourseID"] = courseID.Value;
                viewModel.Enrollments = viewModel.Courses.Where(c => c.CourseID == courseID).Single().Enrollments;
            }

            return View(viewModel);
        }

        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructors/Create
        public IActionResult Create()
        {
            var instructor = new Instructor();

            instructor.CourseAssignments = new List<CourseAssignment>();
            PopulateAssignedCourseData(instructor);
            return View();
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LastName,FirstName,HireDate,OfficeAssignment")] Instructor instructor, string[] selectedCourses)
        {

            if(selectedCourses != null)
            {
                instructor.CourseAssignments = new List<CourseAssignment>();

                foreach (var course in selectedCourses)
                {
                    var courseToAdd = new CourseAssignment { CourseID = Convert.ToInt32(course), InstructorID = instructor.ID };
                    instructor.CourseAssignments.Add(courseToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateAssignedCourseData(instructor);

            return View(instructor);
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                //.AsNoTracking()
                .FirstOrDefaultAsync(i => i.ID == id);

            if (instructor == null)
            {
                return NotFound();
            }

            PopulateAssignedCourseData(instructor);

            return View(instructor);
        }

        // use for dislay course
        private void PopulateAssignedCourseData(Instructor instructor)
        {
            var allCourses = _context.Courses;
            var instructorCourses = new List<int>(instructor.CourseAssignments.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseData>();

            foreach (var item in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = item.CourseID,
                    Title = item.Title,
                    Assigned = instructorCourses.Contains(item.CourseID)
                });
            }
            ViewData["Courses"] = viewModel;
        }

        // add or remove course assignment selected by user
        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorUpdate)
        {
            if(selectedCourses == null)
            {
                instructorUpdate.CourseAssignments = new List<CourseAssignment>();
                return;
            }

            var selectedCoursesList = new List<string>(selectedCourses);

            var instructorCourses = new List<int>(instructorUpdate.CourseAssignments.Select(c => c.Course.CourseID));

            foreach (var course in _context.Courses)
            {
                // if user selected a course
                if(selectedCourses.Contains(course.CourseID.ToString()))
                {
                    // if course not in course assignment
                    if(!instructorCourses.Contains(course.CourseID))
                    {
                        // add a new course assignment
                        instructorUpdate.CourseAssignments.Add(new CourseAssignment { InstructorID = instructorUpdate.ID, CourseID = course.CourseID });

                    }
                }
                // if user not selected a course
                else
                {
                    // a course in a assignment course 
                    if(instructorCourses.Contains(course.CourseID))
                    {
                        // remove it
                        CourseAssignment removeCourseAssignment = instructorUpdate.CourseAssignments.FirstOrDefault(c => c.CourseID == course.CourseID);
                        _context.Remove(removeCourseAssignment);
                    }
                }
            }
        }


        // POST: Instructors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedCourses)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorToUpdate = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                //.AsNoTracking()
                .FirstOrDefaultAsync(i => i.ID == id);

            if (await TryUpdateModelAsync<Instructor>(
                instructorToUpdate,
                "",
                i => i.FirstName, i => i.LastName, i => i.HireDate, i => i.OfficeAssignment))
            {
                if(String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
                {
                    instructorToUpdate.OfficeAssignment = null;
                }
                UpdateInstructorCourses(selectedCourses, instructorToUpdate);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save change");
                }

                return RedirectToAction(nameof(Index));
            }

            UpdateInstructorCourses(selectedCourses, instructorToUpdate);
            PopulateAssignedCourseData(instructorToUpdate);
            return View(instructorToUpdate);
        }

        // GET: Instructors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors
                .Include(i => i.CourseAssignments)
                .SingleOrDefaultAsync(i => i.ID == id);

            var departments = await _context.Departments
                .Where(d => d.InstructorID == id)
                .ToListAsync();

            departments.ForEach(d => d.InstructorID = null);

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.ID == id);
        }
    }
}
