namespace MenuApp.Core.Helpers;

public static class HelperMethods
{
    public static string GenerateSlug()
    {
        // Generate a random reference code using a combination of letters and numbers
        const string characters = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
        var random = new Random();
        var trackingCode = new string(Enumerable.Repeat(characters, 5)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        return trackingCode;
    }
}