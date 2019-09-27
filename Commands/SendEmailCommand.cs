////////////////////////////////////////////////////////////////////////////////
// Copyright 2019 Jeff Boenig
//
// This file is part of Headway.Dynamo.
//
// Headway.Dynamo is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// Headway.Dynamo is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// Headway.Dynamo. If not, see http://www.gnu.org/licenses/.
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Net.Mail;

namespace Headway.Dynamo.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SendEmailCommand : Command
    {
        /// <summary>
        /// 
        /// </summary>
        public string From
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string To
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Subject
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Body
        {
            get;
            set;
        }

        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider</param>
        /// <param name="context">User defined context data</param>
        /// <returns>
        /// Returns a <see cref="CommandResult"/> object that describes
        /// the result.
        /// </returns>
        public override CommandResult Execute(IServiceProvider serviceProvider, object context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var mailMsg = new MailMessage(this.From, this.To);
            mailMsg.Body = this.Body;

            var smtpClient = new SmtpClient();

            throw new NotImplementedException();
        }
    }
}
