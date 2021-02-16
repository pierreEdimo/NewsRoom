﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using newsroom.DBContext;
using newsroom.Model;
using newsroom.DTO;
using AutoMapper;
using System.Linq.Dynamic.Core; 

namespace newsroom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesArticlesController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mappper; 

        public FavoritesArticlesController(DatabaseContext context, 
                                           IMapper mapper )
        {
            _context = context;
            _mappper = mapper; 
        }

        // GET: api/FavoritesArticles
        [HttpGet]
        public async Task<ActionResult<List<FavoriteDTO>>> GetFavorites()
        {
            var favorites = await _context.Favorites.Include(x => x.Article).ToListAsync();

            return _mappper.Map<List<FavoriteDTO>>(favorites); 
        }

        // GET: api/FavoritesArticles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FavoriteDTO>> GetFavoritesArticles(int Id)
        {
            var favoritesArticle = await _context.Favorites.FirstOrDefaultAsync(x => x.ArticleId == Id);

            if (favoritesArticle == null)
            {
                return NotFound();
            }

            var favorite = _mappper.Map<FavoriteDTO>(favoritesArticle); 

            return favorite;
        }


        [HttpGet("[action]")]
        public async Task<ActionResult<List<FavoriteDTO>>> FilterFavorites([FromQuery] FilterDTO filter)
        {
            var queryable = _context.Favorites.AsQueryable();

            if (!String.IsNullOrWhiteSpace(filter.UserId))
            {
                queryable = queryable.Where(x => x.OwnerId.Contains(filter.UserId)); 
            }

            if (!String.IsNullOrWhiteSpace(filter.OrderingField))
            {
                queryable = queryable.OrderBy($"{ filter.OrderingField } {( filter.AscendingOrder ? "ascending" : "descending" )} " ); 
            }

            var favorites = await queryable.ToListAsync();

            return _mappper.Map<List<FavoriteDTO>>(favorites); 
        }

     
        // POST: api/FavoritesArticles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostFavoritesArticles(AddFavoriteDTO addFavoriteDTO)
        {
            var favorite = _mappper.Map<FavoritesArticles>(addFavoriteDTO);

            _context.Add(favorite); 
           
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FavoritesArticlesExists(favorite.ArticleId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var favoriteDTO = _mappper.Map<FavoriteDTO>(favorite); 

            return CreatedAtAction("GetFavoritesArticles", new { id = favoriteDTO.ArticleId }, favoriteDTO );
        }

        // DELETE: api/FavoritesArticles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavoritesArticles(int Id)
        {
            var exists = _context.Favorites.AnyAsync(x => x.ArticleId == Id); 

            if(!await exists)
            {
                return NotFound(); 
            }

            _context.Remove(new FavoritesArticles() { ArticleId = Id });
            await _context.SaveChangesAsync(); 

            return NoContent();
        }

        private bool FavoritesArticlesExists(int id)
        {
            return _context.Favorites.Any(e => e.ArticleId == id);
        }
    }
}