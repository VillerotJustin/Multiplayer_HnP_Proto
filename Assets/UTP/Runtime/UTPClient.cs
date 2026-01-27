#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLEUTPWORKS
#endif

#if UTP_LOBBYRELAY
#define UTP_NET_PACKAGE
#endif

using System;
using System.Collections;
using PurrNet.Transports;
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
using System.Runtime.InteropServices;
using PurrNet.Logging;
// Unity Lobby Equivalent of Steamworks
#endif

namespace PurrNet.UTP
{
    public class UTPClient
    {
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
        const int MAX_MESSAGES = 256;
        // TODO: Add UTP-specific connection callback/event handling
        
        // TODO: Add UTP connection handle/identifier
        private object _connection; // Replace with actual UTP connection type
        private bool _isDedicated;
        static byte[] buffer = new byte[1024];
        // TODO: Add UTP message buffer/queue structure
#endif

#pragma warning disable CS0067 // Event is never used
        public event Action<ByteData> onDataReceived;
#pragma warning restore CS0067 // Event is never used
        public event Action<ConnectionState> onConnectionState;

        private ConnectionState _state = ConnectionState.Disconnected;

        public ConnectionState connectionState
        {
            get => _state;
            set
            {
                if (_state == value)
                    return;

                _state = value;
                onConnectionState?.Invoke(_state);
            }
        }

        public IEnumerator Connect(string address, ushort port, bool dedicated = false)
        {
            yield return null;
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
            _isDedicated = dedicated;

            // TODO: Implement UTP connection via IP address and port
            // Example: Use Unity Transport NetworkDriver to connect
            
            PostConnect();
#endif
        }

        public IEnumerator ConnectP2P(string lobbyId, bool dedicated = false)
        {
            yield return null;
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
            // TODO: Validate lobbyId or relay code
            if (string.IsNullOrEmpty(lobbyId))
            {
                PurrLogger.LogError("Invalid Lobby ID provided as address to connect");
                yield break;
            }

            _isDedicated = dedicated;

            // TODO: Connect via Unity Lobby/Relay P2P
            // Example: Use Unity Lobby and Relay services for P2P connection
            
            PostConnect();
#endif
        }

        public void Send(ByteData data, Channel channel)
        {
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
            // TODO: Check if connection is valid
            // if (_connection == null) return;

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
                // Example: _driver.BeginSend(_connection, out var writer);
                //          writer.WriteBytes(data.data, data.offset, data.length);
                //          _driver.EndSend(writer);
            }
            catch (Exception e)
            {
                PurrLogger.LogException(e);
            }
#endif
        }

        public void SendMessages()
        {
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
            // TODO: Flush messages if needed by UTP
            // Example: _driver.ScheduleFlushSend(default).Complete();
#endif
        }

        public void ReceiveMessages()
        {
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
            // TODO: Implement message receiving from UTP
            // Example:
            // DataStreamReader stream;
            // NetworkEvent.Type cmd;
            // while ((cmd = _connection.PopEvent(_driver, out stream)) != NetworkEvent.Type.Empty)
            // {
            //     if (cmd == NetworkEvent.Type.Data)
            //     {
            //         int packetLength = stream.Length;
            //         MakeSureBufferCanFit(packetLength);
            //         stream.ReadBytes(buffer, packetLength);
            //         var byteData = new ByteData(buffer, 0, packetLength);
            //         onDataReceived?.Invoke(byteData);
            //     }
            // }
#endif
        }
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS

        private static void MakeSureBufferCanFit(int packetLength)
        {
            if (buffer.Length < packetLength)
                Array.Resize(ref buffer, packetLength);
        }

        private void PostConnect()
        {
            // TODO: Check if connection is valid
            // if (_connection == null)
            // {
            //     connectionState = ConnectionState.Disconnecting;
            //     connectionState = ConnectionState.Disconnected;
            //     PurrLogger.LogError("Failed to connect to host");
            //     return;
            // }

            connectionState = ConnectionState.Connecting;
            // TODO: Setup connection state change callback/handler
        }

        private void OnLocalConnectionState(/* TODO: Add UTP connection state parameter */)
        {
            // TODO: Implement connection state change handling
            // Example: Check connection events from NetworkDriver
            // switch (connectionEvent)
            // {
            //     case NetworkEvent.Type.Connect:
            //         connectionState = ConnectionState.Connected;
            //         break;
            //     case NetworkEvent.Type.Disconnect:
            //         connectionState = ConnectionState.Disconnecting;
            //         connectionState = ConnectionState.Disconnected;
            //         break;
            // }
        }

        void Disconnect()
        {
            // TODO: Check if connection is valid and disconnect
            // if (_connection != null)
            // {
            //     if (connectionState != ConnectionState.Disconnected)
            //         connectionState = ConnectionState.Disconnecting;
            //
            //     try
            //     {
            //         // Example: _driver.Disconnect(_connection);
            //     }
            //     catch
            //     {
            //         // ignored
            //     }
            //
            //     connectionState = ConnectionState.Disconnected;
            //     _connection = null;
            // }
        }
#endif

        public void Stop()
        {
#if UTP_NET_PACKAGE && !DISABLEUTPWORKS
            // TODO: Cleanup connection state handlers/callbacks
            
            Disconnect();
            
            // TODO: Dispose UTP NetworkDriver if needed
            // Example: if (_driver.IsCreated) _driver.Dispose();
#endif
        }
    }
}