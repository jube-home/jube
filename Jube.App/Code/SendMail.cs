/* Copyright (C) 2022-present Jube Holdings Limited.
 *
 * This file is part of Jube™ software.
 *
 * Jube™ is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License 
 * as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * Jube™ is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty  
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

 * You should have received a copy of the GNU Affero General Public License along with Jube™. If not, 
 * see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Net;
using System.Net.Mail;
using System.Web;
using log4net;

namespace Jube.App.Code
{
    public class SendMail
    {
        private readonly DynamicEnvironment.DynamicEnvironment _dynamicEnvironment;
        private readonly ILog _log;

        public SendMail(DynamicEnvironment.DynamicEnvironment dynamicEnvironment, ILog log)
        {
            _dynamicEnvironment = dynamicEnvironment;
            _log = log;
        }

        public void Send(string toEmail, string subject, string body)
        {
            try
            {
                var smtpServer = new SmtpClient();
                var eMail = new MailMessage();
                smtpServer.UseDefaultCredentials = false;
                smtpServer.Credentials = new NetworkCredential(_dynamicEnvironment.AppSettings("SMTPUser"),
                    _dynamicEnvironment.AppSettings("SMTPPassword"));
                smtpServer.Port = int.Parse(_dynamicEnvironment.AppSettings("SMTPPort"));
                smtpServer.EnableSsl = true;
                smtpServer.Host = _dynamicEnvironment.AppSettings("SMTPHost");

                eMail.From = new MailAddress(_dynamicEnvironment.AppSettings("SMTPFrom"));
                eMail.To.Add(toEmail);
                eMail.Subject = subject;
                eMail.IsBodyHtml = true;
                eMail.Body = HttpUtility.UrlDecode(body);
                smtpServer.Send(eMail);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
    }
}