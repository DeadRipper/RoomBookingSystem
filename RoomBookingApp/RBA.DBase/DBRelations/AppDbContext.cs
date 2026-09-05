using Microsoft.EntityFrameworkCore;
using RBA.Models.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.DBRelations
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserModel> Users => Set<UserModel>();
        public DbSet<RoomModel> Rooms => Set<RoomModel>();
        public DbSet<ReservationModel> Reservations => Set<ReservationModel>();

        public AppDbContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}