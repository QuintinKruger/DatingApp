using System;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// to redirect urls that are meant for angular to the angular application 
// rather than have dotnet app handle it - this is the case because dotnet is serving 
// the angular app located in wwwroot folder. This controller will be responsible for routing 
// something like for e.g. https://localhost:5001/members/lisa-id/photos to 
public class FallbackController : Controller // MVC controller with view support (thus can return html template files)
{
    // any root dotnet does not know about will be served this file
    public ActionResult Index()
    {
        return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/index.html"), "text/html");
    }
}
