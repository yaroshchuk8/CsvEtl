using System.Data;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;

namespace CsvEtl;

public class EtlService
{
    public List<TaxiTrip> ExtractRecords()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            IgnoreBlankLines = true
        };

        using var reader = new StreamReader(Configuration.CsvFilePath);
        using var csv = new CsvReader(reader, config);
        
        var records = csv.GetRecords<TaxiTrip>().ToList();
        Console.WriteLine($"Extracted {records.Count} records.");
        
        return records;
    }

    public void TransformRecords(List<TaxiTrip> records)
    {
        var visited = new HashSet<string>();
        var duplicateRecords = new List<TaxiTrip>();
        
        for (var i = 0; i < records.Count(); i++)
        {
            var key = $"{records[i].PickupDatetime}-{records[i].DropoffDatetime}-{records[i].PassengerCount}";
            if (visited.Contains(key))
            {
                duplicateRecords.Add(records[i]);
                records.RemoveAt(i);
                i--;
            }
            else
            {
                visited.Add(key);
            }
        }

        if (duplicateRecords.Any())
        {
            var fileName = $"duplicates-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.csv";
            var path = Path.Combine(Configuration.DuplicatesFolderPath, fileName);

            using var writer = new StreamWriter(path);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(duplicateRecords);
            
            Console.WriteLine($"Found {duplicateRecords.Count} duplicates. Saved duplicates to {path}.");
        }
    }
    
    public void InsertRecords(List<TaxiTrip> records)
    {
        using var connection = new SqlConnection(Configuration.DbConnectionString);
        connection.Open();
        
        using var bulkCopy = new SqlBulkCopy(connection);
        bulkCopy.DestinationTableName = Configuration.DbTableName;
        AddColumnMappings(bulkCopy);

        var dataTable = ConvertCollectionToDataTable(records);
        bulkCopy.WriteToServer(dataTable);
        Console.WriteLine($"Inserted {dataTable.Rows.Count} records.");
    }
    
    private DataTable ConvertCollectionToDataTable(List<TaxiTrip> records)
    {
        DataTable table = new DataTable();
        
        table.Columns.Add(nameof(TaxiTrip.PickupDatetime), typeof(DateTime));
        table.Columns.Add(nameof(TaxiTrip.DropoffDatetime), typeof(DateTime));
        table.Columns.Add(nameof(TaxiTrip.PassengerCount), typeof(int));
        table.Columns.Add(nameof(TaxiTrip.TripDistance), typeof(float));
        table.Columns.Add(nameof(TaxiTrip.StoreAndFwdFlag), typeof(string));
        table.Columns.Add(nameof(TaxiTrip.PULocationID), typeof(int));
        table.Columns.Add(nameof(TaxiTrip.DOLocationID), typeof(int));
        table.Columns.Add(nameof(TaxiTrip.FareAmount), typeof(float));
        table.Columns.Add(nameof(TaxiTrip.TipAmount), typeof(float));
        
        var estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        
        foreach (var record in records)
        {
            table.Rows.Add(
                // from ets to utc
                TimeZoneInfo.ConvertTimeToUtc(record.PickupDatetime, estZone), 
                TimeZoneInfo.ConvertTimeToUtc(record.DropoffDatetime, estZone), 
                record.PassengerCount, 
                record.TripDistance, 
                record.StoreAndFwdFlag == "Y" ? "Yes" : "No", 
                record.PULocationID, 
                record.DOLocationID, 
                record.FareAmount, 
                record.TipAmount
            );
        }

        return table;
    }

    // Column mappings are needed to bind model fields to database table columns
    private void AddColumnMappings(SqlBulkCopy bulkCopy)
    {
        bulkCopy.ColumnMappings.Add(nameof(TaxiTrip.PickupDatetime), "tpep_pickup_datetime");
        bulkCopy.ColumnMappings.Add(nameof(TaxiTrip.DropoffDatetime), "tpep_dropoff_datetime");
        bulkCopy.ColumnMappings.Add(nameof(TaxiTrip.PassengerCount), "passenger_count");
        bulkCopy.ColumnMappings.Add(nameof(TaxiTrip.TripDistance), "trip_distance");
        bulkCopy.ColumnMappings.Add(nameof(TaxiTrip.StoreAndFwdFlag), "store_and_fwd_flag");
        bulkCopy.ColumnMappings.Add(nameof(TaxiTrip.PULocationID), "PULocationID");
        bulkCopy.ColumnMappings.Add(nameof(TaxiTrip.DOLocationID), "DOLocationID");
        bulkCopy.ColumnMappings.Add(nameof(TaxiTrip.FareAmount), "fare_amount");
        bulkCopy.ColumnMappings.Add(nameof(TaxiTrip.TipAmount), "tip_amount");
    }
}