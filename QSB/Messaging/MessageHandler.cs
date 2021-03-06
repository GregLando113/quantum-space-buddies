﻿using OWML.Common;
using QSB.Utility;
using System;
using System.Linq;
using UnityEngine.Networking;

namespace QSB.Messaging
{
    // Extend this to create new message handlers.
    public class MessageHandler<T> where T : MessageBase, new()
    {
        public event Action<T> OnClientReceiveMessage;
        public event Action<T> OnServerReceiveMessage;

        private readonly EventType _eventType;

        public MessageHandler(EventType eventType)
        {
            _eventType = eventType + MsgType.Highest + 1;
            if (QSBNetworkManager.Instance.IsReady)
            {
                Init();
            }
            else
            {
                QSBNetworkManager.Instance.OnNetworkManagerReady += Init;
            }
        }

        private void Init()
        {
            if (NetworkServer.handlers.Keys.Contains((short)_eventType))
            {
                DebugLog.LogState($"{_eventType} HANDLER", false);
                DebugLog.ToConsole($"Warning - NetworkServer already contains a handler for EventType {_eventType}", MessageType.Warning);
                NetworkServer.handlers.Remove((short)_eventType);
            }
            NetworkServer.RegisterHandler((short)_eventType, OnServerReceiveMessageHandler);
            NetworkManager.singleton.client.RegisterHandler((short)_eventType, OnClientReceiveMessageHandler);
            DebugLog.LogState($"{_eventType} HANDLER", true);
        }

        public void SendToAll(T message)
        {
            if (!QSBNetworkManager.Instance.IsReady)
            {
                return;
            }
            NetworkServer.SendToAll((short)_eventType, message);
        }

        public void SendToServer(T message)
        {
            if (!QSBNetworkManager.Instance.IsReady)
            {
                return;
            }
            NetworkManager.singleton.client.Send((short)_eventType, message);
        }

        private void OnClientReceiveMessageHandler(NetworkMessage netMsg)
        {
            var message = netMsg.ReadMessage<T>();
            OnClientReceiveMessage?.Invoke(message);
        }

        private void OnServerReceiveMessageHandler(NetworkMessage netMsg)
        {
            var message = netMsg.ReadMessage<T>();
            OnServerReceiveMessage?.Invoke(message);
        }

    }
}
