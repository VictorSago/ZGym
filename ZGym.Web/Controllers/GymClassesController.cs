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
using ZGym.Core.Repositories;
using ZGym.Core.ViewModels;
using ZGym.Data.Data;
using ZGym.Data.Repositories;
using ZGym.Web.Extensions;

namespace ZGym.Web.Controllers
{
    // [Authorize(Roles = "Member")]
    // [Authorize(Policy = "PolicyName1")]
    public class GymClassesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork uow;

        public GymClassesController(UserManager<ApplicationUser> userManager, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
            uow = unitOfWork;
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
                    GymClasses = uow.GymClassRepository.GetWithBookingsAsync().Result
                                            .Select(g => new GymClassesViewModel 
                                            {
                                                Id = g.Id,
                                                Name = g.Name,
                                                Duration = g.Duration,
                                                Description = g.Description
                                                // Attending = g.AttendingMembers.Any(a => a.ApplicationUserId == userId)
                                            })
                };
                return View(model1);
            }

            var userId = _userManager.GetUserId(User);
            var m = _mapper.Map<IndexViewModel>(await uow.GymClassRepository.GetAllAsync(), opt => opt.Items.Add("Id", userId));
            /* var model = new IndexViewModel
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
            }; */
            // var model = new IndexViewModel();
            // return View(model);
            return View(m);
        }

        // GET: GymClasses/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await uow.GymClassRepository.GetWithUsersAsync(id);
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
                uow.GymClassRepository.Add(gymClass);
                await uow.CompeteAsync();
                
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
            if (id is null)
            {
                return NotFound();
            }

            var gymClass = await uow.GymClassRepository.FindAsync(id);
            if (gymClass is null)
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
                    uow.GymClassRepository.Update(gymClass);
                    await uow.CompeteAsync();
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

            var gymClass = await uow.GymClassRepository.FindAsync(id);

            if (await TryUpdateModelAsync(gymClass, "", g => g.Name, g => g.Duration))
            {
                try
                {
                    // _dbContext.Update(gymClass);
                    await uow.CompeteAsync();
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

            var gymClass = await uow.GymClassRepository.GetAsync(id);
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
            var gymClass = await uow.GymClassRepository.FindAsync(id);
            uow.GymClassRepository.Remove(gymClass);
            await uow.CompeteAsync();
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
            var attending = await uow.UserGymClassRepository.GetAttending(id, loggedInUser);

            if (attending is null)
            {
                var booking = new ApplicationUserGymClass
                {
                    GymClassId = (int)id,
                    ApplicationUserId = loggedInUser
                };

                uow.UserGymClassRepository.Add(booking);
            }
            else
            {
                uow.UserGymClassRepository.Remove(attending);
            }

            await uow.CompeteAsync();
            return RedirectToAction(nameof(Index));
        }

/* 
        [Authorize]
        public async Task<IActionResult> BookingToggle2(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }

            var gymClass = uow.GymClassRepository.GetAsync(id);
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
 */
/* 
        public async Task<IActionResult> BookingsOld()
        {
            var userId = _userManager.GetUserId(User);
            
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
 */

        public async Task<IActionResult> Bookings()
        {
            var userId = _userManager.GetUserId(User);
            
            var model = _mapper.Map<IndexViewModel>(
                await uow.UserGymClassRepository.GetBookingsAsync(userId),
                opt => opt.Items.Add("Id", userId)
            );
            
            // return View(nameof(Index), await model.ToListAsync());
            return View(nameof(Index), model);
        }

        private bool GymClassExists(int id)
        {
            return uow.GymClassRepository.Any(id);
        }

        
    }
}
