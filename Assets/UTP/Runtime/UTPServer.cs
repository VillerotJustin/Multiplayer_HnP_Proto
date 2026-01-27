#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLEUTPWORKS
#endif

#if UTP_LOBBYRELAY
#define UTP_NET_PACKAGE
#endif

using System;
using PurrNet.Transports;
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PurrNet.Logging;
// Unity Transport (UTP) equivalents
#endif

namespace PurrNet.UTP
{
    public class UTPServer
    {
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
        const int MAX_MESSAGES = 256;

        // TODO: Add UTP listen socket/endpoint handle
        private object _listenSocket; // Replace with actual UTP listen handle type

        private bool _isDedicated;
        static byte[] buffer = new byte[1024];

        // TODO: Add UTP connection status change callback/event handler

        private readonly List<object> _connections = new List<object>(); // Replace with actual UTP connection type
        private readonly Dictionary<int, object> _connectionById = new Dictionary<int, object>();
        private readonly Dictionary<object, int> _idByConnection = new Dictionary<object, int>();

        // TODO: Add UTP message buffer/queue structure
#endif

#pragma warning disable CS0067 // Event is never used
        public event Action<int> onRemoteConnected;
        public event Action<int> onRemoteDisconnected;
        public event Action<int, ByteData> onDataReceived;
#pragma warning restore CS0067 // Event is never used

#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
        public bool listening => _listenSocket != null; // TODO: Replace with proper UTP listen socket check
#else
        public bool listening => false;
#endif

        public void Listen(ushort port, bool dedicated = false)
        {
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
            _isDedicated = dedicated;

            // TODO: Implement UTP listen socket creation
            // Example using Unity Transport:
            // var endpoint = NetworkEndPoint.AnyIpv4;
            // endpoint.Port = port;
            // _driver.Bind(endpoint);
            // _driver.Listen();

            PostListen();
#endif
        }

        public void ListenP2P(bool dedicated = false)
        {
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
            _isDedicated = dedicated;

            // TODO: Implement UTP P2P listen via Unity Lobby/Relay
            // Example: Setup relay allocation and create listen endpoint
            // using Unity Relay service

            PostListen();
#endif
        }

#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
        private void PostListen()
        {
            // TODO: Check if listen socket is valid
            // if (_listenSocket == null)
            // {
            //     PurrLogger.LogError("Failed to create listen socket.");
            //     return;
            // }

            // TODO: Setup connection status change callback/handler
            // Example: Register for connection events from NetworkDriver
        }
#endif

        public void SendMessages()
        {
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
            for (var i = 0; i < _connections.Count; i++)
            {
                var conn = _connections[i];

                // TODO: Check if connection is valid
                // if (conn == null)
                //     continue;

                // TODO: Flush messages for this connection
                // Example: _driver.ScheduleFlushSend(default).Complete();
            }
#endif
        }

        public void ReceiveMessages()
        {
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
            for (var i = 0; i < _connections.Count; i++)
            {
                var conn = _connections[i];

                if (!_idByConnection.TryGetValue(conn, out var connId))
                    continue;

                // TODO: Implement message receiving from UTP
                // Example:
                // DataStreamReader stream;
                // NetworkEvent.Type cmd;
                // while ((cmd = conn.PopEvent(_driver, out stream)) != NetworkEvent.Type.Empty)
                // {
                //     if (cmd == NetworkEvent.Type.Data)
                //     {
                //         int packetLength = stream.Length;
                //         MakeSureBufferCanFit(packetLength);
                //         stream.ReadBytes(buffer, packetLength);
                //         var byteData = new ByteData(buffer, 0, packetLength);
                //         onDataReceived?.Invoke(connId, byteData);
                //     }
                // }
            }
#endif
        }

        public void Kick(int id)
        {
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
            if (!_connectionById.TryGetValue(id, out var conn))
                return;

            // TODO: Disconnect the connection
            // Example: _driver.Disconnect(conn);
#endif
        }

#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
        private static void MakeSureBufferCanFit(int packetLength)
        {
            if (buffer.Length < packetLength)
                Array.Resize(ref buffer, packetLength);
        }
#endif
        public void SendToConnection(int connId, ByteData data, Channel channel)
        {
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
            if (!_connectionById.TryGetValue(connId, out var conn))
                return;

            MakeSureBufferCanFit(data.length);

            // TODO: Implement UTP send based on channel type
            // byte sendFlag = channel switch {
            //     Channel.Unreliable => /* UTP Unreliable flag */,
            //     Channel.UnreliableSequenced => /* UTP flag */,
            //     Channel.ReliableOrdered => /* UTP Reliable flag */,
            //     Channel.ReliableUnordered => /* UTP flag */,
            //     _ => 0
            // };

            try
            {
                // TODO: Send message using UTP NetworkDriver
                // Example: _driver.BeginSend(conn, out var writer);
                //          writer.WriteBytes(data.data, data.offset, data.length);
                //          _driver.EndSend(writer);
            }
            catch (Exception e)
            {
                PurrLogger.LogException(e);
            }
#endif
        }


#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
        private int _nextConnectionId;

        private void AddConnection(object connection) // TODO: Replace with actual UTP connection type
        {
            int id = _nextConnectionId++;
            _connections.Add(connection);
            _connectionById.Add(id, connection);
            _idByConnection.Add(connection, id);

            onRemoteConnected?.Invoke(id);
        }

        private void RemoveConnection(object connection) // TODO: Replace with actual UTP connection type
        {
            if (_connections.Remove(connection) && _idByConnection.Remove(connection, out var _id))
            {
                _connectionById.Remove(_id);
                onRemoteDisconnected?.Invoke(_id);
            }
        }

        private void OnRemoteConnectionState(/* TODO: Add UTP connection state change parameters */)
        {
            // TODO: Implement connection state change handling
            // Example: Handle NetworkEvent.Type.Connect and NetworkEvent.Type.Disconnect
            // from NetworkDriver
            
            // switch (eventType)
            // {
            //     case NetworkEvent.Type.Connect:
            //     {
            //         // Accept connection
            //         AddConnection(connection);
            //         break;
            //     }
            //     case NetworkEvent.Type.Disconnect:
            //     {
            //         RemoveConnection(connection);
            //         break;
            //     }
            // }
        }
#endif

        public void Stop()
        {
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
            // TODO: Cleanup connection status change callbacks/handlers
            
            // TODO: Check if listen socket is valid
            // if (_listenSocket == null)
            //     return;

            for (var o = 0; o < _connections.Count; o++)
            {
                try
                {
                    var conn = _connections[o];
                    // TODO: Close/disconnect each connection
                    // Example: _driver.Disconnect(conn);
                }
                catch
                {
                    // ignored
                }
            }

            _connections.Clear();
            _connectionById.Clear();
            _idByConnection.Clear();

            try
            {
                // TODO: Close listen socket
                // Example: if (_driver.IsCreated) _driver.Dispose();
            }
            catch
            {
                // ignored
            }

            _listenSocket = null; // TODO: Set to proper invalid state
#endif
        }
    }
}