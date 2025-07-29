using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication6.Models
{
    public class FicheDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Equipements { get; set; }
        public string Accessoires { get; set; }
        public string TravauxPromamec { get; set; }
        public string Commentaires { get; set; }

        public int FicheProjetId { get; set; }
        public FicheProjet? FicheProjet { get; set; }
    }
} 