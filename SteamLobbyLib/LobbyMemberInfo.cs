using System;

namespace SiroccoLobby.Services
{
    /// <summary>
    /// Simple data transfer object representing a lobby member. Returned by the
    /// library-level ISteamLobbyService so consumers do not need to call Steamworks directly.
    /// </summary>
    public sealed class LobbyMemberInfo
    {
        public ulong SteamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Team { get; set; }
        public int CaptainIndex { get; set; }
        public bool IsHost { get; set; }
        public bool IsReady { get; set; }
    }
}
