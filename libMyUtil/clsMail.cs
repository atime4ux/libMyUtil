using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace libMyUtil
{
    /// <summary>
    /// Gmail을 통한 메일 발송
    /// </summary>
    public class clsMail
    {
        System.Threading.Thread t1;

        private string mailAccountID;
        private string mailAccountPass;

        private string from;
        private string to;
        private string title;
        private string msg;

        public clsMail(string AccountID, string AccountPass)
        {
            this.mailAccountID = AccountID;
            this.mailAccountPass = AccountPass;
        }

        public void SendMail(string From, string To, string Title, string Msg)
        {
            this.from = From;
            this.to = To;
            this.title = Title;
            this.msg = Msg;

            t1 = new System.Threading.Thread(new System.Threading.ThreadStart(send));
            t1.Start();
        }

        private void send()
        {
            libCommon.clsUtil objUtil = new libCommon.clsUtil();

            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            string retMsg = string.Empty;

            try
            {
                MailAddress fromAddress = new MailAddress(this.from);
                MailAddress toAddress = new MailAddress(this.to);
                
                message.From = fromAddress;
                message.To.Add(toAddress);
                message.Subject = this.title;
                message.IsBodyHtml = false;
                message.Body = this.msg;

                smtpClient.Host = "smtp.gmail.com";
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(this.mailAccountID, this.mailAccountPass);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                libMyUtil.clsFile.writeLog(ex.ToString());
            }
            
            message.Dispose();
        }
    }
}