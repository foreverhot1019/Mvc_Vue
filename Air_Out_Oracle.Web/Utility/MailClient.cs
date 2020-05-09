using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirOut.Web.Utility
{
    public class MailClient
    {
        static MailClient()
        {
        }

        #region 使用MS发送SMTP邮件

        static public bool SendSMTPMail(string vFromName, string vFrom, string[] vTo, string vSubject, string vBodyText, string[] vAttachment, bool vHTML, string vEmailLoginUser, string vEmailLoginPwd, string vEmailHost,int vPort)
        {
            #region
            bool blnReturn = false;
            try
            {
                // create  message
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

                // set sender
                if (vFrom == "" || vFrom == null) { return blnReturn; }
                message.From = new System.Net.Mail.MailAddress(vFrom, vFromName);

                // add a recipient
                foreach (string strTo in vTo) { message.To.Add(strTo); }

                // set the subject & body
                message.Subject = vSubject;
                message.IsBodyHtml = vHTML;
                message.Body = vBodyText;

                //add an attachment
                if (null != vAttachment)
                {
                    foreach (string strAttachment in vAttachment)
                    {
                        if (null != strAttachment && "" != strAttachment.Trim())
                        {
                            message.Attachments.Add(new System.Net.Mail.Attachment(strAttachment));
                        }
                    }
                }
                message.BodyEncoding = System.Text.Encoding.GetEncoding("gb2312");

                // send the message
                System.Net.Mail.SmtpClient client;
                if (string.IsNullOrEmpty(vEmailLoginUser))
                {
                    client = new System.Net.Mail.SmtpClient(vEmailHost);
                    client.Port = vPort;
                    client.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                    client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    client.Timeout = 500;
                    client.EnableSsl = false;
                    client.Send(message);
                }
                else
                {

                    client = new System.Net.Mail.SmtpClient(vEmailHost);
                    client.Port = vPort;
                    client.Credentials = new System.Net.NetworkCredential(vEmailLoginUser, vEmailLoginPwd);
                    client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    //client.UseDefaultCredentials = true;
                    client.EnableSsl = true;

                    //client.Timeout = 5000;
                    client.Send(message);

                }

                blnReturn = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("邮件发送出现异常，异常信息为：{0}", ex.Message);
                //WriteLogInfo(string.Format("邮件发送出现异常，异常信息为：{0}", ex.Message), "SendSMTPMail");
            }
            return blnReturn;
            #endregion
        }

        #endregion

        static public bool SendGrNotifications(string[] to, string subject, string content)
        {
           return SendSMTPMail(MailServerSettings.Settings.User,
                        MailServerSettings.Settings.User,
                        to, subject, content, null, true,
                        MailServerSettings.Settings.User,
                        MailServerSettings.Settings.Password,
                        MailServerSettings.Settings.Server,
                        MailServerSettings.Settings.Port);
        }
        static public bool SendKittingNotifications(string[] to, string subject, string content)
        {
            return SendSMTPMail(MailServerSettings.Settings.User,
                         MailServerSettings.Settings.User,
                         to, subject, content, null, true,
                         MailServerSettings.Settings.User,
                         MailServerSettings.Settings.Password,
                         MailServerSettings.Settings.Server,
                         MailServerSettings.Settings.Port);
        }
    }
}