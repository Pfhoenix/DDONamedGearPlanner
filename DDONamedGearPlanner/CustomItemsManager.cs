using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace DDONamedGearPlanner
{
	public struct CraftingIngredient
	{
		public string Name { get; set; }
		public int Amount { get; set; }
	}

	public class CraftedItemProperty
	{
		public string Name;
		public List<ItemProperty> AppliedProperties;
		public List<CraftingIngredient> Cost;

		public override string ToString()
		{
			return Name;
		}
	}

	public abstract class ACustomItemContainer
	{
		public string Name;
		public ItemDataSource Source;
		public string WikiURL;

		public virtual List<SlotType> GetDisallowedSlots()
		{
			return null;
		}

		public abstract DDOItemData GetItem();

		public abstract void ToXml(XmlElement xci, XmlDocument doc);

		public abstract bool FromXml(XmlElement xci);
	}

	public class CustomItemContainer : ACustomItemContainer
	{
		public DDOItemData Item;

		public CustomItemContainer()
		{
			Source = ItemDataSource.Custom;
		}

		public override DDOItemData GetItem()
		{
			return Item;
		}

		public override void ToXml(XmlElement xci, XmlDocument doc)
		{
			XmlElement xi = Item.ToXml(doc);
			xci.AppendChild(xi);
		}

		public override bool FromXml(XmlElement xci)
		{
			try
			{
				XmlElement xi = (XmlElement)xci.GetElementsByTagName("Item")[0];
				Item = DDOItemData.FromXml(xi);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}

	public class CustomItemsManager
	{
		public static List<ACustomItemContainer> CustomItems { get; private set; }
		public static List<T> GetItemsFromSource<T>(ItemDataSource ids) where T : ACustomItemContainer
		{
			List<T> list = new List<T>();
			foreach (var ci in CustomItems)
				if (ci.Source == ids) list.Add((T)ci);

			return list;
		}

		public static bool Load()
		{
			try
			{
				CustomItems = new List<ACustomItemContainer>();
				XmlDocument doc = new XmlDocument();
				doc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customitems.xml"));
				foreach (XmlElement xi in doc.GetElementsByTagName("CustomItem"))
				{
					string name = xi.Attributes["name"].Value;
					ItemDataSource source = (ItemDataSource)Enum.Parse(typeof(ItemDataSource), xi.Attributes["source"].Value);
					switch (source)
					{
						case ItemDataSource.LegendaryGreenSteel:
							LGSCrafting.LGSItemContainer lic = new LGSCrafting.LGSItemContainer { Name = name };
							if (lic.FromXml(xi)) CustomItems.Add(lic);
							break;

						case ItemDataSource.SlaveLord:
							SlaveLordCrafting.SlaveLordItemContainer slic = new SlaveLordCrafting.SlaveLordItemContainer { Name = name };
							if (slic.FromXml(xi)) CustomItems.Add(slic);
							break;

						case ItemDataSource.Custom:
							CustomItemContainer cic = new CustomItemContainer { Name = name, Source = source };
							if (cic.FromXml(xi)) CustomItems.Add(cic);
							break;
					}
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
					XmlElement xci = doc.CreateElement("CustomItem");
					XmlAttribute xa = doc.CreateAttribute("name");
					xa.InnerText = item.Name;
					xci.Attributes.Append(xa);
					xa = doc.CreateAttribute("source");
					xa.InnerText = item.Source.ToString();
					xci.Attributes.Append(xa);
					item.ToXml(xci, doc);
					xr.AppendChild(xci);
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
