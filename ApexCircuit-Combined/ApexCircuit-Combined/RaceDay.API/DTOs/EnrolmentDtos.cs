namespace RaceDay.API.DTOs;

public record EnrolDto(int EventId, int CategoryId);
public record EnrolmentDto(int EnrolmentId, string ParticipantName,
    string EventName, string CategoryName, DateTime EnrolledAt);
