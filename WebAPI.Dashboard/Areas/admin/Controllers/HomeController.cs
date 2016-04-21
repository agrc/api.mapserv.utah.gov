using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Raven.Client;
using WebAPI.Common.Extensions;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Admin;
using WebAPI.Dashboard.Controllers;
using WebAPI.Dashboard.Models.ViewModels.Usage;

namespace WebAPI.Dashboard.Areas.admin.Controllers
{
    [Authorize]
    public class HomeController : RavenController
    {
        public HomeController(IDocumentStore store)
            : base(store)
        {
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (!Session.Query<AdminContainer>().Any(x => x.Emails.Any(y => y == Account.Email)))
            {
                ErrorMessage = "Nothing to see there.";

                return RedirectToRoute("default", new
                    {
                        controller = "Home"
                    });
            }

            var stats = Session.Query<StatsPerService.Stats, StatsPerService>()
                               .Select(x => x).ToList();

            var statsModel = Mapper.Map<List<StatsPerService.Stats>, List<ServiceStatsViewModel>>(stats);

            var then = DateTime.Parse("2/24/2013").Ticks;
            var lineChart = Session.Query<RequestsPerDay.Stats, RequestsPerDay>()
                                   .Where(x => x.Date > then)
                                   .OrderByDescending(x=>x.Date)
                                   .ToList();

            var line = TransformToD3JsLine(lineChart);

            var model = new
                {
                    stats = statsModel,
                    lineChart = line
                };

            return View("Index", model.ToExpando());
        }

        private static string TransformToD3JsLine(IEnumerable<RequestsPerDay.Stats> statsModel)
        {
            return string.Join(",", statsModel.
                Select(x => string.Format("[{0}]", string.Join(",", new[]
                    {
                       string.Format("'{0}'", new DateTime(x.Date).ToShortDateString()), 
                        x.Requests.ToString()
                    }))));
        }
    }
}