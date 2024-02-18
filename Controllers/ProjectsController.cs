using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication22.Data;
using WebApplication22.Models;
namespace WebApplication22.Controllers
{
    [Route("Projects")]
    public class ProjectsController : Controller
    {
        private readonly AppDbContext _db;
        public ProjectsController(AppDbContext db)
        {

            _db = db;

        }

        /**
         * GET: Projects
         * Accessible at /Projects
        **/
        [HttpGet("")]
        public IActionResult Index()
        {
          /*     var projects = new List<Project>()

                   {

                       new Project { ProjectId = 1, Name = "Project 1", Description = "First Project" },
                       new Project { ProjectId = 2, Name = "Project 2", Description = "Second Project" }

                       //feel free to add more sample projects here

                   };*/

                       return View(_db.Projects.ToList());
                   
           
        }

        /***** 
         * GET: Projects/Details/5
         * The ":int" constraint ensures id is an integer
         ***/
        [HttpGet("Details/{id:int}")]
        public IActionResult Details(int id)
        {
            var project = _db.Projects.FirstOrDefault(p => p.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }
        /****
        * GET: Projects/Create
         * */
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        /** 
         * POST: Projects/Create
         */
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                _db.Projects.Add(project);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }
        /**
         * GET: Projects/Edit/5
         */
        [HttpGet("Edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            var project = _db.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        /***
         * POST: Projects/Edit/5
         */
        [HttpPost("Edit/{id:int}")]
        
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("ProjectId, Name, Description, StartDate, EndDate, Status")] Project project)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(project);
                    _db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        private bool ProjectExists(int id)
        {
            return _db.Projects.Any(e => e.ProjectId == id);
        }
        /**** 
         * GET: Projects/Delete/5
            */
        [HttpGet("Delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var project = _db.Projects.FirstOrDefault(p => p.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }
        /*** 
         * POST: Projects/DeleteConfirmed/5
         */
        [HttpPost("DeleteConfirmed/{id:int}")]
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int ProjectId)
        {
            var project = _db.Projects.Find(ProjectId);
            if (project != null)
            {
                _db.Projects.Remove(project);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Handle the case where the project might not be found
            return NotFound();
        }

        // Custom route for search functionality
        // Accessible at /Projects/Search/{searchString?}
          [HttpGet("Search/{searchString?}")]
        public async Task<IActionResult> Search(string searchString)
         {
             var projectsQuery = from p in _db.Projects
                                 select p;

             bool searchPerformed = !String.IsNullOrEmpty(searchString);

             if (searchPerformed)
             {
                 projectsQuery = projectsQuery.Where(p => p.Name.Contains(searchString)
                                                || p.Description.Contains(searchString));
             }

             var projects = await projectsQuery.ToListAsync();
             ViewData["SearchPerformed"] = searchPerformed;
             ViewData["SearchString"] = searchString;
             return View("Index", projects); // Reuse the Index view to display results
         }
       
    }
}
