namespace Model;

public class ImportFlightResult
{
    public ImportFlightStatus status { get; set; }
    public List<string> errorMessages { get; set; } = new ();
}