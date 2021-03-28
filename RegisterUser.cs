#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"



using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;



public static async Task<IActionResult> Run(HttpRequest req, 
CloudTable objUserProfileTable,
IAsyncCollector<string> objUserProfileQueueItem,
IAsyncCollector<string> NotificationQueueItem,
ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");
    string firstname = null, lastname = null, email = null;

    
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic inputjson = JsonConvert.DeserializeObject(requestBody);

    firstname = firstname ?? inputjson?.firstname;
    lastname = inputjson?.lastname;
    email = inputjson.email;

    //////////////////////////
    // Save picture profile
    //////////////////////////

    string profileURL = inputjson.ProfilePicURL;
    await objUserProfileQueueItem.AddAsync(profileURL);  
    

 

    //////////////////////////////
    // Save userprofile to Table
    /////////////////////////////
    UserProfile objUserProfile = new UserProfile(firstname, lastname, profileURL, email);
    TableOperation objTblOperationInsert = TableOperation.Insert(objUserProfile);
    await objUserProfileTable.ExecuteAsync(objTblOperationInsert);
    
   
    // send grid email
    await NotificationQueueItem.AddAsync(JsonConvert.SerializeObject(objUserProfile));



    return (lastname + firstname) != null?
    (ActionResult)new OkObjectResult($"Hello, {firstname + " " + lastname}")
    : new BadRequestObjectResult("Please pass a name on the query " + " string or in the request body");
}


class UserProfile : TableEntity
{
    public UserProfile(string firstName, string lastName, string profilePicURL, string email)
    {
        this.PartitionKey = "p1";
        this.RowKey = Guid.NewGuid().ToString();
        this.FirstName = firstName;
        this.LastName = lastName;
        this.ProfilePicURL = profilePicURL;
        this.Email = email;
    }
    UserProfile() {}
    public string FirstName {get;set;}
    public string LastName {get;set;}
 
    public string Email {get;set;}
    public string ProfilePicURL {get;set;}


}
