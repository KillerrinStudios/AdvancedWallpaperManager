using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;

namespace Killerrin_Studios_Toolkit
{
    public partial class KillerrinApplicationData
    {
        public Package AppPackage { get { return Package.Current; } }

        public string PackageID { get; set; }
        public string PublisherID { get; set; }

        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }

        public string PublisherName { get; set; }
        public string Website { get; set; } = "http://www.killerrin.com";
        public string Twitter { get; set; } = "https://www.twitter.com/killerrin";
        public string Facebook { get; set; } = "https://www.facebook.com/KillerrinStudios";

        public string FeedbackUrl { get; set; } = "support@killerrin.com";
        public string SupportUrl { get; set; } = "support@killerrin.com";
        public string FeedbackSubject { get; set; } = "feedback - ";
        public string SupportSubject { get; set; } = "support - ";

        public string OtherWebsite;

        public KillerrinApplicationData(string otherWebsite = "")
        {
            PackageID = AppPackage.Id.FamilyName;
            PublisherID = AppPackage.Id.PublisherId;

            Name = AppPackage.DisplayName;
            PublisherName = AppPackage.PublisherDisplayName;
            Description = AppPackage.Description;

            PackageVersion packageVersion = AppPackage.Id.Version;
            Version = string.Format("{0}.{1}.{2}.{3}", packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);


            OtherWebsite = otherWebsite;
            FeedbackSubject += Name + ": ";
            SupportSubject += Name + ": ";
        }

        public async Task<bool> LaunchReview()
        {
            bool result = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:review?PFN=" + PackageID));
            return result;
        }

        public static async Task SendEmail(string emailAddress, string subject, string body)
        {
            //predefine Recipient
            EmailRecipient sendTo = new EmailRecipient()
            {
                Address = emailAddress
            };

            //generate mail object
            EmailMessage mail = new EmailMessage();
            mail.Subject = subject;
            mail.Body = body;

            //add recipients to the mail object
            mail.To.Add(sendTo);
            //mail.Bcc.Add(sendTo);
            //mail.CC.Add(sendTo);

            //open the share contract with Mail only:
            await EmailManager.ShowComposeNewEmailAsync(mail);
        }
    }
}
