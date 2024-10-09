namespace GatewayService.Entities
{
    public class TaskModel
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public string? Description { get; set; }
        public bool IsDone { get; set; }

        public DateTime Deadline { get; set; }

    }
    public class TaskCreateModel
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public bool IsDone { get; set; }
        public DateTime Deadline { get; set; }
    }
}