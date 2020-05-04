using HR.Model;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace HR.DataAccess
{
    public class HrDbContext : DbContext
    {
        public HrDbContext() : base("HrDb")
        {

        }

        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CandidatePhoneNumber> CandidatePhoneNumbers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //Additional Configuration
            //modelBuilder.Configurations.Add(new CandidateConfiguration());
        }
    }

    /* Additional Configuration
    public class CandidateConfiguration : EntityTypeConfiguration<Candidate>
    {
        public CandidateConfiguration()
        {
            Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
    */
}
