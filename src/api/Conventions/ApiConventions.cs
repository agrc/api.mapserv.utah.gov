using AGRC.api.Models.ResponseContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGRC.api.Conventions;
public static class ApiConventions {
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponseContract))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponseContract))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponseContract))]
    public static void Default() { }
}
