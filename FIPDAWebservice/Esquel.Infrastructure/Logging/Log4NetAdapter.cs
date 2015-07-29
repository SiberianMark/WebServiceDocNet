using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Esquel.Library.Infrastructure.Configuration;
using log4net;
using log4net.Config;

namespace Esquel.Library.Infrastructure.Logging
{
    public class Log4NetAdapter : ILogger
    {
        private readonly log4net.ILog _log;

        public Log4NetAdapter()
        {
            XmlConfigurator.Configure();
            _log = LogManager.GetLogger(ApplicationSettingsFactory.GetApplicationSettings().LoggerName);
        }

        #region ILogger Implementation
        bool ILogger.IsDebugEnabled
        {
            get { return _log.IsDebugEnabled; }
        }

        bool ILogger.IsInfoEnabled
        {
            get { return _log.IsInfoEnabled; }
        }

        bool ILogger.IsWarnEnabled
        {
            get { return _log.IsWarnEnabled; }
        }

        bool ILogger.IsErrorEnabled
        {
            get { return _log.IsErrorEnabled; }
        }

        bool ILogger.IsFatalEnabled
        {
            get { return _log.IsFatalEnabled; }
        }

        void ILogger.Debug(object message)
        {
            _log.Debug(message);
        }

        void ILogger.Debug(object message, Exception exception)
        {
            _log.Debug(message, exception);
        }

        void ILogger.DebugFormat(string format, object arg0)
        {
            _log.DebugFormat(format, arg0);
        }

        void ILogger.DebugFormat(string format, object arg0, object arg1)
        {
            _log.DebugFormat(format, arg0, arg1);
        }

        void ILogger.DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            _log.DebugFormat(format, arg0, arg1, arg2);
        }

        void ILogger.DebugFormat(string format, params object[] args)
        {
            _log.DebugFormat(format, args);
        }

        void ILogger.DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.DebugFormat(provider, format, args);
        }

        void ILogger.Info(object message)
        {
            _log.Info(message);
        }

        void ILogger.Info(object message, Exception exception)
        {
            _log.Info(message, exception);
        }

        void ILogger.InfoFormat(string format, object arg0)
        {
            _log.InfoFormat(format, arg0);
        }

        void ILogger.InfoFormat(string format, object arg0, object arg1)
        {
            _log.InfoFormat(format, arg0, arg1);
        }

        void ILogger.InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            _log.InfoFormat(format, arg0, arg1, arg2);
        }

        void ILogger.InfoFormat(string format, params object[] args)
        {
            _log.InfoFormat(format, args);
        }

        void ILogger.InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.InfoFormat(provider, format, args);
        }

        void ILogger.Warn(object message)
        {
            _log.Warn(message);
        }

        void ILogger.Warn(object message, Exception exception)
        {
            _log.Warn(message, exception);
        }

        void ILogger.WarnFormat(string format, object arg0)
        {
            _log.WarnFormat(format, arg0);
        }

        void ILogger.WarnFormat(string format, object arg0, object arg1)
        {
            _log.WarnFormat(format, arg0, arg1);
        }

        void ILogger.WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            _log.WarnFormat(format, arg0, arg1, arg2);
        }

        void ILogger.WarnFormat(string format, params object[] args)
        {
            _log.WarnFormat(format, args);
        }

        void ILogger.WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.WarnFormat(provider, format, args);
        }

        void ILogger.Error(object message)
        {
            _log.Error(message);
        }

        void ILogger.Error(object message, Exception exception)
        {
            _log.Error(message, exception);
        }

        void ILogger.ErrorFormat(string format, object arg0)
        {
            _log.ErrorFormat(format, arg0);
        }

        void ILogger.ErrorFormat(string format, object arg0, object arg1)
        {
            _log.ErrorFormat(format, arg0, arg1);
        }

        void ILogger.ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            _log.ErrorFormat(format, arg0, arg1, arg2);
        }

        void ILogger.ErrorFormat(string format, params object[] args)
        {
            _log.ErrorFormat(format, args);
        }

        void ILogger.ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.ErrorFormat(provider, format, args);
        }

        void ILogger.Fatal(object message)
        {
            _log.Fatal(message);
        }

        void ILogger.Fatal(object message, Exception exception)
        {
            _log.Fatal(message, exception);
        }

        void ILogger.FatalFormat(string format, object arg0)
        {
            _log.FatalFormat(format, arg0);
        }

        void ILogger.FatalFormat(string format, object arg0, object arg1)
        {
            _log.FatalFormat(format, arg0, arg1);
        }

        void ILogger.FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            _log.FatalFormat(format, arg0, arg1, arg2);
        }

        void ILogger.FatalFormat(string format, params object[] args)
        {
            _log.FatalFormat(format, args);
        }

        void ILogger.FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.FatalFormat(provider, format, args);
        }
        #endregion
    }

}
