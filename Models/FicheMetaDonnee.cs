using System;

namespace WebApplication6.Models
{
    public class FicheMetaDonnee
    {
        public int Id { get; set; }
        public int FicheProjetId { get; set; }
        public string CreePar { get; set; }
        public string AdresseIP { get; set; }
        public string SessionMachine { get; set; }
        public DateTime DateCreation { get; set; }

        public FicheProjet FicheProjet { get; set; }
    }
} 