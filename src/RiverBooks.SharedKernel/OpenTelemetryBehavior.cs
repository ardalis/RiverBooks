using System.Diagnostics;
using System.Diagnostics.Metrics;
using Ardalis.GuardClauses;
using Mediator;

namespace RiverBooks.SharedKernel;

public static class OpenTelemetryBehaviorInstrumentation
{
  public const string ActivitySourceName = "RiverBooks.SharedKernel.Mediator";
  public const string MeterName = "RiverBooks.SharedKernel.Mediator";
  public const string RequestDurationMetricName = "riverbooks.mediator.request.duration";
  public const string RequestNameTag = "riverbooks.mediator.request.name";
  public const string RequestTypeTag = "riverbooks.mediator.request.type";
  public const string ResponseTypeTag = "riverbooks.mediator.response.type";
  public const string ErrorTypeTag = "riverbooks.mediator.error.type";
  public const string DurationMillisecondsTag = "riverbooks.mediator.duration.ms";
  public static readonly double[] RequestDurationBoundaries = [1, 5, 10, 25, 50, 100, 250, 500, 1000, 2500, 5000];
}

public class OpenTelemetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IMessage
{
  private static readonly ActivitySource ActivitySource = new(OpenTelemetryBehaviorInstrumentation.ActivitySourceName);
  private static readonly Meter Meter = new(OpenTelemetryBehaviorInstrumentation.MeterName);
  private static readonly Histogram<double> RequestDuration = Meter.CreateHistogram<double>(
    OpenTelemetryBehaviorInstrumentation.RequestDurationMetricName,
    unit: "ms",
    description: "Time spent handling mediator requests.");

  public async ValueTask<TResponse> Handle(TRequest request, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
  {
    Guard.Against.Null(request);

    var requestName = typeof(TRequest).Name;
    var requestType = request.GetType().FullName ?? typeof(TRequest).FullName ?? requestName;
    var responseType = typeof(TResponse).FullName ?? typeof(TResponse).Name;

    using var activity = ActivitySource.StartActivity(requestName, ActivityKind.Internal);
    activity?.SetTag(OpenTelemetryBehaviorInstrumentation.RequestNameTag, requestName);
    activity?.SetTag(OpenTelemetryBehaviorInstrumentation.RequestTypeTag, requestType);
    activity?.SetTag(OpenTelemetryBehaviorInstrumentation.ResponseTypeTag, responseType);

    var startedAt = Stopwatch.GetTimestamp();

    try
    {
      var response = await next(request, cancellationToken);
      RecordDuration(activity, requestName, requestType, responseType, Stopwatch.GetElapsedTime(startedAt));
      return response;
    }
    catch (Exception ex)
    {
      var elapsed = Stopwatch.GetElapsedTime(startedAt);
      activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
      activity?.SetTag(OpenTelemetryBehaviorInstrumentation.ErrorTypeTag, ex.GetType().FullName ?? ex.GetType().Name);
      RecordDuration(activity, requestName, requestType, responseType, elapsed, ex);
      throw;
    }
  }

  private static void RecordDuration(
    Activity? activity,
    string requestName,
    string requestType,
    string responseType,
    TimeSpan elapsed,
    Exception? exception = null)
  {
    activity?.SetTag(OpenTelemetryBehaviorInstrumentation.DurationMillisecondsTag, elapsed.TotalMilliseconds);

    var tags = new TagList
    {
      { OpenTelemetryBehaviorInstrumentation.RequestNameTag, requestName },
      { OpenTelemetryBehaviorInstrumentation.RequestTypeTag, requestType },
      { OpenTelemetryBehaviorInstrumentation.ResponseTypeTag, responseType }
    };

    if (exception is not null)
    {
      tags.Add(OpenTelemetryBehaviorInstrumentation.ErrorTypeTag, exception.GetType().FullName ?? exception.GetType().Name);
    }

    RequestDuration.Record(elapsed.TotalMilliseconds, tags);
  }
}
