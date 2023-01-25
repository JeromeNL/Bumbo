using System.Text;

namespace Bumbo.Data.Utils;

public static class CsvHandler
{
    /// <summary>
    /// Creates a string usable for CSV from a IEnumerable
    /// </summary>
    /// <param name="listToBeExported"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>A string usable for a CSV file</returns>
    public static string ListToCsv<T>(IEnumerable<T> listToBeExported)
    {
        var csvBuilder = new StringBuilder();

        var type = typeof(T);
        var propertyInfos = type.GetProperties();
        csvBuilder.Append(string.Join(",", propertyInfos.Select(p => p.Name)));
        csvBuilder.Append(Environment.NewLine);

        foreach (var element in listToBeExported)
        {
            csvBuilder.Append(string.Join(",", propertyInfos.Select(p => p.GetValue(element, null))));
            csvBuilder.Append(Environment.NewLine);
        }

        return csvBuilder.ToString();
    }
}