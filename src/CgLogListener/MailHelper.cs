using IniParser;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace CgLogListener
{
    public static class MailHelper
    {
        private const string MailConfigFileName = "mail.ini";

        public static bool IsConfigured()
        {
            if (!File.Exists(MailConfigFileName))
            {
                return false;
            }

            try
            {
                var ini = new FileIniDataParser().ReadFile(MailConfigFileName);
                return !string.IsNullOrEmpty(ini["smtp"]["host"]) &&
                       !string.IsNullOrEmpty(ini["smtp"]["from"]) &&
                       !string.IsNullOrEmpty(ini["smtp"]["to"]);
            }
            catch
            {
                return false;
            }
        }

        public static void SendMail(string subject, string body)
        {
            if (!File.Exists(MailConfigFileName))
            {
                return;
            }

            try
            {
                var appName = Settings.GetInstance().AppName;
                var fullSubject = $"[{appName}] {subject}";

                var ini = new FileIniDataParser().ReadFile(MailConfigFileName);
                var smtpSection = ini["smtp"];

                var host = smtpSection["host"];
                var portStr = smtpSection["port"];
                var username = smtpSection["username"];
                var password = smtpSection["password"];
                var from = smtpSection["from"];
                var to = smtpSection["to"];
                var enableSslStr = smtpSection["enableSsl"];

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
                {
                    return;
                }

                int port = 587;
                if (!string.IsNullOrEmpty(portStr))
                {
                    int.TryParse(portStr, out port);
                }

                bool enableSsl = true;
                if (!string.IsNullOrEmpty(enableSslStr))
                {
                    enableSsl = enableSslStr == "1" || enableSslStr.ToLower() == "true";
                }

                using (var client = new SmtpClient(host, port))
                {
                    client.EnableSsl = enableSsl;

                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {
                        client.Credentials = new NetworkCredential(username, password);
                    }

                    var message = new MailMessage(from, to, fullSubject, body);
                    client.Send(message);
                }
            }
            catch
            {
                // 發送失敗時靜默處理，避免影響主程式
            }
        }

        public static void GenerateDefaultConfig()
        {
            if (File.Exists(MailConfigFileName))
            {
                return;
            }

            var content = @"[smtp]
; SMTP 伺服器設定
host=smtp.gmail.com
port=587
enableSsl=1

; 認證資訊 (如果 SMTP 需要認證)
username=
password=

; 寄件者與收件者
from=your-email@gmail.com
to=recipient@example.com
";
            File.WriteAllText(MailConfigFileName, content);
        }
    }
}
