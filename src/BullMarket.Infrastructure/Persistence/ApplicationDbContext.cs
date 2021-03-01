using BullMarket.Application.Interfaces;
using BullMarket.Application.Interfaces.Services;
using BullMarket.Domain.Common;
using BullMarket.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BullMarket.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>, IApplicationDbContext
    {
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

        public Task<int> SaveAuditableChangesAsync<T>(string username, T auditableEntityIdentifier)
            where T : struct
        {
            AddAuditInfo(username, auditableEntityIdentifier);
            return SaveChangesAsync();
        }

        private void AddAuditInfo<T>(string username, T auditableEntityIdentifier)
            where T : struct
        {
            this
                .ChangeTracker
                .Entries()
                .Where(entry => entry.State == EntityState.Added || entry.State == EntityState.Modified)
                .Select(entry => new
                {
                    entry.Entity,
                    entry.State
                })
                .ToList()
                .ForEach(entry =>
                {
                    if (entry.Entity is AuditableEntity<T> entity)
                    {
                        var currentTime = GetTime();

                        if (entry.State == EntityState.Added)
                        {
                            entity.CreatedOn = currentTime;
                            entity.CreatedBy = username;
                        }
                        else if (entry.State == EntityState.Modified)
                        {
                            entity.UpdatedOn = currentTime;
                            entity.UpdatedBy = username;
                        }
                    }
                });
        }

        private DateTime GetTime()
        {
            var destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

            if (destinationTimeZone == null)
            {
                var timeZones = TimeZoneInfo.GetSystemTimeZones();
                destinationTimeZone = timeZones.FirstOrDefault(tz => tz.DisplayName.Contains("Sofia"));
            }

            var utcTime = DateTime.UtcNow;

            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, destinationTimeZone);
        }
    }
}
