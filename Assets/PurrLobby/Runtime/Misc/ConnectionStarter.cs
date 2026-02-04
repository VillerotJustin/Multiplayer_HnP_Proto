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
        
        // Static to persist across duplicate instances if scene loads multiple times
        private static bool _hasStarted = false;
        
        private void Awake()
        {
            PurrLogger.Log("ConnectionStarter.Awake() - Initializing relay data BEFORE NetworkManager.Start()", this);
            
            if(!TryGetComponent(out _networkManager)) {
                PurrLogger.LogError($"Failed to get {nameof(NetworkManager)} component.", this);
                return;
            }
            
            // Prevent NetworkManager from auto-starting in its own Start() method
            _networkManager.enabled = false;
            
            _lobbyDataHolder = FindFirstObjectByType<LobbyDataHolder>();
            if(!_lobbyDataHolder) {
                PurrLogger.LogError($"Failed to get {nameof(LobbyDataHolder)} component.", this);
                return;
            }
            
            // Configure relay data in Awake so it's ready before NetworkManager.Start() runs
            _ = ConfigureRelayData();
        }

        private async System.Threading.Tasks.Task ConfigureRelayData()
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
                    PurrLogger.Log("Initializing UTP Relay Server (for host)...", this);
                    
                    // Unity Relay host-client uses traditional client-server architecture, not P2P
                    utpTransport.peerToPeer = false;
                    
                    bool serverInit = utpTransport.InitializeRelayServer((Allocation)lobby.ServerObject);
                    PurrLogger.Log($"Relay Server initialized: {serverInit}", this);
                    
                    // Host also needs relay client to connect to itself through relay
                    PurrLogger.Log($"Initializing UTP Relay Client for host with JoinCode: {lobby.Properties["JoinCode"]}", this);
                    try {
                        bool clientInit = await utpTransport.InitializeRelayClient(lobby.Properties["JoinCode"]);
                        PurrLogger.Log($"Relay Client initialized for host: {clientInit}", this);
                    }
                    catch (System.Exception ex) {
                        PurrLogger.LogError($"Failed to initialize relay client for host: {ex.Message}", this);
                    }
                }
                else {
                    PurrLogger.Log($"Initializing UTP Relay Client with JoinCode: {lobby.Properties["JoinCode"]}", this);
                    
                    // Unity Relay host-client uses traditional client-server architecture, not P2P
                    utpTransport.peerToPeer = false;
                    
                    try {
                        bool clientInit = await utpTransport.InitializeRelayClient(lobby.Properties["JoinCode"]);
                        PurrLogger.Log($"Relay Client initialized: {clientInit}", this);
                    }
                    catch (System.Exception ex) {
                        PurrLogger.LogError($"Failed to initialize relay client: {ex.Message}", this);
                    }
                }
                
                // Now that relay is configured, start the network
                StartNetworkAfterRelayConfig();
            }
#else
            if(_networkManager.transport is UTPTransport || _networkManager.transport is CompositeTransport) {
                //P2P Connection without relay - requires manual IP/Port configuration
                PurrLogger.LogWarning("UTP transport detected but UTP_LOBBYRELAY is not defined. You need to manually configure the transport for direct connection or enable Unity Relay service.", this);
            }
#endif
        }

        private void StartNetworkAfterRelayConfig()
        {
            // Prevent multiple calls if scene loads multiple times
            if (_hasStarted)
            {
                PurrLogger.LogWarning("StartNetworkAfterRelayConfig() called again - ignoring to prevent duplicate server/client start", this);
                return;
            }
            
            if (!_networkManager || !_lobbyDataHolder || !_lobbyDataHolder.CurrentLobby.IsValid)
                return;

            // Note: NetworkManager remains disabled to prevent its Start() from auto-executing
            // We can still call StartServer/StartClient methods on disabled components

            _hasStarted = true;

            if(_lobbyDataHolder.CurrentLobby.IsOwner)
            {
                PurrLogger.Log("Starting as Host (Server + Client through relay)", this);
                // Start server first, then client after a delay
                _networkManager.StartServer();
                StartCoroutine(StartHostClient());
            }
            else
            {
                PurrLogger.Log("Starting as Client", this);
                StartCoroutine(StartClient());
            }
        }

        private IEnumerator StartHostClient()
        {
            // Give server time to fully initialize before client connects
            yield return new WaitForSeconds(2f);
            PurrLogger.Log("Starting host's client connection...", this);
            
            // Verify relay data is set
            var transport = _networkManager.GetComponent<UTPTransport>();
            if (transport != null)
            {
                var relayClientDataField = transport.GetType().GetField("_relayClientData", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (relayClientDataField != null)
                {
                    var relayData = relayClientDataField.GetValue(transport);
                    PurrLogger.Log($"RelayClientData before StartClient: {(relayData != null ? "SET" : "NULL")}", this);
                }
            }
            
            _networkManager.StartClient();
        }

        private IEnumerator StartClient()
        {
            yield return new WaitForSeconds(1f);
            _networkManager.StartClient();
        }
    }
}
