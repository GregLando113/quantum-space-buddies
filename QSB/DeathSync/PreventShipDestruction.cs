﻿using Harmony;
using OWML.ModHelper.Events;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace QSB.DeathSync
{
    public class PreventShipDestruction : MonoBehaviour
    {
        private void Awake()
        {
            QSB.Helper.HarmonyHelper.Transpile<ShipDetachableLeg>("Detach", typeof(Patch), nameof(Patch.ReturnNull));
            QSB.Helper.HarmonyHelper.Transpile<ShipDetachableModule>("Detach", typeof(Patch), nameof(Patch.ReturnNull));

            QSB.Helper.HarmonyHelper.EmptyMethod<ShipEjectionSystem>("OnPressInteract");

            QSB.Helper.Events.Subscribe<ShipDamageController>(OWML.Common.Events.AfterAwake);
            QSB.Helper.Events.Event += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, OWML.Common.Events ev)
        {
            if (behaviour is ShipDamageController shipDamageController &&
                ev == OWML.Common.Events.AfterAwake)
            {
                shipDamageController.SetValue("_exploded", true);
            }
        }

        private static class Patch
        {
            public static IEnumerable<CodeInstruction> ReturnNull(IEnumerable<CodeInstruction> instructions)
            {
                return new List<CodeInstruction>
                {
                    new CodeInstruction(OpCodes.Ldnull),
                    new CodeInstruction(OpCodes.Ret)
                };
            }
        }
    }
}
