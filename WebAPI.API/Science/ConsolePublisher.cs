using GitHub;
using System;
using Serilog;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;

namespace WebAPI.API.Science
{
    public class ConsolePublisher : IResultPublisher
    {
        public Task Publish<T, TClean>(Result<T, TClean> result)
        {
            Debug.WriteLine($"Publishing results for experiment '{result.ExperimentName}'");
            Debug.WriteLine($"Result: {(result.Matched ? "MATCH" : "MISMATCH")}");
            Debug.WriteLine($"Control value: {result.Control.Value}");
            Debug.WriteLine($"Control duration: {result.Control.Duration}");

            foreach (var observation in result.Candidates)
            {
                Debug.WriteLine($"Candidate name: {observation.Name}");
                Debug.WriteLine($"Candidate value: {observation.Value}");
                Debug.WriteLine($"Candidate duration: {observation.Duration}");
            }

            return Task.FromResult(0);
        }
    }

    public class LogPublisher : IResultPublisher
    {
        public Task Publish<T, TClean>(Result<T, TClean> result)
        {
            Log.Debug($"Publishing results for experiment '{result.ExperimentName}'");
            Log.Debug($"Result: {(result.Matched ? "MATCH" : "MISMATCH")}");
            Log.Debug($"Control value: {result.Control.Value}");
            Log.Debug($"Control duration: {result.Control.Duration}");

            return Task.FromResult(0);
        }
    }

}