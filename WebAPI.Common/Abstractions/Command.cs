using System;
using System.Diagnostics;
using System.Globalization;
using NLog;

namespace WebAPI.Common.Abstractions
{
    /// <summary>
    ///     A class with no return value
    /// </summary>
    public abstract class Command
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
       
        public string ErrorMessage { get; set; }

        public void Run()
        {
            try
            {
                var timer = new Stopwatch();
                timer.Start();
                Execute();
                timer.Stop();
                Debug.Print("{0}-{1} Duration: {2}ms", ToString(), "Done",
                            timer.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Debug.Print("Error processing task:" + ToString(), ex);
                Debug.WriteLine("Error processing task:" + ToString(), ex.Message);
                Log.Error(ex, "Error processing task:" + ToString());
            }
        }

        public abstract override string ToString();

        protected abstract void Execute();
    }

    /// <summary>
    ///     A command with a return value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Command<T> : Command
    {
        public T Result { get; protected set; }

        public T GetResult()
        {
            Run();
            return Result;
        }
    }
}