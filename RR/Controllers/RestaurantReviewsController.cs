using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RR.Data;
using RR.Models;

namespace RR.Controllers
{
    public static class ApplicationRoles
    {
        public const string Admin = "Admin";
    }
    public class RestaurantReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RestaurantReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RestaurantReviews
        [HttpGet]
        public async Task<IActionResult> Index(string searchField, string searchString)
        {
            if (_context.RestaurantReview == null)
            {
                return Problem("Entity set Restaurant Review  is null.");
            }
            var reviews = from m in _context.RestaurantReview
                         select m;

            if (!String.IsNullOrEmpty(searchField) && !String.IsNullOrEmpty(searchString))
            {
                switch (searchField.ToLower())
                {
                    case "restaurantname":
                        reviews = reviews.Where(r => r.RestaurantName.Contains(searchString));
                        break;
                    case "foodname":
                        reviews = reviews.Where(r => r.FoodName.Contains(searchString));
                        break;
                    case "price":
                        reviews = reviews.Where(r => r.Price.Equals(decimal.Parse(searchString)));
                        break;
                    case "reviewscore":
                        reviews = reviews.Where(r => r.ReviewScore.Equals(decimal.Parse(searchString)));
                        break;
                    case "publishingdate":
                        reviews = reviews.Where(r => r.PublishingDate.ToString().Contains(searchString));
                        break;
                    default:
                        return BadRequest("Invalid search field");
                }
            }
            var model = await reviews.ToListAsync();
            return View(model);
        }

        // GET: RestaurantReviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurantReview = await _context.RestaurantReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurantReview == null)
            {
                return NotFound();
            }

            return View(restaurantReview);
        }

        [HttpGet]
        // GET: RestaurantReviews/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetImage(int id)
        {
            var restaurant = await _context.RestaurantReview.FindAsync(id);
            if (restaurant?.FoodImage != null)
            {
                return File(restaurant.FoodImage, "image/jpeg"); // Change "image/jpeg" based on your image type
            }
            return NotFound();
        }

        // POST: RestaurantReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RestaurantName,FoodName,Price,ReviewScore,PublishingDate,FoodPhoto")] CreateReview restaurantReview)
        {
            if (restaurantReview.FoodPhoto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await restaurantReview.FoodPhoto.CopyToAsync(memoryStream);

                    RestaurantReview restaurantObj = new RestaurantReview()
                    {
                        Id = restaurantReview.Id,
                        RestaurantName = restaurantReview.RestaurantName,
                        FoodName = restaurantReview.FoodName,
                        Price = restaurantReview.Price,
                        ReviewScore = restaurantReview.ReviewScore,
                        PublishingDate = restaurantReview.PublishingDate,
                        FoodImage = memoryStream.ToArray()
                    };
                    _context.Add(restaurantObj);
                    await _context.SaveChangesAsync();

                    ViewBag.success = "record added";
                    return RedirectToAction(nameof(Index));

                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        // GET: RestaurantReviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurantReview = await _context.RestaurantReview.FindAsync(id);
            if (restaurantReview == null)
            {
                return NotFound();
            }
            var restaurantViewModel = ConvertToViewModel(restaurantReview);
            return View(restaurantViewModel);
        }

        private CreateReview ConvertToViewModel(RestaurantReview restaurant)
        {
            // Convert byte array to IFormFile
            IFormFile? foodPhotoFile = null;
            if (restaurant.FoodImage != null)
            {
                using (var stream = new MemoryStream(restaurant.FoodImage))
                {
                    foodPhotoFile = new FormFile(stream, 0, restaurant.FoodImage.Length, "FoodImage", "FoodImage.jpg");
                }
            }
            return new CreateReview
            {
                Id = restaurant.Id,
                RestaurantName = restaurant.RestaurantName,
                FoodName = restaurant.FoodName,
                Price = restaurant.Price,
                ReviewScore = restaurant.ReviewScore,
                PublishingDate = restaurant.PublishingDate,
                FoodPhoto = foodPhotoFile
            };
        }

        // POST: RestaurantReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateReview restaurantReview)
        {
            if (id != restaurantReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing restaurant from the database
                    var existingRestaurant = await _context.RestaurantReview.FindAsync(id);
                    // Check if the existing restaurant is found
                    if (existingRestaurant != null)
                    {
                        // Update only the modified fields
                        _context.Entry(existingRestaurant).CurrentValues.SetValues(restaurantReview);
                        // Check if the user uploaded a new image
                        if (restaurantReview.FoodPhoto != null)
                        {
                            // User uploaded a new image, update the image
                            using (var memoryStream = new MemoryStream())
                            {
                                await restaurantReview.FoodPhoto.CopyToAsync(memoryStream);
                                existingRestaurant.FoodImage = memoryStream.ToArray();
                            }
                        }

                        // Save changes to the database
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantReviewExists(restaurantReview.Id))
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
            return View(restaurantReview);
        }

        // GET: RestaurantReviews/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurantReview = await _context.RestaurantReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurantReview == null)
            {
                return NotFound();
            }

            return View(restaurantReview);
        }

        // POST: RestaurantReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restaurantReview = await _context.RestaurantReview.FindAsync(id);
            if (restaurantReview != null)
            {
                _context.RestaurantReview.Remove(restaurantReview);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantReviewExists(int id)
        {
            return _context.RestaurantReview.Any(e => e.Id == id);
        }
    }
}
