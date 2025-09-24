using SteamLobbyLib;
using Steamworks;

namespace SteamLobbyTester;

internal class SteamLobbyTester : ILobbyEvents
{
    private SteamLobbyManager _manager = null!;

    private static void Main()
    {
        var program = new SteamLobbyTester();
        program.Run();
    }

    private void Run()
    {
        Console.CancelKeyPress += (_, e) =>
        {
            Console.WriteLine("[Shutdown] Ctrl+C detected. Cleaning up...");
            _manager?.Shutdown(); // Add this method to SteamLobbyManager
            SteamAPI.Shutdown();
            e.Cancel = true; // Prevent default termination
            Environment.Exit(0);
        };

        _manager = new SteamLobbyManager(this, Console.WriteLine, enableLogging: true);
        _manager.Initialize();
        _manager.RequestLobbyList();

        while (true)
        {
            _manager.Tick();
            Thread.Sleep(100);
        }
    }

    public void OnLobbyListReceived(List<LobbyData> lobbies)
    {
        Console.WriteLine($"Received {lobbies.Count} lobbies:");
        foreach (var lobby in lobbies)
            Console.WriteLine($"- {lobby.Id}: {lobby.Name} ({lobby.CurrentPlayers}/{lobby.MaxPlayers})");
    }

    public void OnLobbyJoined(LobbyId lobbyId) =>
        Console.WriteLine($"Joined lobby: {lobbyId}");

    public void OnLobbyDataUpdated(LobbyId lobbyId) =>
        Console.WriteLine($"Lobby data updated: {lobbyId}");

    public void OnLobbyMemberChanged(LobbyId lobbyId, LobbyId memberId, EChatMemberStateChange change) =>
        Console.WriteLine($"Member change in {lobbyId}: {memberId} → {change}");
}