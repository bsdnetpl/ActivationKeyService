using ActivationKeyService.Models;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ActivationKeyService.Mutation
    {
    public class Mutation
        {
        /// <summary>
        /// Generuje nowy klucz aktywacyjny i zapisuje go w bazie
        /// </summary>
        public async Task<ActivationKey> GenerateActivationKey(string productName, [Service] AppDbContext dbContext)
            {
            string activationKey = GenerateKey();

            using var rsa = RSA.Create();

            // Ścieżka do klucza prywatnego
            string privateKeyPath = System.IO.Path.Combine("KEY", "private.pem");


            // Sprawdzenie, czy plik istnieje
            if (!File.Exists(privateKeyPath))
                {
                throw new Exception($"Błąd: Plik klucza {privateKeyPath} nie istnieje.");
                }

            // Wczytanie klucza prywatnego
            string privateKeyPem;
            try
                {
                privateKeyPem = File.ReadAllText(privateKeyPath).Trim();
                rsa.ImportFromPem(privateKeyPem);
                }
            catch (Exception ex)
                {
                throw new Exception("Błąd: Nieprawidłowy format klucza RSA w private.pem.", ex);
                }

            // Generowanie podpisu
            byte[] keyBytes = Encoding.UTF8.GetBytes(activationKey);
            byte[] signature = rsa.SignData(keyBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            var newKey = new ActivationKey
                {
                Key = activationKey,
                ProductName = productName,
                Signature = Convert.ToBase64String(signature),
                CreatedAt = DateTime.UtcNow
                };

            // Zapisywanie do bazy
            try
                {
                dbContext.ActivationKeys.Add(newKey);
                await dbContext.SaveChangesAsync();
                }
            catch (DbUpdateException ex)
                {
                Console.WriteLine("Błąd zapisu do bazy danych: " + ex.InnerException?.Message);
                throw new Exception("Błąd zapisu klucza do bazy danych.", ex);
                }

            return newKey;
            }

        /// <summary>
        /// Generuje 25-znakowy unikalny klucz aktywacyjny w formacie XXXXX-XXXXX-XXXXX-XXXXX-XXXXX
        /// </summary>
        private string GenerateKey()
            {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random(Guid.NewGuid().GetHashCode()); // Lepsze losowanie
            var sb = new StringBuilder();

            for (int i = 0; i < 25; i++)
                {
                if (i > 0 && i % 5 == 0)
                    sb.Append('-');

                sb.Append(chars[random.Next(chars.Length)]);
                }

            return sb.ToString();
            }
        }
    }
