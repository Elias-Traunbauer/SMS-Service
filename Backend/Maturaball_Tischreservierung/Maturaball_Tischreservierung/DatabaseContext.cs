using Maturaball_Tischreservierung.Models;
using Microsoft.EntityFrameworkCore;

namespace Maturaball_Tischreservierung
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ServerSettings> ServerSettings { get; set; }
        public DbSet<TableReservation> TableReservations { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientSession> ClientSessions { get; set; }
        public DbSet<ClientAccessCode> ClientAccessCodes { get; set; }

        private ApiConfig _apiConfig;

        public DatabaseContext(ApiConfig apiConfig)
        {
            _apiConfig = apiConfig;
            Database.EnsureCreated();
        }

        public DatabaseContext()
        {
            _apiConfig = null!;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(_apiConfig?.ConnectionString ?? "Server=localhost;Database=hh_plandata;Uid=ad;Pwd=ad");
        }
    }
}
