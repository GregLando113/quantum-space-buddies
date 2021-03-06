﻿using QSB.Events;
using QSB.Messaging;
using QSB.Utility;
using QSB.WorldSync;

namespace QSB.ElevatorSync
{
    public class ElevatorEvent : QSBEvent<ElevatorMessage>
    {
        public override EventType Type => EventType.Elevator;

        public override void SetupListener()
        {
            GlobalMessenger<int, ElevatorDirection>.AddListener(EventNames.QSBStartLift, Handler);
        }

        public override void CloseListener()
        {
            GlobalMessenger<int, ElevatorDirection>.RemoveListener(EventNames.QSBStartLift, Handler);
        }

        private void Handler(int id, ElevatorDirection direction) => SendEvent(CreateMessage(id, direction));

        private ElevatorMessage CreateMessage(int id, ElevatorDirection direction) => new ElevatorMessage
        {
            Direction = direction,
            ObjectId = id
        };

        public override void OnReceiveRemote(ElevatorMessage message)
        {
            DebugLog.DebugWrite($"Get ElevatorMessage {message.Direction} for {message.ObjectId}");
            var elevator = WorldRegistry.GetObject<QSBElevator>(message.ObjectId);
            elevator?.RemoteCall(message.Direction);
        }
    }
}
