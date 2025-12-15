using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Controllers
{
    public class AccountController : Controller
    {

        private readonly SignInManager<AppUser> _signInManager;

        private readonly UserManager<AppUser> _userManager;


        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("Admin","Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }
            return View(model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Form verilerini alır ve kullanıcıyı kaydeder
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.Email, Email = model.Email };
                // Kullanıcıyı veritabanına kaydetme ve şifreyi hash'leme
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Başarılı kayıt sonrası Login sayfasına yönlendirme yapabilirsiniz
                    return RedirectToAction("Login");
                }

                // Hata varsa (örneğin email zaten kullanılıyorsa)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Model geçerli değilse veya kayıtta hata oluştuysa aynı View'i geri döndür.
            return View(model);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
