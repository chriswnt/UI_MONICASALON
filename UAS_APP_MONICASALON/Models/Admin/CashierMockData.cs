using UAS_APP_MONICASALON.Icons;

namespace UAS_APP_MONICASALON.Models.Admin;

/// <summary>
/// UI-only cashier data: the service/product catalogue and a seeded transaction
/// history. Dates are relative to today so the history filters always have
/// something to show. No backend calls anywhere in this module.
/// </summary>
public static class CashierMockData
{
    public const string CategoryAll = "Semua";
    public const string CategoryHair = "Potong & Styling";
    public const string CategoryCare = "Perawatan";
    public const string CategoryChemical = "Kimia";
    public const string CategoryProduct = "Produk";

    /// <summary>Discount codes accepted by the review screen (UI demo only).</summary>
    public static IReadOnlyDictionary<string, decimal> DiscountCodes { get; } =
        new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
        {
            ["MONICA10"] = 0.10m,
            ["HEMAT20"] = 0.20m,
            ["NEWCUST15"] = 0.15m
        };

    public static string DiscountHintText => "Kode demo: MONICA10, HEMAT20, NEWCUST15";

    public static IReadOnlyList<CashierService> Services { get; } = CreateServices();

    public static IReadOnlyList<CashierCategory> CreateCategories() => new List<CashierCategory>
    {
        new() { Name = CategoryAll, IconGlyph = AdminIcons.Category, IsSelected = true },
        new() { Name = CategoryHair, IconGlyph = AdminIcons.ServiceCut },
        new() { Name = CategoryCare, IconGlyph = AdminIcons.ServiceSpa },
        new() { Name = CategoryChemical, IconGlyph = AdminIcons.ServiceSmoothing },
        new() { Name = CategoryProduct, IconGlyph = AdminIcons.Products }
    };

    public static CashierService? FindService(string name) =>
        Services.FirstOrDefault(service =>
            string.Equals(service.Name, name, StringComparison.OrdinalIgnoreCase));

    private static List<CashierService> CreateServices() => new()
    {
        new CashierService
        {
            Name = "Potong Rambut",
            Category = CategoryHair,
            Price = 50000,
            DurationMinutes = 45,
            IconGlyph = AdminIcons.ServiceCut
        },
        new CashierService
        {
            Name = "Creambath",
            Category = CategoryCare,
            Price = 80000,
            DurationMinutes = 45,
            IconGlyph = AdminIcons.ServiceSpa
        },
        new CashierService
        {
            Name = "Facial",
            Category = CategoryCare,
            Price = 120000,
            DurationMinutes = 60,
            IconGlyph = AdminIcons.ServiceFacial
        },
        new CashierService
        {
            Name = "Smoothing",
            Category = CategoryChemical,
            Price = 250000,
            DurationMinutes = 150,
            IconGlyph = AdminIcons.ServiceSmoothing
        },
        new CashierService
        {
            Name = "Pewarnaan Rambut",
            Category = CategoryChemical,
            Price = 200000,
            DurationMinutes = 150,
            IconGlyph = AdminIcons.ServiceColor
        },
        new CashierService
        {
            Name = "Rebonding",
            Category = CategoryChemical,
            Price = 300000,
            DurationMinutes = 210,
            IconGlyph = AdminIcons.ServiceRebonding
        },
        new CashierService
        {
            Name = "Hair Tonic",
            Category = CategoryProduct,
            Price = 45000,
            IconGlyph = AdminIcons.Redeem
        },
        new CashierService
        {
            Name = "Sampo Premium",
            Category = CategoryProduct,
            Price = 65000,
            IconGlyph = AdminIcons.Bag
        },
        new CashierService
        {
            Name = "Hair Mask",
            Category = CategoryProduct,
            Price = 90000,
            IconGlyph = AdminIcons.Sell
        },
        new CashierService
        {
            Name = "Vitamin Rambut",
            Category = CategoryProduct,
            Price = 55000,
            IconGlyph = AdminIcons.Tag
        }
    };

