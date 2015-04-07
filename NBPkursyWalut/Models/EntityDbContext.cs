using NBPkursyWalut.Migrations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NBPkursyWalut.Models
{
    public class EntityDbContext:DbContext
    {
        public DbSet<Position> Postions { get; set; }

        public DbSet<Dir> Dirs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EntityDbContext, Configuration>());
        }

    }
}