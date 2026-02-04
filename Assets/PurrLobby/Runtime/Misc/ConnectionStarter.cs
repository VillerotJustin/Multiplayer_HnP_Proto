using System.Collections;
using PurrNet;
using PurrNet.Logging;
using PurrNet.Transports;
using UnityEngine;

#if UTP_LOBBYRELAY
using PurrNet.UTP;
using Unity.Services.Relay.Models;
#endif

namespace PurrLobby
{
    public class ConnectionStarter : MonoBehaviour
    {
        private NetworkManager _networkManager;
        private LobbyDataHolder _lobbyDataHolder;
        
        private void Awake()
        {
            if(!TryGetComponent(out _networkManager)) {
                PurrLogger.LogError($"Failed to get {nameof(NetworkManager)} component.", this);
            }
            
            _lobbyDataHolder = FindFirstObjectByType<LobbyDataHolder>();
            if(!_lobbyDataHolder)
                PurrLogger.LogError($"Failed to get {nameof(LobbyDataHolder)} component.", this);
        }

        private void Start()
        {
            if (!_networkManager)
            {
                PurrLogger.LogError($"Failed to start connection. {nameof(NetworkManager)} is null!", this);
                return;
            }
            
            if (!_lobbyDataHolder)
            {
                PurrLogger.LogError($"Failed to start connection. {nameof(LobbyDataHolder)} is null!", this);
                return;
            }
            
            if (!_lobbyDataHolder.CurrentLobby.IsValid)
            {
                PurrLogger.LogError($"Failed to start connection. Lobby is invalid!", this);
                return;
            }

            if(_networkManager.transport is PurrTransport) {
                (_networkManager.transport as PurrTransport).roomName = _lobbyDataHolder.CurrentLobby.LobbyId;
            } 
            
#if UTP_LOBBYRELAY
            else if(_networkManager.transport is UTPTransport) {
                // Validate relay data is available before initializing
                if(_lobbyDataHolder.CurrentLobby.ServerObject == null && _lobbyDataHolder.CurrentLobby.IsOwner) {
                    PurrLogger.LogError("Cannot start UTP server: Relay ServerObject is null! Make sure SetAllReadyAsync() was called on the lobby provider.", this);
                    return;
                }
                
                if(!_lobbyDataHolder.CurrentLobby.Properties.ContainsKey("JoinCode") || 
                   string.IsNullOrEmpty(_lobbyDataHolder.CurrentLobby.Properties["JoinCode"])) {
                    PurrLogger.LogError("Cannot connect UTP client: JoinCode is missing! Make sure SetAllReadyAsync() was called on the lobby provider.", this);
                    return;
                }
                
                if(_lobbyDataHolder.CurrentLobby.IsOwner) {
                    (_networkManager.transport as UTPTransport).InitializeRelayServer((Allocation)_lobbyDataHolder.CurrentLobby.ServerObject);
                }
                (_networkManager.transport as UTPTransport).InitializeRelayClient(_lobbyDataHolder.CurrentLobby.Properties["JoinCode"]);
            }
#else
            else if(_networkManager.transport is UTPTransport) {
                //P2P Connection without relay - requires manual IP/Port configuration
                PurrLogger.LogWarning("UTP transport detected but UTP_LOBBYRELAY is not defined. You need to manually configure the transport for direct connection or enable Unity Relay service.", this);
            }
#endif

            if(_lobbyDataHolder.CurrentLobby.IsOwner)
                _networkManager.StartServer();
            StartCoroutine(StartClient());
        }

        private IEnumerator StartClient()
        {
            yield return new WaitForSeconds(1f);
            _networkManager.StartClient();
        }
    }
}
