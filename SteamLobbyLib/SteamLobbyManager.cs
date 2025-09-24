using System;
using Steamworks;

namespace SteamLobbyLib;

public class SteamLobbyManager
{
    private readonly ILobbyEvents _events;
    private readonly Action<string> _log;
    private readonly bool _enableLogging;

    public SteamLobbyManager(ILobbyEvents events, Action<string>? logger = null, bool enableLogging = true)
    {
        _events = events;
        _enableLogging = enableLogging;
        _log = enableLogging ? logger ?? Console.WriteLine : _ => { };
        SteamCallbackBinder.Bind(events);
        Log("Manager", "SteamCallbackBinder bound.");
    }

    public void Initialize()
    {
        if (!SteamAPI.Init())
            throw new InvalidOperationException("SteamAPI failed to initialize. Check DLL placement and architecture.");
        Log("Lifecycle", $"Steam initialized: {SteamAPI.IsSteamRunning()}");
    }

    public void Shutdown()
    {
        SteamAPI.Shutdown();
        Log("Lifecycle", "Steam shutdown complete.");
    }

    public void Tick()
    {
        SteamAPI.RunCallbacks();
        Log("Lifecycle", "SteamAPI callbacks ticked.");
    }

    public void CreateLobby(int maxPlayers)
    {
        Log("Lobby", $"Creating lobby with maxPlayers={maxPlayers}");
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, maxPlayers);
    }

    public void JoinLobby(LobbyId lobbyId)
    {
        Log("Lobby", $"Joining lobby {lobbyId}");
        SteamMatchmaking.JoinLobby(lobbyId.ToSteamId());
    }

    public void LeaveLobby(LobbyId lobbyId)
    {
        Log("Lobby", $"Leaving lobby {lobbyId}");
        SteamMatchmaking.LeaveLobby(lobbyId.ToSteamId());
    }

    public void SetLobbyData(LobbyId lobbyId, string key, string value)
    {
        Log("Lobby", $"Setting lobby data: {key} = {value} for {lobbyId}");
        SteamMatchmaking.SetLobbyData(lobbyId.ToSteamId(), key, value);
    }

    public void RequestLobbyList(int count = 10)
    {
        Log("Lobby", $"Requesting lobby list (max {count})");
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(count);
        SteamMatchmaking.RequestLobbyList();
    }

    public LobbyData GetLobbyData(LobbyId lobbyId)
    {
        var steamId = lobbyId.ToSteamId();
        var name = SteamMatchmaking.GetLobbyData(steamId, "name");
        var maxPlayers = SteamMatchmaking.GetLobbyMemberLimit(steamId);
        var currentPlayers = SteamMatchmaking.GetNumLobbyMembers(steamId);

        Log("Lobby", $"Hydrating LobbyData for {lobbyId}: name='{name}', players={currentPlayers}/{maxPlayers}");

        return new LobbyData
        {
            Id = lobbyId,
            Name = name,
            MaxPlayers = maxPlayers,
            CurrentPlayers = currentPlayers
        };
    }

    private void Log(string category, string message)
    {
        if (_enableLogging)
            _log($"[{category}] {message}");
    }
}
