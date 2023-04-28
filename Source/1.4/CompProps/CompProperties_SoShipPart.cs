﻿using System;
using Verse;

namespace RimWorld
{
	public class CompProperties_SoShipPart : CompProperties
	{
		public bool isPlating = false;
		public bool isHardpoint = false;
		public bool isHull = false;
		public bool hermetic = false;

		public bool roof = false;

		//types for terrain, roof
		public bool mechanoid = false;
		public bool archotech = false;
		public bool wreckage = false;
		public bool foam = false;

		public CompProperties_SoShipPart()
		{
			compClass = typeof(CompSoShipPart);
		}
	}
}
