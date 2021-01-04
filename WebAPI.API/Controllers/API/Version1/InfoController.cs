using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.API.Commands.Info;
using WebAPI.Common.Executors;
using WebAPI.Common.Extensions;
using WebAPI.Domain;

namespace WebAPI.API.Controllers.API.Version1
{
    public class InfoController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage FeatureClassNames(string sgidCategory = null)
        {
            try
            {
                var result = CommandExecutor.ExecuteCommand(new GetFeatureClassNamesCommand
                    {
                        SgidCategory = sgidCategory
                    });

                var response = Request.CreateResponse(HttpStatusCode.OK,
                                                      new ResultContainer<string[]>
                                                          {
                                                              Status = (int) HttpStatusCode.OK,
                                                              Result = result
                                                          });

                return response.AddCache();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                              new ResultContainer<string[]>
                                                  {
                                                      Status = (int) HttpStatusCode.InternalServerError,
                                                      Message = ex.Message
                                                  });
            }
        }

        [HttpGet]
        public HttpResponseMessage FeatureClassAttributes(string featureClass, string category = null)
        {
            try
            {
                var result =
                    CommandExecutor.ExecuteCommand(new GetFeatureClassAttributesCommand(featureClass, category));
                var message = "";

                if (!result.Any())
                {
                    message = "Please check your spelling, add a category, or use /info/FeatureClassNames to find your layer.";
                }

                var response = Request.CreateResponse(HttpStatusCode.OK,
                                                      new ResultContainer<string[]>
                                                          {
                                                              Status = (int) HttpStatusCode.OK,
                                                              Result = result,
                                                              Message = message
                                                          });

                return response.AddCache();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                              new ResultContainer<string[]>
                                                  {
                                                      Status = (int) HttpStatusCode.InternalServerError,
                                                      Message = ex.Message
                                                  });
            }
        }
    }
}