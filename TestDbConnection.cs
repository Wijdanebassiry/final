using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

class TestDbConnection
{
    static void Main()
    {
        // Chaîne de connexion à adapter si besoin
        var connectionString = "Server=DESKTOP-183MOE2;Database=WebApplication6Db;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
        using (var connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                Console.WriteLine("Connexion à la base de données réussie !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de connexion : {ex.Message}");
            }
        }
    }
} 