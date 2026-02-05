namespace EventHomepage.DTOs;

public record CreateEventDTO(
    string Name,
    string Description,
    DateTime StartDateTime,
    DateTime EndDateTime,
    string Location,
    int? MaxParticipants
);
