using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ContactManager.Core.Domain.IdentityEntites;
namespace Entities
{
    public class PersonDbContext:IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {
       public virtual DbSet<Person> Persons {  get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public PersonDbContext(DbContextOptions<PersonDbContext> options):base
            (options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table configurations
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            // Navigation property configuration
            modelBuilder.Entity<Person>()
                .HasOne(p => p.Country)
                .WithMany(c => c.persons)
                .HasForeignKey(p => p.CountryID);

            // TIN configuration
            modelBuilder.Entity<Person>()
                .Property(p => p.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC");
            //base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Country>().ToTable("Countries");
            //modelBuilder.Entity<Person>().ToTable("Persons");
            string countriesString = System.IO.File.ReadAllText("countries.json");
            List<Country> countryList = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesString);
            foreach (Country country in countryList)
               modelBuilder.Entity<Country>().HasData(country);
            string personssString = System.IO.File.ReadAllText("persons.json");
            List<Person>PeronsList=System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personssString);
            foreach (Person person in PeronsList)
                modelBuilder.Entity<Person>().HasData(person);
            modelBuilder.Entity<Person>().Property(p => p.TIN).HasColumnName("TaxIdentificationNumber").HasColumnType("varchar(8)").HasDefaultValue("ABC");

            //modelBuilder.Entity<Person>().HasOne(C=>C.country).WithMany(p=>p.country)
        }
        public List<Person>sp_GetAllPerson()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }
    }
}
