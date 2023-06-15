namespace AGRC.api.Models.ResponseContracts;
public class ApiResponseContract : IApiResponse {
    /// <summary>
    /// The result of the request. This is only populated when the status code is 200
    /// </summary>
    public object? Result { get; set; }
    /// <summary>
    /// The matching http status code
    /// </summary>
    /// <example>404</example>
    public int Status { get; set; }
    /// <summary>
    /// The reason for the status code. This is only sent for status codes other than 200
    /// </summary>
    /// <example>No address candidates found with a score of 70 or better.</example>
    public string? Message { get; set; }
}

public class ApiResponseContract<TValue> : IApiResponse, IApiResponse<TValue> {
    /// <summary>
    /// The result of the request. This is only populated when the status code is 200
    /// </summary>
    public TValue? Result { get; set; }
    object? IApiResponse.Result { get => Result; set => Result = (TValue?)value; }
    /// <summary>
    /// The matching http status code
    /// </summary>
    /// <example>404</example>
    public int Status { get; set; }
    /// <summary>
    /// The reason for the status code. This is only sent for status codes other than 200
    /// </summary>
    /// <example>No address candidates found with a score of 70 or better.</example>
    public string? Message { get; set; }
}

public interface IApiResponse {
    string? Message { get; set; }
    int Status { get; set; }
    object? Result { get; set; }
}

public interface IApiResponse<TValue> {
    string? Message { get; set; }
    int Status { get; set; }
    TValue? Result { get; set; }
}
