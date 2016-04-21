#region License

// 
// Copyright (C) 2012 AGRC
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

#endregion

using System;
using System.Diagnostics;
using ESRI.ArcGIS.SOESupport;
using Soe.Common.Extensions;

namespace Soe.Common.Infastructure.Commands
{
    /// <summary>
    ///     A command with no return value
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        ///     The message code to be used for all failed commands
        /// </summary>
        internal const int MessageCode = 2472;

        public string ErrorMessage { get; set; }

        public ServerLogger Logger { get; set; }

        public void Run()
        {
            var commandName = ToString();
            if (Logger == null)
                Logger = new ServerLogger();

            try
            {
                Debug.Print("Executing\r\n{0}".With(commandName));
#if !DEBUG
                Logger.LogMessage(ServerLogger.msgType.debug, "{0}.{1}".With(commandName, "execute"), MessageCode,
                                  "Executing\r\n{0}".With(commandName));
#endif
                Execute();
                Debug.Print("Done Executing\r\n{0}".With(commandName));

#if !DEBUG
                Logger.LogMessage(ServerLogger.msgType.debug, "{0}.{1}".With(commandName, "execute"), MessageCode,
                                  "Done Executing");
#endif
            }
            catch (Exception ex)
            {
                Debug.Print("Error processing task: {0}".With(commandName), ex);
#if !DEBUG
                Logger.LogMessage(ServerLogger.msgType.error, "{0}.{1}".With(commandName, "execute"), MessageCode,
                                  "error running command: {0}. Error: {1}".With(commandName, ex.Message));
#endif
                ErrorMessage = ex.Message;
            }
            finally
            {
                Logger = null;
            }
        }

        public abstract override string ToString();

        /// <summary>
        ///     code to execute when command is run.
        /// </summary>
        protected abstract void Execute();
    }

    /// <summary>
    ///     A command with a return value
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    public abstract class Command<T> : Command
    {
        public T Result { get; protected set; }

        public T GetResult()
        {
            Run();

#if !DEBUG
            Logger = new ServerLogger();

            Logger.LogMessage(ServerLogger.msgType.debug, ToString(), MessageCode,
                              "Done Executing\r\n{0}\r\nResult: {1}".With(ToString(), Result));
            Logger = null;
#endif
            return Result;
        }
    }
}