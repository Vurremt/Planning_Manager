namespace EventService.Entities
{
    public class GroupModel
    {
        public int Id { get; set; }
        public required string Titre { get; set; }
        public string? Description { get; set; }
        public List<int> ManagerIds { get; set; } = new();
        public List<int> SubscriberIds { get; set; } = new();
    }

public class GroupCreate
    {
        public required string Titre { get; set; }
        public string? Description { get; set; }
        public required List<int> ManagerIds { get; set; }
        public List<int>? SubscriberIds { get; set; }

    }
}
