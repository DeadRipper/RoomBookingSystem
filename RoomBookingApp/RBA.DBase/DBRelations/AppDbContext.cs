using Microsoft.EntityFrameworkCore;
using RBA.Models.Models.AdminModels;
using RBA.Models.Models.AuthModels;
using RBA.Models.Models.RoomBookingModels;
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
        public DbSet<AmenityModel> Amenities => Set<AmenityModel>();
        public DbSet<AdminModel> Admins => Set<AdminModel>();
        public DbSet<AdminInfo> AdminInfo => Set<AdminInfo>();
        public DbSet<AuthModel> Auths => Set<AuthModel>();
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}