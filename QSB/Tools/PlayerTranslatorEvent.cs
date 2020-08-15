﻿using QSB.Events;
using QSB.Messaging;
using QSB.Utility;

namespace QSB.Tools
{
    public class PlayerTranslatorEvent : QSBEvent<ToggleMessage>
    {
        public override MessageType Type => MessageType.TranslatorActiveChange;

        public override void SetupListener()
        {
            GlobalMessenger.AddListener(EventNames.EquipTranslator, () => SendEvent(CreateMessage(true)));
            GlobalMessenger.AddListener(EventNames.UnequipTranslator, () => SendEvent(CreateMessage(false)));
        }

        public override void CloseListener()
        {
            GlobalMessenger.RemoveListener(EventNames.EquipTranslator, () => SendEvent(CreateMessage(true)));
            GlobalMessenger.RemoveListener(EventNames.UnequipTranslator, () => SendEvent(CreateMessage(false)));
        }

        private ToggleMessage CreateMessage(bool value) => new ToggleMessage
        {
            SenderId = LocalPlayerId,
            ToggleValue = value
        };

        public override void OnReceiveRemote(ToggleMessage message)
        {
            var player = PlayerRegistry.GetPlayer(message.SenderId);
            player.UpdateState(State.Translator, message.ToggleValue);
            if (!IsInUniverse)
            {
                return;
            }
            player.Translator?.ChangeEquipState(message.ToggleValue);
        }

        public override void OnReceiveLocal(ToggleMessage message)
        {
            PlayerRegistry.LocalPlayer.UpdateState(State.Translator, message.ToggleValue);
        }
    }
}