    public static IReadOnlyList<CashierTransaction> CreateTransactions()
    {
        var today = DateTime.Today;

        var transactions = new List<CashierTransaction>
        {
            Build("TRX-1001", today.AddHours(9).AddMinutes(15), "Bella Anisa", "0812 3456 7890", "Dewi",
                PaymentMethod.Cash, PaymentStatus.Paid, 100000m,
                ("Potong Rambut", 1), ("Hair Tonic", 1)),

            Build("TRX-1002", today.AddHours(10).AddMinutes(5), "Siti Rahma", "0813 2345 6789", "Sari",
                PaymentMethod.Qris, PaymentStatus.Paid, 80000m,
                ("Creambath", 2)),

            Build("TRX-1003", today.AddHours(11).AddMinutes(40), "Nadia Putri", "0857 1122 3344", "Rina",
                PaymentMethod.DebitCard, PaymentStatus.Paid, 250000m,
                ("Smoothing", 1)),

            Build("TRX-1004", today.AddHours(13).AddMinutes(20), "Ayu Lestari", "0821 5566 7788", "Nadia",
                PaymentMethod.Cash, PaymentStatus.Paid, 150000m,
                ("Facial", 1), ("Vitamin Rambut", 1)),

            Build("TRX-1005", today.AddHours(14).AddMinutes(50), "Kirana Dewi", "0899 8877 6655", "Rina",
                PaymentMethod.EWallet, PaymentStatus.Paid, 250000m,
                ("Smoothing", 1)),

            Build("TRX-1006", today.AddHours(16).AddMinutes(10), "Maya Sari", "0838 4455 6677", "Sari",
                PaymentMethod.Qris, PaymentStatus.Paid, 200000m,
                ("Pewarnaan Rambut", 1)),

            Build("TRX-1007", today.AddDays(-1).AddHours(10).AddMinutes(30), "Citra Dewi", "0813 5566 2211", "Sari",
                PaymentMethod.Cash, PaymentStatus.Paid, 165000m,
                ("Facial", 1), ("Hair Mask", 1)),

            Build("TRX-1008", today.AddDays(-1).AddHours(15), "Lina Marlina", "0857 8899 1122", "Rina",
                PaymentMethod.CreditCard, PaymentStatus.Refunded, 250000m,
                ("Smoothing", 1)),

            Build("TRX-1009", today.AddDays(-2).AddHours(11).AddMinutes(45), "Putri Handayani", "0812 1122 3344", "Dewi",
                PaymentMethod.Cash, PaymentStatus.Paid, 100000m,
                ("Potong Rambut", 2)),

            Build("TRX-1010", today.AddDays(-2).AddHours(14).AddMinutes(20), "Rani Oktaviani", "0838 2233 4455", "Sari",
                PaymentMethod.DebitCard, PaymentStatus.Paid, 80000m,
                ("Creambath", 1)),

            Build("TRX-1011", today.AddDays(-3).AddHours(13), "Tika Lestari", "0821 3344 5566", "Nadia",
                PaymentMethod.Qris, PaymentStatus.Paid, 365000m,
                ("Rebonding", 1), ("Sampo Premium", 1)),

            Build("TRX-1012", today.AddDays(-4).AddHours(16).AddMinutes(15), "Wulan Sari", "0899 4455 6677", "Sari",
                PaymentMethod.Cash, PaymentStatus.Paid, 255000m,
                ("Pewarnaan Rambut", 1), ("Hair Tonic", 1))
        };

        return transactions
            .OrderByDescending(transaction => transaction.CreatedAt)
            .ToList();
    }

    private static CashierTransaction Build(
        string id,
        DateTime createdAt,
        string customerName,
        string customerPhone,
        string staffName,
        PaymentMethod method,
        PaymentStatus status,
        decimal amountPaid,
        params (string ServiceName, int Quantity)[] items)
    {
        var lines = items
            .Select(item => new CartLine(
                FindService(item.ServiceName) ?? new CashierService { Name = item.ServiceName },
                item.Quantity))
            .ToList();

        var subtotal = CashierPricing.Subtotal(lines);
        var tax = CashierPricing.Tax(subtotal);

        return new CashierTransaction
        {
            Id = id,
            CreatedAt = createdAt,
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            StaffName = staffName,
            CashierName = "Admin Monica",
            Lines = lines,
            Subtotal = subtotal,
            DiscountAmount = 0,
            TaxAmount = tax,
            Total = subtotal + tax,
            AmountPaid = amountPaid,
            Method = method,
            ReferenceCode = method == PaymentMethod.Cash ? string.Empty : $"{id}-REF",
            Status = status
        };
    }
}
