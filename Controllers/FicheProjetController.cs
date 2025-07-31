using Microsoft.AspNetCore.Mvc;
using WebApplication6.Models;
using WebApplication6.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace WebApplication6.Controllers
{
    public class FicheProjetController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FicheProjetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FicheProjet
        public IActionResult Index(string searchNumero, string searchDate)
        {
            var projets = _context.FicheProjets.AsQueryable();
            
            if (!string.IsNullOrEmpty(searchNumero))
            {
                projets = projets.Where(p => p.NumeroProjet != null && p.NumeroProjet.Contains(searchNumero));
            }
            
            if (!string.IsNullOrEmpty(searchDate))
            {
                if (DateTime.TryParse(searchDate, out DateTime date))
                {
                    projets = projets.Where(p => p.DernierDelaiLivraison.HasValue && 
                                                p.DernierDelaiLivraison.Value.Date == date.Date);
                }
            }
            
            ViewBag.SearchNumero = searchNumero;
            ViewBag.SearchDate = searchDate;
            
            return View(projets.ToList());
        }

        // GET: FicheProjet/Create
        public IActionResult Create()
        {
            var ficheProjet = new FicheProjet
            {
                Detail = new FicheDetail()
            };
            return View(ficheProjet);
        }

        // POST: FicheProjet/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FicheProjet ficheProjet)
        {
            if (ModelState.IsValid)
            {
                _context.FicheProjets.Add(ficheProjet);
                _context.SaveChanges();

                // Ajout automatique d'une FicheMetaDonnee
                var meta = new FicheMetaDonnee
                {
                    FicheProjetId = ficheProjet.Id,
                    CreePar = User.Identity?.Name ?? "system",
                    AdresseIP = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    SessionMachine = HttpContext.Session.Id,
                    DateCreation = DateTime.Now
                };
                _context.FicheMetaDonnees.Add(meta);
                _context.SaveChanges();

                if (ficheProjet.Detail != null)
                {
                    ficheProjet.Detail.FicheProjetId = ficheProjet.Id;
                    ficheProjet.Detail.FicheProjet = null;
                    _context.FicheDetails.Add(ficheProjet.Detail);
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            
            // Afficher les erreurs de validation spécifiques
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            if (errors.Any())
            {
                TempData["Error"] = "Erreurs de validation : " + string.Join(", ", errors);
            }
            else
            {
                TempData["Error"] = "Erreur de validation côté serveur.";
            }
            
            return View(ficheProjet);
        }

        // GET: FicheProjet/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            var ficheProjet = _context.FicheProjets
                .Include(f => f.Detail)
                .Include(f => f.FicheMetaDonnees)
                .Include(f => f.FicheModifications)
                .FirstOrDefault(f => f.Id == id);
            if (ficheProjet == null) return NotFound();
            return View(ficheProjet);
        }

        // GET: FicheProjet/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var ficheProjet = _context.FicheProjets
                .Include(f => f.Detail)
                .FirstOrDefault(f => f.Id == id);
            if (ficheProjet == null) return NotFound();
            return View(ficheProjet);
        }

        // POST: FicheProjet/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, FicheProjet ficheProjet)
        {
            System.Diagnostics.Debug.WriteLine($"Projet: {ficheProjet.NumeroProjet}, Détail: {ficheProjet.Detail?.Equipements}");
            if (id != ficheProjet.Id) return NotFound();
            if (ModelState.IsValid)
            {
                // Charger l'entité existante
                var existingProjet = _context.FicheProjets
                    .Include(f => f.Detail)
                    .FirstOrDefault(f => f.Id == id);

                if (existingProjet == null) return NotFound();

                // Mettre à jour les propriétés du projet
                existingProjet.NumeroProjet = ficheProjet.NumeroProjet;
                existingProjet.NumeroAffaire = ficheProjet.NumeroAffaire;
                existingProjet.ContratNumero = ficheProjet.ContratNumero;
                existingProjet.NomClient = ficheProjet.NomClient;
                existingProjet.TelephoneContact = ficheProjet.TelephoneContact;
                existingProjet.AdresseLivraison = ficheProjet.AdresseLivraison;
                existingProjet.ModalitePaiement = ficheProjet.ModalitePaiement;
                existingProjet.DernierDelaiLivraison = ficheProjet.DernierDelaiLivraison;
                existingProjet.DernierDelaiExecution = ficheProjet.DernierDelaiExecution;
                existingProjet.DureeGarantie = ficheProjet.DureeGarantie;
                existingProjet.DetailFormation = ficheProjet.DetailFormation;
                existingProjet.DossierAMSSNUR = ficheProjet.DossierAMSSNUR;
                existingProjet.ContratMaintenance = ficheProjet.ContratMaintenance;

                // Mettre à jour le détail
                if (existingProjet.Detail != null && ficheProjet.Detail != null)
                {
                    existingProjet.Detail.Equipements = ficheProjet.Detail.Equipements;
                    existingProjet.Detail.Accessoires = ficheProjet.Detail.Accessoires;
                    existingProjet.Detail.TravauxPromamec = ficheProjet.Detail.TravauxPromamec;
                    existingProjet.Detail.Commentaires = ficheProjet.Detail.Commentaires;
                }

                _context.SaveChanges();

                // Ajout automatique d'une FicheModification
                var modif = new FicheModification
                {
                    FicheProjetId = ficheProjet.Id,
                    ChampsModifie = "Modification fiche projet",
                    AncienneValeur = "-",
                    NouvelleValeur = "-",
                    DateModification = DateTime.Now
                };
                _context.FicheModifications.Add(modif);
                _context.SaveChanges();

                TempData["Success"] = "La fiche projet a été modifiée avec succès.";
                return RedirectToAction(nameof(Index));
            }
            return View(ficheProjet);
        }

        // GET: FicheProjet/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var ficheProjet = _context.FicheProjets.FirstOrDefault(f => f.Id == id);
            if (ficheProjet == null) return NotFound();
            return View(ficheProjet);
        }

        // POST: FicheProjet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var ficheProjet = _context.FicheProjets.FirstOrDefault(f => f.Id == id);
            if (ficheProjet != null)
            {
                _context.FicheProjets.Remove(ficheProjet);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: FicheProjet/TestDbConnection
        public IActionResult TestDbConnection()
        {
            string message;
            try
            {
                var conn = _context.Database.GetDbConnection();
                conn.Open();
                message = "Connexion à la base de données réussie !";
                conn.Close();
            }
            catch (Exception ex)
            {
                message = $"Erreur de connexion : {ex.Message}";
            }
            ViewBag.DbMessage = message;
            return View();
        }
    }
} 