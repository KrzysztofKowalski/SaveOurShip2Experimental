using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    class CompProperties_PowerPlantSolarShip : CompProperties_Power
    {
#pragma warning disable CS0649
        public float bonusPower;
#pragma warning restore CS0649

        public CompProperties_PowerPlantSolarShip()
        {
            this.compClass = typeof(CompPowerPlantSolarShip);
        }
    }
}
