namespace Data.Cqrs.Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PublisherIdAsString : DbMigration
    {
		const int PublisherIdLenght = 10;
		public override void Up()
		{

			DropForeignKey("dbo.Book", "PublisherId", "dbo.Publisher");
			DropIndex("dbo.Book", new[] { "PublisherId" });
			DropPrimaryKey("dbo.Publisher");
			AlterColumn("dbo.Book", "PublisherId", c => c.String(maxLength: PublisherIdLenght));

			CreateTable(
				"dbo.Tmp_Publisher",
				c => new
				{
					Id = c.String(nullable: false, maxLength: PublisherIdLenght),
					Name = c.String(nullable: false),
				})
				.PrimaryKey(t => t.Id);

			Sql(@"IF EXISTS(SELECT * FROM dbo.Publisher)
					EXEC('INSERT INTO dbo.Tmp_Publisher (Id, Name)
					SELECT CONVERT(nvarchar(10), Id), Name FROM dbo.Publisher WITH (HOLDLOCK TABLOCKX)');
				");

			DropTable("dbo.Publisher");
			RenameTable("dbo.Tmp_Publisher", "Publisher");
			CreateIndex("dbo.Book", "PublisherId");
			AddForeignKey("dbo.Book", "PublisherId", "dbo.Publisher", "Id");
		}

		public override void Down()
        {
            DropForeignKey("dbo.Book", "PublisherId", "dbo.Publisher");
            DropIndex("dbo.Book", new[] { "PublisherId" });
            DropPrimaryKey("dbo.Publisher");
            AlterColumn("dbo.Publisher", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Book", "PublisherId", c => c.Int());
            AddPrimaryKey("dbo.Publisher", "Id");
            CreateIndex("dbo.Book", "PublisherId");
            AddForeignKey("dbo.Book", "PublisherId", "dbo.Publisher", "Id");
        }
    }
}
