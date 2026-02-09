using System.Diagnostics;
using EventHomepage.Models;
using EventCore.Entities;
using EventInfrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventHomepage.Controllers;

public class HomeController : Controller
{
    private readonly EventDbContext _context;

    public HomeController(EventDbContext context)
    {
        _context = context;
    }
    
    // gör om till asynkronsk för att läsa in från DB
    public async Task<IActionResult> Index()
    {
        // hämta events 
        var allEvents = await _context.Events.ToListAsync();
        return View(allEvents);
        
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    // GET /home/create
    public IActionResult Create()
    {
        return View();
    }

    // POST /home/create
    [HttpPost]
    public async Task<IActionResult> Create(CreateEventViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.EndDateTime <= model.StartDateTime)
        {
            ModelState.AddModelError("EndDateTime", "End time must be after start time.");
            return View(model);

        }

        var evnt = new Event(
            model.Name,
            model.Description,
            model.StartDateTime,
            model.EndDateTime,
            model.Location,
            model.MaxParticipants
        );

        _context.Events.Add(evnt);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");

    }


    // GET /home/events/{id}
    [HttpGet("events/{id}")] 
    public async Task<IActionResult> EventDetails(int id)
    {
        var evnt = await _context.Events.Include(e => e.Registrations)
        .FirstOrDefaultAsync(e => e.Id == id);

        if (evnt == null)
        {
            return NotFound();
        }

        var viewModel = new EventDetailsViewModel
        {
            Id = evnt.Id,
            Name = evnt.Name,
            Description = evnt.Description,
            StartDateTime = evnt.StartDateTime,
            EndDateTime = evnt.EndDateTime,
            Location = evnt.Location,
            MaxParticipants = evnt.MaxParticipants,
            ParticipantCount = evnt.Registrations?.Count ?? 0,
            IsFull = evnt.IsFull(),
            CreatedAt = evnt.CreatedAt
        };

        return View(viewModel);
    }


    // POST /home/register/{id}
    [HttpPost] 
    public async Task<IActionResult> Register(int id, RegisterViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
        {
            ModelState.AddModelError("Name", "Name is required for registration");
        }

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            ModelState.AddModelError("Email", "Email is required for registration");
        }

        if (!ModelState.IsValid)
        {
            var evnt = await _context.Events.Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == id);

            if (evnt == null)
                return NotFound();

            var viewModel = new EventDetailsViewModel
            {
                Id = evnt!.Id,
                Name = evnt.Name,
                Description = evnt.Description,
                StartDateTime = evnt.StartDateTime,
                EndDateTime = evnt.EndDateTime,
                Location = evnt.Location,
                MaxParticipants = evnt.MaxParticipants,
                ParticipantCount = evnt.Registrations?.Count ?? 0,
                IsFull = evnt.IsFull(),
                CreatedAt = evnt.CreatedAt
            };

            return View("EventDetails", viewModel);

        }

        var eventToRegister = await _context.Events.Include(e => e.Registrations)
        .FirstOrDefaultAsync(e => e.Id == id);

        if (eventToRegister == null)
        {
            return NotFound();
        }

        if (eventToRegister.IsFull())
        {
            ModelState.AddModelError("", "This event is full!");
            return RedirectToAction("EventDetails", new { id });
        }

        try
        {
            eventToRegister.RegisterParticipant(model.Name, model.Email);

            await _context.SaveChangesAsync();

            TempData["SuccesMessage"] = "Registration successfull! See you at the event!";
            return RedirectToAction("EventDetails", new { id });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);

            var viewModel = new EventDetailsViewModel
            {
                Id = eventToRegister.Id,
                Name = eventToRegister.Name,
                Description = eventToRegister.Description,
                StartDateTime = eventToRegister.StartDateTime,
                EndDateTime = eventToRegister.EndDateTime,
                Location = eventToRegister.Location,
                MaxParticipants = eventToRegister.MaxParticipants,
                ParticipantCount = eventToRegister.Registrations?.Count ?? 0,
                IsFull = eventToRegister.IsFull(),
                CreatedAt = eventToRegister.CreatedAt
            };

            return View("EventDetails", viewModel);
        }
        
    }
        
}
