using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FallbackController : Controller
    {
        public ActionResult Index()
        {
                //API FOLDER THEN WWW ROOT AND GET INDEX TYPE TEXTHTML
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "index.html"), "text/HTML");
        }
    }
}
