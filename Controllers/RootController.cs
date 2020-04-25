using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace findaDoctor.Controllers
{
    [ApiController]
    [Route("/")]
    public class RootController : ControllerBase
    {

        [HttpGet(Name = nameof(GetRoot))]
        public IActionResult GetRoot()
        {
            var response = new
            {
                href = Url.Link(nameof(GetRoot), null),
                doctors = new
                {
                    href = Url.Link(nameof(DoctorController.GetDoctors), null)
                },
                articles = new
                {
                    href = Url.Link(nameof(ArticleController.GetArticles), null)
                },
                authors = new
                {
                    href = Url.Link(nameof(AuthorController.GetAuthors), null)
                },
                themes = new
                {
                    href = Url.Link(nameof(ThemeController.GetThemes), null)
                }

            };

            return Ok(response);
        }
    }
}