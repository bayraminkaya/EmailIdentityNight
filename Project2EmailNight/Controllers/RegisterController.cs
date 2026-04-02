using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using Project2EmailNight.Dtos;
using Project2EmailNight.Entities;

namespace Project2EmailNight.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public RegisterController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserRegisterDto userRegisterDto)
        {
            var random = new Random();
            string confirmCode = random.Next(100000, 999999).ToString();

            AppUser appUser = new AppUser()
            {
                Name = userRegisterDto.Name,
                Surname = userRegisterDto.Surname,
                UserName = userRegisterDto.Username,
                Email = userRegisterDto.Email,
                ConfirmCode = confirmCode
            };

            var result = await _userManager.CreateAsync(appUser, userRegisterDto.Password);

            if (result.Succeeded)
            {
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];
                var appPassword = _configuration["EmailSettings:AppPassword"];

                MimeMessage mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(senderName, senderEmail));
                mimeMessage.To.Add(new MailboxAddress("User", userRegisterDto.Email));
                mimeMessage.Subject = "E-Posta Doğrulama Kodunuz";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = $"Merhaba {userRegisterDto.Name},\n\nDoğrulama kodunuz: {confirmCode}\n\nBu kodu doğrulama sayfasına giriniz.";
                mimeMessage.Body = bodyBuilder.ToMessageBody();

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Connect("smtp.gmail.com", 587, false);
                smtpClient.Authenticate(senderEmail, appPassword);
                smtpClient.Send(mimeMessage);
                smtpClient.Disconnect(true);

                return RedirectToAction("VerifyEmail", new { userId = appUser.Id });
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }

            return View(userRegisterDto);
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return RedirectToAction("CreateUser");

            var dto = new VerifyEmailDto
            {
                UserId = userId,
                Email = user.Email
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailDto verifyEmailDto)
        {
            var user = await _userManager.FindByIdAsync(verifyEmailDto.UserId);
            if (user == null)
                return RedirectToAction("CreateUser");

            if (user.ConfirmCode == verifyEmailDto.ConfirmCode)
            {
                user.EmailConfirmed = true;
                user.ConfirmCode = null;
                await _userManager.UpdateAsync(user);
                return RedirectToAction("UserLogin", "Login");
            }
            else
            {
                ModelState.AddModelError("", "Girilen kod hatalı. Lütfen tekrar deneyiniz.");
            }

            return View(verifyEmailDto);
        }
    }
}