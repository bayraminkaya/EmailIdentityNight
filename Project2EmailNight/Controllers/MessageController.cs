using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2EmailNight.Context;
using Project2EmailNight.Entities;
using System.Linq;

namespace Project2EmailNight.Controllers
{
    public class MessageController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;
        private const int PageSize = 13;

        public MessageController(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ===== Sidebar sayılarını ViewBag'e atan yardımcı metot =====
        private void SetSidebarCounts(string userEmail)
        {
            ViewBag.InboxCount = _context.Messages.Count(x => x.ReceiverEmail == userEmail && !x.IsDeleted);
            ViewBag.StarredCount = _context.Messages.Count(x => (x.ReceiverEmail == userEmail || x.SenderEMail == userEmail) && x.IsStarred && !x.IsDeleted);
            ViewBag.SendBoxCount = _context.Messages.Count(x => x.SenderEMail == userEmail && !x.IsDeleted);
            ViewBag.DraftsCount = 0;
            ViewBag.SpamCount = 0;
            ViewBag.TrashCount = _context.Messages.Count(x => (x.ReceiverEmail == userEmail || x.SenderEMail == userEmail) && x.IsDeleted);

            // Kategori sayıları (sadece gelen kutusundaki)
            ViewBag.IsCount = _context.Messages.Count(x => x.ReceiverEmail == userEmail && !x.IsDeleted && x.Category == "İş");
            ViewBag.AileCount = _context.Messages.Count(x => x.ReceiverEmail == userEmail && !x.IsDeleted && x.Category == "Aile");
            ViewBag.EgitimCount = _context.Messages.Count(x => x.ReceiverEmail == userEmail && !x.IsDeleted && x.Category == "Eğitim");
            ViewBag.SosyalCount = _context.Messages.Count(x => x.ReceiverEmail == userEmail && !x.IsDeleted && x.Category == "Sosyal");
        }

        // ===== Pagination ViewBag'lerini atan yardımcı metot =====
        private List<Message> ApplyPagination(IQueryable<Message> query, int page)
        {
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return query.Skip((page - 1) * PageSize).Take(PageSize).ToList();
        }

        // ===== GELEN KUTUSU =====
        public async Task<IActionResult> Inbox(int page = 1)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.CurrentUser = user;
            ViewBag.ActiveFolder = "Inbox";
            SetSidebarCounts(user.Email);

            var query = _context.Messages
                .Where(x => x.ReceiverEmail == user.Email && !x.IsDeleted)
                .OrderByDescending(x => x.SendDate);

            var messageList = ApplyPagination(query, page);
            return View(messageList);
        }

        // ===== GÖNDERİLENLER =====
        public async Task<IActionResult> Sendbox(int page = 1)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.CurrentUser = user;
            ViewBag.ActiveFolder = "SendBox";
            SetSidebarCounts(user.Email);

            var query = _context.Messages
                .Where(x => x.SenderEMail == user.Email && !x.IsDeleted)
                .OrderByDescending(x => x.SendDate);

            var messageList = ApplyPagination(query, page);
            return View("Inbox", messageList);
        }

        // ===== YILDIZLILAR =====
        public async Task<IActionResult> Starred(int page = 1)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.CurrentUser = user;
            ViewBag.ActiveFolder = "Starred";
            SetSidebarCounts(user.Email);

            var query = _context.Messages
                .Where(x => (x.ReceiverEmail == user.Email || x.SenderEMail == user.Email) && x.IsStarred && !x.IsDeleted)
                .OrderByDescending(x => x.SendDate);

            var messageList = ApplyPagination(query, page);
            return View("Inbox", messageList);
        }

        // ===== KATEGORİ =====
        public async Task<IActionResult> Category(string id, int page = 1)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.CurrentUser = user;
            ViewBag.ActiveFolder = id; // "İş", "Aile", "Eğitim", "Sosyal"
            SetSidebarCounts(user.Email);

            var query = _context.Messages
                .Where(x => x.ReceiverEmail == user.Email && !x.IsDeleted && x.Category == id)
                .OrderByDescending(x => x.SendDate);

            var messageList = ApplyPagination(query, page);
            return View("Inbox", messageList);
        }

        // ===== ÇÖP KUTUSU =====
        public async Task<IActionResult> Trash(int page = 1)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.CurrentUser = user;
            ViewBag.ActiveFolder = "Trash";
            SetSidebarCounts(user.Email);

            var query = _context.Messages
                .Where(x => (x.ReceiverEmail == user.Email || x.SenderEMail == user.Email) && x.IsDeleted)
                .OrderByDescending(x => x.SendDate);

            var messageList = ApplyPagination(query, page);
            return View("Inbox", messageList);
        }

        // ===== ARAMA =====
        public async Task<IActionResult> Search(string query, int page = 1)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.CurrentUser = user;
            ViewBag.ActiveFolder = "Search";
            SetSidebarCounts(user.Email);

            var q = _context.Messages
                .Where(x => x.ReceiverEmail == user.Email && !x.IsDeleted &&
                    (x.Subject.Contains(query) || x.MessageDetail.Contains(query) || x.SenderEMail.Contains(query)))
                .OrderByDescending(x => x.SendDate);

            var messageList = ApplyPagination(q, page);
            return View("Inbox", messageList);
        }

        // ===== YILDIZ TOGGLE =====
        public IActionResult ToggleStar(int id)
        {
            var message = _context.Messages.Find(id);
            if (message != null)
            {
                message.IsStarred = !message.IsStarred;
                _context.SaveChanges();
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // ===== ÇÖPE TAŞI =====
        public IActionResult MoveToTrash(int id)
        {
            var message = _context.Messages.Find(id);
            if (message != null)
            {
                message.IsDeleted = true;
                _context.SaveChanges();
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // ===== MESAJ DETAY =====
        public async Task<IActionResult> MessageDetail(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.CurrentUser = user;

            var message = _context.Messages.Find(id);
            if (message != null && !message.IsStatus)
            {
                message.IsStatus = true;
                _context.SaveChanges();
            }
            return View(message);
        }

        // ===== MESAJ SİL (kalıcı) =====
        public IActionResult MessageDelete(int id)
        {
            var message = _context.Messages.Find(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
                _context.SaveChanges();
            }
            return RedirectToAction("Inbox");
        }

        // ===== YENİ MESAJ =====
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
            message.IsStarred = false;
            message.IsDeleted = false;
            _context.Messages.Add(message);
            _context.SaveChanges();
            return RedirectToAction("Inbox");
        }
    }
}
