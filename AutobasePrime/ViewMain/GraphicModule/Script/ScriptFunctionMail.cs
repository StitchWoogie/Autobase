using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoLibLocal;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Net.Mime;

namespace GraphicModule
{
    public class ScriptFunctionMail
    {
        public static bool SendMail(string server, string username, string password, bool ssl_flag, string mail_to, string mail_fr, string title, string text, bool text_format, string filename, int port, out string err_msg)
        {
            err_msg = "";

            if (port < 0 || port > 65535)
            {
                err_msg = "@MailSend, The used port is invalid. Please modify the 'ssl_port' parameter.";
                return false;
            }

            try
            {
                MailAddress address_fr = new MailAddress(mail_fr);
                MailAddress address_to = new MailAddress(mail_to);

                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage(address_fr, address_to);

                mail.Subject = title;
                mail.Body = text;

                mail.IsBodyHtml = text_format;

                mail.SubjectEncoding = Encoding.UTF8;
                mail.BodyEncoding = Encoding.UTF8;
                if (filename.Length > 0)
                {
                    if (File.Exists(filename))
                    {
                        Attachment data = new Attachment(filename, MediaTypeNames.Application.Octet);
                        // Add time stamp information for the file.
                        ContentDisposition disposition = data.ContentDisposition;
                        disposition.CreationDate = System.IO.File.GetCreationTime(filename);
                        disposition.ModificationDate = System.IO.File.GetLastWriteTime(filename);
                        disposition.ReadDate = System.IO.File.GetLastAccessTime(filename);
                        // Add the file attachment to this e-mail message.
                        mail.Attachments.Add(data);
                    }
                    else
                    {
                        err_msg = filename + " file not found";
                        return false;
                    }
                }

                SmtpClient client;

                if (server == "127.0.0.1" || server == "localhost")
                {
                    client = new SmtpClient(server);
                }
                else
                {
                    client = new SmtpClient(server);
                    // client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(username, password);
                    client.EnableSsl = ssl_flag;
                    client.Port = port; //port 추가 20240509 PSU  
                }
                client.Send(mail);

                return true;
            }
            catch (Exception exception)
            {
                err_msg = exception.Message;

                return false;
            }
        }

        static int Run_Common(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string server = (string)args[0];
                string username = (string)args[1];
                string password = (string)args[2];
                int ssl_port = (int)args[3];
                string mail_to = (string)args[4];
                string mail_fr = (string)args[5];
                string title = (string)args[6];
                string text = (string)args[7];
                int text_format = (int)args[8];
                string filename = (string)args[9];
                string err_msg;

                //port 할당 추가 20240509 PSU
                bool ssl_flag = false;
                int port = 25;

                if (ssl_port == 0)
                {
                    ssl_flag = false;
                    port = 25;
                }
                else if (ssl_port == 1)
                {
                    ssl_flag = true;
                    port = 25;
                }
                else
                {
                    ssl_flag = true;
                    port = ssl_port;
                }
                /////

                if (!SendMail(server, username, password, ssl_flag, mail_to, mail_fr, title, text, (text_format == 1), filename, port, out err_msg))
                {
                    val = 0;
                    scriptClass.SetLastError(err_msg);
                }
                else
                {
                    val = 1;
                }

                return 1;

            }

            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "Mail";

            prepare.AddMethod(prename, "MailSend", "int", new ScriptExternalRun.DeleMethod(Run_Common), "in:string:mail_server", "in:string:username", "in:string:password", "in:int:ssl", "in:string:mail_to", "in:string:mail_from", "in:string:title", "in:string:text", "in:int:text_format", "in:string:filename");
        }
    }
}
