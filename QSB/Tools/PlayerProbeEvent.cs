﻿using QSB.Events;
using QSB.Messaging;

namespace QSB.Tools
{
    public class PlayerProbeEvent : QSBEvent<ToggleMessage>
    {
        public override EventType Type => EventType.ProbeActiveChange;

        public override void SetupListener()
        {
            GlobalMessenger<SurveyorProbe>.AddListener(EventNames.LaunchProbe, HandleLaunch);
            GlobalMessenger<SurveyorProbe>.AddListener(EventNames.RetrieveProbe, HandleRetrieve);
        }

        public override void CloseListener()
        {
            GlobalMessenger<SurveyorProbe>.RemoveListener(EventNames.LaunchProbe, HandleLaunch);
            GlobalMessenger<SurveyorProbe>.RemoveListener(EventNames.RetrieveProbe, HandleRetrieve);
        }

        private void HandleLaunch(SurveyorProbe probe) => SendEvent(CreateMessage(true));
        private void HandleRetrieve(SurveyorProbe probe) => SendEvent(CreateMessage(false));

        private ToggleMessage CreateMessage(bool value) => new ToggleMessage
        {
            AboutId = LocalPlayerId,
            ToggleValue = value
        };

        public override void OnReceiveRemote(ToggleMessage message)
        {
            var player = PlayerRegistry.GetPlayer(message.AboutId);
            player.UpdateState(State.ProbeActive, message.ToggleValue);
            player.Probe?.SetState(message.ToggleValue);
        }

        public override void OnReceiveLocal(ToggleMessage message)
        {
            PlayerRegistry.LocalPlayer.UpdateState(State.ProbeActive, message.ToggleValue);
        }
    }
}
