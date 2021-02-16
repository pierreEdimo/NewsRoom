﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using newsroom.DBContext;
using newsroom.DTO;
using newsroom.Model;
using newsroom.Helpers;
using System.IO;
using newsroom.Services;
using System.Linq.Dynamic.Core; 

namespace newsroom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private String containerName = "Article"; 

        public ArticlesController(DatabaseContext context, 
                                  IMapper mapper, 
                                  IFileStorageService fileStorageService
                                  )
        {
            _context = context;
            _mapper = mapper;
            _fileStorageService = fileStorageService; 
        }

        // GET: api/Articles
        [HttpGet]
        public async Task<ActionResult<List<ArticleDTO>>> GetArticles([FromQuery] PaginationDTO pagination )
        {
            var queryable = _context.Articles.AsQueryable();

            await HttpContext.InsertPaginationParametersInResponse(queryable, pagination.RecordsPerPage);

            var articles = await queryable.Paginate(pagination)
                                          .Include(x => x.Author)
                                          .Include(x => x.Topic)
                                          .ToListAsync();

            var articleDTO = _mapper.Map<List<ArticleDTO>>(articles);

            return articleDTO; 
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<ArticleDTO>>> Filter([FromQuery] FilterDTO filterDTO)
        {
            var articleQueryable = _context.Articles.AsQueryable();

            if (!String.IsNullOrWhiteSpace(filterDTO.Title))
            {
                articleQueryable = articleQueryable.Where(x => x.Title.Contains(filterDTO.Title)); 
            }

            if(filterDTO.TopicId != 0)
            {
                articleQueryable = articleQueryable
                          .Where(x => x.Topic.Id.ToString().Contains(filterDTO.TopicId.ToString())); 
            }

            if (!String.IsNullOrWhiteSpace(filterDTO.OrderingField))
            {
                articleQueryable = articleQueryable.OrderBy($"{filterDTO.OrderingField} {(filterDTO.AscendingOrder ? "ascending" : "descending")} "); 
            }

            await HttpContext.InsertPaginationParametersInResponse(articleQueryable, filterDTO.RecordsPerPage);

            var articles = await articleQueryable.Include(a => a.Author)
                                                 .Paginate(filterDTO.Pagination).ToListAsync();

            return _mapper.Map<List<ArticleDTO>>(articles); 
        }

        // GET: api/Articles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleDetailsDTO>> GetArticle(int Id)
        {
            var article = await _context.Articles.Include(x => x.Author)
                                                 .Include(x => x.Topic)
                                                 .Include(x => x.Comments)
                                                 .FirstOrDefaultAsync(x => x.Id == Id);

            article.CommentCount = article.Comments.Count(); 

            if (article == null)
            {
                return NotFound();
            }

            var articleDTO = _mapper.Map<ArticleDetailsDTO>(article); 

            return articleDTO;
        }

        // PUT: api/Articles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(int Id, CreateArticleDTO updateArticle )
        {
            var articleDB = await _context.Articles.FirstOrDefaultAsync(x => x.Id == Id); 

            if(articleDB == null)
            {
                return NotFound(); 
            }

            articleDB = _mapper.Map(updateArticle, articleDB); 

            if(updateArticle.ImageUrl != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await updateArticle.ImageUrl.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(updateArticle.ImageUrl.FileName);
                    articleDB.ImageUrl = await _fileStorageService.EditFile(content, extension, containerName, articleDB.ImageUrl, updateArticle.ImageUrl.ContentType); 
                }
            }

            await _context.SaveChangesAsync(); 

            return NoContent();
        }

        // POST: api/Articles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostArticle([FromForm] CreateArticleDTO createArticleDTO )
        {
            var article = _mapper.Map<Article>(createArticleDTO);

            if(createArticleDTO.ImageUrl != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await createArticleDTO.ImageUrl.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(createArticleDTO.ImageUrl.FileName);
                    article.ImageUrl = await _fileStorageService.SaveFile(content, extension, containerName , createArticleDTO.ImageUrl.ContentType); 
                }
            }

            _context.Add(article); 

            await _context.SaveChangesAsync();

            var articleDTO = _mapper.Map<ArticleDTO>(article);

            return CreatedAtAction("GetArticle", new { id = articleDTO.Id }, article);
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int Id)
        {
            var exists = _context.Articles.AnyAsync(x => x.Id == Id); 

            if(!await exists)
            {
                return NotFound(); 
            }

            _context.Remove(new Article() { Id = Id });
            await _context.SaveChangesAsync(); 

            return NoContent();
        }

    }
}