using Microsoft.AspNetCore.Mvc;
using PersonalSiteRevamped.Models;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using PersonalSiteRevamped.Models;
using MimeKit;
using MailKit.Net.Smtp;
namespace PersonalSiteRevamped.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
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

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Resume()
        {
            return View();
        }
        public IActionResult PortfolioOverview()
        {
            return View();
        }
        public IActionResult DungeonApp()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult Contact(ContactViewModel cvm)
        {
          
            if (!ModelState.IsValid)
            {
           
                return View(cvm);
            }

            string message = $"You have received a new email from your site's contact form!<br />" +
                $"Sender: {cvm.Name} <br />Email: {cvm.Email}<br />Subject: {cvm.Subject}<br />Message: {cvm.Message}";

 
            var mm = new MimeMessage();

            mm.From.Add(new MailboxAddress("Sender", _config.GetValue<string>("Credentials:Email:User")));

       
            mm.To.Add(new MailboxAddress("Personal", _config.GetValue<string>("Credentials:Email:Recipient")));

            mm.Subject = cvm.Subject;

            mm.Body = new TextPart("HTML") { Text = message };

            mm.Priority = MessagePriority.Urgent;

            mm.ReplyTo.Add(new MailboxAddress("User", cvm.Email));

            using (var client = new SmtpClient())
            {
                client.Connect(_config.GetValue<string>("Credentials:Email:Client"), 8889);

                client.Authenticate(

                    _config.GetValue<string>("Credentials:Email:User"),
                     _config.GetValue<string>("Credentials:Email:Password")
                    );
                try
                {
                    client.Send(mm);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"There was an error processing your request. Please " +
                        $"try again later. <br />ErrorMessage: {ex.StackTrace}";

                    return View(cvm);
                }
            }
            return View("EmailConfirmation", cvm);
        }
        
     
        public IActionResult Links()
        {
            return View();
        }
    }
}