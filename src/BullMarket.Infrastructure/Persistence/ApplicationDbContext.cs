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
        private readonly ICurrentUserService _currentUserService;

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public ApplicationDbContext
            (DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AddAuditInfo();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AddAuditInfo();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void AddAuditInfo()
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
                    if (entry.Entity is AuditableEntity<object> entity)
                    {
                        var currentTime = GetTime();

                        if (entry.State == EntityState.Added)
                        {
                            entity.CreatedOn = currentTime;
                            entity.CreatedBy = _currentUserService.GetUsername();
                        }
                        else if (entry.State == EntityState.Modified)
                        {
                            entity.UpdatedOn = currentTime;
                            entity.UpdatedBy = _currentUserService.GetUsername();
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
