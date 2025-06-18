namespace Kolekcje.Models
{
    public class CollectionCategory
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public ICollection<Collection>? Collections { get; set; }
    }

}
