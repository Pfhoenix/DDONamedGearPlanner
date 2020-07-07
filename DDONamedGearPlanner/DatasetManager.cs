using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DDONamedGearPlanner
{
	public class DatasetManager
	{
		public static readonly string[] CategoryProperties =
		{
			"Armor Category",
			"Offhand Category",
			"Weapon Category"
		};

		public static readonly List<string> RuneArmCompatibleTwoHandedWeaponTypes = new List<string>
		{
			"Great Crossbow",
			"Heavy Crossbow",
			"Light Crossbow",
			"Repeating Heavy Crossbow",
			"Repeating Light Crossbow"
		};

		public static DDODataset Dataset;

		public static string Load()
		{
			FileStream fs = new FileStream("ddodata.dat", FileMode.Open);
			try
			{
				BinaryFormatter bf = new BinaryFormatter();
				Dataset = (DDODataset)bf.Deserialize(fs);

				return null;
			}
			catch (Exception e)
			{
				return e.Message;
			}
			finally
			{
				fs.Close();
			}
		}

		public static bool CanBeUsedTogether(DDOItemData weapon, DDOItemData offhand)
		{
			if (weapon.Handedness == 1) return true;
			else
			{
				if (offhand.Slot != SlotType.Offhand) return false;

				return RuneArmCompatibleTwoHandedWeaponTypes.Contains(weapon.WeaponType) && (OffhandCategory)offhand.Category == OffhandCategory.RuneArm;
			}
		}
	}
}
