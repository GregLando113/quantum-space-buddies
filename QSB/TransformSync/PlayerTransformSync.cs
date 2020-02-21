﻿using QSB.Animation;
using UnityEngine;

namespace QSB.TransformSync
{
    public class PlayerTransformSync : TransformSync
    {
        public static PlayerTransformSync LocalInstance { get; private set; }

        private Transform _playerModel;

        private Transform GetPlayerModel()
        {
            if (!_playerModel)
            {
                _playerModel = Locator.GetPlayerBody().transform.Find("Traveller_HEA_Player_v2");
            }
            return _playerModel;
        }

        protected override Transform InitLocalTransform()
        {
            LocalInstance = this;
            var body = GetPlayerModel();

            GetComponent<AnimationSync>().Init(body);

            return body;
        }

        protected override Transform InitRemoteTransform()
        {
            var body = Instantiate(GetPlayerModel());
            body.GetComponent<PlayerAnimController>().enabled = false;
            body.Find("player_mesh_noSuit:Traveller_HEA_Player/player_mesh_noSuit:Player_Head").gameObject.layer = 0;
            body.Find("Traveller_Mesh_v01:Traveller_Geo/Traveller_Mesh_v01:PlayerSuit_Helmet").gameObject.layer = 0;

            GetComponent<AnimationSync>().Init(body);

            return body;
        }
    }
}