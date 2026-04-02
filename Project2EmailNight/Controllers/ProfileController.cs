using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2EmailNight.Context;
using Project2EmailNight.Dtos;
using Project2EmailNight.Entities;

namespace Project2EmailNight.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailContext _context;

        public ProfileController(UserManager<AppUser> userManager, EmailContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var dto = new UserEditDto
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                About = user.About
            };

            // İstatistikler
            ViewBag.TotalReceived = _context.Messages.Count(x => x.ReceiverEmail == user.Email && !x.IsDeleted);
            ViewBag.TotalSent = _context.Messages.Count(x => x.SenderEMail == user.Email && !x.IsDeleted);
            ViewBag.TotalMessages = ViewBag.TotalReceived + ViewBag.TotalSent;
            ViewBag.TotalStarred = _context.Messages.Count(x => (x.ReceiverEmail == user.Email || x.SenderEMail == user.Email) && x.IsStarred && !x.IsDeleted);
            ViewBag.TotalDeleted = _context.Messages.Count(x => (x.ReceiverEmail == user.Email || x.SenderEMail == user.Email) && x.IsDeleted);
            ViewBag.TotalUnread = _context.Messages.Count(x => x.ReceiverEmail == user.Email && !x.IsStatus && !x.IsDeleted);
            ViewBag.IsCount = _context.Messages.Count(x => x.ReceiverEmail == user.Email && x.Category == "İş" && !x.IsDeleted);
            ViewBag.AileCount = _context.Messages.Count(x => x.ReceiverEmail == user.Email && x.Category == "Aile" && !x.IsDeleted);
            ViewBag.EgitimCount = _context.Messages.Count(x => x.ReceiverEmail == user.Email && x.Category == "Eğitim" && !x.IsDeleted);
            ViewBag.SosyalCount = _context.Messages.Count(x => x.ReceiverEmail == user.Email && x.Category == "Sosyal" && !x.IsDeleted);

            ViewBag.CurrentUser = user;

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserEditDto dto)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            user.Name = dto.Name;
            user.Surname = dto.Surname;
            user.Email = dto.Email;
            user.About = dto.About;

            // Şifre güncelleme
            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, dto.Password);
            }

            // Fotoğraf yükleme
            if (dto.Image != null && dto.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                Directory.CreateDirectory(uploadsFolder);
                var extension = Path.GetExtension(dto.Image.FileName);
                var imageName = Guid.NewGuid() + extension;
                var savePath = Path.Combine(uploadsFolder, imageName);
                using var stream = new FileStream(savePath, FileMode.Create);
                await dto.Image.CopyToAsync(stream);
                user.ImageUrl = imageName;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(dto);
        }
    }
}