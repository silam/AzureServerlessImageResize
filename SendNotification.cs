#r "SendGrid"
#r "Newtonsoft.Json"


using System;
using SendGrid.Helpers.Mail;
using Newtonsoft.Json;

public static void Run(string myQueueItem, 
out SendGridMessage message,
TextWriter outpubBlob,
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
    
    outpubBlob.WriteLine(emailContent); 
}
