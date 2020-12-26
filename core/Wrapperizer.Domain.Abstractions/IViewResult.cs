using System;
using System.Collections.Generic;
using static Wrapperizer.Domain.Abstractions.ViewResultMessage;

namespace Wrapperizer.Domain.Abstractions
{
    public class ViewResultMessage
    {
        public ViewResultMessageSeverity Severity { get; init; }
        public string Message { get; init; }

        public static implicit operator ViewResultMessage(string str) =>
            new ViewResultMessage {Message = str, Severity = ViewResultMessageSeverity.Done};

        public static explicit operator string(ViewResultMessage message) => message?.Message ?? string.Empty;

        public override string ToString() => Message;

        public static ViewResultMessage ToViewResultMessage(string message, ViewResultMessageSeverity viewResultMessageSeverity) =>
            new ViewResultMessage {Message = message, Severity = viewResultMessageSeverity};
    }

    public enum ViewResultMessageSeverity
    {
        Done = 0,
        Info = 1,
        Warn = 2,
        Fail = 3
    }

    public interface IViewResult
    {
        bool Success { get; }
        IReadOnlyList<ViewResultMessage> Messages { get; }

        void Done(string message);
        void Info(string message);
        void Warn(string message);
        void Fail(string message);
    }

    internal interface IConvertibleToResult : IViewResult
    {
        internal ViewResult ParentResult { get; }
    }

    public class ViewResult : IViewResult
    {
        public bool Success { get; private set; }

        private readonly List<ViewResultMessage> _messages;
        public IReadOnlyList<ViewResultMessage> Messages => _messages;

        internal ViewResult(bool isSuccess = true, params string[] messages)
        {
            Success = isSuccess;

            _messages = new List<ViewResultMessage>();

            foreach (var message in messages)
                _messages.Add(message);
        }

        internal ViewResult(bool isSuccess, ViewResultMessageSeverity defaultViewResultMessageSeverity, IEnumerable<string> messages)
        {
            Success = isSuccess;

            _messages = new List<ViewResultMessage>();

            foreach (var message in messages)
                _messages.Add(ToViewResultMessage(message, defaultViewResultMessageSeverity));
        }

        internal ViewResult(bool success, IEnumerable<ViewResultMessage> messages)
        {
            Success = success;
            _messages = new List<ViewResultMessage>(messages);
        }

        public void Done(string message) => _messages.Add(ToViewResultMessage(message, ViewResultMessageSeverity.Done));
        public void Info(string message) => _messages.Add(ToViewResultMessage(message, ViewResultMessageSeverity.Info));
        public void Warn(string message) => _messages.Add(ToViewResultMessage(message, ViewResultMessageSeverity.Warn));
        public void Fail(string message) => _messages.Add(ToViewResultMessage(message, ViewResultMessageSeverity.Fail));



        public static ViewResult Done(params string[] messages) => new ViewResult(true, messages);
        public static ViewResult Done(IEnumerable<ViewResultMessage> messages) => new ViewResult(true, messages);

        public static ViewResult<T> Done<T>(T data, params string[] messages) =>
            new ViewResult<T>(new ViewResult(true, messages), data);
        public static ViewResult<T> Done<T>(T data, IEnumerable<ViewResultMessage> messages) =>
            new ViewResult<T>(new ViewResult(true, messages), data);


        public static ViewResult Info(params string[] messages) => new ViewResult(true, ViewResultMessageSeverity.Info, messages);
        public static ViewResult Info(IEnumerable<ViewResultMessage> messages) => new ViewResult(true, messages);

        public static ViewResult<T> Info<T>(T data, params string[] messages) =>
            new ViewResult<T>(new ViewResult(true, ViewResultMessageSeverity.Info, messages), data);
        public static ViewResult<T> Info<T>(T data, IEnumerable<ViewResultMessage> messages) =>
            new ViewResult<T>(new ViewResult(true, messages), data);


        public static ViewResult Fail(params string[] messages) => new ViewResult(false, ViewResultMessageSeverity.Fail, messages);
        public static ViewResult Fail(IEnumerable<ViewResultMessage> messages) => new ViewResult(false, messages);

        public static ViewResult<T> Fail<T>(T data, params string[] messages) =>
            new ViewResult<T>(new ViewResult(false, ViewResultMessageSeverity.Fail, messages), data);
        public static ViewResult<T> Fail<T>(T data, IEnumerable<ViewResultMessage> messages) =>
            new ViewResult<T>(new ViewResult(false, messages), data);



        public static ViewResult Warn(params string[] messages) => new ViewResult(true, ViewResultMessageSeverity.Warn, messages);
        public static ViewResult Warn(IEnumerable<ViewResultMessage> messages) => new ViewResult(true, messages);
        
        public static ViewResult<T> Warn<T>(T data, params string[] messages) =>
            new ViewResult<T>(new ViewResult(true, ViewResultMessageSeverity.Warn, messages), data);
        public static ViewResult<T> Warn<T>(T data, IEnumerable<ViewResultMessage> messages) =>
            new ViewResult<T>(new ViewResult(true, messages), data);
    }

    public class ViewResult<T> : IConvertibleToResult
    {
        public bool Success => ParentResult.Success;

        public IReadOnlyList<ViewResultMessage> Messages => ParentResult.Messages;

        public void Done(string message) => ParentResult.Done(message);
        public void Info(string message)=> ParentResult.Info(message);
        public void Warn(string message)=> ParentResult.Warn(message);
        public void Fail(string message)=> ParentResult.Fail(message);


        public T Data { get; private set; }

        private ViewResult ParentResult { get; set; }

        ViewResult IConvertibleToResult.ParentResult => ParentResult;

        private ViewResult(ViewResult result)
        {
            ParentResult = result ?? throw new NullReferenceException(nameof(result));
        }

        internal ViewResult(ViewResult result, T data)
            : this(result)
        {
            Data = data ?? throw new NullReferenceException(nameof(data));
        }

        public static implicit operator ViewResult<T>(T data) => new ViewResult<T>(new ViewResult(true), data);
        public static implicit operator ViewResult<T>(ViewResult result) => new ViewResult<T>(result);
    }
}
