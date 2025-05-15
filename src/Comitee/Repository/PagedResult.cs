namespace Comitee.Repository;

public record PagedResult<TDataModel>(
    int TotalPages,
    long TotalDataCount,
    IReadOnlyList<TDataModel> Data);