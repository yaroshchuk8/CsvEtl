using CsvHelper.Configuration.Attributes;

namespace CsvEtl;

// I created this model to extract rows from a .csv file with ease
// 'Name' attributes bind class properties with columns in a .csv file
public class TaxiTrip
{
    [Name("tpep_pickup_datetime")]
    public DateTime PickupDatetime { get; set; }
    
    [Name("tpep_dropoff_datetime")]
    public DateTime DropoffDatetime { get; set; }
    
    [Name("passenger_count")]
    public int? PassengerCount { get; set; }
    
    [Name("trip_distance")]
    public float? TripDistance { get; set; }
    
    [Name("store_and_fwd_flag")]
    public string StoreAndFwdFlag { get; set; }
    
    [Name("PULocationID")]
    public int? PULocationID { get; set; }
    
    [Name("DOLocationID")]
    public int? DOLocationID { get; set; }
    
    [Name("fare_amount")]
    public float? FareAmount { get; set; }
    
    [Name("tip_amount")]
    public float? TipAmount { get; set; }
}