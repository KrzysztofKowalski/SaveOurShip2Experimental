using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    class CompProperties_ArchoHullConversion : CompProperties
    {
#pragma warning disable CS0649
        public SimpleCurve radiusPerDayCurve;
#pragma warning restore CS0649

        public CompProperties_ArchoHullConversion()
        {
            compClass = typeof(CompArchoHullConversion);
        }
    }
}
