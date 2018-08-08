using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using api.mapserv.utah.gov.Exceptions;
using Serilog;

namespace api.mapserv.utah.gov.Models
{
    public abstract class Command
    {
        public string ErrorMessage { get; set; }

        public void Run()
        {
            var errorList = new List<string>();
            try
            {
                var timer = new Stopwatch();
                timer.Start();
                Execute();
                timer.Stop();
                Log.Debug("{Task} Duration: {Duration}ms", ToString(),
                           timer.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture));
            }
            catch (AggregateException ex)
            {
                var throwForGeocoding = false;
                GeocodingException exception = null;
                foreach (var e in ex.Flatten().InnerExceptions)
                {
                    if (e is GeocodingException geocodingException)
                    {
                        throwForGeocoding = true;
                        exception = geocodingException;
                    }

                    errorList.Add(e.Message);
                }

                if (throwForGeocoding)
                {
                    throw exception;
                }

                Log.Fatal(ex, "Geocoding error occurred: {Task} {@errorList}", ToString(), errorList);
            }
            catch (CommandValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Log.Fatal(ex, "Error processing task: {Task} {@errorList}", ToString(), errorList);
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
