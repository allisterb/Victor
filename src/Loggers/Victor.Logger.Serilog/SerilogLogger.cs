using System;
using Serilog;
using SerilogTimings;
using SerilogTimings.Extensions;

namespace Victor
{
    public class SerilogLogger : Logger
    {
        public SerilogLogger(bool console = false, string logFileName = null, bool debug = false)
        {
            Config = new LoggerConfiguration();
            if (console)
            {
                Config = new LoggerConfiguration().WriteTo.Console();
            }
            Config = Config.WriteTo.File(logFileName ?? "Victor.log");
            if (debug)
            {
                Config = Config.MinimumLevel.Debug();
            }
            Logger = Config.CreateLogger();
        }

        public SerilogLogger(Serilog.Core.Logger logger)
        {
            Logger = logger;
        }

        public LoggerConfiguration Config { get; protected set; }

        public ILogger Logger { get; protected set; }

        public override void Info(string messageTemplate, params object[] args) => Logger.Information(messageTemplate, args);

        public override void Debug(string messageTemplate, params object[] args) => Logger.Debug(messageTemplate, args);

        public override void Error(string messageTemplate, params object[] args) => Logger.Error(messageTemplate, args);

        public override void Error(Exception ex, string messageTemplate, params object[] args) => Logger.Error(ex, messageTemplate, args);

        public override Op Begin(string messageTemplate, params object[] args)
        {
            Info(messageTemplate + "...", args);
            return new SerilogOp(this, Logger.BeginOperation(messageTemplate, args));
        }
    }

    public class SerilogOp : Logger.Op
    {
        public SerilogOp(SerilogLogger logger, Operation op): base(logger)
        {
            Op = op;
        }

        public override void Cancel()
        {
            Op.Cancel();
            isCancelled = true;
        }

        public override void Complete()
        {
            Op.Complete();
        }

        public override void Dispose()
        {
            if (!(isCancelled || isCompleted))
            {
                Op.Cancel();
            }
        }

        protected Operation Op;
        
    }
}
