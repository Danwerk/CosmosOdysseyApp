#pragma warning disable 1591

using System.Security.Claims;
using App.BLL.DTO;
using App.BLL.Services;
using App.Contracts.DAL;
using App.Domain.Identity;
using Base.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.ViewModels.Reservations;
using Reservation = App.Domain.Reservation;

namespace WebApp.Areas.Admin.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAppUOW _uow;
        private readonly ReservationService _reservationService;
        private readonly RouteService _routeService;


        public ReservationsController(UserManager<AppUser> userManager, IAppUOW uow,
            ReservationService reservationService, RouteService routeService)
        {
            _reservationService = reservationService;
            _routeService = routeService;
            _userManager = userManager;
            _uow = uow;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("admin"))
            {
                var vm = await _uow.ReservationRepository.AllAsync();
                return View(vm);
            }
            else
            {
                var vm = await _uow.ReservationRepository.AllAsync(User.GetUserId());
                return View(vm);
            }
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details([FromRoute] Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _uow.ReservationRepository.FindAsync(id.Value);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }


        [HttpGet]
        public async Task<IActionResult> Create(Guid? providerReportId)
        {
            if (providerReportId == null)
            {
                return RedirectToAction("Index", "Routes");
            }

            var provider = await _routeService.getProviderById(providerReportId);

            // ViewData["AppUserId"] = new SelectList(_userManager.Users, nameof(AppUser.Id), nameof(AppUser.Email));

            var reservationModel = new CreateReservationModel()
            {
                ProviderReport = new ProviderReport()
                {
                    Id = provider!.Id,
                    Price = provider.Price,
                    TravelTime = provider.FlightEnd - provider.FlightStart,
                    FromLocation = provider.Leg!.RouteInfo!.FromLocation!.Name,
                    ToLocation = provider.Leg!.RouteInfo!.ToLocation!.Name,
                }
            };
            return View(reservationModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateReservationModel reservationModel)
        {
            var providerReportId = reservationModel.ProviderReport.Id;
            var provider = await _routeService.getProviderById(providerReportId);

            // ViewData["AppUserId"] = new SelectList(_userManager.Users, nameof(AppUser.Id), nameof(AppUser.Email));
            reservationModel = new CreateReservationModel()
            {
                FirstName = reservationModel.FirstName,
                LastName = reservationModel.LastName,
                ProviderReport = new ProviderReport()
                {
                    Price = provider!.Price,
                    TravelTime = provider.FlightEnd - provider.FlightStart,
                    FromLocation = provider.Leg!.RouteInfo!.FromLocation!.Name,
                    ToLocation = provider.Leg!.RouteInfo!.ToLocation!.Name,
                    FlightStart = provider.FlightStart,
                    FlightEnd = provider.FlightEnd
                }
            };

            var reservationId = await _reservationService.CreateReservation(reservationModel, User.GetUserId());
            if (reservationId != null)
            {
                TempData["SuccessMessage"] = "Reservation Confirmed";
            }
            return RedirectToAction("Details", new { id = reservationId });
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _uow.ReservationRepository.FindAsync(id.Value);
            if (reservation == null)
            {
                return NotFound();
            }

            ViewData["AppUserId"] = new SelectList(_userManager.Users, nameof(AppUser.Id), nameof(AppUser.Email),
                reservation.AppUserId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,
            Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _uow.ReservationRepository.Update(reservation);
                    await _uow.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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

            ViewData["AppUserId"] = new SelectList(_userManager.Users, nameof(AppUser.Id), nameof(AppUser.Email),
                reservation.AppUserId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _uow.ReservationRepository.FindAsync(id.Value);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var reservation = await _uow.ReservationRepository.FindAsync(id);
            if (reservation != null)
            {
                _uow.ReservationRepository.Remove(reservation);
            }

            await _uow.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(Guid id)
        {
            return (_uow.ReservationRepository.AllAsync().Result?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}