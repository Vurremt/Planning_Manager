using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventService.Entities;

namespace EventService.Data
{
    public class EventServiceContext : DbContext
    {
        public EventServiceContext (DbContextOptions<EventServiceContext> options)
            : base(options)
        {
        }

        public DbSet<EventModel> Event { get; set; } = default!;
    }
}
