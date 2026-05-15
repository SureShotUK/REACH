namespace CompaniesHouseSearch.Helpers;

public class RateLimiter
{
    private readonly TimeSpan _delay;
    private DateTime _lastRequest = DateTime.MinValue;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public RateLimiter(int requestsPerSecond = 2)
    {
        _delay = TimeSpan.FromSeconds(1.0 / requestsPerSecond);
    }

    public async Task WaitAsync()
    {
        await _lock.WaitAsync();
        try
        {
            var elapsed = DateTime.UtcNow - _lastRequest;
            if (elapsed < _delay)
                await Task.Delay(_delay - elapsed);
            _lastRequest = DateTime.UtcNow;
        }
        finally
        {
            _lock.Release();
        }
    }
}
