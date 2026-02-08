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
}


/* 
Följande routes skall hanteras av MVC:

Route                           Beskrivning
                            
/home/index	                    Visar en lista på kommande event
/home/events/{id}	            Visar detaljer för ett specifikt event
(POST) /home/register/{id}	    Tar emot anmälan för ett event
*/
