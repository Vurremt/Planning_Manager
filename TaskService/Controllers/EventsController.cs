using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using EventService.Entities;
using EventService.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {

        private EventDb EventDb { get; set; }

        public EventController(EventDb EventDb)
        {
            EventDb = EventDb;
        }

        // GET: api/Events/list/:UserId
        [HttpGet("list/{UserId}")]
        public ActionResult<IEnumerable<Entities.EventModel>> Get(int UserId)
        {
            List<Entities.EventModel>? Events;
            if (EventDb.Events.TryGetValue(UserId, out Events) && Events != null)
            {
                return Events;
            }
            else
            {
                EventDb.Events[UserId] = new List<Entities.EventModel>();
                return Ok(EventDb.Events[UserId]);
            }
        }

        // POST api/Events/create
        [HttpPost("create/{UserId}")]
        public ActionResult<Entities.EventModel> CreateEvent(int UserId, EventCreate Event)
        {
            List<Entities.EventModel>? Events;
            if (!EventDb.Events.TryGetValue(UserId, out Events) || Events == null)
            {
                Events = new List<Entities.EventModel>();
                EventDb.Events[UserId] = Events;
            }
            var index = 0;
            if (Events.Count > 0)
            {
                index = Events.Max(t => t.Id) + 1;
            }

            var NewEvent = new Entities.EventModel
            {
                Id = index,
                IsDone = Event.IsDone,
                Title = Event.Title,
                Description = Event.Description,
                Deadline  = Event.Deadline,
            };

            EventDb.Events[UserId].Add(NewEvent);
            return Ok(NewEvent);
        }

        // PUT api/Events/5
        [HttpPut("update/{UserId}/{id}")]
        public ActionResult<Entities.EventModel> Put(int UserId, int id, EventCreate EventUpdate)
        {
            List<Entities.EventModel>? Events;
            if (!EventDb.Events.TryGetValue(UserId, out Events) || Events == null)
            {
                Events = new List<EventModel>();
                EventDb.Events[UserId] = Events;
            }
            var Event = Events.Find(t => t.Id == id);
            if (Event == null)
            {
                return NotFound();
            }
            Event.Title = EventUpdate.Title;
            Event.Description = EventUpdate.Description;
            Event.IsDone = EventUpdate.IsDone;
            Event.Deadline = EventUpdate.Deadline;

            return Ok(Event);
        }

        // DELETE api/Events/5
        [HttpDelete("delete/{UserId}/{id}")]
        public ActionResult<bool> Delete(int UserId, int id)
        {
            List<Entities.EventModel>? Events;
            if (!EventDb.Events.TryGetValue(UserId, out Events) || Events == null)
            {
                Events = new List<EventModel>();
                EventDb.Events[UserId] = Events;
            }
            var index = Events.FindIndex(t => t.Id == id);
            if (index == -1)
            {
                return NotFound();
            }
            Events.RemoveAt(index);
            return Ok(true);
        }
    }
}