#r "SendGrid"
#r "Newtonsoft.Json"
#r "Microsoft.Azure.WebJobs.Extensions.Storage"
#r "Twilio"
#r "Microsoft.Azure.WebJobs.Extensions.Twilio"

using System;
using SendGrid.Helpers.Mail;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using Microsoft.Azure.WebJobs.Extensions.Twilio;
using Twilio.Rest.Api.V2010.Account; 
using Twilio.Types;

public static void Run(string myQueueItem, 
out SendGridMessage message,
//TextWriter outpubBlob,
IBinder binder,
out CreateMessageOptions objsmsmessage,
ILogger log)
{
    log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
    dynamic inputJson = JsonConvert.DeserializeObject(myQueueItem);

    
    string FirstName = null, LastName = null, Email = null, emailContent = null;

    FirstName = inputJson.FirstName;
    LastName = inputJson.LastName;
    Email = inputJson.Email;

    log.LogInformation($"Email {Email}, {FirstName}, {LastName}");

    emailContent = "Thank you <b>" + FirstName + " " + LastName + "</b> for your registration. <br><br>" + "Below are the details that you have provided us<br><br>"
                + "<b>First Name: </b>" + FirstName 
                + "<b>Last Name: </b>" + LastName
                + "<b>Email: </b>" + Email 
                + "<br><br><br>" 
                + "Best Regards, " + "<br>" 
                + "Web Site Team";
    

    message = new SendGridMessage();



    message.SetSubject("New User got registered successfully");
    message.SetFrom("silam@hotmail.com");
    message.AddTo(Email, FirstName + " " + LastName);

    message.AddContent("text/html", emailContent);
    // add attachment
    message.AddAttachment(FirstName + "_" + LastName + ".log", 
        System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(emailContent)),
        "text/plain",
        "attachment",
        "Logs");
    // outpubBlob.WriteLine(emailContent); 
    using (var emailLogBloboutput = binder.Bind<TextWriter>(new BlobAttribute($"userregistrationemaillogs/{inputJson.RowKey}.log", FileAccess.Write)))
    {
        emailLogBloboutput.WriteLine(emailContent);
    }


    objsmsmessage = new CreateMessageOptions(new PhoneNumber("16123083370"));
    objsmsmessage.Body = "Hello Thanks for registering";
}
