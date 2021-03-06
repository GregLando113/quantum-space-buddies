﻿using QSB.Tools;
using QSB.Utility;
using UnityEngine;

namespace QSB.TransformSync
{
    public class PlayerProbeSync : TransformSync
    {
        public static PlayerProbeSync LocalInstance { get; private set; }

        protected override uint PlayerIdOffset => 3;

        private Transform _disabledSocket;

        public override void OnStartLocalPlayer()
        {
            LocalInstance = this;
        }

        private Transform GetProbe()
        {
            return Locator.GetProbe().transform.Find("CameraPivot").Find("Geometry");
        }

        protected override Transform InitLocalTransform()
        {
            var body = GetProbe();

            _disabledSocket = Player.Camera.transform;
            Player.ProbeBody = body.gameObject;

            return body;
        }

        protected override Transform InitRemoteTransform()
        {
            var probe = GetProbe();

            var body = probe.InstantiateInactive();

            Destroy(body.GetComponentInChildren<ProbeAnimatorController>());

            PlayerToolsManager.CreateProbe(body, Player);

            _disabledSocket = Player.ProbeLauncher.ToolGameObject.transform;
            Player.ProbeBody = body.gameObject;

            return body;
        }

        protected override void UpdateTransform()
        {
            base.UpdateTransform();
            if (Player.GetState(State.ProbeActive) || ReferenceSector.Sector == null)
            {
                return;
            }
            if (hasAuthority)
            {
                transform.position = ReferenceSector.Transform.InverseTransformPoint(_disabledSocket.position);
                return;
            }
            if (SyncedTransform.position == Vector3.zero ||
                SyncedTransform.position == Locator.GetAstroObject(AstroObject.Name.Sun).transform.position)
            {
                return;
            }
            SyncedTransform.localPosition = ReferenceSector.Transform.InverseTransformPoint(_disabledSocket.position);
        }

        public override bool IsReady => Locator.GetProbe() != null && PlayerRegistry.PlayerExists(PlayerId) && Player.IsReady;
    }
}
