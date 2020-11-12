using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace newsroom.Controllers
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

                articles = new
                {
                    href = Url.Link(nameof(ArticleController.GetArticles), null)
                },
                articleFromAuthor = new {
                    href = Url.Link(nameof(ArticleController.GetArticleFromAuthor), null)
                } ,
                themes = new
                {
                    href = Url.Link(nameof(ThemeController.GetThemes), null)
                },
                users = new
                {
                    href = Url.Link(nameof(UserController.GetAllUsers), null)
                },
                favoritesarticles = new
                {
                    href = Url.Link(nameof(FavoriteArticlesController.GetFavoriteeArticles), null)
                },
                comments = new
                {
                    href = Url.Link(nameof(CommentController.GetComments), null)
                },
                suggestions = new
                {
                    href = Url.Link(nameof(SuggestionsController.GetSuggestions), null)
                }
            

            };

            return Ok(response);
        }
    }
}