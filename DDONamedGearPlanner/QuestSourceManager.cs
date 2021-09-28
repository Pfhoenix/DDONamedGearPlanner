using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DDONamedGearPlanner
{
	public class QuestSourceManager
	{
		public static readonly string FreeToVIP = "Free to VIP";
		public static readonly string RaidDrops = "Raid Drops";

		static readonly Dictionary<string, bool> QuestSourceAllowed = new Dictionary<string, bool>()
		{
			{ FreeToVIP, true },
			{ RaidDrops, true }
		};

		public static void InitializeFromDataset()
		{
			foreach (var adp in DatasetManager.Dataset.AdventurePacks)
				QuestSourceAllowed[adp.Name] = true;
		}

		public static void SaveSettings()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var kvp in QuestSourceAllowed)
				sb.AppendLine(kvp.Key + " = " + (kvp.Value ? "yes" : "no"));

			string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "questsources.cfg");
			File.WriteAllText(filepath, sb.ToString());
		}

		public static void SetQuestSourceAllowed(string questsource, bool allow, bool save)
		{
			QuestSourceAllowed[questsource] = allow;
			if (save) SaveSettings();
		}

		public static bool IsAllowed(string questsource)
		{
			if (QuestSourceAllowed.ContainsKey(questsource)) return QuestSourceAllowed[questsource];
			else return false;
		}

		public static bool IsAllowed(DDOAdventurePackData apd)
		{
			return IsAllowed(apd.Name);
		}

		public static bool IsItemAllowed(DDOItemData item)
		{
			if (item.QuestFoundIn == null) return false;
			if (item.QuestFoundIn.Adpack != null && !QuestSourceAllowed[item.QuestFoundIn.Adpack.Name]) return false;
			if (item.QuestFoundIn.IsFree) return true;
			if (item.QuestFoundIn.Adpack == null) return QuestSourceAllowed[FreeToVIP];
			if (item.QuestFoundIn.Adpack.FreeToVIP && !QuestSourceAllowed[FreeToVIP]) return false;
			if (!item.MinorArtifact && item.QuestFoundIn.IsRaid && !QuestSourceAllowed[RaidDrops]) return false;

			return true;
		}
	}
}
