using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
    class CompProperties_HologramGlower : CompProperties
    {
#pragma warning disable CS0649
        public Color glowColor;
        public float glowRadius;
#pragma warning restore CS0649
        public CompProperties_HologramGlower()
        {
            this.compClass = typeof(CompHologramGlower);
        }
    }
}
