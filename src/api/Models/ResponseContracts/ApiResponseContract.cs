namespace AGRC.api.Models.ResponseContracts;
public class ApiResponseContract {
    /// <summary>
    /// The matching http status code
    /// </summary>
    /// <example>404</example>
    public int Status { get; set; }
    /// <summary>
    /// The reason for the status code. This is only sent for status codes other than 200
    /// </summary>
    /// <example>No address candidates found with a score of 70 or better.</example>
    public string Message { get; set; } = string.Empty;
}

public class ApiResponseContract<T> : ApiResponseContract where T : class {
    /// <summary>
    /// The result of the request. This is only populated when the status code is 200
    /// </summary>
    public T Result { get; set; } = default!;
}
