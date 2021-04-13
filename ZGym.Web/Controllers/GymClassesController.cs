using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZGym.Core.Entities;
using ZGym.Core.ViewModels;
using ZGym.Data.Data;
using ZGym.Web.Extensions;

namespace ZGym.Web.Controllers
{
    // [Authorize(Roles = "Member")]
    // [Authorize(Policy = "PolicyName1")]
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _dbContext = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: GymClasses
        // [Authorize(Roles = "Member")]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                var model1 = new IndexViewModel
                {
                    GymClasses = await _dbContext.GymClasses.Include(g => g.AttendingMembers)
                                            .Select(g => new GymClassesViewModel 
                                            {
                                                Id = g.Id,
                                                Name = g.Name,
                                                Duration = g.Duration,
                                                Description = g.Description
                                                // Attending = g.AttendingMembers.Any(a => a.ApplicationUserId == userId)
                                            })
                                            .ToListAsync()
                };
                return View(model1);
            }

            var userId = _userManager.GetUserId(User);
            // var m = _mapper.Map<IEnumerable<GymClassesViewModel>>(_dbContext.GymClasses, opt => opt.Items.Add("Id", userId));
            var model = new IndexViewModel
            {
                GymClasses = await _dbContext.GymClasses.Include(g => g.AttendingMembers)
                                        .Select(g => new GymClassesViewModel 
                                        {
                                            Id = g.Id,
                                            Name = g.Name,
                                            StartTime = g.StartTime,
                                            Duration = g.Duration,
                                            Description = g.Description,
                                            Attending = g.AttendingMembers.Any(a => a.ApplicationUserId == userId)
                                        })
                                        .ToListAsync()
            };
            return View(model);
        }

        // GET: GymClasses/Details/5
        [Authorize]
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
        [Authorize]
        public IActionResult Create()
        {
            return Request.IsAjax() ? PartialView("CreatePartial") : View();
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

                if (Request.IsAjax())
                {
                    // return PartialView("GymClassesPartial", await _dbContext.GymClasses.ToListAsync());
                    return PartialView("GymClassPartial", gymClass);
                }
                
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // ToDo
        //[IsAjax]
        public ActionResult Fetch()
        {
            return PartialView("CreatePartial");
        }

        // GET: GymClasses/Edit/5
        [Authorize(Roles = "Admin")]
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

            return View(nameof(Edit), gymClass);
        }

        // GET: GymClasses/Delete/5
        [Authorize(Roles = "Admin")]
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

        [Authorize]
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

        [Authorize]
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

        public async Task<IActionResult> Bookings()
        {
            var userId = _userManager.GetUserId(User);

            // var model1 = _dbContext.UserGymClasses
            //                     .IgnoreQueryFilters()
            //                     .Where(a => a.ApplicationUserId == userId)
            //                     .Select(a => a.GymClass);
            
            var model = new IndexViewModel
            {
                GymClasses = await _dbContext.UserGymClasses
                                .IgnoreQueryFilters()
                                .Where(a => a.ApplicationUserId == userId)
                                .Select(a => new GymClassesViewModel
                                {
                                    Id = a.GymClass.Id,
                                    Name = a.GymClass.Name,
                                    StartTime = a.GymClass.StartTime,
                                    Duration = a.GymClass.Duration,
                                    Description = a.GymClass.Description,
                                    Attending = true
                                })
                                .ToListAsync()
            };
            
            // return View(nameof(Index), await model.ToListAsync());
            return View(nameof(Index), model);
        }

        private bool GymClassExists(int id)
        {
            return _dbContext.GymClasses.Any(e => e.Id == id);
        }
    }
}
