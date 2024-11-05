namespace EventService.Entities
{
    public class GroupModel
    {
        public int Id { get; set; }
        public required string Titre { get; set; }
        public string? Description { get; set; }
        public required List<int> Managers { get; set; }

        public List<int>? Subscribers { get; set; }

    }

    public class GroupCreate
    {
        public required string Titre { get; set; }
        public string? Description { get; set; }
        public required List<int> Managers { get; set; }

        public List<int>? Subscribers { get; set; }

    }
}
