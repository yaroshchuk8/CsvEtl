using System.Data;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;

namespace CsvEtl;

class Program
{
    static void Main(string[] args)
    {
        // Extract records from a .csv file using path in Configuration.cs
        var records = ExtractRecords();
        
        // Insert records (bulk insertion) into database using path in Configuration.cs
        InsertRecords(records);
    }

    static void InsertRecords(IEnumerable<TaxiTrip> records)
    {
        using var connection = new SqlConnection(Configuration.DbConnectionString);
        connection.Open();
        
        using var bulkCopy = new SqlBulkCopy(connection);
        bulkCopy.DestinationTableName = "TaxiTrips";
        AddColumnMappings(bulkCopy);

        var dataTable = ConvertCollectionToDataTable(records);
        bulkCopy.WriteToServer(dataTable);
    }
    
    static IEnumerable<TaxiTrip> ExtractRecords()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            IgnoreBlankLines = true
        };

        using var reader = new StreamReader(Configuration.CsvFilePath);
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<TaxiTrip>().ToList();
    }
    
    static DataTable ConvertCollectionToDataTable(IEnumerable<TaxiTrip> records)
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
        
        foreach (var record in records)
        {
            table.Rows.Add(
                record.PickupDatetime, 
                record.DropoffDatetime, 
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
    static void AddColumnMappings(SqlBulkCopy bulkCopy)
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