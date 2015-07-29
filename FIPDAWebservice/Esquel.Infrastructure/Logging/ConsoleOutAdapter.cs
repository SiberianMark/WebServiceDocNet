using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Esquel.Library.Infrastructure.Logging
{
    public class ConsoleOutAdapter : ILogger
    {
        private const string PrefixDebug = "DEBUG";
        private const string PrefixInfo = "INFO";
        private const string PrefixWarn = "WARN";
        private const string PrefixError = "ERROR";
        private const string PrefixFatal = "FATAL";

        private TextWriter writer;
        public ConsoleOutAdapter()
        {
            writer = Console.Out;
        }

        private void Dump(string prefix, string message, params object[] args)
        {
            var line = String.Format("{0:yyyy-MM-dd' 'HH:mm:ss} [{1}] {2} {3} - ", DateTime.Now, prefix, Thread.CurrentThread.ManagedThreadId, this.ToString()) + String.Format(message, args);

            lock (this.writer)
            {
                this.writer.WriteLine(line);
                this.writer.Flush();
            }
        }

        private void Dump(string prefix, object message)
        {
            var line = String.Format("{0:yyyy-MM-dd' 'HH:mm:ss} [{1}] {2} {3} - {4}", DateTime.Now, prefix, Thread.CurrentThread.ManagedThreadId, this.ToString(), message);

            lock (this.writer)
            {
                this.writer.WriteLine(line);
                this.writer.Flush();
            }
        }

        bool ILogger.IsDebugEnabled
        {
            get { return true; }
        }

        bool ILogger.IsInfoEnabled
        {
            get { return true; }
        }

        bool ILogger.IsWarnEnabled
        {
            get { return true; }
        }

        bool ILogger.IsErrorEnabled
        {
            get { return true; }
        }

        bool ILogger.IsFatalEnabled
        {
            get { return true; }
        }

        void ILogger.Debug(object message)
        {
            this.Dump(PrefixDebug, message);
        }

        void ILogger.Debug(object message, Exception exception)
        {
            this.Dump(PrefixDebug, message + " - " + exception);
        }

        void ILogger.DebugFormat(string format, object arg0)
        {
            this.Dump(PrefixDebug, format, arg0);
        }

        void ILogger.DebugFormat(string format, object arg0, object arg1)
        {
            this.Dump(PrefixDebug, format, arg0, arg1);
        }

        void ILogger.DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            this.Dump(PrefixDebug, format, arg0, arg1, arg2);
        }

        void ILogger.DebugFormat(string format, params object[] args)
        {
            this.Dump(PrefixDebug, format, args);
        }

        void ILogger.DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            this.Dump(PrefixDebug, String.Format(provider, format, args));
        }

        void ILogger.Info(object message)
        {
            this.Dump(PrefixInfo, message);
        }

        void ILogger.Info(object message, Exception exception)
        {
            this.Dump(PrefixInfo, message + " - " + exception);
        }

        void ILogger.InfoFormat(string format, object arg0)
        {
            this.Dump(PrefixInfo, format, arg0);
        }

        void ILogger.InfoFormat(string format, object arg0, object arg1)
        {
            this.Dump(PrefixInfo, format, arg0, arg1);
        }

        void ILogger.InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            this.Dump(PrefixInfo, format, arg0, arg1, arg2);
        }

        void ILogger.InfoFormat(string format, params object[] args)
        {
            this.Dump(PrefixInfo, format, args);
        }

        void ILogger.InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            this.Dump(PrefixInfo, String.Format(provider, format, args));
        }

        void ILogger.Warn(object message)
        {
            this.Dump(PrefixWarn, message);
        }

        void ILogger.Warn(object message, Exception exception)
        {
            this.Dump(PrefixWarn, message + " - " + exception);
        }

        void ILogger.WarnFormat(string format, object arg0)
        {
            this.Dump(PrefixWarn, format, arg0);
        }

        void ILogger.WarnFormat(string format, object arg0, object arg1)
        {
            this.Dump(PrefixWarn, format, arg0, arg1);
        }

        void ILogger.WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            this.Dump(PrefixWarn, format, arg0, arg1, arg2);
        }

        void ILogger.WarnFormat(string format, params object[] args)
        {
            this.Dump(PrefixWarn, format, args);
        }

        void ILogger.WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            this.Dump(PrefixWarn, String.Format(provider, format, args));
        }

        void ILogger.Error(object message)
        {
            this.Dump(PrefixError, message);
        }

        void ILogger.Error(object message, Exception exception)
        {
            this.Dump(PrefixError, message + " - " + exception);
        }

        void ILogger.ErrorFormat(string format, object arg0)
        {
            this.Dump(PrefixError, format, arg0);
        }

        void ILogger.ErrorFormat(string format, object arg0, object arg1)
        {
            this.Dump(PrefixError, format, arg0, arg1);
        }

        void ILogger.ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            this.Dump(PrefixError, format, arg0, arg1, arg2);
        }

        void ILogger.ErrorFormat(string format, params object[] args)
        {
            this.Dump(PrefixError, format, args);
        }

        void ILogger.ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            this.Dump(PrefixError, String.Format(provider, format, args));
        }

        void ILogger.Fatal(object message)
        {
            this.Dump(PrefixFatal, message);
        }

        void ILogger.Fatal(object message, Exception exception)
        {
            this.Dump(PrefixFatal, message + " - " + exception);
        }

        void ILogger.FatalFormat(string format, object arg0)
        {
            this.Dump(PrefixFatal, format, arg0);
        }

        void ILogger.FatalFormat(string format, object arg0, object arg1)
        {
            this.Dump(PrefixFatal, format, arg0, arg1);
        }

        void ILogger.FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            this.Dump(PrefixFatal, format, arg0, arg1, arg2);
        }

        void ILogger.FatalFormat(string format, params object[] args)
        {
            this.Dump(PrefixFatal, format, args);
        }

        void ILogger.FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            this.Dump(PrefixFatal, String.Format(provider, format, args));
        }
    }
}
