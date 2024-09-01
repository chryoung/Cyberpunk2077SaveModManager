using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Concurrent;

namespace Cyberpunk2077SaveModManager.Logger
{
    public class TextBoxLoggerProvider : ILoggerProvider
    {
        private readonly TextBox _textBox;
        private readonly string _logFilePath;
        private readonly ConcurrentDictionary<string, TextBoxLogger> _loggers = new ConcurrentDictionary<string, TextBoxLogger>();

        public TextBoxLoggerProvider(TextBox textBox, string logFilePath)
        {
            _textBox = textBox;
            _logFilePath = logFilePath;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new TextBoxLogger(name, _textBox, _logFilePath));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
