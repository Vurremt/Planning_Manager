using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventService.Entities;
using EventService.Data;
using Microsoft.EntityFrameworkCore;

namespace EventService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventServiceContext _context;

        public EventController(EventServiceContext context)
        {
            _context = context;
        }

        // GET: api/Event/list/{UserId}
        [HttpGet("list/{UserId}")]
        public async Task<ActionResult<IEnumerable<EventModel>>> GetEventsByUserId(int UserId)
        {
            // Étape 1 : Récupérer les groupes de l'utilisateur
            var groupIds = _context.Groups
                .AsEnumerable()
                .Where(g => g.ManagerIds.Contains(UserId) || g.SubscriberIds.Contains(UserId))
                .Select(g => g.Id)
                .ToList();

            if (!groupIds.Any())
            {
                return NotFound("L'utilisateur n'appartient à aucun groupe.");
            }

            // Étape 2 : Récupérer tous les événements associés à ces groupes
            var events = await _context.Events
                .Where(e => groupIds.Contains(e.GroupId))
                .ToListAsync();

            return Ok(events);
        }

        // POST api/Event/create
        [HttpPost("create")]
        public async Task<ActionResult<EventModel>> CreateEvent(EventModel newEvent)
        {
            // Penser à ajouter le user à ManagersIds
            if (!_context.Groups.Any(g => g.Id == newEvent.GroupId))
            {
                return BadRequest("Le groupe spécifié n'existe pas.");
            }

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEventById), new { id = newEvent.Id }, newEvent);
        }

        // GET api/Event/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EventModel>> GetEventById(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return Ok(eventItem);
        }

        // PUT api/Event/update/{id}
        [HttpPut("update/{id}")]
        public async Task<ActionResult<EventModel>> UpdateEvent(int id, EventModel updatedEvent)
        {
            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            existingEvent.Title = updatedEvent.Title;
            existingEvent.Place = updatedEvent.Place;
            existingEvent.Description = updatedEvent.Description;
            existingEvent.StartingDate = updatedEvent.StartingDate;
            existingEvent.Duration = updatedEvent.Duration;
            existingEvent.GroupId = updatedEvent.GroupId;

            await _context.SaveChangesAsync();
            return Ok(existingEvent);
        }

        // DELETE api/Event/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();
            return Ok(true);
        }
    }
}
