using System.ComponentModel.DataAnnotations;

namespace Kolekcje.Models
{

    /*
    public class Collection
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int CollectionCategoryId { get; set; }

        public required CollectionCategory Category { get; set; }
    }
    */
    //Zmiana wprowadzona 17.05.2025r.
    public class Collection
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tytuł")] // 👈 To ustawia etykietę w formularzu
        public required string Title { get; set; }

        public int CollectionCategoryId { get; set; }

        [Required]
        [Display(Name = "Kategoria")]
        public CollectionCategory? Category { get; set; }
        // public required string Kategoria { get; set; };

        [Display(Name = "Opis")] // 👈 Nowe pole
        public string? Description { get; set; }

        //26.05.2025r. dodaję pole na obraz:

        //public string ObrazSciezka { get; set; } // Ścieżka do zapisanego pliku

        public string? ImagePath { get; set; }


        /*
        // Dodałem 24.05.2025r.
        public virtual ICollection<Pozycja> Pozycje { get; set; }

        
        
        // Można też dodać pomocniczą właściwość:
        public int LiczbaPozycji => Pozycje?.Count ?? 0;
        */
    }


}
