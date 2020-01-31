using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace DDONamedGearPlanner
{
	public enum CustomItemSource { Custom, Cannith, SlaveLord, ThunderForge, GreenSteel }

	public class CustomItem
	{
		public DDOItemData Item;
		public CustomItemSource Source;

		public XmlElement ToXml(XmlDocument doc)
		{
			XmlElement xi = Item.ToXml(doc);
			XmlAttribute xa = doc.CreateAttribute("source");
			xa.InnerText = Source.ToString();
			xi.Attributes.Append(xa);

			return xi;
		}

		public static CustomItem FromXml(XmlElement xi)
		{
			CustomItem ci = new CustomItem();
			ci.Item = DDOItemData.FromXml(xi);
			ci.Source = (CustomItemSource)Enum.Parse(typeof(CustomItemSource), xi.GetAttribute("source"));

			return ci;
		}
	}

	public class CustomItemsManager
	{
		public static List<CustomItem> CustomItems { get; private set; }
		public static List<CustomItem> GetItemsFromSource(CustomItemSource cis)
		{
			return CustomItems.Where(i => i.Source == cis).ToList();
		}

		public static bool Load()
		{
			try
			{
				CustomItems = new List<CustomItem>();
				XmlDocument doc = new XmlDocument();
				doc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customitems.xml"));
				foreach (XmlElement xi in doc.GetElementsByTagName("Item"))
				{
					CustomItem item = CustomItem.FromXml(xi);
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
