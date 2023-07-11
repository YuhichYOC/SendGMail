using Newtonsoft.Json.Linq;
using System.Net.Mail;

namespace SendGMail {

    internal class Program {

        private static void Main(string[] args) {
            if (args.Length < 2) {
                return;
            }
            string subject = args[0];
            string body = args[1];

            var s = ReadSettings();
            using (var mail = new MailMessage(new MailAddress(s.FromAddr), new MailAddress(s.ToAddr))) {
                mail.Subject = subject;
                mail.Body = body;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.BodyEncoding = System.Text.Encoding.UTF8;

                var client = new SmtpClient();
                client.Host = s.MailServer;
                client.Port = s.MailPort;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new System.Net.NetworkCredential(s.FromAddr, s.GAppPassword);
                client.EnableSsl = true;
                client.Send(mail);
            }
        }

        private static Settings ReadSettings() {
            using (var sr = new StreamReader(@"settings.json", System.Text.Encoding.UTF8)) {
                var j = sr.ReadToEnd();
                JObject node = JObject.Parse(j);
                return new Settings {
                    MailServer = node[@"MailServer"]!.ToString(),
                    MailPort = (int)node[@"MailPort"]!,
                    FromAddr = node[@"FromAddr"]!.ToString(),
                    ToAddr = node[@"ToAddr"]!.ToString(),
                    GAppPassword = node[@"GAppPassword"]!.ToString()
                };
            }
        }

        private class Settings {
            public string MailServer { get; set; } = string.Empty;
            public int MailPort { get; set; } = 0;
            public string ToAddr { get; set; } = string.Empty;
            public string FromAddr { get; set; } = string.Empty;
            public string GAppPassword { get; set; } = string.Empty;
        }
    }
}