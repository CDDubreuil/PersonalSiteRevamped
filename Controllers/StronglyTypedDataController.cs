using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; //Grants easy access to the info in appsettings.json
using PersonalSiteRevamped.Models; //Grants easy access to our ContactViewModel
using MimeKit; //Added for access to MimeMessage class
using MailKit.Net.Smtp; //Added for access to SmtpClient class
using Microsoft.Data.SqlClient;
using PersonalSiteRevamped.Models;

namespace CORE1.Controllers
{
    //We won't be using an Index Action/View for this controller, so we can simply
    //comment out or delete the one that is provided. 

    public class StronglyTypedDataController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        #region Adding Credentials to appsettings.json
        /*
         * Before creating any Actions or Views related to this Controller, we will
         * add a Credentials section to the appsettings.json file. This lets
         * us store the sensitive login information for our email account in that file so 
         * it does not need to be written here directly.
         * 
         * When using source control with an MVC Web App like this one, you can add 
         * the following lines to your .gitignore file to prevent the 
         * appsettings.json file from being pushed to the remote repo: 
         * 
         *  /SolutionName/ProjectName/appsettings.json
         *  /ProjectName/appsettings.json
         *  
         *  The lines above will handle cases where you choose to place the solution and
         *  projects in separate folders (recommended, top), or if you check the "place solution and project in
         *  same directory" box when creating the project (bottom). Alternatively, you can use this line 
         *  to handle either file structure (replace asterisk with *):
         *  
         *  asterisk/appsettings.json
         */


        #endregion

        //In order to access the info in our appsettings.json file, we need to: 
        //Add the information to the file
        //Add a using statement to the controller for Microsoft.Extensions.Configuration
        //Create a field in the Controller to store the configuration info
        //Create/update a constructor for the Controller that accepts the configuration 
        //info as a parameter and assigns it to the field. 

        //Create a field for the Configuration information in appsettings.json
        private readonly IConfiguration _config;

        //Create a constructor for our Controller that accepts the config info as a parameter.
        public StronglyTypedDataController(IConfiguration config)
        {
            //Assign the configuration info to the config field.
            _config = config;
        }

        //If your Controller already has a constructor, you CANNOT add another
        //constructor. So, you must instead update the constructor by adding the 
        //parameter and assignment to it. 

        //Mini Lab
        //Open your Personal Site MVC solution and update the HomeController:
        //Add "using Microsoft.Extensions.Configuration;"
        //Create the field for the configuration info
        //Update the constructor to accept the config info as a parameter
        //and assign it to the field.

        //Controller Actions are meant to handle certain types of requests. The 
        //most common request is GET, which is used to request the info to load a page.
        //We will also create actions to handle POST requests, which are used to 
        //send info back to the application.

        //GET is the default request type to be handled, so no extra info is needed here.
        //If we wanted to be explicit, we could decorate this action as follows: 
        [HttpGet] //This is unnecessary, as HttpGet is assigned by default. 
        public IActionResult Contact()
        {
            //We want the info from our Contact form to use the ContactViewModel we created. 
            //We can do this easily by generating the necessary code using the following steps:
            #region Code Generation Steps

            //1. Go to Tools > NuGet Package Manager > Manage NuGet Packages for Solution
            //2. Go to the Browse tab and search for Microsoft.VisualStudio.Web
            //3. Click Microsoft.VisualStudio.Web.CodeGeneration.Design
            //4. On the right, check the box next to the CORE1 project, then click "Install"
            //Note: Ensure the version you're installing is compatible with your project's version of .NET.
            //In this case, we installed version 6.0.16 since our version of .NET is version 6.
            //5. Once installed, return here and right click the Contact action
            //6. Select Add View, then select the Razor View template and click "Add"
            //7. Enter the following settings:
            //      - View Name: Contact
            //      - Template: Create
            //      - Model Class: ContactViewModel
            //8. Leave all other settings as-is and click "Add"

            #endregion

            return View();
        }

