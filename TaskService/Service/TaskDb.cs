namespace TaskService.Service
{
    public class TaskDb
    {
        public Dictionary<int, List<Entities.TaskModel>> Tasks { get; } = new Dictionary<int, List<Entities.TaskModel>>();

    }
}