namespace UAS_APP_MONICASALON.Services;

/// <summary>
/// In-memory customer identity for the UI phase. History and booking APIs use
/// <see cref="CustomerId"/> as their owner key.
/// </summary>
public static class UserSession
{
    public const string DemoCustomerId = "cust-bella";

    public static string CustomerId { get; private set; } = DemoCustomerId;

    public static string FullName { get; private set; } = "Bella Anisa";

    public static string Phone { get; private set; } = "+62 812-3456-7890";

    public static string Email { get; private set; } = "bella@email.com";

    public static bool IsLoggedIn { get; private set; }

    public static string FirstName
    {
        get
        {
            var name = FullName.Trim();
            return string.IsNullOrWhiteSpace(name) ? "Tamu" : name.Split(' ')[0];
        }
    }

    public static string Initials
    {
        get
        {
            var parts = FullName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Take(2)
                .Select(part => char.ToUpperInvariant(part[0]));

            var initials = string.Concat(parts);
            return string.IsNullOrWhiteSpace(initials) ? "MS" : initials;
        }
    }

    public static void SignIn(string identifier)
    {
        IsLoggedIn = true;

        if (identifier.Contains('@', StringComparison.Ordinal))
        {
            Email = identifier.Trim();
        }
        else if (!string.IsNullOrWhiteSpace(identifier))
        {
            Phone = identifier.Trim();
        }
    }

    public static void Register(string fullName, string phone, string email)
    {
        FullName = string.IsNullOrWhiteSpace(fullName) ? "Pelanggan Monica" : fullName.Trim();
        Phone = phone.Trim();
        Email = email.Trim();
        IsLoggedIn = true;
    }

    public static void SignOut()
    {
        IsLoggedIn = false;
        CustomerId = DemoCustomerId;
        FullName = "Bella Anisa";
        Phone = "+62 812-3456-7890";
        Email = "bella@email.com";
    }
}
