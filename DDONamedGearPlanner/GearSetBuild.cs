using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace DDONamedGearPlanner
{
	public class BuildFilter
	{
		// this will be SlotType.None for gear set filters
		public SlotType Slot;
		public string Property { get; set; }
		public string Type { get; set; }
		public bool Include { get; set; }

		public XmlElement ToXml(XmlDocument doc)
		{
			XmlElement xe = doc.CreateElement("Filter");
			XmlAttribute xa = doc.CreateAttribute("slot");
			xa.InnerText = Slot.ToString();
			xe.Attributes.Append(xa);
			if (Type != null)
			{
				xa = doc.CreateAttribute("type");
				xa.InnerText = Type;
				xe.Attributes.Append(xa);
			}
			xa = doc.CreateAttribute("include");
			xa.InnerText = Include.ToString();
			xe.Attributes.Append(xa);
			xe.InnerText = Property;

			return xe;
		}

		public static BuildFilter FromXml(XmlElement xe)
		{
			BuildFilter bf = new BuildFilter();

			bf.Property = xe.InnerText;
			bf.Slot = (SlotType)Enum.Parse(typeof(SlotType), xe.GetAttribute("slot"));
			bf.Type = xe.GetAttribute("type");
			if (string.IsNullOrWhiteSpace(bf.Type)) bf.Type = null;
			bf.Include = bool.Parse(xe.GetAttribute("include"));

			return bf;
		}
	}

	public class BuildItem
	{
		// this is for UI bookkeeping
		public bool InUse;
		public DDOItemData Item;
		// to support builds using two weapons
		public EquipmentSlotType Slot;
		// this is how we simulate property options being set
		public List<ItemProperty> OptionProperties = new List<ItemProperty>();

		public BuildItem(DDOItemData item, EquipmentSlotType slot)
		{
			Item = item;
			Slot = slot;
		}

		public XmlElement ToXml(XmlDocument doc)
		{
			XmlElement xb = doc.CreateElement("BuildItem");
			XmlAttribute xa = doc.CreateAttribute("item");
			xa.InnerText = Item.Name;
			xb.Attributes.Append(xa);
			xa = doc.CreateAttribute("slot");
			xa.InnerText = Slot.ToString();
			xb.Attributes.Append(xa);

			foreach (var o in OptionProperties)
			{
				XmlElement xp = doc.CreateElement("OptionProperty");
				xb.AppendChild(xp);
				xa = doc.CreateAttribute("type");
				xa.InnerText = string.IsNullOrWhiteSpace(o.Type) ? null : o.Type;
				xp.Attributes.Append(xa);
				xp.InnerText = o.Property;
			}

			return xb;
		}

		public static BuildItem FromXml(XmlElement xe, DDODataset ds)
		{
			try
			{
				DDOItemData item = ds.Items.Find(i => i.Name == xe.GetAttribute("item"));
				if (item == null) return null;
				EquipmentSlotType est = (EquipmentSlotType)Enum.Parse(typeof(EquipmentSlotType), xe.GetAttribute("slot"));
				if (est == EquipmentSlotType.None) return null;
				BuildItem bi = new BuildItem(item, est);
				if (!xe.HasChildNodes) return bi;
				List<ItemProperty> found = new List<ItemProperty>();
				foreach (XmlElement xc in xe.ChildNodes)
				{
					string property = xc.InnerText;
					string type = xc.GetAttribute("type");
					foreach (var ip in item.Properties)
					{
						if (ip.Options != null && !found.Contains(ip))
						{
							foreach (var op in ip.Options)
							{
								if (op.Property == property && ((string.IsNullOrWhiteSpace(op.Type) && type == "untyped") || op.Type == type))
								{
									bi.OptionProperties.Add(op);
									found.Add(ip);
									break;
								}
							}
						}
					}
				}

				return bi;
			}
			catch
			{
				return null;
			}
		}
	}

	public class GearSetProperty
	{
		public bool IsGroup;
		public string Property;
		public float TotalValue;
		public List<ItemProperty> ItemProperties = new List<ItemProperty>();

		// this adds an item property, sorting by highest value and type alphabetically
		public void AddItemProperty(ItemProperty ip)
		{
			int place = -1;
			for (int i = 0; i < ItemProperties.Count; i++)
			{
				// no type means sort only by value
				if (string.IsNullOrWhiteSpace(ip.Type))
				{
					if (ItemProperties[i].Value >= ip.Value) continue;
					place = i;
					break;
				}
				// any type should appear before no type
				if (string.IsNullOrWhiteSpace(ItemProperties[i].Type))
				{
					place = i;
					break;
				}
				// alphabetical listing of types please
				int sc = string.Compare(ItemProperties[i].Type, ip.Type);
				if (sc < 0) continue;
				// in order by value next
				else if (sc == 0)
				{
					if (ItemProperties[i].Value > ip.Value) continue;
					else
					{
						place = i;
						break;
					}
				}
				else
				{
					place = i;
					break;
				}
			}
			if (place == -1) ItemProperties.Add(ip);
			else ItemProperties.Insert(place, ip);
		}
	}

	public class GearSet
	{
		public List<BuildItem> Items = new List<BuildItem>();
		public List<GearSetProperty> Properties = new List<GearSetProperty>();

		void AddProperties(List<ItemProperty> properties)
		{
			foreach (var p in properties)
			{
				GearSetProperty gsp = Properties.Find(pr => pr.Property == p.Property);
				if (gsp == null)
				{
					// options get ignored, as the optionals will be applied separately
					if (p.Options != null && p.Options.Count > 0) continue;
					// we never want to show minimum level or handedness
					if (p.Property == "Minimum Level" || p.Property == "Handedness") continue;

					gsp = new GearSetProperty { Property = p.Property };
					if (gsp.Property == "Damage Reduction") gsp.IsGroup = true;
					else if (gsp.Property == "Augment Slot") gsp.IsGroup = true;
					else if (p.Type == "set") gsp.IsGroup = true;
					Properties.Add(gsp);
				}

				gsp.AddItemProperty(p);
			}
		}

		public void AddItem(BuildItem item)
		{
			Items.Add(item);
			AddProperties(item.Item.Properties);
			if (item.OptionProperties != null) AddProperties(item.OptionProperties);
		}

		void CalculatePropertyGroups()
		{
			foreach (var gsp in Properties)
			{
				if (gsp.IsGroup) continue;

				gsp.TotalValue = 0;
				string lasttype = null;
				foreach (var ip in gsp.ItemProperties)
				{
					if (string.IsNullOrWhiteSpace(lasttype) || lasttype != ip.Type)
					{
						gsp.TotalValue += ip.Value;
						lasttype = ip.Type;
					}
				}
			}
		}

		public void ProcessItems(DDODataset dataset)
		{
			// need to find all set properties with qualifying set bonuses and expand them into the item properties
			for (int i = 0; i < Properties.Count; i++)
			{
				var gsp = Properties[i];
				if (gsp.IsGroup && gsp.ItemProperties[0].Type == "set")
				{
					List<ItemProperty> ips = new List<ItemProperty>();
					DDOItemSet set = dataset.Sets[gsp.ItemProperties[0].Property];
					DDOItemSetBonus sb = null;
					foreach (var sbs in set.SetBonuses)
					{
						if (sbs.MinimumItems > gsp.ItemProperties.Count) break;
						sb = sbs;
					}

					if (sb == null) continue;

					foreach (var sip in sb.Bonuses)
					{
						ItemProperty ip = new ItemProperty { Property = sip.Property, Type = sip.Type, Value = sip.Value };
						ip.SetBonusOwner = set.Name;
						ips.Add(ip);
					}

					if (ips.Count > 0) AddProperties(ips);
				}
			}

			CalculatePropertyGroups();
		}

		public XmlElement ToXml(XmlDocument doc)
		{
			XmlElement xg = doc.CreateElement("GearSet");

			foreach (var item in Items)
				xg.AppendChild(item.ToXml(doc));

			return xg;
		}

		public static GearSet FromXml(XmlElement xe, DDODataset ds)
		{
			try
			{
				GearSet gs = new GearSet();
				foreach (XmlElement xb in xe.ChildNodes)
				{
					BuildItem bi = BuildItem.FromXml(xb, ds);
					if (bi != null) gs.AddItem(bi);
				}
				gs.ProcessItems(ds);

				return gs;
			}
			catch { return null; }
		}
	}

	public class GearSetEvaluation
	{
		public GearSet GearSet;
		public List<EquipmentSlotType> LockedSlots = new List<EquipmentSlotType>();
		public int Rating;
		public int Penalty;

		public GearSetEvaluation(GearSet gs, List<EquipmentSlotType> ls)
		{
			GearSet = gs;
			LockedSlots.AddRange(ls);
		}

		public XmlElement ToXml(XmlDocument doc)
		{
			XmlElement xg = doc.CreateElement("GearSetEvaluation");
			XmlAttribute xa = doc.CreateAttribute("rating");
			xa.InnerText = Rating.ToString();
			xg.Attributes.Append(xa);
			xa = doc.CreateAttribute("penalty");
			xa.InnerText = Penalty.ToString();
			xg.Attributes.Append(xa);

			XmlElement xe = GearSet.ToXml(doc);
			xg.AppendChild(xe);

			xe = doc.CreateElement("LockedSlots");
			xg.AppendChild(xe);
			foreach (var est in LockedSlots)
			{
				XmlElement ls = doc.CreateElement("EquipmentSlot");
				ls.InnerText = est.ToString();
				xe.AppendChild(ls);
			}

			return xg;
		}

		public static GearSetEvaluation FromXml(XmlElement xe, DDODataset ds)
		{
			try
			{
				GearSet gs = GearSet.FromXml(xe.GetElementsByTagName("GearSet")[0] as XmlElement, ds);
				XmlElement xls = xe.GetElementsByTagName("LockedSlots")[0] as XmlElement;
				List<EquipmentSlotType> ls = new List<EquipmentSlotType>();
				foreach (XmlElement xc in xls.ChildNodes)
				{
					ls.Add((EquipmentSlotType)Enum.Parse(typeof(EquipmentSlotType), xc.InnerText));
				}
				GearSetEvaluation gse = new GearSetEvaluation(gs, ls);
				gse.Rating = int.Parse(xe.GetAttribute("rating"));
				gse.Penalty = int.Parse(xe.GetAttribute("penalty"));

				return gse;
			}
			catch { return null; }
		}
	}

    public class GearSetBuild
    {
		public int MinimumLevel = 1;
		public int MaximumLevel = 30;

		public int CurrentBuildResult;

		public bool FiltersResultsMismatch;

		public string AppVersion;

		public Dictionary<SlotType, List<BuildFilter>> Filters = new Dictionary<SlotType, List<BuildFilter>>()
		{
			{ SlotType.None, new List<BuildFilter>() },
			{ SlotType.Back, new List<BuildFilter>() },
			{ SlotType.Body, new List<BuildFilter>() },
			{ SlotType.Eye, new List<BuildFilter>() },
			{ SlotType.Feet, new List<BuildFilter>() },
			{ SlotType.Finger, new List<BuildFilter>() },
			{ SlotType.Hand, new List<BuildFilter>() },
			{ SlotType.Head, new List<BuildFilter>() },
			{ SlotType.Neck, new List<BuildFilter>() },
			{ SlotType.Offhand, new List<BuildFilter>() },
			{ SlotType.Trinket, new List<BuildFilter>() },
			{ SlotType.Waist, new List<BuildFilter>() },
			{ SlotType.Weapon, new List<BuildFilter>() },
			{ SlotType.Wrist, new List<BuildFilter>() }
		};
		// so we can remember the lock state of the slots
		public List<EquipmentSlotType> LockedSlots = new List<EquipmentSlotType>();
		public List<BuildItem> LockedSlotItems = new List<BuildItem>();
		public List<GearSetEvaluation> BuildResults = new List<GearSetEvaluation>();

		public void SetLockStatus(EquipmentSlotType est, bool locked)
		{
			if (locked && !LockedSlots.Contains(est)) LockedSlots.Add(est);
			else if (!locked && LockedSlots.Contains(est)) LockedSlots.Remove(est);
		}

		public void Clear()
		{
			MinimumLevel = 1;
			MaximumLevel = 30;
			CurrentBuildResult = -1;
			FiltersResultsMismatch = false;

			foreach (var kv in Filters)
				kv.Value.Clear();

			LockedSlots.Clear();
			BuildResults.Clear();

			AppVersion = PlannerWindow.VERSION;
		}

		public bool ValidateFilters(bool test)
		{
			var filters = test ? TestFilters : Filters;

			foreach (var fg in filters)
				foreach (var f in fg.Value)
				{
					if (f.Include) return true;
				}

			return false;
		}

		#region Build process support
		public Dictionary<EquipmentSlotType, List<DDOItemData>> DiscoveredItems, FilterTestItems;
		public Dictionary<SlotType, List<BuildFilter>> TestFilters = new Dictionary<SlotType, List<BuildFilter>>()
		{
			{ SlotType.None, new List<BuildFilter>() },
			{ SlotType.Back, new List<BuildFilter>() },
			{ SlotType.Body, new List<BuildFilter>() },
			{ SlotType.Eye, new List<BuildFilter>() },
			{ SlotType.Feet, new List<BuildFilter>() },
			{ SlotType.Finger, new List<BuildFilter>() },
			{ SlotType.Hand, new List<BuildFilter>() },
			{ SlotType.Head, new List<BuildFilter>() },
			{ SlotType.Neck, new List<BuildFilter>() },
			{ SlotType.Offhand, new List<BuildFilter>() },
			{ SlotType.Trinket, new List<BuildFilter>() },
			{ SlotType.Waist, new List<BuildFilter>() },
			{ SlotType.Weapon, new List<BuildFilter>() },
			{ SlotType.Wrist, new List<BuildFilter>() }
		};

		public void SetupLockedSlots(Dictionary<EquipmentSlotType, EquipmentSlotControl> EquipmentSlots)
		{
			LockedSlots.Clear();
			LockedSlotItems.Clear();
			foreach (var kv in EquipmentSlots)
				if (kv.Value.IsLocked)
				{
					LockedSlots.Add(kv.Key);
					if (kv.Value.Item != null) LockedSlotItems.Add(kv.Value.Item);
				}
		}

		public void SetupFilterTest(Dictionary<EquipmentSlotType, EquipmentSlotControl> EquipmentSlots)
		{
			foreach (var kvp in TestFilters)
				kvp.Value.Clear();

			FilterTestItems = new Dictionary<EquipmentSlotType, List<DDOItemData>>();
			SetupLockedSlots(EquipmentSlots);
		}

		public void SetupBuildProcess(Dictionary<EquipmentSlotType, EquipmentSlotControl> EquipmentSlots)
		{
			BuildResults.Clear();
			DiscoveredItems = new Dictionary<EquipmentSlotType, List<DDOItemData>>();
			FilterTestItems = null;
			SetupLockedSlots(EquipmentSlots);
		}

		public void AddBuildGearSet(GearSet gs)
		{
			foreach (var ls in LockedSlotItems) gs.AddItem(ls);
			BuildResults.Add(new GearSetEvaluation(gs, new List<EquipmentSlotType>(LockedSlots)));
		}
		#endregion

		public XmlDocument ToXml(bool filters, bool results)
		{
			XmlDocument doc = new XmlDocument();
			XmlElement xe = doc.CreateElement("Build");
			XmlAttribute xa = doc.CreateAttribute("version");
			xa.InnerText = AppVersion;
			xe.Attributes.Append(xa);
			doc.AppendChild(xe);
			xe = doc.CreateElement("MinimumLevel");
			xe.InnerText = MinimumLevel.ToString();
			doc.DocumentElement.AppendChild(xe);
			xe = doc.CreateElement("MaximumLevel");
			xe.InnerText = MaximumLevel.ToString();
			doc.DocumentElement.AppendChild(xe);

			if (filters)
			{
				xe = doc.CreateElement("Filters");
				doc.DocumentElement.AppendChild(xe);
				foreach (var kv in Filters)
				{
					if (kv.Value.Count == 0) continue;
					foreach (var f in kv.Value)
					{
						XmlElement xf = f.ToXml(doc);
						xe.AppendChild(xf);
					}
				}
			}

			if (results)
			{
				xe = doc.CreateElement("BuildResults");
				doc.DocumentElement.AppendChild(xe);
				foreach (var gse in BuildResults)
				{
					XmlElement xg = gse.ToXml(doc);
					xe.AppendChild(xg);
				}
			}

			return doc;
		}

		public static GearSetBuild FromXml(DDODataset dataset, XmlDocument doc, bool filters, bool results)
		{
			GearSetBuild build = new GearSetBuild();

			try
			{
				build.AppVersion = doc.DocumentElement.GetAttribute("version");
				build.MinimumLevel = int.Parse(doc.GetElementsByTagName("MinimumLevel")[0].InnerText);
				build.MaximumLevel = int.Parse(doc.GetElementsByTagName("MaximumLevel")[0].InnerText);

				if (filters)
				{
					XmlElement xe = doc.GetElementsByTagName("Filters")[0] as XmlElement;
					foreach (XmlElement xf in xe.ChildNodes)
					{
						BuildFilter bf = BuildFilter.FromXml(xf);
						build.Filters[bf.Slot].Add(bf);
					}
				}

				if (results)
				{
					XmlElement xe = doc.GetElementsByTagName("BuildResults")[0] as XmlElement;
					foreach (XmlElement xb in xe.ChildNodes)
					{
						GearSetEvaluation gse = GearSetEvaluation.FromXml(xb, dataset);
						build.BuildResults.Add(gse);
					}
				}

				return build;
			}
			catch
			{
				return null;
			}
		}
	}
}
