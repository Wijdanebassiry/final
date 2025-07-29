using Microsoft.AspNetCore.Mvc;
using WebApplication6.Models;
using WebApplication6.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;

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
        public IActionResult Index()
        {
            var projets = _context.FicheProjets.ToList();
            return View(projets);
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
        public IActionResult Create(FicheProjet ficheProjet, IFormFile FichierExcel)
        {
            if (ModelState.IsValid)
            {
                // Si un fichier Excel est uploadé, le convertir en HTML et stocker dans Equipements
                if (FichierExcel != null && FichierExcel.Length > 0)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"Fichier Excel uploadé: {FichierExcel.FileName}, Taille: {FichierExcel.Length}");
                        
                        // Vérifier l'extension du fichier
                        var extension = Path.GetExtension(FichierExcel.FileName)?.ToLower() ?? "";
                        System.Diagnostics.Debug.WriteLine($"Extension du fichier: {extension}");
                        
                        // Si c'est un fichier CSV, le traiter différemment
                        if (extension == ".csv")
                        {
                            using (var reader = new StreamReader(FichierExcel.OpenReadStream()))
                            {
                                var sb = new System.Text.StringBuilder();
                                sb.Append("<table border='1' style='border-collapse:collapse;width:100%'>");
                                string? line;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    sb.Append("<tr>");
                                    var columns = line?.Split(',') ?? new string[0];
                                    foreach (var column in columns)
                                    {
                                        sb.Append($"<td>{System.Net.WebUtility.HtmlEncode(column?.Trim() ?? "")}</td>");
                                    }
                                    sb.Append("</tr>");
                                }
                                sb.Append("</table>");
                                var htmlContent = sb.ToString();
                                System.Diagnostics.Debug.WriteLine($"HTML généré (CSV): {htmlContent}");
                                if (ficheProjet.Detail == null)
                                    ficheProjet.Detail = new WebApplication6.Models.FicheDetail();
                                ficheProjet.Detail.Equipements = htmlContent;
                            }
                        }
                        else
                        {
                            // Traitement Excel
                            using (var package = new OfficeOpenXml.ExcelPackage(FichierExcel.OpenReadStream()))
                            {
                                var worksheet = package.Workbook.Worksheets[0];
                                System.Diagnostics.Debug.WriteLine($"Feuille trouvée: {worksheet.Name}, Dimensions: {worksheet.Dimension}");
                                
                                if (worksheet.Dimension != null)
                                {
                                    var sb = new System.Text.StringBuilder();
                                    sb.Append("<table border='1' style='border-collapse:collapse;width:100%'>");
                                    for (int row = worksheet.Dimension.Start.Row; row <= worksheet.Dimension.End.Row; row++)
                                    {
                                        sb.Append("<tr>");
                                        for (int col = worksheet.Dimension.Start.Column; col <= worksheet.Dimension.End.Column; col++)
                                        {
                                            var value = "";
                                            try
                                            {
                                                var cell = worksheet.Cells[row, col];
                                                value = cell?.Text ?? "";
                                            }
                                            catch
                                            {
                                                value = "";
                                            }
                                            System.Diagnostics.Debug.WriteLine($"Cellule [{row},{col}]: '{value}'");
                                            sb.Append($"<td>{System.Net.WebUtility.HtmlEncode(value)}</td>");
                                        }
                                        sb.Append("</tr>");
                                    }
                                    sb.Append("</table>");
                                    var htmlContent = sb.ToString();
                                    System.Diagnostics.Debug.WriteLine($"HTML généré: {htmlContent}");
                                    if (ficheProjet.Detail == null)
                                        ficheProjet.Detail = new WebApplication6.Models.FicheDetail();
                                    ficheProjet.Detail.Equipements = htmlContent;
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("Feuille Excel vide ou sans données");
                                }
                            }
                        }
                    }
                    catch (Exception ex) 
                    { 
                        System.Diagnostics.Debug.WriteLine($"Erreur lecture Excel/CSV: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Type d'erreur: {ex.GetType().Name}");
                        
                        // Message d'erreur pour l'utilisateur
                        TempData["Error"] = $"Erreur lors de la lecture du fichier: {ex.Message}. Assurez-vous que le fichier n'est pas protégé par mot de passe.";
                        /* ignore, fallback to champ texte */ 
                    }
                }
                // Si pas de fichier Excel, garder le texte manuel dans Equipements
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

                // Gestion de l'upload du fichier Excel
                if (FichierExcel != null && FichierExcel.Length > 0)
                {
                    var uploadsDir = Path.Combine("wwwroot", "uploads");
                    if (!Directory.Exists(uploadsDir))
                        Directory.CreateDirectory(uploadsDir);
                    var filePath = Path.Combine(uploadsDir, Path.GetFileName(FichierExcel.FileName));
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        FichierExcel.CopyTo(stream);
                    }
                    // Optionnel : stocker le nom ou chemin du fichier dans la base
                }

                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Erreur de validation côté serveur.";
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