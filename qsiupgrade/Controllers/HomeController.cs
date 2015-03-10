using System.Net.Mail;
using System.Web.Mvc;

namespace qsiupgrade.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmailService _emailService;

        public HomeController()
        {
            _emailService = new EmailService();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(IndexModel model)
        {
            _emailService.SendEmail(model.Email, model.Name, model.Company, model.Phone);
            return View();
        }

    }

    public class EmailService
    {
        private readonly string _from;
        private readonly string _subject;
        private readonly string _body;
        private readonly SmtpClient _smtp;

        public EmailService()
        {
            var server = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("server");
            _from = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("from");
            _subject = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("subject");
            _body = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("body");
            _smtp = new SmtpClient(server);
        }

        public void SendEmail(string to, string name, string company, string phone)
        {
            var body = _body.Replace("{{Name}}", name);
            _smtp.Send(_from, to, _subject, body);
        }
    }

    public class IndexModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Company { get; set; }
    }
}