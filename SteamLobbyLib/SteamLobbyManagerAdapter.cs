using System.Collections.Generic;
using System.Linq;
using Steamworks;
using SiroccoLobby.Services;

namespace SteamLobbyLib
{
    /// <summary>
    /// Adapter that exposes a library-side implementation of <see cref="SiroccoLobby.Services.ISteamLobbyService"/>.
    /// This keeps the Steam boundary inside the SteamLobbyLib project and provides a stable API
    /// for the mod/controller code to consume.
    /// </summary>
    public class SteamLobbyManagerAdapter : ISteamLobbyService
    {
        private readonly SteamLobbyManager _manager;

        public SteamLobbyManagerAdapter(SteamLobbyManager manager)
        {
            _manager = manager;
        }

        private CSteamID ToSteamID(object? obj)
        {
            if (obj is ulong ul) return new CSteamID(ul);
            if (obj is CSteamID cs) return cs;
            if (obj is LobbyId lid) return lid.ToSteamId();
            return CSteamID.Nil;
        }

        public object GetLocalSteamId() => SteamUser.GetSteamID().m_SteamID;

        public void RequestLobbyList() => _manager.RequestLobbyList();

        public IEnumerable<object> GetCachedLobbies(int max = 20)
        {
            return _manager.CachedLobbies
                .Take(max)
                .Select(l => (object)l.Id.Value);
        }

        public void CreateLobby(int visibility, int maxPlayers)
        {
            // SLL currently treats visibility as public by default.
            _manager.CreateLobby(maxPlayers);
        }

        public void JoinLobby(object lobbyId)
        {
            _manager.JoinLobby(new LobbyId(ToSteamID(lobbyId).m_SteamID));
        }

        public void LeaveLobby(object lobbyId)
        {
            _manager.LeaveLobby();
        }

        public object GetLobbyOwner(object lobbyId)
        {
            return SteamMatchmaking.GetLobbyOwner(ToSteamID(lobbyId)).m_SteamID;
        }

        public string GetLobbyName(object? lobbyId = null)
        {
            CSteamID id;
            if (lobbyId != null)
            {
                id = ToSteamID(lobbyId);
            }
            else if (_manager.CurrentLobby.HasValue)
            {
                id = _manager.CurrentLobby.Value.ToSteamId();
            }
            else
            {
                id = CSteamID.Nil;
            }

            if (id == CSteamID.Nil) return "Lobby";
            var name = SteamMatchmaking.GetLobbyData(id, "name");
            if (string.IsNullOrEmpty(name)) return $"Lobby {id.m_SteamID}";
            return name;
        }

        public string GetLobbyData(object lobbyId, string key)
        {
            return SteamMatchmaking.GetLobbyData(ToSteamID(lobbyId), key);
        }

        public string GetSteamIDString(object steamId)
        {
            if (steamId is CSteamID cid) return cid.m_SteamID.ToString();
            if (steamId is ulong ul) return ul.ToString();
            return steamId?.ToString() ?? "";
        }

        public void SetLobbyData(object lobbyId, string key, string value)
        {
            SteamMatchmaking.SetLobbyData(ToSteamID(lobbyId), key, value);
        }

        public int GetMemberCount(object lobbyId)
        {
            return SteamMatchmaking.GetNumLobbyMembers(ToSteamID(lobbyId));
        }

        public int GetMemberLimit(object lobbyId)
        {
            return SteamMatchmaking.GetLobbyMemberLimit(ToSteamID(lobbyId));
        }

        public object GetLobbyMemberByIndex(object lobbyId, int index)
        {
            return SteamMatchmaking.GetLobbyMemberByIndex(ToSteamID(lobbyId), index).m_SteamID;
        }

        public void SetLobbyMemberData(object lobbyId, string key, string value)
        {
            SteamMatchmaking.SetLobbyMemberData(ToSteamID(lobbyId), key, value);
        }

        public string GetLobbyMemberData(object lobbyId, object userId, string key)
        {
            return SteamMatchmaking.GetLobbyMemberData(ToSteamID(lobbyId), ToSteamID(userId), key);
        }

        public string GetLocalPersonaName()
        {
            return SteamFriends.GetPersonaName();
        }

        public string GetFriendPersonaName(object userId)
        {
            return SteamFriends.GetFriendPersonaName(ToSteamID(userId));
        }

        public IEnumerable<SiroccoLobby.Services.LobbyMemberInfo> GetLobbyMembers(object lobbyId)
        {
            var list = new List<SiroccoLobby.Services.LobbyMemberInfo>();
            var steamId = ToSteamID(lobbyId);
            if (steamId == CSteamID.Nil) return list;

            int count = SteamMatchmaking.GetNumLobbyMembers(steamId);
            var owner = SteamMatchmaking.GetLobbyOwner(steamId);

            for (int i = 0; i < count; i++)
            {
                var userId = SteamMatchmaking.GetLobbyMemberByIndex(steamId, i);
                if (userId == CSteamID.Nil) continue;

                string teamStr = SteamMatchmaking.GetLobbyMemberData(steamId, userId, "team");
                string captainStr = SteamMatchmaking.GetLobbyMemberData(steamId, userId, "captain_index");
                string readyStr = SteamMatchmaking.GetLobbyMemberData(steamId, userId, "is_ready");

                int.TryParse(teamStr, out int team);
                if (team == 0) team = 1;
                int.TryParse(captainStr, out int captain);
                bool.TryParse(readyStr, out bool isReady);

                string name = SteamFriends.GetFriendPersonaName(userId);
                if (userId == SteamUser.GetSteamID())
                    name = SteamFriends.GetPersonaName();

                list.Add(new SiroccoLobby.Services.LobbyMemberInfo
                {
                    SteamId = userId.m_SteamID,
                    Name = name,
                    Team = team,
                    CaptainIndex = captain,
                    IsHost = (userId == owner),
                    IsReady = isReady
                });
            }

            return list;
        }

        public bool CSteamIDEquals(object id1, object id2)
        {
            return ToSteamID(id1) == ToSteamID(id2);
        }

        public object CSteamIDNil()
        {
            return CSteamID.Nil.m_SteamID;
        }
    }
}
