using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TaskService.Entities;
using TaskService.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {

        private TaskDb TaskDb { get; set; }

        public TaskController(TaskDb taskDb)
        {
            TaskDb = taskDb;
        }

        // GET: api/Tasks/list/:UserId
        [HttpGet("list/{UserId}")]
        public ActionResult<IEnumerable<Entities.TaskModel>> Get(int UserId)
        {
            List<Entities.TaskModel>? tasks;
            if (TaskDb.Tasks.TryGetValue(UserId, out tasks) && tasks != null)
            {
                return tasks;
            }
            else
            {
                TaskDb.Tasks[UserId] = new List<Entities.TaskModel>();
                return Ok(TaskDb.Tasks[UserId]);
            }
        }

        // POST api/Tasks/create
        [HttpPost("create/{UserId}")]
        public ActionResult<Entities.TaskModel> CreateTask(int UserId, TaskCreate task)
        {
            List<Entities.TaskModel>? tasks;
            if (!TaskDb.Tasks.TryGetValue(UserId, out tasks) || tasks == null)
            {
                tasks = new List<Entities.TaskModel>();
                TaskDb.Tasks[UserId] = tasks;
            }
            var index = 0;
            if (tasks.Count > 0)
            {
                index = tasks.Max(t => t.Id) + 1;
            }

            var NewTask = new Entities.TaskModel
            {
                Id = index,
                IsDone = task.IsDone,
                Title = task.Title,
                Description = task.Description,
                Deadline  = task.Deadline,
            };

            TaskDb.Tasks[UserId].Add(NewTask);
            return Ok(NewTask);
        }

        // PUT api/Tasks/5
        [HttpPut("update/{UserId}/{id}")]
        public ActionResult<Entities.TaskModel> Put(int UserId, int id, TaskCreate taskUpdate)
        {
            List<Entities.TaskModel>? tasks;
            if (!TaskDb.Tasks.TryGetValue(UserId, out tasks) || tasks == null)
            {
                tasks = new List<TaskModel>();
                TaskDb.Tasks[UserId] = tasks;
            }
            var task = tasks.Find(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            task.Title = taskUpdate.Title;
            task.Description = taskUpdate.Description;
            task.IsDone = taskUpdate.IsDone;
            task.Deadline = taskUpdate.Deadline;

            return Ok(task);
        }

        // DELETE api/Tasks/5
        [HttpDelete("delete/{UserId}/{id}")]
        public ActionResult<bool> Delete(int UserId, int id)
        {
            List<Entities.TaskModel>? tasks;
            if (!TaskDb.Tasks.TryGetValue(UserId, out tasks) || tasks == null)
            {
                tasks = new List<TaskModel>();
                TaskDb.Tasks[UserId] = tasks;
            }
            var index = tasks.FindIndex(t => t.Id == id);
            if (index == -1)
            {
                return NotFound();
            }
            tasks.RemoveAt(index);
            return Ok(true);
        }
    }
}