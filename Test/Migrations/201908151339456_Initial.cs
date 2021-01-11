namespace Data.Cqrs.Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Author",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BookAuthor",
                c => new
                    {
                        BookId = c.Int(nullable: false),
                        AuthorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BookId, t.AuthorId })
                .ForeignKey("dbo.Author", t => t.AuthorId, cascadeDelete: true)
                .ForeignKey("dbo.Book", t => t.BookId, cascadeDelete: true)
                .Index(t => t.BookId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.Book",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        PublisherId = c.Int(),
                        HasElectronic = c.Boolean(nullable: false),
                        Published = c.Int(nullable: false),
                        Raiting = c.Int(),
                        Author_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Publisher", t => t.PublisherId)
                .ForeignKey("dbo.Author", t => t.Author_Id)
                .Index(t => t.PublisherId)
                .Index(t => t.Author_Id);
            
            CreateTable(
                "dbo.BookGenre",
                c => new
                    {
                        BookId = c.Int(nullable: false),
                        GenreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BookId, t.GenreId })
                .ForeignKey("dbo.Book", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.Genre", t => t.GenreId, cascadeDelete: true)
                .Index(t => t.BookId)
                .Index(t => t.GenreId);
            
            CreateTable(
                "dbo.Genre",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Publisher",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Book", "Author_Id", "dbo.Author");
            DropForeignKey("dbo.BookAuthor", "BookId", "dbo.Book");
            DropForeignKey("dbo.Book", "PublisherId", "dbo.Publisher");
            DropForeignKey("dbo.BookGenre", "GenreId", "dbo.Genre");
            DropForeignKey("dbo.BookGenre", "BookId", "dbo.Book");
            DropForeignKey("dbo.BookAuthor", "AuthorId", "dbo.Author");
            DropIndex("dbo.BookGenre", new[] { "GenreId" });
            DropIndex("dbo.BookGenre", new[] { "BookId" });
            DropIndex("dbo.Book", new[] { "Author_Id" });
            DropIndex("dbo.Book", new[] { "PublisherId" });
            DropIndex("dbo.BookAuthor", new[] { "AuthorId" });
            DropIndex("dbo.BookAuthor", new[] { "BookId" });
            DropTable("dbo.Publisher");
            DropTable("dbo.Genre");
            DropTable("dbo.BookGenre");
            DropTable("dbo.Book");
            DropTable("dbo.BookAuthor");
            DropTable("dbo.Author");
        }
    }
}
