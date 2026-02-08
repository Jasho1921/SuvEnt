namespace EventHomepage.Models;

public class CreateEventViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Description {get;set;} = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public int? MaxParticipants { get; set; }
}
