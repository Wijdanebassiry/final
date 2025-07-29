using System;

namespace WebApplication6.Models
{
    public class FicheModification
    {
        public int Id { get; set; }
        public int FicheProjetId { get; set; }
        public string ChampsModifie { get; set; }
        public string AncienneValeur { get; set; }
        public string NouvelleValeur { get; set; }
        public DateTime DateModification { get; set; }

        public FicheProjet FicheProjet { get; set; }
    }
} 