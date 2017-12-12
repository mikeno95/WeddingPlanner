using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WeddingInfo;


namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private WeddingContext _context; 
        public HomeController(WeddingContext context)
        {
            _context = context;
        }
        private Dictionary<string, object> GetLatLong(string address)
        {
            var WeddingInfo = new Dictionary<string, object>();
            WebRequest.GetAddressDataAsync(address, Response=>{
                WeddingInfo = Response;
            }).Wait();
            // var location = WeddingInfo.results[0].geometry.location;
            return WeddingInfo;
        }
        private User GetLoggedUser()
        {
            return _context.Users.SingleOrDefault(user => user.UserId == (int)HttpContext.Session.GetInt32("UserId"));
        }
        private List<Wedding> GetWeddings()
        {
            return _context.Weddings.Include(ww=>ww.Guests).ThenInclude(www=>www.Guest).ToList();
        }
        private List<string> GetStates()
        {
            List<string> states = new List<string>(new string[]{"AK","AL","AR","AS",
                      "AZ",
                      "CA",
                      "CO",
                      "CT",
                      "DC",
                      "DE",
                      "FL",
                      "GA",
                      "GU",
                      "HI",
                      "IA",
                      "ID",
                      "IL",
                      "IN",
                      "KS",
                      "KY",
                      "LA",
                      "MA",
                      "MD",
                      "ME",
                      "MI",
                      "MN",
                      "MO",
                      "MS",
                      "MT",
                      "NC",
                      "ND",
                      "NE",
                      "NH",
                      "NJ",
                      "NM",
                      "NV",
                      "NY",
                      "OH",
                      "OK",
                      "OR",
                      "PA",
                      "PR",
                      "RI",
                      "SC",
                      "SD",
                      "TN",
                      "TX",
                      "UT",
                      "VA",
                      "VI",
                      "VT",
                      "WA",
                      "WI",
                      "WV",
                      "WY"}); 
            return states;
        }
        public IActionResult Index()
        {
            if(HttpContext.Session.GetInt32("UserId") != null)
            {
                ViewBag.LoggedUser = GetLoggedUser();
                ViewBag.WeddingList = GetWeddings();
                ViewBag.RSVPSuccess = TempData["RSVP"];
                ViewBag.DeleteSuccess = TempData["Delete"];
                ViewBag.unRSVPSuccess = TempData["unRSVP"];
                return View("Dashboard");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            User CheckUser = _context.Users.SingleOrDefault(user => user.Email == model.Email);
            if (CheckUser != null)
            {
                ViewBag.UserExists = "Email is already registered";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    User NewUser = new User();
                    NewUser.FirstName = model.FirstName;
                    NewUser.LastName = model.LastName;
                    NewUser.Email = model.Email;
                    NewUser.Password = Hasher.HashPassword(NewUser, model.Password);
                    _context.Users.Add(NewUser);
                    _context.SaveChanges();
                    User loggedUser = _context.Users.SingleOrDefault(user => user.Email == model.Email);
                    HttpContext.Session.SetInt32("UserId", loggedUser.UserId);
                    return RedirectToAction("Index");
                }
            }
            return View("Index");
        }
        [HttpPost]
        [Route("login")]
        public IActionResult Login(string LogEmail, string LogPassword)
        {
            if (LogPassword!=null || LogEmail!=null)
            {
                User CheckUser = _context.Users.SingleOrDefault(user => user.Email == LogEmail);
                if (CheckUser != null) // if user was found
                {
                    var Hasher = new PasswordHasher<User>();
                    if (0 != Hasher.VerifyHashedPassword(CheckUser, CheckUser.Password, LogPassword))
                    {
                        HttpContext.Session.SetInt32("UserId", CheckUser.UserId);
                        // if password matched
                        return RedirectToAction("Index");
                    }

                }
                ViewBag.LoginErr = "Email and/or Password is incorrect";
            }
            return View("Index");
        }
        [HttpGet]
        [Route("wedding")]
        public IActionResult PlanWedding()
        {
            ViewBag.States = GetStates();
            return View();
        }
        [HttpPost]
        [Route("create/wedding")]
        public IActionResult CreateWedding(WeddingViewModel model)
        {
            User loggedUser = GetLoggedUser();
            ViewBag.States = GetStates();
            if(ModelState.IsValid)
            {
                string Address = $"{model.StreetAddress} {model.City}, {model.State} {model.Zipcode}";
                Wedding NewWedding = new Wedding();
                NewWedding.WedderOne = model.WedderOne; 
                NewWedding.WedderTwo = model.WedderTwo;
                NewWedding.UserId = loggedUser.UserId;
                NewWedding.Date = model.Date;
                NewWedding.Address = Address;
                _context.Weddings.Add(NewWedding);
                _context.SaveChanges();
                Wedding GrabWedd = _context.Weddings.SingleOrDefault(ww=>ww.WedderOne==model.WedderOne && ww.WedderTwo==model.WedderTwo);
                
                return RedirectToAction("ShowWedding", new { @id = GrabWedd.WeddingId});
            }
            return View("PlanWedding");
        }
        [HttpGet]
        [Route("Home/ShowWedding/{weddingId}")]
        public IActionResult ShowWedding(int weddingId)
        {
            Wedding wedding = _context.Weddings.Include(ww=>ww.Guests).ThenInclude(rr=>rr.Guest).SingleOrDefault(w=>w.WeddingId==weddingId);
            ViewBag.Wedding = wedding;
            ViewBag.Source = $"//www.google.com/maps/embed/v1/place?q={wedding.Address}&zoom=17&key=AIzaSyDQJuFR9Mp0wblWcNJ39PyB1hijK6Kjpq8";
            return View(); 
        }
        [HttpGet]
        [Route("wedding/delete/{weddingId}")]
        public IActionResult DeleteWedding(int weddingId)
        {
            Wedding DeleteWedding = _context.Weddings.Include(ww=>ww.Guests).SingleOrDefault(ww => ww.WeddingId == weddingId);
            _context.Weddings.Remove(DeleteWedding);
            foreach(RSVP rsvp in DeleteWedding.Guests)
            {
                _context.RSVPs.Remove(rsvp);
            }
            _context.SaveChanges();
            TempData["Delete"] = $"Succesfully deleted {DeleteWedding.WedderOne} and {DeleteWedding.WedderTwo}'s wedding"; 
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("RSVP/{weddingId}")]
        public IActionResult RSVP(int weddingId)
        {
            Wedding wedding = _context.Weddings.SingleOrDefault(ww => ww.WeddingId == weddingId);
            RSVP NewRSVP = new RSVP();
            NewRSVP.UserId = (int)HttpContext.Session.GetInt32("UserId"); 
            NewRSVP.WeddingId = weddingId;
            _context.RSVPs.Add(NewRSVP);
            _context.SaveChanges();
            TempData["RSVP"] = $"Succesfully RSVP to {wedding.WedderOne} and {wedding.WedderTwo}'s Wedding"; 
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("unRSVP/{weddingId}")]
        public IActionResult unRSVP(int weddingId)
        {
            RSVP removeRSVP = _context.RSVPs.SingleOrDefault(rsvp=>(rsvp.WeddingId==weddingId && rsvp.UserId==(int)HttpContext.Session.GetInt32("UserId")));
            Wedding wedding = _context.Weddings.SingleOrDefault(ww=>ww.WeddingId==weddingId);
            _context.Remove(removeRSVP);
            _context.SaveChanges();
            TempData["unRSVP"] = $"You un-RSVP to {wedding.WedderOne} and {wedding.WedderTwo}'s Wedding";
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
