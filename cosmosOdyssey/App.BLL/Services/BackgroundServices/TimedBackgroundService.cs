using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.BLL.Services.BackgroundServices;

public abstract class TimedBackgroundService : IHostedService, IDisposable
{
    private readonly object _lock = new();
    private bool _isRunning;
    private Timer? _timer;

    protected readonly ILogger Logger;

    protected TimedBackgroundService(ILogger logger, IServiceProvider serviceProvider, TimeSpan period)
    {
        Logger = logger;
        _timer = new Timer(DoWorkWrapper, null, TimeSpan.Zero, period);
    }

    protected abstract Task DoWork(object? state);

    protected virtual Task AfterWork() => Task.CompletedTask;

    private void DoWorkWrapper(object? state)
    {
        lock (_lock)
        {
            if (_isRunning)
            {
                Logger.LogInformation($"Previous execution of the background worker is still running. Skipping execution.");
                return;
            }

            _isRunning = true;
        }

        try
        {
            DoWork(state).Wait(); // Use Wait to make this method synchronous
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"Exception occurred when executing background service {GetType()}.");
        }
        finally
        {
            lock (_lock)
            {
                _isRunning = false;
            }
        }

        AfterWork();
    }

    public Task StartAsync(CancellationToken stoppingToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}