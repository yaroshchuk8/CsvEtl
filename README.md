# CsvEtl - C# Task Assessment

The goal of this task is to implement a simple ETL project in CLI that inserts data from a CSV into a single, flat table.

### IMPORTANT!!! 
Before starting the program, you need to:
- Create database using script in *script.txt*. 
- Change configuration in *Configuration.cs*:
   - *CsvFilePath* - path to your csv file.
   - *DuplicatesFolderPath* - folder path where detected duplicates will be saved
   - *DbConnectionString* - your database connection string

---

This chapter is explanation of what is going on in my program.

1. I've detected a few rows with missing fields in the provided csv file. Missing fields are saved as NULL.
2. I use CsvHelper (NuGet package) to manipulate (extract/create) csv files.
3. I use Microsoft.Data.SqlClient (NuGet package) to insert data into a database.
4. I created TaxiTrip model to extract specific fields only from a csv file (see TaxiTrip.cs).
5. In order to provide query optimisations I added indexes (see script.txt).
6. Duplicates are detected using HashSet and saved with such name format "duplicates-yyyy-MM-dd-HH-mm-ss.csv".
7. For the `store_and_fwd_flag` column, 'N' is converted to 'No' and 'Y' to 'Yes'.
8. All text-based fields are free from leading or trailing whitespace (ensured by CsvHelper configuration).
9. Assuming that my program will work on larger datasets (10GB CSV file), I would change it to extract, transform and insert smaller amount of records at a time (for example, 100.000 records). The program works fine with 30.000 records in provided csv file. 
10. The input data is converted from EST to UTC timezone.

The program output:
Extracted 30000 records.
Found 111 duplicates. Saved duplicates to D:\Storage\dotnet\Test tasks\duplicates-2025-03-18-22-10-54.csv.
Inserted 29889 records.