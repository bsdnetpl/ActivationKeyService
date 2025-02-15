using ActivationKeyService.Models;
using HotChocolate;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ActivationKeyService.Query
    {
    public class Query
        {
        /// <summary>
        /// Pobiera wszystkie klucze aktywacyjne z bazy danych
        /// </summary>
        [GraphQLName("getActivationKeys")]
        public IQueryable<ActivationKey> GetActivationKeys([Service] AppDbContext dbContext)
            {
            return dbContext.ActivationKeys;
            }

        /// <summary>
        /// Sprawdza poprawność klucza aktywacyjnego poprzez weryfikację podpisu RSA
        /// </summary>
        [GraphQLName("validateActivationKey")]
        public bool ValidateActivationKey(string key, [Service] AppDbContext dbContext)
            {
            var record = dbContext.ActivationKeys.FirstOrDefault(k => k.Key == key);
            if (record == null)
                {
                Console.WriteLine("Błąd: Klucz aktywacyjny nie istnieje w bazie.");
                return false;
                }

            try
                {
                // Wczytaj klucz publiczny z katalogu KEY
                string publicKeyPath = System.IO.Path.Combine("KEY", "public.pem");

                if (!File.Exists(publicKeyPath))
                    {
                    Console.WriteLine($"Błąd: Plik klucza {publicKeyPath} nie istnieje.");
                    return false;
                    }

                string publicKeyPem = File.ReadAllText(publicKeyPath).Trim();
                using var rsa = RSA.Create();
                rsa.ImportFromPem(publicKeyPem);

                byte[] keyBytes = Encoding.UTF8.GetBytes(record.Key);
                byte[] signature = Convert.FromBase64String(record.Signature); // Pobieramy rzeczywistą sygnaturę z bazy

                return rsa.VerifyData(
                    keyBytes,
                    signature,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1
                );
                }
            catch (ArgumentException ex)
                {
                Console.WriteLine("Błąd: Format klucza publicznego jest niepoprawny.");
                Console.WriteLine(ex.Message);
                return false;
                }
            catch (Exception ex)
                {
                Console.WriteLine($"Błąd walidacji klucza: {ex.Message}");
                return false;
                }
            }
        }
    }
