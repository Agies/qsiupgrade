using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using SendGrid;

namespace qsiupgrade.Controllers
{
    public class HomeController : AsyncController
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
        public async Task<ActionResult> Index(IndexModel model)
        {
            try
            {
                await _emailService.SendServerEmail(model.Email, model.Name, model.Company, model.Phone);
            }
            catch (Exception e)
            {
                
            }
            try
            {
                await _emailService.SendClientEmail(model.Email, model.Name, model.Company, model.Phone);
            }
            catch (Exception e)
            {
                
            }
            return View();
        }

    }

    public class EmailService
    {
        private readonly string _from;
        private readonly string _subject;
        private readonly string _body;
        private readonly string _serverBody;
        private readonly SmtpClient _smtp;

        public EmailService()
        {
            var server = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("server");
            _from = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("from");
            _subject = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("subject");
            _body = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("body");
            _serverBody = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("serverBody");
            _smtp = new SmtpClient(server)
                    {
                        Credentials = new NetworkCredential()
                    };
        }

        public Task SendClientEmail(string to, string name, string company, string phone)
        {
            var body = _body.Replace("{{Name}}", name);

            var gridMessage = new SendGridMessage();
            gridMessage.AddTo(to);
            gridMessage.From = new MailAddress(_from);
            gridMessage.Subject = _subject;
            gridMessage.Text = body;
            //gridMessage.Html = body;

            var credentials = new NetworkCredential(
                Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("Keys.Email.Account"),
                Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("Keys.Email.Password")
                );

            var transportWeb = new Web(credentials);
            return transportWeb.DeliverAsync(gridMessage);
        }
        public Task SendServerEmail(string to, string name, string company, string phone)
        {
            var body = _serverBody
                .Replace("{{Name}}", name)
                .Replace("{{Company}}", company)
                .Replace("{{Email}}", to)
                .Replace("{{Phone}}", phone);
            return _smtp.SendMailAsync(to, _from, "Information Requested", body);
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