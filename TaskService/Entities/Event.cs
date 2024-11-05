namespace EventService.Entities
{
    public class EventModel
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Place { get; set; }
        public string? Description { get; set; }
        public DateTime StartingDate { get; set; }
        public uint Duration { get; set; } // En heure
        public int GroupId { get; set; }
    }
    public class EventCreateModel
    {
        public required string Title { get; set; }
        public required string Place { get; set; }
        public string? Description { get; set; }
        public DateTime StartingDate { get; set; }
        public uint Duration { get; set; } // En heure
        public int GroupId { get; set; }
    }
}
