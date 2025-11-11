using System.Globalization;
using System.Text.Json;

/// <summary>
/// Represents a single sale record as read from <c>sales.json</c>.
/// </summary>
/// <remarks>
/// This is a simple DTO used for JSON deserialization. The JSON is expected to contain
/// fields for date, amount, and currency (e.g., "USD", "EUR", "GBP").
/// </remarks>
struct Sale
{
    /// <summary>
    /// The date of the sale.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// The numeric amount of the sale in the specified <see cref="Currency"/>.
    /// </summary>
    public double Amount { get; set; }

    /// <summary>
    /// ISO-like currency code (case-insensitive). Supported: USD, EUR, GBP.
    /// </summary>
    public required string Currency { get; set; }
}

/// <summary>
/// Provides reporting utilities for sales data.
/// </summary>
public class ReportService
{
    /// <summary>
    /// Convert an amount from a given currency into USD.
    /// </summary>
    /// <param name="amount">The monetary value to convert.</param>
    /// <param name="currency">Currency code (USD, EUR, GBP).</param>
    /// <returns>The amount converted to USD.</returns>
    /// <exception cref="NotSupportedException">Thrown if the currency code is not supported.</exception>
    private static double ToUsd(double amount, string currency) =>
        currency.ToUpperInvariant() switch
        {
            "USD" => amount,
            "EUR" => amount * 1.10,
            "GBP" => amount * 1.30,
            _ => throw new NotSupportedException($"Unsupported currency: {currency}")
        };
    
    /// <summary>
    /// Creates a simple monthly text report for the specified year and month.
    /// </summary>
    /// <param name="year">Target calendar year (e.g., 2025).</param>
    /// <param name="month">Target calendar month (1-12).</param>
    /// <remarks>
    /// The method reads sales from <c>sales.json</c> in the current working directory, filters entries
    /// for the given month/year, converts all amounts to USD using static demo rates, sums them up,
    /// and writes a German-language text report to a file named like <c>report_{year}_{month}.txt</c>.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if <c>sales.json</c> could not be parsed.</exception>
    public void CreateReportForYearAndMonth(int year, int month)
    {
        var json = File.ReadAllText("sales.json");
        var allSales = JsonSerializer.Deserialize<List<Sale>>(json)
                       ?? throw new InvalidOperationException("Could not read sales.json");
        var filtered = allSales.Where(s => s.Date.Year == year && s.Date.Month == month);
        var totalUsd = filtered.Sum(s => ToUsd(s.Amount, s.Currency));

        var lines = new List<string>
        {
            $"Monatlicher Verkaufsbericht ({month}/{year})",
            "--------------------------------------------",
            $"Gesamt Umsatz in USD: {totalUsd.ToString("0.00", CultureInfo.InvariantCulture)}"
        };

        File.WriteAllLines($"report_{year}_{month}.txt", lines);
        Console.WriteLine("Report generated successfully.");
    }
}