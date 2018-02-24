﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WarpCannon
{
    public class WarpCannon : PlayerTool
    {
        public override string animToolName => "propulsioncannon";

        public GameObject warpInEffectPrefab;
        public GameObject warpOutEffectPrefab;

        public Material warpedMaterial;

        public FMOD_StudioEventEmitter warpInSound;
        public FMOD_StudioEventEmitter warpOutSound;

        public float overlayFXDuration;

        public GameObject effectPrefab;

        public Transform muzzle;

        public void Init()
        {
            var warper = (Resources.Load("WorldEntities/Creatures/Warper") as GameObject).GetComponent<Warper>();
            warpInEffectPrefab = warper.warpInEffectPrefab;
            warpOutEffectPrefab = warper.warpOutEffectPrefab;

            warpedMaterial = warper.warpedMaterial;

            warpInSound = warper.warpInSound;
            warpOutSound = warper.warpOutSound;

            overlayFXDuration = warper.overlayFXduration;

            Resources.UnloadAsset(warper);
        }

        public override bool OnRightHandDown()
        {
            base.OnRightHandDown();

            var aimingTransform = Player.main.camRoot.GetAimingTransform();
            var dist = 0f;
            var go = default(GameObject);
            var hitSomething = Targeting.GetTarget(Player.main.gameObject, 30, out go, out dist, null);

            var newPos = Vector3.zero;

            if (hitSomething)
            {
                newPos = aimingTransform.forward * (dist - 1f) + aimingTransform.position;
            }
            else
            {
                newPos = aimingTransform.forward * 30f + aimingTransform.position;
            }

            // Warp out.
            Utils.SpawnPrefabAt(warpOutEffectPrefab, null, Player.main.transform.position);
            Utils.PlayEnvSound(warpOutSound, Player.main.transform.position, 20f);

            // Warp in
            Player.main.transform.position = newPos;
            Utils.SpawnPrefabAt(warpInEffectPrefab, null, newPos);
            Player.main.gameObject.AddComponent<VFXOverlayMaterial>().ApplyAndForgetOverlay(warpedMaterial, "VFXOverlay: Warped", Color.clear, overlayFXDuration);
            Utils.PlayEnvSound(warpOutSound, Player.main.transform.position, 20f);

            return true;
        }

    }
}