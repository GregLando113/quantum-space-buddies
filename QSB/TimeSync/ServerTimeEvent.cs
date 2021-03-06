﻿using QSB.Events;
using QSB.Messaging;

namespace QSB.TimeSync
{
    public class ServerTimeEvent : QSBEvent<ServerTimeMessage>
    {
        public override EventType Type => EventType.ServerTime;

        public override void SetupListener()
        {
            GlobalMessenger<float, int>.AddListener(EventNames.QSBServerTime, Handler);
        }

        public override void CloseListener()
        {
            GlobalMessenger<float, int>.RemoveListener(EventNames.QSBServerTime, Handler);
        }

        private void Handler(float time, int count) => SendEvent(CreateMessage(time, count));

        private ServerTimeMessage CreateMessage(float time, int count) => new ServerTimeMessage
        {
            AboutId = LocalPlayerId,
            ServerTime = time,
            LoopCount = count
        };

        public override void OnReceiveRemote(ServerTimeMessage message)
        {
            WakeUpSync.LocalInstance.OnClientReceiveMessage(message);
        }
    }
}
