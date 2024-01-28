

using Maturaball_Tischreservierung.Models;

namespace Maturaball_Tischreservierung.Services

{
    public class SessionBlacklistService
    {
        private readonly ApiConfig _configuration;
        private readonly HashSet<Guid> _blacklistedSessions = new();
        private readonly Dictionary<Guid, DateTime> _blacklistedSessionsWithExpiration = new();

        public SessionBlacklistService(ApiConfig config)
        {
            _configuration = config;
        }

        public async Task BlacklistSessionAsync(Guid sessionId)
        {
            await Task.Run(() =>
            {
                _blacklistedSessions.Add(sessionId);
                _blacklistedSessionsWithExpiration.TryAdd(sessionId, DateTime.UtcNow.Add(_configuration.AccessTokenLifetime));
            });
        }
        public async Task<bool> IsSessionBlacklistedAsync(Guid sessionId)
        {
            RemoveOldBlacklists();
            var result = await Task.Run(() => _blacklistedSessions.Contains(sessionId));
            return result;
        }

        private async void RemoveOldBlacklists()
        {
            await Task.Run(() =>
            {
                var expiredBlacklists = _blacklistedSessionsWithExpiration.Where(x => x.Value < DateTime.UtcNow).ToList();
                foreach (var expiredBlacklist in expiredBlacklists)
                {
                    _blacklistedSessions.Remove(expiredBlacklist.Key);
                    _blacklistedSessionsWithExpiration.Remove(expiredBlacklist.Key);
                }
            });
        }
    }
}
