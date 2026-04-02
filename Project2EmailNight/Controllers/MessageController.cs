using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2EmailNight.Context;
using Project2EmailNight.Entities;

namespace Project2EmailNight.Controllers
{
    public class MessageController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MessageController(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateMessage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(Message message)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            message.SenderEMail = user.Email;
            message.SendDate = DateTime.Now;
            message.IsStatus = false;
            _context.Messages.Add(message);
            _context.SaveChanges();
            return RedirectToAction("Inbox");
        }

        public async Task<IActionResult> Inbox()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messageList = _context.Messages
                .Where(x => x.ReceiverEmail == user.Email)
                .OrderByDescending(x => x.SendDate)
                .ToList();

            // Üst panelde kullanıcı bilgisi için
            ViewBag.CurrentUser = user;

            return View(messageList);
        }

        public async Task<IActionResult> Sendbox()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messageList = _context.Messages
                .Where(x => x.SenderEMail == user.Email)
                .OrderByDescending(x => x.SendDate)
                .ToList();
            ViewBag.CurrentUser = user;
            return View(messageList);
        }
    }
}