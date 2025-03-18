namespace CsvEtl;

class Program
{
    static void Main(string[] args)
    {
        var etlService = new EtlService();
        
        // Extract data from a .csv file using path in Configuration.cs
        var records = etlService.ExtractRecords();

        // Transform data; remove duplicates and save them in .csv file using folder path in Configuration.cs
        etlService.TransformRecords(records);
        
        // Insert data (bulk insertion) into database using path in Configuration.cs
        etlService.InsertRecords(records);
    }
}