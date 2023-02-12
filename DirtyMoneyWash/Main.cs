using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirtyMoneyWash.Helpers;
using DirtyMoneyWash.Models;
using DirtyMoneyWash.Storage;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Items;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace DirtyMoneyWash
{
    public class Main : RocketPlugin<Configuration>
    {
        public DataStorage<List<WashBasin>> BasinDataStorage { get; private set; }
        private List<WashBasin> basinData;

        public static Main Instance;
        protected override void Load()
        {
            BasinDataStorage = new DataStorage<List<WashBasin>>(Directory, "Basins.json");
            ReloadBasinData();

            Instance = this;

            BarricadeManager.onBarricadeSpawned += OnBarricadeSpawned;
            ItemManager.onServerSpawningItemDrop += OnDropItem;

            BarricadeDrop.OnSalvageRequested_Global += OnBarricadeSalvaged;
        }

        private void OnBarricadeSalvaged(BarricadeDrop barricade, SteamPlayer instigatorclient, ref bool shouldallow)
        {
            var tool =
                Main.Instance.Configuration.Instance.ToolItems.First(config_tool => config_tool.ToolType == ToolType.FullBasin);

            if (tool.ToolID == barricade.asset.id)
            {
                var basin = RetrieveBasin(barricade.model.position, out var exists);

                if (exists)
                {
                    RemoveBasin(basinData.IndexOf(basin));
                }

                shouldallow = false;

                BarricadeManager.tryGetInfo(barricade.model, out var x, out var y, out var plant, out var index, out var region);
                BarricadeManager.destroyBarricade(region, x, y, plant, index);

                var player = UnturnedPlayer.FromSteamPlayer(instigatorclient);

                player.GiveItem(
                    Main.Instance.Configuration.Instance.ToolItems
                        .First(config_tool => config_tool.ToolType == ToolType.EmptyBasin).ToolID, 1);
            }
        }

        private void ReloadBasinData() =>
            basinData = BasinDataStorage.Read() ?? new List<WashBasin>();

        private void AddBasin(WashBasin basin)
        {
            basinData.Add(basin);
            BasinDataStorage.Save(basinData);
        }

        private void RemoveBasin(int index)
        {
            basinData.RemoveRange(index, 1);
            BasinDataStorage.Save(basinData);
        }

        private void UpdateBasin(WashBasin basin, int water_level)
        {
            basinData.Remove(basin);

            basin.WaterLevel = water_level;
            basinData.Add(basin);

            BasinDataStorage.Save(basinData);
        }

        private WashBasin RetrieveBasin(Vector3 location, out bool exists)
        {
            exists = basinData.Exists(basin =>
                basin.LocationX == location.x && basin.LocationY == location.y && basin.LocationZ == location.z);

            return basinData.FirstOrDefault(basin =>
                basin.LocationX == location.x && basin.LocationY == location.y && basin.LocationZ == location.z);
        }
        
        private void OnDropItem(Item item, ref Vector3 location, ref bool should_allow)
        {
            if (item.id == 14 || Instance.Configuration.Instance.MoneyItems.Any(money => money.DirtMoneyID == item.id))
            {
                foreach (var drop in BarricadeHelper.GetBarricadesNearby(1, location))
                {
                    if (drop == null) continue;
                    if (!Instance.Configuration.Instance.ToolItems.Exists(tool =>
                            tool.ToolID == drop.asset.id)) continue;

                    var tool_item = Configuration.Instance.ToolItems.First(tool => tool.ToolID == drop.asset.id);
                    if (tool_item.ToolType == ToolType.FullBasin || tool_item.ToolType == ToolType.EmptyBasin)
                    {
                        var basin = RetrieveBasin(drop.model.position, out var exists);
                        if (!exists) continue;

                        if (tool_item.ToolType == ToolType.EmptyBasin && item.id == 14)
                        {
                            UpdateBasin(basin, basin.WaterLevel += 1);
                            if (basin.WaterLevel == Configuration.Instance.WaterToFill)
                            {
                                var filled_basin =
                                    Configuration.Instance.ToolItems.First(ba => ba.ToolType == ToolType.FullBasin);
                                BarricadeManager.dropNonPlantedBarricade(new Barricade(Assets.find(EAssetType.ITEM,
                                        filled_basin.ToolID) as ItemBarricadeAsset),
                                    drop.model.position,
                                    drop.model.localRotation,
                                    basin.Owner,
                                    basin.Group);

                                BarricadeManager.tryGetInfo(drop.model, out var x, out var y, out var plant, out var index, out var region);
                                BarricadeManager.destroyBarricade(region, x, y, plant, index);

                                RemoveBasin(basinData.IndexOf(basin));
                            }
                            location = new Vector3(0, 0, 0);
                        }

                        if (tool_item.ToolType == ToolType.FullBasin &&
                            Main.Instance.Configuration.Instance.MoneyItems.Exists(money => money.DirtMoneyID == item.id))
                        {

                            if (DestoryChanceHelper.RollChance(Main.Instance.Configuration.Instance.DestroyChance))
                            {
                                location = new Vector3(0, 0, 0);
                                continue;
                            }

                            var wet_money =
                                Main.Instance.Configuration.Instance.MoneyItems.First(money =>
                                    money.DirtMoneyID == item.id).WetMoneyID;

                            var asset = Assets.find(EAssetType.ITEM, wet_money) as ItemBarricadeAsset;

                            UpdateBasin(basin, basin.WaterLevel -= 1);

                            BarricadeManager.dropNonPlantedBarricade(
                                new Barricade(asset),
                                location,
                                new Quaternion(basin.Rot_X, basin.Rot_Y + 6, basin.Rot_Z, basin.Rot_Z),
                                basin.Owner,
                                basin.Group
                            );

                            EffectManager.sendEffect(Configuration.Instance.WashedEffect, 5, new Vector3(basin.LocationX, basin.LocationY + 6, basin.LocationZ));

                            if (basin.WaterLevel == 0)
                            {
                                var empty_basin =
                                    Configuration.Instance.ToolItems.First(ba => ba.ToolType == ToolType.EmptyBasin);
                                BarricadeManager.dropNonPlantedBarricade(new Barricade(Assets.find(EAssetType.ITEM,
                                        empty_basin.ToolID) as ItemBarricadeAsset),
                                    drop.model.position,
                                    drop.model.localRotation,
                                    basin.Owner,
                                    basin.Group);

                                BarricadeManager.tryGetInfo(drop.model, out var x, out var y, out var plant, out var index, out var region);
                                BarricadeManager.destroyBarricade(region, x, y, plant, index);

                                RemoveBasin(basinData.IndexOf(basin));
                            }
                            location = new Vector3(0, 0, 0);
                        }
                    }
                }
            }
        }

        private void OnBarricadeSpawned(BarricadeRegion region, BarricadeDrop drop)
        {
            if (Instance.Configuration.Instance.ToolItems.Any(item => item.ToolID == drop.asset.id))
            {
                var tool = Instance.Configuration.Instance.ToolItems.First(item => item.ToolID == drop.asset.id);
                var barricade = drop.model.GetComponent<Interactable2SalvageBarricade>();
                
                if (barricade == null) return;

                if (tool.ToolType == ToolType.EmptyBasin)
                {
                    var new_basin = new WashBasin(drop.model.position.x,
                        drop.model.position.y, drop.model.position.z,
                        drop.model.rotation.x, drop.model.rotation.y,
                        drop.model.rotation.z, drop.model.rotation.w, barricade.owner, barricade.group);

                    AddBasin(new_basin);

                    return;
                }
                if (tool.ToolType == ToolType.FullBasin)
                {
                    var new_basin = new WashBasin(drop.model.position.x,
                        drop.model.position.y, drop.model.position.z,
                        drop.model.rotation.x, drop.model.rotation.y,
                        drop.model.rotation.z, drop.model.rotation.w, barricade.owner, barricade.group, Instance.Configuration.Instance.WaterToFill);

                    AddBasin(new_basin);

                    return;
                }
            }

            if (Instance.Configuration.Instance.MoneyItems.All(item => item.WetMoneyID != drop.asset.id)) return;

            foreach (var drop_b in BarricadeHelper.GetBarricadesNearby(1, drop.model.position))
            {
                if (drop_b == null) continue;
                if (Instance.Configuration.Instance.ToolItems.All(tool => tool.ToolID != drop_b.asset.id)) continue;

                var tool_item = Instance.Configuration.Instance.ToolItems.First(tool => tool.ToolID == drop_b.asset.id);

                if (tool_item.ToolType != ToolType.DryingRack || Instance.Configuration.Instance.MoneyItems.All(item => item.WetMoneyID != drop.asset.id))
                    continue;

                StartDryingHelper.StartDrying(drop.asset.id, drop.model);
                break;
            }
        }

        protected override void Unload()
        {
            Instance = null;

            BarricadeManager.onBarricadeSpawned -= OnBarricadeSpawned;
            ItemManager.onServerSpawningItemDrop -= OnDropItem;

            BarricadeDrop.OnSalvageRequested_Global -= OnBarricadeSalvaged;

            StopAllCoroutines();
        }
    }
}
