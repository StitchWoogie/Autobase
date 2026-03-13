using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace WebTools
{
    public class MailTool
    {
        static bool SendMailPublic(string mail_to, string name_to, string title, string body, out string err_msg, bool bBodyHtml, bool usemailserver)
        {
            try
            {
                MailAddress address_fr = new MailAddress("master@autobase.biz", "(주)오토베이스", Encoding.UTF8);
                MailAddress address_to = new MailAddress(mail_to);

                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage(address_fr, address_to);

                mail.Subject = title;
                mail.Body = body;

                mail.IsBodyHtml = bBodyHtml;

                mail.SubjectEncoding = Encoding.UTF8;
                mail.BodyEncoding = Encoding.UTF8;

                SmtpClient client;

                /*
                // SMTP 를 발송하는 곳의 서버에 설치하여 하려 했으나 여기서 바로 보내나 메일 서버를 통해서 보내나 
                // 이 서버에서의 부하는 어차피 동일할 것이므로 단순함을 위해서 메일서버를 사용해서 보내기로 했다.
                // 2012-04-28 */

                if (usemailserver)
                {
                    client = new SmtpClient("mail.autobase.biz");
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("send1@autobase.biz", "Techisn1cu");
                }
                else
                {

                    // 2012-5-8 autobase.biz 로 무제한 메일이 들어와서 SMTP 서비스를 중지시키고 서버에 직접 SMTP서버를 설치하고 바로 보내는 것으로 변경하였음
                    // 서버관리자에서 기능 추가에서 SMTP 서버를 설치 (기본 사용) 참고:http://cafe.naver.com/newprograming/146  
                    client = new SmtpClient("localhost");
                }

                client.Send(mail);

                err_msg = "";

                return true;
            }
            catch (Exception ex)
            {
                err_msg = String.Format("{0}", ex.Message);

                return false;
            }
        }

        public static bool SendMailText(string mail_to, string name_to, string title, string body, out string err_msg)
        {
            return SendMailPublic(mail_to, name_to, title, body, out err_msg, false, false);
        }

        public static bool SendMailHtml(string mail_to, string name_to, string title, string body, out string err_msg)
        {
            return SendMailPublic(mail_to, name_to, title, body, out err_msg, true, false);
        }

        public static bool SendMailText(string mail_to, string name_to, string title, string body, out string err_msg, bool usemailserver)
        {
            return SendMailPublic(mail_to, name_to, title, body, out err_msg, false, true);
        }

        public static bool SendMailHtml(string mail_to, string name_to, string title, string body, out string err_msg, bool usemailserver)
        {
            return SendMailPublic(mail_to, name_to, title, body, out err_msg, true, true);
        }


    }
}
