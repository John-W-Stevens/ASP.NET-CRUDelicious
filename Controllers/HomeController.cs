using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRUDelicious.Models;

namespace CRUDelicious.Controllers
{
    public class HomeController : Controller
    {

        // Routes:
        //   ""                -> GET:  Index.cshtml
        //   "{dishId}"        -> GET:  DisplayDish.cshtml
        //   "new"             -> GET:  New.cshtml
        //   "new"             -> POST: Creates a new Dish object in Db
        //   "edit/{dishId}"   -> GET:  EditDish.cshtml
        //   "edit/{dishId}"   -> POST: Updates a Dish object in Db
        //   "delete/{dishId}" -> POST: Deletes a Dish object from the Db


        private DishesContext dbContext;

        public HomeController(DishesContext context)
        {
            dbContext = context;
        }

        // Base route
        [HttpGet("")]
        public IActionResult Index()
        {
            // all dishes
            List<Dish> AllDishes = dbContext.Dishes.ToList();

            // all dishes ordered by created date (newest first)
            List<Dish> MostRecentDishes = dbContext.Dishes
                .OrderByDescending(d => d.CreatedAt)
                .ToList();

            ViewBag.AllDishes = MostRecentDishes;
            return View();
        }

        // Display create dish page
        [HttpGet("new")]
        public IActionResult New()
        {
            return View();
        }

        // Create dish route 
        [HttpPost("new")]
        public IActionResult CreateDish(Dish dish) {
            if (ModelState.IsValid)
            {
                dbContext.Add(dish);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("New");
        }

        // Display OneSingleDish page
        [HttpGet("{dishId}")]
        public IActionResult DisplayDish(int dishId)
        {
            Dish RetrievedDish = dbContext.Dishes.FirstOrDefault(d => d.DishId == dishId);
            ViewBag.OneSingleDish = RetrievedDish;
            ViewBag.EditUrl = $"edit/{ViewBag.OneSingleDish.DishId}";
            ViewBag.DeleteUrl = $"delete/{dishId}";
            return View("DisplayDish");
        }

        // Display Edit OneSingleDish page
        [HttpGet("edit/{dishId}")]
        public IActionResult EditDish(int dishId)
        {
            Dish RetrievedDish = dbContext.Dishes.FirstOrDefault(d => d.DishId == dishId);
            ViewBag.OneSingleDish = RetrievedDish;
            ViewBag.Url = $"/edit/{dishId}";
            // Pass the Retrieved Dish object so asp autofills form fields
            return View("EditDish", RetrievedDish);
        }

        // Edit dish route
        [HttpPost("edit/{dishId}")]
        public IActionResult Update(int dishId, Dish newDish)
        {
            Dish RetrievedDish = dbContext.Dishes.FirstOrDefault(d => d.DishId == dishId);
            if (ModelState.IsValid)
            {
                RetrievedDish.Name = newDish.Name;
                RetrievedDish.Chef = newDish.Chef;
                RetrievedDish.Calories = newDish.Calories;
                RetrievedDish.Tastiness = newDish.Tastiness;
                RetrievedDish.Description = newDish.Description;
                RetrievedDish.UpdatedAt = DateTime.Now;
                dbContext.SaveChanges();
                // redirect to base route
                return RedirectToAction("Index");
            }
            else
            {
                // this will display errors
                return EditDish(dishId);
            }
        }

        // Delete dish route
        [HttpGet("delete/{dishId}")]
        public IActionResult DeleteUser(int dishId)
        {
            Dish RetrievedDish = dbContext.Dishes.FirstOrDefault(d => d.DishId == dishId);
            dbContext.Dishes.Remove(RetrievedDish);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
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
}
