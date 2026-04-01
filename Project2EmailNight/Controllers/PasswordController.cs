using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using Project2EmailNight.Dtos;
using Project2EmailNight.Entities;

namespace Project2EmailNight.Controllers
{
    public class PasswordController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public PasswordController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {

                ModelState.AddModelError("", "Eğer bu email kayıtlıysa sıfırlama linki gönderildi.");
                return View(dto);
            }


            var token = await _userManager.GeneratePasswordResetTokenAsync(user);


            var resetLink = Url.Action("ResetPassword", "Password",
                new { email = user.Email, token = token },
                Request.Scheme);

            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Identity Admin", "pawpuffzone@gmail.com"));
            mimeMessage.To.Add(new MailboxAddress("User", user.Email));
            mimeMessage.Subject = "Şifre Sıfırlama Talebi";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = $"Merhaba {user.Name},\n\nŞifrenizi sıfırlamak için aşağıdaki linke tıklayınız:\n\n{resetLink}\n\nBu linki siz talep etmediyseniz dikkate almayınız.";
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Connect("smtp.gmail.com", 587, false);
            smtpClient.Authenticate("pawpuffzone@gmail.com", "ipkb lbdc gzyx mrqr");
            smtpClient.Send(mimeMessage);
            smtpClient.Disconnect(true);

            ViewBag.Message = "Şifre sıfırlama linki email adresinize gönderildi.";
            return View(dto);
        }


        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            var dto = new ResetPasswordDto
            {
                Email = email,
                Token = token
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
            {
                ModelState.AddModelError("", "Şifreler eşleşmiyor.");
                return View(dto);
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return RedirectToAction("UserLogin", "Login");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            if (result.Succeeded)
            {
                return RedirectToAction("UserLogin", "Login");
            }

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }

            return View(dto);
        }
    }
}