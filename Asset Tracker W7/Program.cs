// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

class Asset
{
    public string Type { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public double PriceInDollars { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string OfficeLocation { get; set; }
    public string Currency { get; set; }

    public string DisplayInfo()
    {
        return $"{Type,-15} {Brand,-15} {Model,-15} {OfficeLocation,-15} {PriceInDollars,15:C} {Currency,-5} {PurchaseDate.ToShortDateString()}";
    }
}

class Office
{
    public string Location { get; set; }
    public string Currency { get; set; }
}

class AssetTracker
{
    private List<Asset> assets = new List<Asset>();
    private List<Office> offices = new List<Office>();

    public void AddAsset(Asset asset)
    {
        asset.Currency = GetCurrencyForOffice(asset.OfficeLocation);
        assets.Add(asset);
    }

    public void AddOffice(Office office)
    {
        offices.Add(office);
    }

    public void DisplayAssets()
    {
        if (!assets.Any())
        {
            Console.WriteLine("No assets added yet.");
            return;
        }

        var sortedAssets = assets.OrderBy(a => a.OfficeLocation).ThenBy(a => a.PurchaseDate).ToList();

        Console.WriteLine("\nList of Assets:");

        Console.WriteLine("===================================================================================================================================================");
        Console.WriteLine($"{"Type",-15} {"Brand",-15} {"Model",-15} {"Office Location",-15} {"Price (USD)",-15} {"Price (Currency)",-20} {"Purchase Date"}");
        Console.WriteLine("===================================================================================================================================================");

        foreach (Asset asset in sortedAssets)
        {
            Console.ForegroundColor = GetColorForAsset(asset.PurchaseDate);
            Console.WriteLine($"{asset.Type,-15} {asset.Brand,-15} {asset.Model,-15} {asset.OfficeLocation,-15} {asset.PriceInDollars,15:C} {ConvertToCurrency(asset.PriceInDollars, asset.Currency),-20} {asset.PurchaseDate.ToShortDateString()}");
            Console.ResetColor();
        }

    }

    private ConsoleColor GetColorForAsset(DateTime purchaseDate)
    {
        TimeSpan remainingLifespan = purchaseDate.AddYears(3) - DateTime.Today;

        if (remainingLifespan.TotalDays < 90)
        {
            return ConsoleColor.Red;
        }
        else if (remainingLifespan.TotalDays < 180)
        {
            return ConsoleColor.Yellow;
        }

        return ConsoleColor.White;
    }

    private string GetCurrencyForOffice(string officeLocation)
    {
        Office office = offices.FirstOrDefault(o => o.Location.Equals(officeLocation, StringComparison.OrdinalIgnoreCase));
        return office?.Currency ?? "USD";
    }

    private string ConvertToCurrency(double amount, string currency)
    {
        switch (currency)
        {
            case "USD":
                return $"{amount:C}";
            case "GBP":
                return $"{amount * 0.74:C}"; // GBP to USD conversion rate
            case "SEK":
                return $"{amount * 8.53:C}"; // SEK to USD conversion rate
            case "INR":
                return $"{amount * 74.52:C}"; // INR to USD conversion rate
            default:
                return $"{amount:C}";
        }
    }
}

class Program
{
    static void Main()
    {
        AssetTracker assetTracker = new AssetTracker();

        assetTracker.AddOffice(new Office { Location = "New York", Currency = "USD" });
        assetTracker.AddOffice(new Office { Location = "London", Currency = "GBP" });
        assetTracker.AddOffice(new Office { Location = "Stockholm", Currency = "SEK" });
        assetTracker.AddOffice(new Office { Location = "Mumbai", Currency = "INR" });

        Console.WriteLine("Enter asset details (type 'q' to quit):");

        while (true)
        {
            Console.Write("Enter asset type: ");
            string assetType = Console.ReadLine();

            if (assetType.ToLower() == "q")
                break;

            Console.Write("Enter asset brand: ");
            string assetBrand = Console.ReadLine();

            Console.Write("Enter asset model: ");
            string assetModel = Console.ReadLine();

            Console.Write("Enter asset price in dollars: ");
            double assetPrice;
            while (!double.TryParse(Console.ReadLine(), NumberStyles.Currency, CultureInfo.CurrentCulture, out assetPrice) || assetPrice < 0)
            {
                Console.WriteLine("Invalid input. Please enter a valid price.");
            }

            Console.Write("Enter purchase date (MM/dd/yyyy): ");
            DateTime purchaseDate;
            while (!DateTime.TryParse(Console.ReadLine(), out purchaseDate))
            {
                Console.WriteLine("Invalid date format. Please enter a valid date (MM/dd/yyyy).");
            }

            Console.Write("Enter office location: ");
            string officeLocation = Console.ReadLine();

            Asset newAsset = new Asset { Type = assetType, Brand = assetBrand, Model = assetModel, PriceInDollars = assetPrice, PurchaseDate = purchaseDate, OfficeLocation = officeLocation };
            assetTracker.AddAsset(newAsset);
        }

        assetTracker.DisplayAssets();
    }
}