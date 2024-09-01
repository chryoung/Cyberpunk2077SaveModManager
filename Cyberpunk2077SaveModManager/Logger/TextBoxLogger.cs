using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Cyberpunk2077SaveModManager.Logger
{
    public class TextBoxLogger(string name, TextBox textBox, string logFilePath) : ILogger
    {
        private readonly string _name = name;
        private readonly TextBox _textBox = textBox;
        private readonly string _logFilePath = logFilePath;
        private readonly object _textBoxLock = null;
        private readonly HashSet<LogLevel> _disabledLogLevel = [];

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => !_disabledLogLevel.Contains(logLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var logMessage = formatter(state, exception);
            Task.Run(async () => await _textBox.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lock (_textBoxLock)
                {
                    _textBox.Text += ($"{logLevel} - {logMessage}{Environment.NewLine}");
                }   
            }));

            File.AppendAllText(_logFilePath, $"{logLevel} - {logMessage}{Environment.NewLine}");
        }

        public void DisableLogLevel(LogLevel logLevel) => _disabledLogLevel.Add(logLevel);

        public void EnableLogLevel(LogLevel logLevel) => _disabledLogLevel.Remove(logLevel);
    }
}
