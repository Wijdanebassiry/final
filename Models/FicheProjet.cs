using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication6.Models
{
    public class FicheProjet
    {
        public int Id { get; set; }
        public string? NumeroProjet { get; set; }
        public string? NumeroAffaire { get; set; }
        public string? ContratNumero { get; set; }
        public string? NomClient { get; set; }
        public string? TelephoneContact { get; set; }
        public string? AdresseLivraison { get; set; }
        public string? ModalitePaiement { get; set; }
        public DateTime? DernierDelaiLivraison { get; set; }
        public DateTime? DernierDelaiExecution { get; set; }
        public string? DureeGarantie { get; set; }
        public string? DetailFormation { get; set; }
        public bool DossierAMSSNUR { get; set; }
        public bool ContratMaintenance { get; set; }

        public FicheDetail? Detail { get; set; }
        [InverseProperty("FicheProjet")]
        public List<FicheMetaDonnee> FicheMetaDonnees { get; set; } = new();
        [InverseProperty("FicheProjet")]
        public List<FicheModification> FicheModifications { get; set; } = new();
    }
} 