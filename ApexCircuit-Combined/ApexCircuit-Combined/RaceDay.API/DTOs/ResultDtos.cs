namespace RaceDay.API.DTOs;

public record CaptureResultDto(int EnrolmentId, string FinishTime, int FinishingPosition);
public record ResultDto(int ResultId, string ParticipantName, string EventName,
    string CategoryName, TimeSpan FinishTime, int FinishingPosition);
