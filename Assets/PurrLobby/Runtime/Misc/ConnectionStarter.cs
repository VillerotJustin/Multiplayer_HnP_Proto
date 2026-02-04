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
            PurrLogger.Log("ConnectionStarter.Awake() - Initializing relay data BEFORE NetworkManager.Start()", this);
            
            if(!TryGetComponent(out _networkManager)) {
                PurrLogger.LogError($"Failed to get {nameof(NetworkManager)} component.", this);
                return;
            }
            
            _lobbyDataHolder = FindFirstObjectByType<LobbyDataHolder>();
            if(!_lobbyDataHolder) {
                PurrLogger.LogError($"Failed to get {nameof(LobbyDataHolder)} component.", this);
                return;
            }
            
            // Configure relay data in Awake so it's ready before NetworkManager.Start() runs
            ConfigureRelayData();
        }

        private void ConfigureRelayData()
        {
            if (!_networkManager)
            {
                PurrLogger.LogError($"Failed to configure relay. {nameof(NetworkManager)} is null!", this);
                return;
            }
            
            PurrLogger.Log("NetworkManager found", this);
            
            if (!_lobbyDataHolder)
            {
                PurrLogger.LogError($"Failed to configure relay. {nameof(LobbyDataHolder)} is null!", this);
                return;
            }
            
            PurrLogger.Log($"LobbyDataHolder found. Lobby IsValid: {_lobbyDataHolder.CurrentLobby.IsValid}", this);
            
            if (!_lobbyDataHolder.CurrentLobby.IsValid)
            {
                PurrLogger.LogError($"Failed to configure relay. Lobby is invalid!", this);
                return;
            }

            PurrLogger.Log($"Checking transport type: {_networkManager.transport?.GetType().Name ?? "NULL"}", this);
            
            if(_networkManager.transport is PurrTransport) {
                (_networkManager.transport as PurrTransport).roomName = _lobbyDataHolder.CurrentLobby.LobbyId;
            } 
            
#if UTP_LOBBYRELAY
            // Handle both direct UTPTransport and UTPTransport inside CompositeTransport
            UTPTransport utpTransport = null;
            
            if(_networkManager.transport is UTPTransport) {
                utpTransport = _networkManager.transport as UTPTransport;
            }
            else if(_networkManager.transport is CompositeTransport) {
                // Look for UTPTransport inside CompositeTransport
                var composite = _networkManager.transport as CompositeTransport;
                foreach(var transport in composite.transports) {
                    if(transport is UTPTransport) {
                        utpTransport = transport as UTPTransport;
                        PurrLogger.Log("Found UTPTransport inside CompositeTransport", this);
                        break;
                    }
                }
            }
            
            if(utpTransport != null) {
                var lobby = _lobbyDataHolder.CurrentLobby;
                
                PurrLogger.Log($"Configuring UTP Relay: IsOwner={lobby.IsOwner}, ServerObject={(lobby.ServerObject != null ? "EXISTS" : "NULL")}, JoinCode={(lobby.Properties.ContainsKey("JoinCode") ? lobby.Properties["JoinCode"] : "MISSING")}", this);
                
                // Validate relay data is available before initializing
                if(lobby.ServerObject == null && lobby.IsOwner) {
                    PurrLogger.LogError("Cannot configure UTP server: Relay ServerObject is null! Make sure SetAllReadyAsync() was called on the lobby provider.", this);
                    return;
                }
                
                if(!lobby.Properties.ContainsKey("JoinCode") || 
                   string.IsNullOrEmpty(lobby.Properties["JoinCode"])) {
                    PurrLogger.LogError("Cannot configure UTP client: JoinCode is missing! Make sure SetAllReadyAsync() was called on the lobby provider.", this);
                    return;
                }
                
                if(lobby.IsOwner) {
                    PurrLogger.Log("Initializing UTP Relay Server...", this);
                    utpTransport.InitializeRelayServer((Allocation)lobby.ServerObject);
                }
                else {
                    PurrLogger.Log($"Initializing UTP Relay Client with JoinCode: {lobby.Properties["JoinCode"]}", this);
                    utpTransport.InitializeRelayClient(lobby.Properties["JoinCode"]);
                }
            }
#else
            if(_networkManager.transport is UTPTransport || _networkManager.transport is CompositeTransport) {
                //P2P Connection without relay - requires manual IP/Port configuration
                PurrLogger.LogWarning("UTP transport detected but UTP_LOBBYRELAY is not defined. You need to manually configure the transport for direct connection or enable Unity Relay service.", this);
            }
#endif
        }

        private void Start()
        {
            // Start server/client - relay data should already be configured from Awake()
            if (!_networkManager || !_lobbyDataHolder || !_lobbyDataHolder.CurrentLobby.IsValid)
                return;

            if(_lobbyDataHolder.CurrentLobby.IsOwner)
            {
                _networkManager.StartServer();
            }
            else
            {
                StartCoroutine(StartClient());
            }
        }

        private IEnumerator StartClient()
        {
            yield return new WaitForSeconds(1f);
            _networkManager.StartClient();
        }
    }
}
