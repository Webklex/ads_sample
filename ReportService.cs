using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Text.Json;


struct Sale
{
    public DateTime Date { get; set; }
    public double Amount { get; set; }
    public required string Currency { get; set; }
}

public class ReportService
{
    private static double ToUsd(double amount, string currency) =>
        currency.ToUpperInvariant() switch
        {
            "USD" => amount,
            "EUR" => amount * 1.10,
            "GBP" => amount * 1.30,
            _ => throw new NotSupportedException($"Unsupported currency: {currency}")
        };
    
    public void CreateReportForYearAndMonth(int year, int month)
    {
        var allSales = JsonSerializer.Deserialize<List<Sale>>(File.ReadAllText("sales.json"));
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