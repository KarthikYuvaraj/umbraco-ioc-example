using System.Web.Mvc;

namespace WebApplication12.Controllers
{
    public class ProductController : Controller
    {
        private readonly FooService foo;

        public ProductController(FooService foo)
        {
            this.foo = foo;
        }

        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
    }

    public class FooService
    {

    }
}