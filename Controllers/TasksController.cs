using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication22.Models;
using WebApplication22.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApplication22.Controllers
{
    [Route("Tasks")]
    public class TasksController : Controller
    {
        private readonly AppDbContext _db;

        public TasksController(AppDbContext context)
        {
            _db = context;
        }

        // GET: Tasks
        [HttpGet("Index/{projectId:int}")]
        public IActionResult Index(int projectId)
        {
            var tasks = _db.ProjectTasks
                                .Where(t => t.ProjectId == projectId)
                                .ToList();
            ViewBag.ProjectId = projectId;     // Store projectId in ViewBag
            return View(tasks);
        }

        // GET: Tasks/Details/5
        [HttpGet("Details/{id:int}")]
        public IActionResult Details(int id)
        {
            var task = _db.ProjectTasks
                            .Include(t => t.Project) // Include related project data
                            .FirstOrDefault(t => t.ProjectTaskId == id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpGet("Create/{projectId:int}")]
        public IActionResult Create(int projectId)
        {
            var project = _db.Projects.Find(projectId);
            if (project == null)
            {
                return NotFound(); // Or handle appropriately if project doesn't exist
            }

            var task = new ProjectTask
            {
                ProjectId = projectId
            };

            return View(task);
        }



        [HttpPost("Create/{projectId:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title", "Description", "ProjectId")] ProjectTask task)
        {
            if (ModelState.IsValid)
            {
                _db.ProjectTasks.Add(task);
                _db.SaveChanges();
                // Redirect to the Index action with the projectId of the created task
                return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });
            }

            // Repopulate the Projects SelectList if returning to the form
            ViewBag.Projects = new SelectList(_db.Projects, "ProjectId", "Name", task.ProjectId);
            return View(task);
        }

        [HttpGet("Edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            var task = _db.ProjectTasks
                                .Include(t => t.Project) // Include related project data
                                .FirstOrDefault(t => t.ProjectTaskId == id);

            if (task == null)
            {
                return NotFound();
            }

            ViewBag.Projects = new SelectList(_db.Projects, "ProjectId", "Name", task.ProjectId);
            return View(task);
        }

        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("ProjectTaskId", "Title", "Description", "ProjectId")] ProjectTask task)
        {
            if (id != task.ProjectTaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Update(task);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });
            }

            ViewBag.Projects = new SelectList(_db.Projects, "ProjectId", "Name", task.ProjectId);
            return View(task);
        }

        [HttpGet("Delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var task = _db.ProjectTasks
                                .Include(t => t.Project) // Include related project data
                                .FirstOrDefault(t => t.ProjectTaskId == id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost("DeleteConfirmed/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int ProjectTaskId)
        {
            var task = _db.ProjectTasks.Find(ProjectTaskId);
            if (task != null)
            {
                _db.ProjectTasks.Remove(task);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });
            }

            return NotFound();
        }
        //Lab 5 - Search ProjectTasks
        // GET: Tasks/Search/{projectId}/{searchString?}
      
        [HttpGet("Search/{projectId:int}/{searchString?}")]
        public async Task<IActionResult> Search(int projectId, string searchString)
        {
            // var tasksQuery = _db.ProjectTasks.Where(t => t.ProjectId == projectId);
            var tasksQuery = _db.ProjectTasks.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                tasksQuery = tasksQuery.Where(t => t.Title.Contains(searchString)
                                            || t.Description.Contains(searchString));
            }

            var tasks = await tasksQuery.ToListAsync();
            ViewBag.ProjectId = projectId; // To keep track of the current project
            return View("Index", tasks); // Reuse the Index view to display results
        }


    }
}
