using System.Threading.Tasks;
using WebAPI.Common.Abstractions;

namespace WebAPI.Common.Executors
{
    /// <summary>
    ///     Executes Commands on request
    /// </summary>
    public static class CommandExecutor
    {
        /// <summary>
        ///     Executes the command.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        public static void ExecuteCommand(Command cmd)
        {
            cmd.Run();
        }

        /// <summary>
        ///     Executes the command for commands with a result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        public static TResult ExecuteCommand<TResult>(Command<TResult> cmd)
        {
            ExecuteCommand((Command) cmd);

            return cmd.Result;
        }

        public static async Task<TResult> ExecuteCommandAsync<TResult>(AsyncCommand<TResult> cmd)
        {
            return await cmd.Execute();
        }
    }
}