using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BookClub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace BookClub.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.NotLoggedIn = true;
            return View();
        }

        [HttpPost("/user/register")]
        public IActionResult Register(User newUser)
        {
            ViewBag.NotLoggedIn = true;
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(e => e.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use ");
                    return View("Index");
                }
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                newUser.Password = hasher.HashPassword(newUser, newUser.Password);
                _context.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                return RedirectToAction("Index");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("/user/login")]
        public IActionResult Login(LogUser loginUser)
        {
            if (ModelState.IsValid)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                User userInDb = _context.Users.FirstOrDefault(e => e.Email == loginUser.LogEmail);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (userInDb == null)
                {
                    ModelState.AddModelError("LogEmail", "Invalid Email/Password");
                    return View("Index");
                }
                PasswordHasher<LogUser> hasher = new PasswordHasher<LogUser>();
                var result = hasher.VerifyHashedPassword(loginUser, userInDb.Password, loginUser.LogPassword);
                if (result == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("LogEmail", "Invalid Email/Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }
[HttpGet("/Dashboard")]
public IActionResult Dashboard()
{
    int? userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null)
    {
        return RedirectToAction("Index");
    }
    ViewBag.NotLoggedIn = false;

    // Use null-conditional operator to safely access properties of loggedInUser
    User? loggedInUser = _context.Users.Include(u => u.BooksWritten).FirstOrDefault(u => u.UserId == userId);
    ViewBag.LoggedInUser = loggedInUser;

        List<Book> allBooks = _context.Books
            .Include(b => b.Creator)
            .ToList();

        var userFavorites = _context.Associationss
        .Where(a => a.UserId == userId)
        .Select(a => a.Book)
        .ToList();

    var combinedList = allBooks.Concat(userFavorites).Distinct().OrderByDescending(b => b.CreatedAt).ToList();

    ViewBag.CombinedList = combinedList;
    ViewBag.UserFavorites = userFavorites;

    return View(allBooks);
}


        

        [HttpGet("/book/add")]
        public IActionResult NewBook()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost("/book/add")]
        public IActionResult CreateBook(Book book)
        {
            if (ModelState.IsValid)
            {
                int? userId = HttpContext.Session.GetInt32("UserId");
                if (userId != null)
                {
                    book.CreatorId = userId.Value;
                    _context.Books.Add(book);
                    _context.SaveChanges();
                    return RedirectToAction("OneBook", new { id = book.BookId });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return View("NewBook", book);
            }
        }

        [HttpGet("/books/{id}")]
        public IActionResult OneBook(int id)
        {
            var book = _context.Books
                .Include(b => b.Creator)
                .Include(b => b.UserHowLiked).ToList()
                .FirstOrDefault(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

       [HttpPost("/books/delete/{id}")]
public IActionResult DeleteBook(int id)
{
    int? userId = HttpContext.Session.GetInt32("UserId");

    if (userId == null)
    {
        return RedirectToAction("Index", "Home");
    }

    var book = _context.Books.FirstOrDefault(p => p.BookId == id);

    if (book == null)
    {
        return NotFound();
    }

    // Vérifier si l'utilisateur actuel est le créateur du livre
    if (book.CreatorId != userId)
    {
        // Définir le message d'erreur dans TempData
        TempData["ErrorMessage"] = "You are not allowed to delete this book";

        // Rediriger l'utilisateur vers le tableau de bord
        return RedirectToAction("Dashboard", "Home");
    }

    _context.Books.Remove(book);
    _context.SaveChanges();
    return RedirectToAction("Dashboard", "Home");
}


        [HttpPost("/books/update/{id}")]
        public IActionResult UpdateBookDescription(int id, string newDescription)
        {
            var book = _context.Books.FirstOrDefault(p => p.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            book.Description = newDescription;
            _context.SaveChanges();

            return RedirectToAction("OneBook", new { id = book.BookId });
        }

        [HttpGet("/books/view/{id}")]
        public IActionResult ViewBook(int id)
        {
            var book = _context.Books
                .Include(b => b.Creator)
                .Include(b => b.UserHowLiked).ToList()
                .FirstOrDefault(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpGet("/books/details/{id}")]
        public IActionResult BookDetails(int id)
        {
            var book = _context.Books
                .Include(b => b.Creator)
                .Include(b => b.UserHowLiked).ToList()
                .FirstOrDefault(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }


        [HttpGet("/user/favorites")]
        public IActionResult UserFavorites()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var userFavorites = _context.Associationss
                .Include(a => a.Book)
                .Where(a => a.UserId == userId)
                .Select(a => a.Book)
                .ToList();

            return View(userFavorites);
        }

        [HttpPost("/books/addtofavorites/{id}")]
        public IActionResult AddToFavorites(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var book = _context.Books.Include(b => b.UserHowLiked).FirstOrDefault(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            var existingAssociation = book.UserHowLiked.FirstOrDefault(a => a.UserId == userId);
            if (existingAssociation != null)
            {
                return RedirectToAction("OneBook", new { id = book.BookId });
            }

            var newAssociation = new Association
            {
                UserId = userId.Value,
                BookId = book.BookId
            };

            book.UserHowLiked.Add(newAssociation);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [HttpPost("/books/removefromfavorites/{id}")]
        public IActionResult RemoveFromFavorites(int id)
        {
            
            var association = _context.Associationss.FirstOrDefault(a => a.BookId == id && a.UserId == HttpContext.Session.GetInt32("UserId"));
            if (association == null)
            {
                return RedirectToAction("OneBook", new { id = id });
            }
            _context.Associationss.Remove(association);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }
    }
}