        //Now we need to handle what to do when the user submits the form. For this, 
        //we will make another Contact action, this time intended for handling the POST request.
        [HttpPost]//Post is like mail, it's when someone wants to submit information to us.
        public IActionResult Contact(ContactViewModel cvm)
        {
            //When a class has validation attributes, that validation should be checked before trying
            //to process any of the data they provided. 
            if (!ModelState.IsValid)
            {
                //Send the user back to the form. We can pass the object to the view so the form
                //will contain the original information they provided. 
                return View(cvm);
            }

            //To handle sending the email, we'll need to install a Nuget Package and 
            //add a few using statements. We can do this will the following steps: 

            #region Email Setup Steps & Email Info

            //1. Go to Tools > NuGet Package Manager > Manage NuGet Packages for Solution
            //2. Go to the Browse tab and search for NETCore.MailKit
            //3. Click NETCore.MailKit
            //4. On the right, check the box next to the CORE1 project, then click "Install"
            //5. Once installed, return here
            //6. Add the following using statements & comments:
            //      - using MimeKit; //Added for access to MimeMessage class
            //      - using MailKit.Net.Smtp; //Added for access to SmtpClient class
            //7. Once added, return here to continue coding email functionality

            //MIME - Multipurpose Internet Mail Extensions - Allows email to contain
            //information other than ASCII, including audio, video, images, and HTML

            //SMTP - Secure Mail Transfer Protocol - An internet protocol (similar to HTTP)
            //that specializes in the collection & transfer of email data

            #endregion

            string message = $"You have received a new email from your site's contact form!<br />" +
                $"Sender: {cvm.Name} <br />Email: {cvm.Email}<br />Subject: {cvm.Subject}<br />Message: {cvm.Message}";

            //Create a MimeMessage object to assist with storing/transporting the email information
            //from the contact form.
            var mm = new MimeMessage();

            //Even though the user is the one attempting to reach us, the actual sender of the email
            //will be the email user we set up with our hosting provider. 
            //CREDENTIALS>EMAIL>USER
            //We can access the credentials for this email user from our appsettings.json file as shown below.
            mm.From.Add(new MailboxAddress("Sender", _config.GetValue<string>("Credentials:Email:User")));

            //The recipient of this email will be our personal email address, also stored in appsettings.json.
            mm.To.Add(new MailboxAddress("Personal", _config.GetValue<string>("Credentials:Email:Recipient")));

            //The subject will be the one provided by the user, which is stored in the cvm object. 
            mm.Subject = cvm.Subject;

            //The body of the message will be formatted with the string we created above. 
            mm.Body = new TextPart("HTML") { Text = message };

            //We can set the priority of the messages as "urgent" so it will be flagged in our email client. 
            mm.Priority = MessagePriority.Urgent;

            //We can also add the user's provided email address to the list of ReplyTo addresses so 
            //our replies can be sent directly to them instead of our own email user. 
            mm.ReplyTo.Add(new MailboxAddress("User", cvm.Email));

            //The using directive will create the SmtpClient object used to send the email. Once all
            //of the code inside of the using directive's scope has been executed, it will close any 
            //open connections and dispose of the object for us. 
            using (var client = new SmtpClient())
            {
                //Connect to the mail server using credentials in our appsettings.json
                // client.Connect(_config.GetValue<string>("Credentials:Email:Client"));

                ////Some ISPs may block the default Smtp port (25), so if you encounter issues sending
                ////email with the line above, comment it out and uncomment the line below, which does
                ////the same thing, but specifies to use the alternate SMTP port 8889.
                client.Connect(_config.GetValue<string>("Credentials:Email:Client"), 8889);

                //Log in to the mail server using the credentials for our email user
                client.Authenticate(

                    //Username
                    _config.GetValue<string>("Credentials:Email:User"),
                     //Password
                     _config.GetValue<string>("Credentials:Email:Password")
                    );
                //It's possible the mail server may be down when the user attempts to contact us, 
                //so we can "encapsulate" our code to send the message in a try/catch block.
                try
                {
                    //Try to send the email
                    client.Send(mm);
                }
                catch (Exception ex)
                {
                    //If there is an issue, we can store an error message in a ViewBag variable
                    //to be displayed in the View.
                    ViewBag.ErrorMessage = $"There was an error processing your request. Please " +
                        $"try again later. <br />ErrorMessage: {ex.StackTrace}";

                    //return the user to the View with their form information intact
                    return View(cvm);
                }
            }//When we get here, we will automatically dispose of the client object.

            //If all goes well, return a View that displays a confirmation to the user that their email was sent successfully.

            return View("EmailConfirmation", cvm);
        }
        //GET: DomainModels
  
 
        public IActionResult EntityModels()
        {
            return View();
        }
    }
}
