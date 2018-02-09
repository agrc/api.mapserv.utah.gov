using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WebAPI.API.Commands.Geocode;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.API.Executors.Geocode
{
    public static class GeocodeCommandQueueExecutor
    {
        private static readonly ConcurrentQueue<GetAddressCandidatesCommand> CommandsToExecute =
            new ConcurrentQueue<GetAddressCandidatesCommand>();

        public static void ExecuteLater(GetAddressCandidatesCommand command)
        {
            CommandsToExecute.Enqueue(command);
        }

        public static List<Candidate> StartExecuting()
        {
            var tasks = new Collection<Task<List<Candidate>>>();

            GetAddressCandidatesCommand stagingCommand;
            while (CommandsToExecute.TryDequeue(out stagingCommand))
            {
                tasks.Add(Task.Factory.StartNew(stagingCommand.GetResult).Unwrap());
            }

                Task.WaitAll(tasks.ToArray());

            return BuildResult(tasks);
        }

        public static List<Candidate> BuildResult(Collection<Task<List<Candidate>>> tasks)
        {
            var tempList = tasks.Where(x => x.Result != null).SelectMany(x => x.Result).ToList();

            return tempList;
        }
    }
}
