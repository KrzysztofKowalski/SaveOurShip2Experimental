using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    class CompProperties_Archolife : CompProperties
    {
#pragma warning disable CS0649
        public float shield;
#pragma warning restore CS0649
        public bool purr = false;
        public bool scintillate = false;

        public CompProperties_Archolife()
        {
            this.compClass = typeof(CompArcholife);
        }
    }
}
