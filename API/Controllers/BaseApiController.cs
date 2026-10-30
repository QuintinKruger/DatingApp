using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")] // localhost:5001/api/members
    [ApiController]
    // provides validation and parameter mapping to requests received in actions (or Http methods or methods in controller class that handes http methods)
    public class BaseApiController : ControllerBase // no view support (html response) controller
    {
    }
}
