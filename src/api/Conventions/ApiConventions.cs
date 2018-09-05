using api.mapserv.utah.gov.Models.ApiResponses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.mapserv.utah.gov.Conventions {
    public static class ApiConventions {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponseContainer))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponseContainer))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponseContainer))]
        public static void Default(){}
    }
}
