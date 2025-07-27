using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketBooking.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void SeedFromSqlFile(this ApplicationDbContext db, string path = "seed-data.sql")
        {
            if (db.Movies.Any())
                return;

            var sql = File.ReadAllText(path);
            db.Database.ExecuteSqlRaw(sql);
        }
    }
}
