﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld.BaseGen
{
    public class SymbolResolver_ShipRoomTriangle2 : SymbolResolver
    {
        public string interior;

        public override void Resolve(ResolveParams rp)
        {
            if (!this.interior.NullOrEmpty())
            {
                ResolveParams resolveParams = rp;
                resolveParams.rect = rp.rect.ContractedBy(1);
                if (this.interior.Equals("interior_storagetriangle"))
                {
                    resolveParams.thingSetMakerDef = DefDatabase<ThingSetMakerDef>.GetNamed("SpaceLoot");
                }
                BaseGen.symbolStack.Push(this.interior, resolveParams);
            }
            BaseGen.symbolStack.Push("shipemptyRoomTriangle2", rp);
        }
    }
}
