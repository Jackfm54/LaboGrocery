LaboGrocery

Application ASP.NET Core MVC (.NET 8) avec Entity Framework Core, Identity, localisation (fr-FR / en-US) et SQLite.
Exemple de boutique en ligne : catalogue, panier, commande, et zone d’administration (produits, catégories, langues).

✨ Fonctionnalités

🛒 Catalogue de produits avec catégories et recherche simple

🧺 Panier stocké en session (ajout / retrait / vider)

🧾 Passage de commande (création d’une commande et de ses lignes)

🔐 Authentification et rôles (ASP.NET Core Identity)

🌍 Localisation fr-FR et en-US (Razor + DataAnnotations, ressources dans Resources/)

🎨 UI moderne (Bootstrap 5 + Bootswatch Pulse + Bootstrap Icons)

🖼️ Images produits (dossier wwwroot/images/products ou URL)

⚙️ Données d’exemple au démarrage (seed) via SeedData.InitializeAsync(...)

🧱 Pile technique

ASP.NET Core 8 MVC + Razor Pages (UI d’Identity)

EF Core (SQLite par défaut)

ASP.NET Core Identity (utilisateurs + rôles)

Bootstrap 5 / Bootswatch / Bootstrap Icons

✅ Prérequis

.NET SDK 8.x (ou 9.x compatible)

Certificat HTTPS dev approuvé :

dotnet dev-certs https --trust

🚀 Démarrage rapide
dotnet restore
dotnet run --urls "https://localhost:7123;http://localhost:5123"


Application : https://localhost:7123

Mode Development (optionnel) :

set ASPNETCORE_ENVIRONMENT=Development


Au premier lancement, la base SQLite est créée et les données de démonstration sont injectées (EnsureCreated + Seed).

🔧 Configuration
Connexion SQLite (par défaut)

appsettings.json :

{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  }
}

Program.cs (extrait pertinent)

Enregistrement du DbContext + Identity

Localisation fr-FR par défaut et ressources dans Resources/

Seed des données au démarrage

Si tu changes le fournisseur (ex. SQL Server), adapte UseSqlite(...) → UseSqlServer(...) et la chaîne de connexion.

🗂️ Structure (extrait)
LaboGrocery/
├─ Controllers/
│  ├─ HomeController.cs
│  ├─ ProductsController.cs
│  └─ CartController.cs, OrdersController.cs
├─ Areas/
│  └─ Admin/
│     ├─ Controllers/ (Products, Categories, Languages)
│     └─ Views/
├─ Data/
│  ├─ ApplicationDbContext.cs
│  └─ SeedData.cs
├─ Models/
│  ├─ Product.cs, Category.cs, Language.cs
│  └─ Order.cs, OrderItem.cs
├─ Resources/
│  └─ SharedResource.fr.resx, SharedResource.en.resx, ...
├─ Services/
│  ├─ CartService.cs
│  └─ PayPalService.cs (facultatif / stub)
├─ Views/
│  ├─ Shared/_Layout.cshtml, _LoginPartial.cshtml
│  ├─ Home/ , Products/ , Cart/ , Orders/
│  └─ ...
├─ wwwroot/
│  ├─ css/site.css
│  └─ images/products/ (tes images)
└─ appsettings.json

🖼️ Images produits

Place tes images dans wwwroot/images/products/ (ex. banana.jpg).

Dans la vue, affiche ImageUrl si disponible, sinon une image par défaut :

<img class="product-img"
     src="@(string.IsNullOrWhiteSpace(item.ImageUrl)
            ? Url.Content("~/images/products/default.png")
            : item.ImageUrl)"
     alt="@item.Name" />


Astuce : ajoute wwwroot/images/products/default.png pour les produits sans image.

🌐 Localisation (UI + DataAnnotations)

Ressources dans Resources/ (ex. SharedResource.fr.resx, SharedResource.en.resx)

Injection dans les vues :

@inject Microsoft.Extensions.Localization.IStringLocalizer<LaboGrocery.SharedResource> L
<a class="nav-link" asp-controller="Products" asp-action="Index">@L["Products"]</a>


Sélecteur de langue : via QueryString et Cookie (gérés dans Program.cs).

🧪 Données de démo / comptes

Le seed crée des catégories, produits, langues et (optionnel) un compte admin si SeedData le prévoit.

Si besoin, adapte SeedData.InitializeAsync(...) pour définir un admin :

const string adminEmail = "admin@local";
const string adminPassword = "P@ssw0rd!";

🛠️ Commandes utiles

Nettoyer artefacts :

rmdir /s /q bin obj


Régénérer dépendances :

dotnet restore


Lancer en HTTPS + HTTP :

dotnet run --urls "https://localhost:7123;http://localhost:5123"

🐞 Dépannage (FAQ)

Fichiers verrouillés (MSB3026/MSB3021)
Un LaboGrocery.exe tourne encore. Ferme l’appli/terminal/VS. Ensuite :

dotnet build-server shutdown


Puis supprime bin/ et obj/ et relance.

Conflits de versions EF Core (NU1605)
Aligne les versions de tous les packages Microsoft.EntityFrameworkCore.* (ex. tout en 9.0.x).

Erreur UseSqlite non trouvée
Ajoute le package :

dotnet add package Microsoft.EntityFrameworkCore.Sqlite


Certificat HTTPS non approuvé

dotnet dev-certs https --trust

📦 Publication (aperçu)

Kestrel : dotnet publish -c Release puis exécuter l’EXE.

IIS : activer le Hosting Bundle .NET et déployer le dossier publish.

Docker : ajouter un Dockerfile et publier une image.
