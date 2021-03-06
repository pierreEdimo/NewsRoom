using Microsoft.EntityFrameworkCore;
using newsroom.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace newsroom.DBContext
{


    public class DatabaseContext : IdentityDbContext

    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DatabaseContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Article>().HasOne(x => x.Author).WithMany(x => x.Articles).HasForeignKey(x => x.AuthorId);

            modelBuilder.Entity<Article>().HasOne(x => x.Topic).WithMany(x => x.Articles).HasForeignKey(x => x.TopicId);

            modelBuilder.Entity<Article>().HasMany(x => x.Comments).WithOne(x => x.Article).HasForeignKey(x => x.ArticleId);

            modelBuilder.Entity<Comment>().HasOne(x => x.Author).WithMany(x => x.Comments).HasForeignKey(x => x.AuthorId);

            modelBuilder.Entity<UserEntity>().HasMany(x => x.Comments).WithOne(x => x.Author).HasForeignKey(x => x.AuthorId); 

            modelBuilder.Entity<FavoritesArticles>().HasKey(x => new { x.ArticleId, x.OwnerId });

            modelBuilder.Entity<Article>().HasMany(x => x.HasFavorites).WithOne(x => x.Article).HasForeignKey(x => x.ArticleId); 
                                                     
            modelBuilder.Seed();
        }


        public DbSet<Article> Articles { get; set; }
        public DbSet<Topic> Topics { get; set;  }
        public DbSet<Author> Authors { get; set;  }
        public DbSet<Comment> Comments { get; set;  }
        public DbSet<KeyWord> KeyWords { get; set;  }
        public DbSet<FavoritesArticles> Favorites { get; set;  }
    }
}