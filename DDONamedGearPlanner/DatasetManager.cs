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
	}
}
