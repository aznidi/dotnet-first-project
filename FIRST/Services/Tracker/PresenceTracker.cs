using System.Collections.Concurrent;

public interface IPresenceTracker
{
    void Connected(int userId, string connectionId);
    void Disconnected(int userId, string connectionId);
    bool IsOnline(int userId);
}

public class PresenceTracker : IPresenceTracker
{
    private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, byte>> _map = new();

    public void Connected(int userId, string connectionId)
    {
        var connections = _map.GetOrAdd(userId, _ => new ConcurrentDictionary<string, byte>());
        connections.TryAdd(connectionId, 0);
    }

    public void Disconnected(int userId, string connectionId)
    {
        if (_map.TryGetValue(userId, out var connections))
        {
            connections.TryRemove(connectionId, out _);
            if (connections.IsEmpty) _map.TryRemove(userId, out _);
        }
    }

    public bool IsOnline(int userId) => _map.ContainsKey(userId);
}
