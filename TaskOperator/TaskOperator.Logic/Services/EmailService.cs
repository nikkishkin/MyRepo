using System;
using System.Net.Mail;
using System.Text;
using TaskOperator.Entities;
using TaskOperator.Entities.Enums;
using TaskOperator.Logic.Helpers;
using TaskOperator.Logic.Interfaces;

namespace TaskOperator.Logic.Services
{
    public class EmailService
    {
        private const string TaskNameChangedMessageTemplate = "Task {0}: name was changed to {1}";
        private const string TaskContentChangedMessageTemplate = "Task {0}: content was changed to {1}";
        private const string TaskStateChangedMessageTemplate = "Task {0}: state was changed to {1}";

        private const string NewTaskMessageTemplate = "You've got new task with name {0}";

        private const string TaskCanceledMessageTemplate = "Task with name {0} was canceled for you";

        private const string NewTaskEmailSubject = "New task";
        private const string TaskChangedEmailSubject = "Task was changed";
        private const string TaskCanceledEmailSubject = "Task was canceled";

        private readonly IUserBlo _userBlo;

        public EmailService(IUserBlo userBlo)
        {
            _userBlo = userBlo;
        }

        public void CheckEmailSending(Task oldTask, int newPercentage)
        {

        }

        public void CheckEmailSending(Task oldTask, Task newTask)
        {
            if (oldTask.State == (byte)TaskState.Open)
            {
                // Task was open
                if (newTask.State != (byte)TaskState.Open)
                {
                    // Now task has user
                    SendEmail(NewTaskEmailSubject, String.Format(NewTaskMessageTemplate, newTask.Name.Quote()),
                        _userBlo.GetUser(newTask.WorkerId.Value).Email);
                }
                return;
            }

            if (oldTask.WorkerId == newTask.WorkerId)
            {
                // Task' user remained the same
                StringBuilder message = new StringBuilder();

                if (oldTask.Name.Trim() != newTask.Name.Trim())
                {
                    message.Append(String.Format(TaskNameChangedMessageTemplate, oldTask.Name.Quote(),
                        newTask.Name.Quote()));
                }

                if (oldTask.Content.Trim() != newTask.Content.Trim())
                {
                    message.Append(String.Format(TaskContentChangedMessageTemplate, oldTask.Name.Quote(),
                        newTask.Content.Quote()));
                }

                if (oldTask.State != newTask.State)
                {
                    message.Append(String.Format(TaskStateChangedMessageTemplate, oldTask.Name.Quote(),
                        ((TaskState) newTask.State).ToString().Quote()));
                }

                if (message.Length != 0)
                {
                    SendEmail(TaskChangedEmailSubject, message.ToString(), 
                        _userBlo.GetUser(newTask.WorkerId.Value).Email);
                }
                return;
            }

            // Task' user was changed

            // Notify old worker
            SendEmail(TaskCanceledEmailSubject, String.Format(TaskCanceledMessageTemplate, oldTask.Name.Quote()), 
                _userBlo.GetUser(oldTask.WorkerId.Value).Email);

            // Notify new worker
            SendEmail(NewTaskEmailSubject, String.Format(NewTaskMessageTemplate, newTask.Name.Quote()),
                _userBlo.GetUser(newTask.WorkerId.Value).Email);
        }

        private void SendEmail(string subject, string body, string recipientAddress)
        {
            MailMessage mail = new MailMessage()
            {
                Subject = subject,
                Body = body
            };
            mail.To.Add(recipientAddress);

            using (SmtpClient client = new SmtpClient())
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
            }
        }
    }
}
