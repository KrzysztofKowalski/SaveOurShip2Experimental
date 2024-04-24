﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vehicles;
using Verse;
using static SaveOurShip2.ShipMapComp;

namespace SaveOurShip2.Vehicles
{
    class ShuttleTakeoff : VTOLTakeoff
    {
        public ShuttleTakeoff()
        {

        }

        public ShuttleTakeoff(VTOLTakeoff reference, VehiclePawn vehicle) : base(reference, vehicle)
        {

        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptionsAt(int tile)
        {
            bool isPlayerMap = this.vehicle.Map.GetComponent<ShipMapComp>()?.ShipCombatOriginMap == this.vehicle.Map;
            bool isEnemyMap = this.vehicle.Map.GetComponent<ShipMapComp>()?.ShipCombatTargetMap == this.vehicle.Map;
            if (!isPlayerMap && !isEnemyMap)
                return base.GetFloatMenuOptionsAt(tile);
            if (isPlayerMap)
            {
                if (this.vehicle.Map.GetComponent<ShipMapComp>().ShipCombatTargetMap.Tile == tile) //Launch mission against enemy ship
                    return FloatMenuMissions(tile, false);
                else
                    return base.GetFloatMenuOptionsAt(tile);
            }

            if (this.vehicle.Map.GetComponent<ShipMapComp>().ShipCombatOriginMap.Tile == tile) //Return home
                return FloatMenuOption_ReturnFromEnemy(tile);
            else if (this.vehicle.Map.GetComponent<ShipMapComp>().ShipCombatTargetMap.Tile == tile) //Target enemy ship to change mission
                return FloatMenuMissions(tile, false);
            else
                return base.GetFloatMenuOptionsAt(tile);
        }

        public IEnumerable<FloatMenuOption> FloatMenuMissions(int tile, bool fromEnemy)
        {
            if(!fromEnemy)
                yield return FloatMenuOption_Board(tile);

			var u = vehicle.CompUpgradeTree.upgrades;
			if (u != null)
			{
				bool hasLaser = u.Contains("TurretLaserA") || u.Contains("TurretLaserB") || u.Contains("TurretLaserC");
				bool hasPlasma = u.Contains("TurretPlasmaA") || u.Contains("TurretPlasmaB") || u.Contains("TurretPlasmaC");
				bool hasTorpedo = u.Contains("TurretTorpedoA") || u.Contains("TurretTorpedoB") || u.Contains("TurretTorpedoC")
					&& vehicle.carryTracker.GetDirectlyHeldThings().Any(t => t.HasThingCategory(ResourceBank.ThingCategoryDefOf.SpaceTorpedoes));
				if (hasLaser)
					yield return FloatMenuOption_Intercept(tile);
				if (hasLaser || hasPlasma)
					yield return FloatMenuOption_Strafe(tile);
				if (hasTorpedo)
					yield return FloatMenuOption_Bomb(tile);
			}
        }

        FloatMenuOption FloatMenuOption_Board(int tile)
        {
            return new FloatMenuOption("Mission: Boarding Party", delegate { LaunchShuttleToCombatManager(vehicle, ShuttleMission.BOARD); });
        }

        FloatMenuOption FloatMenuOption_Intercept(int tile)
        {
            return new FloatMenuOption("Mission: Intercept Torpedoes/Fighters", delegate { LaunchShuttleToCombatManager(vehicle, ShuttleMission.INTERCEPT); });
        }

        FloatMenuOption FloatMenuOption_Strafe(int tile)
        {
            return new FloatMenuOption("Mission: Strafe Enemy Ship", delegate { LaunchShuttleToCombatManager(vehicle, ShuttleMission.STRAFE); });
        }

        FloatMenuOption FloatMenuOption_Bomb(int tile)
        {
            return new FloatMenuOption("Mission: Torpedo Enemy Ship", delegate { LaunchShuttleToCombatManager(vehicle, ShuttleMission.BOMB); });
        }

        IEnumerable<FloatMenuOption> FloatMenuOption_ReturnFromEnemy(int tile)
        {
            yield return new FloatMenuOption("Return to Ship", delegate { LaunchShuttleToCombatManager(vehicle, ShuttleMission.BOARD); });
        }

        public static void LaunchShuttleToCombatManager(VehiclePawn vehicle, ShuttleMission mission)
        {
            vehicle.CompVehicleLauncher.inFlight = true;
            vehicle.CompVehicleLauncher.launchProtocol.OrderProtocol(LaunchProtocol.LaunchType.Takeoff);
            VehicleSkyfaller_Leaving vehicleSkyfaller_Leaving = (VehicleSkyfaller_Leaving)VehicleSkyfallerMaker.MakeSkyfaller(vehicle.CompVehicleLauncher.Props.skyfallerLeaving, vehicle);
            vehicleSkyfaller_Leaving.vehicle = vehicle;
            vehicleSkyfaller_Leaving.createWorldObject = false;
            GenSpawn.Spawn(vehicleSkyfaller_Leaving, vehicle.Position, vehicle.Map, vehicle.CompVehicleLauncher.launchProtocol.CurAnimationProperties.forcedRotation ?? vehicle.Rotation);
            vehicle.Map.GetComponent<ShipMapComp>().OriginMapComp.RegisterShuttleMission(vehicle, mission);
            CameraJumper.TryHideWorld();
            vehicle.EventRegistry[VehicleEventDefOf.AerialVehicleLaunch].ExecuteEvents();
        }
    }
}
