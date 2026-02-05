using System.Diagnostics;
using EventHomepage.Models;
using EventCore.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventHomepage.Controllers;

public class HomeController : Controller
{
    
    public IActionResult Index()
    {
        return View();
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
}


/* 
Följande routes skall hanteras av MVC:

Route                           Beskrivning
                            
/home/index	                    Visar en lista på kommande event
/home/events/{id}	            Visar detaljer för ett specifikt event
(POST) /home/register/{id}	    Tar emot anmälan för ett event
*/
