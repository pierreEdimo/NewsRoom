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

            modelBuilder.Entity<Theme>().HasMany(c => c.Articles).WithOne(a => a.Theme).HasForeignKey(a => a.themeId);

            modelBuilder.Entity<Article>().HasOne(f => f.Theme).WithMany(a => a.Articles);

            modelBuilder.Entity<Comments>().HasOne(f => f.author).WithMany(a => a.Comments);

            modelBuilder.Entity<UserEntity>().HasMany(a => a.Comments).WithOne(a => a.author).HasForeignKey(a => a.uid);

            modelBuilder.Entity<Author>().HasMany(a => a.Articles).WithOne(a => a.Author).HasForeignKey(a => a.authorId);

            modelBuilder.Entity<Article>().HasOne(a => a.Author).WithMany(a => a.Articles);

            modelBuilder.Entity<Article>().HasMany(a => a.Comments).WithOne(a => a.Article).HasForeignKey(a => a.articleId);

            modelBuilder.Entity<Comments>().HasOne(a => a.Article).WithMany(a => a.Comments);

            modelBuilder.Entity<Favorites>().HasKey(a => new { a.articleId, a.userId });

            modelBuilder.Entity<Favorites>().HasOne(a => a.Article).WithMany(a => a.Favorites).HasForeignKey(a => a.articleId);

            modelBuilder.Entity<Favorites>().HasOne(a => a.User).WithMany(a => a.Favorites).HasForeignKey(a => a.userId); 
                                                     
            modelBuilder.Seed();
        }


        public DbSet<Article> Articles { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Author> Author { get; set; }
        public DbSet<Favorites> Favorites { get; set;  }
        public DbSet<SearchWord> SearchWords { get; set;  }
        

    }
}