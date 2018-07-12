using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginReg.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoginReg.Controllers
{
    public class HomeController : Controller
    {

        private YourContext _context;//need the next 6 lines for YourContext to work with this controller

        public HomeController(YourContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register(User NewUser)
        {
            if(ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                NewUser.password = Hasher.HashPassword(NewUser, NewUser.password);
                //Save your user object to the database
                _context.Add(NewUser);
                _context.SaveChanges();

                 System.Console.WriteLine("********************Register success********************");
                 return RedirectToAction("Registration");//ad the "Registration" cshtml later

            }
            else
            {
                System.Console.WriteLine("********************Register failed********************");               
                return View("Index",NewUser);

            }
        }

        public IActionResult Login(User ExistingUser)//Login arguments from the platform>> "string email, string PasswordToCheck"
        {
            // Attempt to retrieve a user from your database based on the Email submitted
            //var user = userFactory.GetUserByEmail(Email);//for dapper only
            var user = _context.users.SingleOrDefault(u => u.email == ExistingUser.email);
            if(user != null &&  ExistingUser.password!= null)//platform orginall gave me this in place of password "PasswordToCheck"
            {
                var Hasher = new PasswordHasher<User>();
                // Pass the user object, the hashed password, and the PasswordToCheck
                if(0 != Hasher.VerifyHashedPassword(user, user.password,ExistingUser.password ))//PasswordToCheck
                {
                    //Handle success
                    System.Console.WriteLine("********************Login success********************");
                    return RedirectToAction("Registration");//will add this route later "Registration"

                }

                System.Console.WriteLine("********************Login failed for bad pw********************");
                return RedirectToAction("Index");
            }
            //Handle failure
            else
            {
                System.Console.WriteLine("********************Login failed for bad email and pw********************");
                return RedirectToAction("Index");
               
            }
        }       

        public IActionResult Registration()//this is from Tom's project, will need to verify the route works with a page for the route
        {
            List<User> AllUsers = _context.users.ToList();
            ViewBag.info = AllUsers;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
