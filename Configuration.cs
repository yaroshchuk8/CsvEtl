namespace CsvEtl;

public static class Configuration
{
    // change these 3
    public const string CsvFilePath = @"D:\Storage\dotnet\Test tasks\sample-cab-data.csv";
    public const string DuplicatesFolderPath = @"D:\Storage\dotnet\Test tasks";
    public const string DbConnectionString = "Server=YV;Database=CsvEtl;trusted_connection=true;TrustServerCertificate=True";
    
    // do not change this
    public const string DbTableName = "TaxiTrips";
}