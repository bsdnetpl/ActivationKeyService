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
        public async Task<ActivationKey> GenerateActivationKey(string productName, [Service] AppDbContext dbContext)
            {
            string activationKey = GenerateKey();

            using var rsa = RSA.Create();

            // Wczytanie zawartości pliku private.pem
            string privateKeyPem = File.ReadAllText("KEY/private.pem");

            // Import klucza, sprawdzając poprawność
            try
                {
                rsa.ImportFromPem(privateKeyPem);
                }
            catch (ArgumentException)
                {
                throw new Exception("Błąd: Nieprawidłowy format klucza RSA w private.pem.");
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

            try
                {
                dbContext.ActivationKeys.Add(newKey);
                await dbContext.SaveChangesAsync();
                }
            catch (DbUpdateException ex)
                {
                Console.WriteLine("Błąd zapisu do bazy danych: " + ex.InnerException?.Message);
                throw;
                }

            return newKey;
            }

        /// <summary>
        /// Generuje 25-znakowy unikalny klucz aktywacyjny w formacie XXXXX-XXXXX-XXXXX-XXXXX-XXXXX
        /// </summary>
        private string GenerateKey()
            {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
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
