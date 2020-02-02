using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace DDONamedGearPlanner
{
	public class CustomItemsManager
	{
		public static List<DDOItemData> CustomItems { get; private set; }
		public static List<DDOItemData> GetItemsFromSource(ItemDataSource ids)
		{
			return CustomItems.Where(i => i.Source == ids).ToList();
		}

		public static bool Load()
		{
			try
			{
				CustomItems = new List<DDOItemData>();
				XmlDocument doc = new XmlDocument();
				doc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customitems.xml"));
				foreach (XmlElement xi in doc.GetElementsByTagName("Item"))
				{
					DDOItemData item = DDOItemData.FromXml(xi);
					if (item != null) CustomItems.Add(item);
				}

				return true;
			}
			catch
			{
				return false;
			}
		}

		public static string Save()
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				XmlElement xr = doc.CreateElement("CustomItems");
				doc.AppendChild(xr);
				foreach (var item in CustomItems)
				{
					XmlElement xi = item.ToXml(doc);
					xr.AppendChild(xi);
				}

				using (XmlTextWriter tw = new XmlTextWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customitems.xml"), Encoding.Default))
				{
					tw.Formatting = Formatting.Indented;
					doc.WriteTo(tw);
				}

				return null;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
	}
}
