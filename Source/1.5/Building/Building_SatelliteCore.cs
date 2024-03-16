﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
    class Building_SatelliteCore : Building
    {
        public bool repaired = false;
        public bool hacked = false;
        public float extraFailChance=0;
        public bool mechanoidsSummoned = false;
        public int mechSpawnCountdown = 0;
        Lord mechLord = null;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref repaired, "repaired");
            Scribe_Values.Look<bool>(ref hacked, "hacked");
            Scribe_Values.Look<float>(ref extraFailChance, "extraFailChance");
            Scribe_Values.Look<bool>(ref mechanoidsSummoned, "mechanoidsSummoned");
            Scribe_Values.Look<int>(ref mechSpawnCountdown, "mechSpawnCountdown");
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
            if (mode == DestroyMode.Deconstruct)
            {
                if (Rand.Chance(0.4f) || hacked)
                {
                    Find.LetterStack.ReceiveLetter(TranslatorFormattedStringExtensions.Translate("LetterLabelSatelliteMechAttackBase"), TranslatorFormattedStringExtensions.Translate("LetterSatelliteMechAttackBase"), LetterDefOf.ThreatBig, LookTargets.Invalid);
                    SpawnMechInvasionAtShip();
                }
                else if(Rand.Chance(0.95f))
                {
                    SpawnMechInvasionHere();
                }
            }
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn pawn)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            foreach (FloatMenuOption op in base.GetFloatMenuOptions(pawn))
                options.Add(op);
            if(!repaired)
            {
                options.Add(new FloatMenuOption("Repair", delegate { Job hackSatellite = new Job(DefDatabase<JobDef>.GetNamed("RepairSatellite"), this); pawn.jobs.TryTakeOrderedJob(hackSatellite); }));
            }
            else if(!hacked)
            {
                options.Add(new FloatMenuOption("Hack", delegate { Job hackSatellite = new Job(DefDatabase<JobDef>.GetNamed("HackSatellite"), this); pawn.jobs.TryTakeOrderedJob(hackSatellite); }));
            }
            return options;
        }

        public void HackMe(Pawn pawn)
        {
            if (Rand.Chance(0.05f * pawn.skills.GetSkill(SkillDefOf.Intellectual).levelInt - extraFailChance))
            {
                this.hacked = true;
                Find.LetterStack.ReceiveLetter(TranslatorFormattedStringExtensions.Translate("LetterLabelSatelliteHackSuccess"), TranslatorFormattedStringExtensions.Translate("LetterSatelliteHackSuccess",pawn.LabelShort), LetterDefOf.PositiveEvent, this);
                if (!mechanoidsSummoned && Rand.Chance(0.25f))
                    SpawnMechInvasionHere();
                pawn.skills.GetSkill(SkillDefOf.Intellectual).Learn(2000);
                DispenseTargeters();
            }
            else if (Rand.Chance(0.05f * (20 - pawn.skills.GetSkill(SkillDefOf.Intellectual).levelInt)))
            {
                Find.LetterStack.ReceiveLetter(TranslatorFormattedStringExtensions.Translate("LetterLabelSatelliteHackFailCritical"), TranslatorFormattedStringExtensions.Translate("LetterSatelliteHackFailCritical",pawn.LabelShort), LetterDefOf.ThreatBig, this);
                SpawnMechInvasionAtShip();
            }
            else
            {
                Find.LetterStack.ReceiveLetter(TranslatorFormattedStringExtensions.Translate("LetterLabelSatelliteHackFail"), TranslatorFormattedStringExtensions.Translate("LetterSatelliteHackFail",pawn.LabelShort), LetterDefOf.NegativeEvent, this);
                extraFailChance += 0.1f;
                if (!mechanoidsSummoned && Rand.Chance(0.4f))
                    SpawnMechInvasionHere();
            }
        }

        public void RepairMe(Pawn pawn)
        {
            if(Rand.Chance(0.05f * pawn.skills.GetSkill(SkillDefOf.Construction).levelInt - extraFailChance))
            {
                this.repaired = true;
                Find.LetterStack.ReceiveLetter(TranslatorFormattedStringExtensions.Translate("LetterLabelSatelliteRepairSuccess"), TranslatorFormattedStringExtensions.Translate("LetterSatelliteRepairSuccess",pawn.LabelShort), LetterDefOf.PositiveEvent, this);
                if (!mechanoidsSummoned && Rand.Chance(0.25f))
                    SpawnMechInvasionHere();
                pawn.skills.GetSkill(SkillDefOf.Construction).Learn(2000);
            }
            else if(Rand.Chance(0.05f * (20-pawn.skills.GetSkill(SkillDefOf.Construction).levelInt)))
            {
                Find.LetterStack.ReceiveLetter(TranslatorFormattedStringExtensions.Translate("LetterLabelSatelliteRepairFailCritical"), TranslatorFormattedStringExtensions.Translate("LetterSatelliteRepairFailCritical",pawn.LabelShort), LetterDefOf.ThreatSmall, this);
                GenExplosion.DoExplosion(this.Position, this.Map, 20, DamageDefOf.Bomb, this, 80);
                this.Kill();
            }
            else
            {
                Find.LetterStack.ReceiveLetter(TranslatorFormattedStringExtensions.Translate("LetterLabelSatelliteRepairFail"), TranslatorFormattedStringExtensions.Translate("LetterSatelliteRepairFail",pawn.LabelShort), LetterDefOf.NegativeEvent, this);
                extraFailChance += 0.1f;
                if (!mechanoidsSummoned && Rand.Chance(0.4f))
                    SpawnMechInvasionHere();
            }
        }

        public void DispenseTargeters()
        {
            List<ThingDef> targeters = new List<ThingDef>();
            targeters.Add(ThingDefOf.OrbitalTargeterBombardment);
            targeters.Add(ThingDefOf.OrbitalTargeterPowerBeam);
            targeters.Add(ThingDef.Named("TornadoGenerator"));
            targeters.Add(ThingDef.Named("WeatherCancelDevice"));
            int numTargeters = Rand.RangeInclusive(2, 3);
            for(int i=0;i<numTargeters;i++)
            {
                GenPlace.TryPlaceThing(ThingMaker.MakeThing(targeters.RandomElement()), this.Position, this.Map, ThingPlaceMode.Near);
            }
        }

        public void SpawnMechInvasionAtShip()
        {
            foreach(Map map in Find.Maps)
            {
                if (map.Parent != null && map.Parent is WorldObjectOrbitingShip)
                {
                    IncidentParms parms = new IncidentParms();
                    parms.faction = Faction.OfMechanoids;
                    parms.forced = true;
                    parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeDrop;
                    parms.target = map;
                    Find.Storyteller.TryFire(new FiringIncident(IncidentDefOf.RaidEnemy, null, parms));
                }
            }
        }

        public void SpawnMechInvasionHere()
        {
            mechanoidsSummoned = true;
            Find.LetterStack.ReceiveLetter(TranslatorFormattedStringExtensions.Translate("LetterLabelSatelliteMechAttack"), TranslatorFormattedStringExtensions.Translate("LetterSatelliteMechAttack"), LetterDefOf.ThreatSmall, LookTargets.Invalid);
        }

        public override void TickRare()
        {
            base.TickRare();
            if(mechanoidsSummoned)
            {
                mechSpawnCountdown--;
                if(mechSpawnCountdown<=0)
                {
                    if(mechLord == null)
                    {
                        mechLord = LordMaker.MakeNewLord(Faction.OfMechanoids, new LordJob_DefendPoint(this.Position), this.Map);
                    }
                    PawnKindDef pawnKindDef = (from kind in DefDatabase<PawnKindDef>.AllDefsListForReading
                                               where kind.RaceProps.IsMechanoid && kind.combatPower <= 500
                                               select kind).RandomElementByWeight((PawnKindDef kind) => 1f / kind.combatPower);
                    Pawn pawn = PawnGenerator.GeneratePawn(pawnKindDef);
                    pawn.SetFaction(Faction.OfMechanoids);
                    List<Pawn> pawns = new List<Pawn>();
                    pawns.Add(pawn);
                    IncidentParms parms = new IncidentParms();
                    parms.target = this.Map;
                    parms.forced = true;
                    parms.faction = Faction.OfMechanoids;
                    parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                    mechLord.AddPawn(pawn);
                    pawn.mindState.enemyTarget = this.Map.mapPawns.AllPawns.Where(p => p.Faction == Faction.OfPlayer).RandomElement();
                    PawnsArrivalModeDefOf.EdgeDrop.Worker.Arrive(pawns, parms);
                    mechSpawnCountdown = Rand.RangeInclusive(1, 3);
                }
            }
        }
    }
}
