using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZGym.Core.Entities;
using ZGym.Data.Data;

namespace ZGym.Web.Controllers
{
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _dbContext = context;
            _userManager = userManager;
        }

        // GET: GymClasses
        // [Authorize(Roles = "Member")]
        public async Task<IActionResult> Index()
        {
            return View(await _dbContext.GymClasses.ToListAsync());
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _dbContext.GymClasses
                .Include(g => g.AttendingMembers)
                .ThenInclude(a => a.ApplicationUser)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // GET: GymClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Add(gymClass);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _dbContext.GymClasses.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(gymClass);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
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
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewEdit(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }

            var gymClass = _dbContext.GymClasses.Find(id);

            if (await TryUpdateModelAsync(gymClass, "", g => g.Name, g => g.Duration))
            {
                try
                {
                    // _dbContext.Update(gymClass);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
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

            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _dbContext.GymClasses
                .FirstOrDefaultAsync(g => g.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await _dbContext.GymClasses.FindAsync(id);
            _dbContext.GymClasses.Remove(gymClass);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }

            var loggedInUser = _userManager.GetUserId(User);
            var user = await _dbContext.Users
                                .FirstOrDefaultAsync(u => u.Id == loggedInUser);
            if (user is null)
            {
                return BadRequest();
            }
            var attending = await _dbContext.UserGymClasses.FindAsync(loggedInUser, id);
            
            if (attending is null)
            {
                var booking = new ApplicationUserGymClass
                {
                    GymClassId = (int)id,
                    ApplicationUserId = loggedInUser
                };

                _dbContext.UserGymClasses.Add(booking);
            }
            else
            {
                _dbContext.UserGymClasses.Remove(attending);
            }
            
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> BookingToggle2(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }

            var gymClass = await _dbContext.GymClasses
                                .Include(g => g.AttendingMembers)
                                .ThenInclude(a => a.ApplicationUser)
                                .FirstOrDefaultAsync(g => g.Id == id);
            if (gymClass is null)
            {
                return NotFound();
            }

            var loggedInUser = _userManager.GetUserId(User);
            var user = await _dbContext.Users
                                .FirstOrDefaultAsync(u => u.Id == loggedInUser);
            if (user is null)
            {
                return BadRequest();
            }
            var attendingMembers = gymClass?.AttendingMembers;
            var attendance = attendingMembers.FirstOrDefault(a => a.ApplicationUserId == loggedInUser);
            if (attendance == null)
            {
                attendingMembers.Add(new ApplicationUserGymClass()
                {
                    GymClassId = gymClass.Id,
                    GymClass = gymClass,
                    ApplicationUserId = loggedInUser,
                    ApplicationUser = user
                });
            }
            else
            {
                attendingMembers.Remove(attendance);
            }
            _dbContext.Update(gymClass);
            await _dbContext.SaveChangesAsync();
            return View(nameof(Details), gymClass);
        }

        private bool GymClassExists(int id)
        {
            return _dbContext.GymClasses.Any(e => e.Id == id);
        }
    }
}
