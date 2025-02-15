#  Activation Key Service

> System do generowania i walidacji kluczy aktywacyjnych z wykorzystaniem **GraphQL**, **RSA** oraz **Entity Framework Core**.

##  Funkcjonalności

 **Generowanie kluczy aktywacyjnych**  
 **Przechowywanie kluczy w bazie SQLite**  
 **Walidacja kluczy aktywacyjnych poprzez podpisy RSA**  
 **Obsługa GraphQL dla pobierania i sprawdzania kluczy**  

---

## ⚙️ Wymagania

 **Technologie używane w projekcie:**
- [.NET 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [GraphQL (HotChocolate)](https://chillicream.com/docs/hotchocolate)
- [Entity Framework Core 9.0](https://docs.microsoft.com/en-us/ef/core/)
- [SQLite](https://www.sqlite.org/)
- [OpenSSL](https://www.openssl.org/) (do generowania kluczy RSA)

---

## 🛠 Instalacja

### **1️ Sklonuj repozytorium**
```sh
git clone https://github.com/twoj-uzytkownik/ActivationKeyService.git
cd ActivationKeyService
