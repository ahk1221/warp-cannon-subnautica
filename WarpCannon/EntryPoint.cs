using OasisModLoader;
using SMLHelper.Patchers;
using UnityEngine;
using System.Collections.Generic;
using SMLHelper;
using System.IO;
using Logger = OasisModLoader.Logging.Logger;

namespace WarpCannon
{
    public class EntryPoint : Mod
    {
        public const string WARP_CANNON_CLASS_ID = "WarpCannon";

        public static TechType warpCannonTechType;
        public static TechType warpScalesTechType;
        public static TechType warpBatteryTechType;

        public static AssetBundle AssetBundle;

        public override void OnGameFinishLoad()
        {
        }

        public override void OnGameStart()
        {
            warpCannonTechType = TechTypePatcher.AddTechType("WarpCannon", "Warp Cannon", "A tool that allows you to warp 30 meters using Warper technology.");
            warpScalesTechType = TechTypePatcher.AddTechType("WarpScales", "Warp Scale", "A resource obtained from warpers that can be used to craft Warp Batteries.");
            warpBatteryTechType = TechTypePatcher.AddTechType("WarpBattery", "Warp Battery", "A battery that powers Warp Cannons.");

            var warpCannonData = new TechDataHelper();
            warpCannonData._craftAmount = 1;
            warpCannonData._ingredients = new List<IngredientHelper>();
            warpCannonData._ingredients.Add(new IngredientHelper(warpBatteryTechType, 1));
            warpCannonData._ingredients.Add(new IngredientHelper(TechType.AdvancedWiringKit, 1));
            warpCannonData._techType = warpCannonTechType;

            CraftDataPatcher.customTechData.Add(warpCannonTechType, warpCannonData);
            CraftDataPatcher.customEquipmentTypes.Add(warpCannonTechType, EquipmentType.Hand);
            CraftDataPatcher.customItemSizes.Add(warpCannonTechType, new Vector2int(2, 2));

            CraftDataPatcher.customHarvestTypeList.Add(TechType.Warper, HarvestType.DamageAlive);
            CraftDataPatcher.customHarvestOutputList.Add(TechType.Warper, warpScalesTechType);

            CraftTreePatcher.customCraftNodes.Add("Personal/Tools/WarpCannon", warpCannonTechType);

            // Load AssetBundle
            AssetBundle = AssetBundle.LoadFromFile(Path.Combine(pathToModsFolder, "warpcannon.assets"));
            if (AssetBundle == null)
            {
                Logger.Log("Assetbundle not found!");
                return;
            }

            // Load GameObject
            var warpCannonBattery = AssetBundle.LoadAsset<GameObject>("WarpBattery") as GameObject;
            Utility.AddBasicComponents(ref warpCannonBattery, "WarpBattery");
            warpCannonBattery.AddComponent<Pickupable>();
            warpCannonBattery.AddComponent<Battery>();
            warpCannonBattery.AddComponent<TechTag>().type = warpBatteryTechType;

            var warpCannon = AssetBundle.LoadAsset<GameObject>("WarpCannon");
            Utility.AddBasicComponents(ref warpCannon, WARP_CANNON_CLASS_ID);
            warpCannon.AddComponent<Pickupable>();
            warpCannon.AddComponent<TechTag>().type = warpCannonTechType;

            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab("WarpBattery", "WorldEntities/Tools/WarpBattery", warpCannonBattery, warpBatteryTechType));

            var energyMixin = warpCannon.AddComponent<EnergyMixin>();
            energyMixin.allowBatteryReplacement = true;
            energyMixin.compatibleBatteries = new List<TechType>();
            energyMixin.compatibleBatteries.Add(warpBatteryTechType);
            energyMixin.defaultBattery = warpBatteryTechType;
            energyMixin.defaultBatteryCharge = 100;
            energyMixin.batteryModels = new List<EnergyMixin.BatteryModels>()
            {
                new EnergyMixin.BatteryModels()
                {
                    techType = warpBatteryTechType,
                    model = warpCannonBattery
                }
            }.ToArray();

            var warpCannonComponent = warpCannon.AddComponent<WarpCannon>();
            warpCannonComponent.Init();
            warpCannonComponent.mainCollider = warpCannon.AddComponent<BoxCollider>();
            warpCannonComponent.ikAimRightArm = true;
            warpCannonComponent.useLeftAimTargetOnPlayer = true;

            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(WARP_CANNON_CLASS_ID, "WorldEntities/Tools/WarpCannon", warpCannon, warpCannonTechType));

            // Load Sprites
            var warpCannonSprite = AssetBundle.LoadAsset<Sprite>("Warp_Cannon");
            var warpScalesSprite = AssetBundle.LoadAsset<Sprite>("Warp_Scale");
            var warpBatterySprite = AssetBundle.LoadAsset<Sprite>("Warp_Battery");

            CustomSpriteHandler.customSprites.Add(new CustomSprite(warpCannonTechType, warpCannonSprite));
            CustomSpriteHandler.customSprites.Add(new CustomSprite(warpScalesTechType, warpScalesSprite));
            CustomSpriteHandler.customSprites.Add(new CustomSprite(warpBatteryTechType, warpBatterySprite));
        }
    }
}
