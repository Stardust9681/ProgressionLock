using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Terraria.ID;

namespace ProgressionLock
{
	public enum Entities : sbyte
	{
		SlimeRain = -1,
		//Rain = -1,

		GoblinArmy = -2,
		//Goblins = -2,
		//Gobs = -2,

		OldOnesArmy = -3,
		//OOA = -3,
		//DD2 = -3,
		//OldOnes = -3,

		BloodMoon = -4,
		//BM = -4,
		//Blood = -4,

		FrostLegion = -5,
		//Legion = -5,

		SolarEclipse = -6,
		//Eclipse = -6,

		PirateInvastion = -7,
		//Pirates = -7,

		FrostMoon = -8,
		//FMoon = -8,

		PumpkinMoon = -9,
		//PMoon = -9,

		MartianMadness = -10,
		//Martians = -10,

		LunarPillars = -11,
		//CelestialPillars = -11,
		//Pillars = -11,
		//Lunar = -11,

		//
		UnusedOrError = 0,
		//

		KingSlime = 1,
		//Slime = 1,
		//KS = 1,

		EyeOfCthulhu = 2,
		//Eye = 2,
		//EoC = 2,

		EaterOfWorlds = 3,
		//Eater = 3,
		//EoW = 3,

		BrainOfCthulhu = 4,
		//Brain = 4,
		//BoC = 4,

		Deerclops = 5,
		//Deer = 5,

		QueenBee = 6,
		//Bee = 6,
		//QB = 6,

		Skeletron = 7,
		//Skele = 7,

		WallOfFlesh = 8,
		//Wall = 8,
		//WoF = 8,

		QueenSlime = 9,
		//QS = 9,

		Destroyer = 10,
		//Dest = 10,

		Twins = 11,
		//Retinazer = 11,
		//Spazmatism = 11,

		SkeletronPrime = 12,
		//SPrime = 12,

		DukeFishron = 13,
		//Duke = 13,
		//Fishron = 13,
		//Fish = 13,

		Plantera = 14,
		//Planters = 14,
		//Plant = 14,

		EmpressOfLight = 15,

		Golem = 16,

		LunaticCultist = 17,
		//Lunatic = 16,
		//Cultist = 16,
		//LC = 16,

		MoonLord = 18,
		//ML = 17,
	}

	public static class EntityTypeExtensions
	{
		public static Entities FromName(string name)
		{
			try
			{
				return Enum.GetValues<Entities>().First(x => x.GetType().Name.ToLower().Equals(name.ToLower()));
			}
			catch (Exception x)
			{
				return Entities.UnusedOrError;
			}
		}

		public static Entities FromID(int id)
		{
			switch (id)
			{
				case NPCID.KingSlime:
					return Entities.KingSlime;
				case NPCID.EyeofCthulhu:
					return Entities.EyeOfCthulhu;
				case NPCID.EaterofWorldsHead:
				case NPCID.EaterofWorldsBody:
				case NPCID.EaterofWorldsTail:
					return Entities.EaterOfWorlds;
				case NPCID.BrainofCthulhu:
					return Entities.BrainOfCthulhu;
				case NPCID.Deerclops:
					return Entities.Deerclops;
				case NPCID.QueenBee:
					return Entities.QueenBee;
				case NPCID.SkeletronHead:
				case NPCID.SkeletronHand:
					return Entities.Skeletron;
				case NPCID.WallofFlesh:
				case NPCID.WallofFleshEye:
					return Entities.WallOfFlesh;
				case NPCID.QueenSlimeBoss:
					return Entities.QueenSlime;
				case NPCID.TheDestroyer:
				case NPCID.TheDestroyerBody:
				case NPCID.TheDestroyerTail:
					return Entities.Destroyer;
				case NPCID.Spazmatism:
				case NPCID.Retinazer:
					return Entities.Twins;
				case NPCID.SkeletronPrime:
				case NPCID.PrimeLaser:
				case NPCID.PrimeCannon:
				case NPCID.PrimeSaw:
				case NPCID.PrimeVice:
					return Entities.SkeletronPrime;
				case NPCID.DukeFishron:
					return Entities.DukeFishron;
				case NPCID.Plantera:
				case NPCID.PlanterasTentacle:
					return Entities.Plantera;
				case NPCID.HallowBoss:
					return Entities.EmpressOfLight;
				case NPCID.Golem:
				case NPCID.GolemHead:
				case NPCID.GolemFistLeft:
				case NPCID.GolemFistRight:
					return Entities.Golem;
				case NPCID.CultistBoss:
					return Entities.LunaticCultist;
				case NPCID.MoonLordCore:
				case NPCID.MoonLordHead:
				case NPCID.MoonLordHand:
					return Entities.MoonLord;

				case -1:
					return Entities.GoblinArmy;
				case -2:
					return Entities.FrostLegion;
				case -3:
					return Entities.PirateInvastion;
				case -4:
					return Entities.PumpkinMoon;
				case -5:
					return Entities.FrostMoon;
				case -6:
					return Entities.SolarEclipse;
				case -7:
					return Entities.MartianMadness;
				case -10:
					return Entities.BloodMoon;

				default:
					return Entities.UnusedOrError;
			}
		}

		public static int[] BossID(Entities eType)
		{
			int[] Arr(params int[] arr) => arr;
			switch (eType)
			{
				case Entities.KingSlime:
					return Arr(NPCID.KingSlime);
				case Entities.EyeOfCthulhu:
					return Arr(NPCID.EyeofCthulhu);
				case Entities.EaterOfWorlds:
					return Arr(NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail);
				case Entities.BrainOfCthulhu:
					return Arr(NPCID.BrainofCthulhu);
				case Entities.Deerclops:
					return Arr(NPCID.Deerclops);
				case Entities.QueenBee:
					return Arr(NPCID.QueenBee);
				case Entities.Skeletron:
					return Arr(NPCID.SkeletronHead, NPCID.SkeletronHand);
				case Entities.WallOfFlesh:
					return Arr(NPCID.WallofFlesh, NPCID.WallofFleshEye);
				case Entities.QueenSlime:
					return Arr(NPCID.QueenSlimeBoss);
				case Entities.Destroyer:
					return Arr(NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail);
				case Entities.Twins:
					return Arr(NPCID.Spazmatism, NPCID.Retinazer);
				case Entities.SkeletronPrime:
					return Arr(NPCID.SkeletronPrime, NPCID.PrimeLaser, NPCID.PrimeCannon, NPCID.PrimeSaw, NPCID.PrimeVice);
				case Entities.DukeFishron:
					return Arr(NPCID.DukeFishron);
				case Entities.Plantera:
					return Arr(NPCID.Plantera, NPCID.PlanterasTentacle);
				case Entities.Golem:
					return Arr(NPCID.Golem, NPCID.GolemHead, NPCID.GolemFistLeft, NPCID.GolemFistRight);
				case Entities.LunaticCultist:
					return Arr(NPCID.CultistBoss);
				case Entities.MoonLord:
					return Arr(NPCID.MoonLordCore, NPCID.MoonLordHead, NPCID.MoonLordHand);
				default:
					return Arr((int)eType);
			}
		}
	}
}
