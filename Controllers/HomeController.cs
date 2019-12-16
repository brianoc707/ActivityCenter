using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ActivityCenter.Models;

namespace ActivityCenter.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
     
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        public IActionResult Index()
        {
            int? session = HttpContext.Session.GetInt32("loggedinUser");
            
            if (session != null)
            {
                
                return RedirectToAction("Home");
            }
            return View();
        }

        [HttpGet("Home")]
        public IActionResult Home()
        {
            int? session = HttpContext.Session.GetInt32("loggedinUser");
             
            if (session == null)
            {
                
                return RedirectToAction("Index");
            }
            ViewBag.AllAct = dbContext.Actividads.Include(i => i.Creator).Include(i => i.Guests).OrderBy(i => i.Date);
            ViewBag.CurrentUser = dbContext.Users.FirstOrDefault(i => i.UserId == (int)session);
            ViewBag.DateTime = DateTime.Now;
            return View();
            
        }

          [HttpPost("register")]
        public IActionResult Register(User user)
        {
             if(ModelState.IsValid)
            {
                
                // If a User exists with provided email
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                // Manually add a ModelState error to the Email field, with provided
                // error message
                 ModelState.AddModelError("Email", "Email already in use!");
            
                 // You may consider returning to the View at this point
                 return View("Index");
                }
        
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Add(user);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("loggedinUser", user.UserId);
                return Redirect("/home");
                
                
            }
            
            else 
            {
                return View("Index");
            }
        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                

                
                // If no user exists with provided email
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
                
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    ModelState.AddModelError("Password", "Incorrect Password");
                    return View("Index");
                }
            
                HttpContext.Session.SetInt32("loggedinUser", userInDb.UserId);
                return Redirect("/home");
                
            }
            return View("Index");
        }

        [HttpGet("/new/{UserId}")]
        public IActionResult New(int UserId)
        {
           
            
            return View();
        }

        [HttpPost("/plan")]
        public IActionResult Plan(Actividad actividad)
        {
            int? session = HttpContext.Session.GetInt32("loggedinUser");
             if(ModelState.IsValid)
            {
                if(actividad.Date < DateTime.Now)
                {
                    ModelState.AddModelError("Date", "Date must be in the Future!");
               
                    return View("New");
                
                }                
                    actividad.UserId = (int)session;
                    dbContext.Add(actividad);
                    dbContext.SaveChanges();
                   
                return Redirect($"actividad/{actividad.ActividadId}");
            }
            else
                {
                    return View("New");
                }
            
        }
       [HttpGet("/actividad/{ActividadId}")]
        public IActionResult Actividades(int ActividadId)
        {
            int? session = HttpContext.Session.GetInt32("loggedinUser");
            
            if (session == null)
            {
                HttpContext.Session.Clear();
                return View("Index");
            }
            ViewBag.AllGuests = dbContext.GuestLists.Include(i => i.Guest).FirstOrDefault(i => i.ActividadId == ActividadId);
            ViewBag.PlannedAct = dbContext.Actividads.Include(i => i.Creator).Include(i => i.Guests).FirstOrDefault(i => i.ActividadId == ActividadId);
            
            ViewBag.CurrentAct = dbContext.Actividads.FirstOrDefault(i => i.ActividadId == ActividadId);
            ViewBag.AllAct = dbContext.Actividads.Include(i => i.Creator).Include(i => i.Guests).FirstOrDefault(i => i.ActividadId == ActividadId);
            ViewBag.CurrentUser = dbContext.Users.FirstOrDefault(i => i.UserId == (int)session);
            return View("Actividades");
        }

        [HttpGet("delete/{ActividadId}")]
        public IActionResult Delete(int ActividadId)
        {   
           
           Actividad actToDelete = dbContext.Actividads.SingleOrDefault(i => i.ActividadId == ActividadId);
            
            dbContext.Remove(actToDelete);
            dbContext.SaveChanges();
            return Redirect("/home");
        }

        [HttpGet("join/{ActividadId}")]
        public IActionResult Join(int ActividadId)
        {   
            int? session = HttpContext.Session.GetInt32("loggedinUser");
            GuestList guest = new GuestList();
            guest.UserId = (int)session;
            guest.ActividadId = ActividadId;

            dbContext.Add(guest);
            dbContext.SaveChanges();


            return Redirect("/home");
        }

        [HttpGet("leave/{ActividadId}")]
        public IActionResult Leave(int ActividadId)
        {   
            int? session = HttpContext.Session.GetInt32("loggedinUser");
            GuestList guestToRemove = dbContext.GuestLists.Where(i => i.ActividadId == ActividadId).FirstOrDefault(i => i.UserId == (int)session);
            
           
            dbContext.Remove(guestToRemove);
            dbContext.SaveChanges();


            return Redirect("/home");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        
    }
}
