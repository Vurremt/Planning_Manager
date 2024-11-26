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
    public class GroupController : ControllerBase
    {
        private readonly EventServiceContext _context;

        public GroupController(EventServiceContext context)
        {
            _context = context;
        }

        // GET api/Group/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupModel>> GetGroupById(int id)
        {
            var group = await _context.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound();
            }

            return Ok(group);
        }

        // GET: api/Group/list
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<GroupModel>>> GetAllGroups()
        {
            var groups = await _context.Groups.ToListAsync();
            return Ok(groups);
        }

        // GET: api/Group/list/{UserId}
        [HttpGet("list/{UserId}")]
        public async Task<ActionResult<IEnumerable<GroupModel>>> GetGroupsByUserId(int UserId)
        {
            var groups = await _context.Groups.ToListAsync(); 
            
            var userGroups = groups .Where(g => g.ManagerIds.Contains(UserId) || g.SubscriberIds.Contains(UserId)) .ToList();
            
            if (!userGroups.Any()) {
                return NotFound("L'utilisateur n'appartient à aucun groupe.");
            }

            return Ok(userGroups);
        }

        // POST api/Group/create
        [HttpPost("create")]
        public async Task<ActionResult<GroupModel>> CreateGroup(GroupModel newGroup)
        {
            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGroupById), new { id = newGroup.Id }, newGroup);
        }

        // PUT api/Group/update/{id}
        [HttpPut("update/{id}")]
        public async Task<ActionResult<GroupModel>> UpdateGroup(int id, GroupModel updatedGroup)
        {
            var existingGroup = await _context.Groups.FindAsync(id);
            if (existingGroup == null)
            {
                return NotFound();
            }

            // Mettre à jour les propriétés du groupe
            existingGroup.Titre = updatedGroup.Titre;
            existingGroup.Description = updatedGroup.Description;
            existingGroup.ManagerIds = updatedGroup.ManagerIds;
            existingGroup.SubscriberIds = updatedGroup.SubscriberIds;

            await _context.SaveChangesAsync();
            return Ok(existingGroup);
        }

        // DELETE api/Group/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteGroup(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return Ok(true);
        }

        // GET api/Group/{id}/events
        [HttpGet("{id}/events")]
        public async Task<ActionResult<IEnumerable<EventModel>>> GetEventsByGroupId(int id)
        {
            var groupExists = await _context.Groups.AnyAsync(g => g.Id == id);
            if (!groupExists)
            {
                return NotFound("Le groupe spécifié n'existe pas.");
            }

            var events = await _context.Events
                .Where(e => e.GroupId == id)
                .ToListAsync();

            return Ok(events);
        }
    }
}
