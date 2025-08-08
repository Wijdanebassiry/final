using WebApplication6.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApplication6.Services
{
    public interface INumeroProjetService
    {
        Task<string> GenererNumeroProjetAsync();
        Task<string> GenererNumeroProjetFormatAsync(string format = "PRJ-{YYYY}-{NNNN}");
        Task<bool> EstNumeroUniqueAsync(string numeroProjet);
        Task<string> GenererNumeroUniqueAsync();
    }

    public class NumeroProjetService : INumeroProjetService
    {
        private readonly ApplicationDbContext _context;

        public NumeroProjetService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenererNumeroProjetAsync()
        {
            return await GenererNumeroProjetFormatAsync("PRJ-{YYYY}-{NNNN}");
        }

        public async Task<string> GenererNumeroProjetFormatAsync(string format = "PRJ-{YYYY}-{NNNN}")
        {
            var annee = DateTime.Now.Year;
            
            // Trouver le dernier numéro de projet pour cette année
            var dernierNumero = await _context.FicheProjets
                .Where(p => p.NumeroProjet != null && p.NumeroProjet.Contains(annee.ToString()))
                .OrderByDescending(p => p.NumeroProjet)
                .Select(p => p.NumeroProjet)
                .FirstOrDefaultAsync();

            int compteur = 1;
            
            if (!string.IsNullOrEmpty(dernierNumero))
            {
                // Extraire le numéro séquentiel du dernier numéro
                var parties = dernierNumero.Split('-');
                if (parties.Length >= 3 && int.TryParse(parties[2], out int compteurExtrait))
                {
                    compteur = compteurExtrait + 1;
                }
                else
                {
                    // Si le format ne correspond pas, essayer d'extraire le dernier numéro
                    var match = System.Text.RegularExpressions.Regex.Match(dernierNumero, @"(\d{4})$");
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int compteurRegex))
                    {
                        compteur = compteurRegex + 1;
                    }
                }
            }

            // Générer le nouveau numéro
            var nouveauNumero = format
                .Replace("{YYYY}", annee.ToString())
                .Replace("{YY}", annee.ToString().Substring(2))
                .Replace("{MM}", DateTime.Now.Month.ToString("D2"))
                .Replace("{DD}", DateTime.Now.Day.ToString("D2"))
                .Replace("{NNNN}", compteur.ToString("D4"))
                .Replace("{NNN}", compteur.ToString("D3"))
                .Replace("{NN}", compteur.ToString("D2"));

            return nouveauNumero;
        }

        public async Task<bool> EstNumeroUniqueAsync(string numeroProjet)
        {
            if (string.IsNullOrEmpty(numeroProjet))
                return false;

            return !await _context.FicheProjets
                .AnyAsync(p => p.NumeroProjet == numeroProjet);
        }

        public async Task<string> GenererNumeroUniqueAsync()
        {
            string numeroProjet;
            int tentatives = 0;
            const int maxTentatives = 10;

            do
            {
                numeroProjet = await GenererNumeroProjetAsync();
                tentatives++;
                
                if (tentatives > maxTentatives)
                {
                    // Si trop de tentatives, ajouter un timestamp pour garantir l'unicité
                    numeroProjet = $"{numeroProjet}-{DateTime.Now:HHmmss}";
                    break;
                }
            } while (!await EstNumeroUniqueAsync(numeroProjet));

            return numeroProjet;
        }
    }
} 