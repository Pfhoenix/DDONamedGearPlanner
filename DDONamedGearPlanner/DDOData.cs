using System;
using System.Collections.Generic;
using System.Linq;

namespace DDONamedGearPlanner
{
	public enum EquipmentSlotType
	{
		None,
		Back,
		Body,
		Eye,
		Feet,
		Finger1,
		Finger2,
		Hand,
		Head,
		Neck,
		Offhand,
		Trinket,
		Waist,
		Weapon,
		Wrist
	}

	[Flags]
	public enum SlotType
	{
		None = 0,
		Back = 1,
		Body = 2,
		Eye = 4,
		Feet = 8,
		Finger = 16,
		Hand = 32,
		Head = 64,
		Neck = 128,
		Offhand = 256,
		Trinket = 512,
		Waist = 1024,
		Weapon = 2048,
		Wrist = 4096
	}

	static class EquipmentSlotTypeConversionExtensions
	{
		public static SlotType ToSlotType(this EquipmentSlotType est)
		{
			if (est == EquipmentSlotType.Finger1 || est == EquipmentSlotType.Finger2) return SlotType.Finger;
			else return (SlotType)Enum.Parse(typeof(SlotType), est.ToString());
		}
	}

	[Flags]
	public enum ArmorCategory
	{
		Cloth = 1,
		Light = 2,
		Medium = 4,
		Heavy = 8,
		Docent = 16
	}

	[Flags]
	public enum OffhandCategory
	{
		Buckler = 1,
		Small = 2,
		Large = 4,
		Tower = 8,
		Orb = 16,
		RuneArm = 32
	}

	[Flags]
	public enum WeaponCategory
	{
		Simple = 1,
		Martial = 2,
		Exotic = 4,
		Throwing = 8
	}

	[Serializable]
	public class ItemProperty
	{
		public string Property { get; set; }
		public string Type { get; set; }
		public float Value { get; set; }
		public List<ItemProperty> Options;
		public DDOItemData Owner;
		// this will only ever be used by the interface to create ad hoc item properties in order to track set bonuses in gear sets
		public string SetBonusOwner;

		public ItemProperty Duplicate()
		{
			ItemProperty ip = new ItemProperty
			{
				Property = Property,
				Type = Type,
				Value = Value
			};
			if (Options != null)
			{
				ip.Options = new List<ItemProperty>();
				foreach (var o in Options)
				{
					ItemProperty op = o.Duplicate();
					ip.Options.Add(op);
				}
			}

			return ip;
		}
	}

	[Serializable]
	public class DDOItemData
	{
		public string Name { get; set; }
		public string WikiURL;
		public SlotType Slot { get; set; }
		public int Category;
		public List<ItemProperty> Properties = new List<ItemProperty>();

		// utility because it gets used so often
		int _Handedness = -1;
		public int Handedness
		{
			get
			{
				if (_Handedness > -1) return _Handedness;
				if (Slot != SlotType.Weapon) _Handedness = 0;
				else _Handedness = (int)Properties.Find(p => p.Property == "Handedness").Value;
				return _Handedness;
			}
		}

		// utility because it gets used so often
		string _WeaponType;
		public string WeaponType
		{
			get
			{
				if (_WeaponType != null) return _WeaponType;
				if (Slot != SlotType.Weapon) return "";
				else _WeaponType = Properties.Find(p => p.Property == "Weapon Type").Type;
				return _WeaponType;
			}
		}

		//utility because it gets used so often
		int _ML = -1;
		public int ML
		{
			get
			{
				if (_ML != -1) return _ML;
				_ML = (int)(Properties.Find(p => p.Property == "Minimum Level")?.Value ?? 1);
				return _ML;
			}
		}

		public ItemProperty AddProperty(string prop, string type, float value, List<ItemProperty> options)
		{
			ItemProperty ip = new ItemProperty { Property = prop, Type = type, Value = value, Options = options, Owner = this };
			if (type == "insightful") ip.Type = "insight";
			if (options != null)
				foreach (var i in options) i.Owner = this;
			Properties.Add(ip);
			return ip;
		}

		public override string ToString()
		{
			return Name;
		}

		public DDOItemData Duplicate()
		{
			DDOItemData item = new DDOItemData
			{
				Name = Name,
				WikiURL = WikiURL,
				Slot = Slot,
				Category = Category
			};
			foreach (var p in Properties)
			{
				ItemProperty ip = p.Duplicate();
				ip.Owner = item;
				if (ip.Options != null)
				{
					foreach (ItemProperty ipt in ip.Options)
						ipt.Owner = item;
				}
				item.Properties.Add(ip);
			}

			return item;
		}
	}

	[Serializable]
	public class DDOSlot
	{
		public SlotType Slot;
		public List<DDOItemData> Items = new List<DDOItemData>();
		public Type CategoryEnumType;
	}

	[Serializable]
	public class DDOItemProperty
	{
		public string Property { get; set; }
		public List<string> Types = new List<string>();
		public List<DDOItemData> Items = new List<DDOItemData>();
		public SlotType SlotsFoundOn;
	}

	[Serializable]
	public class DDOItemSetBonusProperty
	{
		public string Property;
		public string Type;
		public float Value;
	}

	[Serializable]
	public class DDOItemSetBonus
	{
		public int MinimumItems;
		public List<DDOItemSetBonusProperty> Bonuses;
	}

	[Serializable]
	public class DDOItemSet
	{
		public string Name;
		public string WikiURL;
		public List<DDOItemSetBonus> SetBonuses;
		public List<DDOItemData> Items = new List<DDOItemData>();
	}

	[Serializable]
	public class DDODataset
	{
		public Dictionary<SlotType, DDOSlot> Slots = new Dictionary<SlotType, DDOSlot>();
		public Dictionary<string, DDOItemProperty> ItemProperties = new Dictionary<string, DDOItemProperty>();
		public List<DDOItemData> Items = new List<DDOItemData>();
		public Dictionary<string, DDOItemSet> Sets = new Dictionary<string, DDOItemSet>();
		public Dictionary<SlotType, List<DDOItemProperty>> SlotExclusiveItemProperties = new Dictionary<SlotType, List<DDOItemProperty>>();

		public void Initialize()
		{
			Slots.Add(SlotType.Back, new DDOSlot { Slot = SlotType.Back });
			Slots.Add(SlotType.Body, new DDOSlot { Slot = SlotType.Body, CategoryEnumType = typeof(ArmorCategory) });
			Slots.Add(SlotType.Eye, new DDOSlot { Slot = SlotType.Eye });
			Slots.Add(SlotType.Feet, new DDOSlot { Slot = SlotType.Feet });
			Slots.Add(SlotType.Finger, new DDOSlot { Slot = SlotType.Finger });
			Slots.Add(SlotType.Hand, new DDOSlot { Slot = SlotType.Hand });
			Slots.Add(SlotType.Head, new DDOSlot { Slot = SlotType.Head });
			Slots.Add(SlotType.Neck, new DDOSlot { Slot = SlotType.Neck });
			Slots.Add(SlotType.Offhand, new DDOSlot { Slot = SlotType.Offhand, CategoryEnumType = typeof(OffhandCategory) });
			Slots.Add(SlotType.Trinket, new DDOSlot { Slot = SlotType.Trinket });
			Slots.Add(SlotType.Waist, new DDOSlot { Slot = SlotType.Waist });
			Slots.Add(SlotType.Weapon, new DDOSlot { Slot = SlotType.Weapon, CategoryEnumType = typeof(WeaponCategory) });
			Slots.Add(SlotType.Wrist, new DDOSlot { Slot = SlotType.Wrist });

			// I hate doing this but there isn't a more expedient way to pull the data from the wiki cleanly

			#region Korvos sets
			Sets.Add("Anger's Wrath", new DDOItemSet
			{
				Name = "Anger's Wrath",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Anger.27s_Wrath",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Arcane Mind", new DDOItemSet
			{
				Name = "Arcane Mind",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Arcane_Mind",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Power",
								Type = "equipment",
								Value = 24
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Power",
								Type = "equipment",
								Value = 24
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Power",
								Type = "equipment",
								Value = 24
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "equipment",
								Value = 24
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Power",
								Type = "equipment",
								Value = 24
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Power",
								Type = "equipment",
								Value = 24
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Power",
								Type = "equipment",
								Value = 24
							},
							new DDOItemSetBonusProperty
							{
								Property = "Poison Spell Power",
								Type = "equipment",
								Value = 24
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "equipment",
								Value = 24
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Spell Power",
								Type = "equipment",
								Value = 24
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sonic Spell Power",
								Type = "equipment",
								Value = 24
							}
						}
					}
				}
			});

			Sets.Add("Archivist", new DDOItemSet
			{
				Name = "Archivist",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Archivist",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Spell Points",
								Type = "enhancement",
								Value = 20
							}
						}
					}
				}
			});

			Sets.Add("Devoted Heart", new DDOItemSet
			{
				Name = "Devoted Heart",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Devoted_Heart",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "equipment",
								Value = 36
							}
						}
					}
				}
			});

			Sets.Add("Nimble Hand", new DDOItemSet
			{
				Name = "Nimble Hand",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Nimble_Hand",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Damage",
								Type = "enhancement",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Pathfinders", new DDOItemSet
			{
				Name = "Pathfinders",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Pathfinders",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Protector's Heart", new DDOItemSet
			{
				Name = "Protector's Heart",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Protector.27s_Heart",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Fortification",
								Type = "enhancement",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class",
								Type = "insight",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Troubleshooter", new DDOItemSet
			{
				Name = "Troubleshooter",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Troubleshooter",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Fortitude",
								Type = "insight",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Reflex",
								Type = "insight",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Will",
								Type = "insight",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Open Lock",
								Type = "competence",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Disable Device",
								Type = "competence",
								Value = 3
							}
						}
					}
				}
			});
			#endregion

			#region Chronoscope sets
			Sets.Add("Might of the Abishai", new DDOItemSet
			{
				Name = "Might of the Abishai",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Might_of_the_Abishai",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class",
								Type = "profane natural armor",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Strength",
								Type = "profane",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Caster Level",
								Value = 1
							}
						}
					},
					new DDOItemSetBonus
					{
						MinimumItems = 5,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class",
								Type = "profane natural armor",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Strength",
								Type = "profane",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Constitution",
								Type = "profane",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Caster Level",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Caster Level",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Greater Might of the Abishai", new DDOItemSet
			{
				Name = "Greater Might of the Abishai",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Greater_Might_of_the_Abishai",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class",
								Type = "profane natural armor",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Strength",
								Type = "profane",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Caster Level",
								Value = 3
							}
						}
					},
					new DDOItemSetBonus
					{
						MinimumItems = 5,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class",
								Type = "profane natural armor",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Strength",
								Type = "profane",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Constitution",
								Type = "profane",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Caster Level",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Caster Level",
								Value = 3
							}
						}
					}
				}
			});
			#endregion

			#region Three Barrel Cove sets
			Sets.Add("Corsair's Cunning", new DDOItemSet
			{
				Name = "Corsair's Cunning",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Corsair.27s_Cunning",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Underwater Action"
							}
						}
					},
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Underwater Action"
							},
							new DDOItemSetBonusProperty
							{
								Property = "Feather Falling"
							}
						}
					}
				}
			});
			#endregion

			#region The Red Fens sets
			Sets.Add("Divine Blessing", new DDOItemSet
			{
				Name = "Divine Blessing",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Divine_Blessing",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "equipment",
								Value = 55
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Power",
								Type = "equipment",
								Value = 55
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Power",
								Type = "equipment",
								Value = 55
							},
							new DDOItemSetBonusProperty
							{
								Property = "Poison Spell Power",
								Type = "equipment",
								Value = 55
							}
						}
					}
				}
			});

			Sets.Add("Greater Divine Blessing", new DDOItemSet
			{
				Name = "Greater Divine Blessing",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Greater_Divine_Blessing",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "equipment",
								Value = 90
							}
						}
					}
				}
			});

			Sets.Add("Elder's Knowledge", new DDOItemSet
			{
				Name = "Elder's Knowledge",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Elder.27s_Knowledge",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Critical Hit Chance",
								Type = "insight",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Universal Spell Power",
								Type = "alchemical",
								Value = 12
							}
						}
					}
				}
			});

			Sets.Add("Greater Elder's Knowledge", new DDOItemSet
			{
				Name = "Greater Elder's Knowledge",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Greater_Elder.27s_Knowledge",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Critical Chance",
								Type = "enhancement",
								Value = 6
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Critical Chance",
								Type = "enhancement",
								Value = 6
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Critical Chance",
								Type = "enhancement",
								Value = 6
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Critical Chance",
								Type = "enhancement",
								Value = 6
							},
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Critical Multiplier",
								Type = "enhancement",
								Value = 0.5f
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Critical Multiplier",
								Type = "enhancement",
								Value = 0.5f
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Critical Multiplier",
								Type = "enhancement",
								Value = 0.5f
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Critical Multiplier",
								Type = "enhancement",
								Value = 0.5f
							},
						}
					}
				}
			});

			Sets.Add("Marshwalker", new DDOItemSet
			{
				Name = "Marshwalker",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Marshwalker",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Move Speed",
								Type = "enhancement",
								Value = 24
							},
							new DDOItemSetBonusProperty
							{
								Property = "Jump",
								Type = "competence",
								Value = 15
							}
						}
					}
				}
			});

			Sets.Add("Greater Marshwalker", new DDOItemSet
			{
				Name = "Greater Marshwalker",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Greater_Marshwalker",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Move Speed",
								Type = "enhancement",
								Value = 30
							},
							new DDOItemSetBonusProperty
							{
								Property = "Jump",
								Type = "competence",
								Value = 15
							}
						}
					}
				}
			});

			Sets.Add("Raven's Eye", new DDOItemSet
			{
				Name = "Raven's Eye",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Raven.27s_Eye",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Spot",
								Type = "competence",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Search",
								Type = "competence",
								Value = 10
							}
						}
					}
				}
			});

			Sets.Add("Greater Raven's Eye", new DDOItemSet
			{
				Name = "Greater Raven's Eye",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Greater_Raven.27s_Eye",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Spot",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Search",
								Value = 3
							}
						}
					}
				}
			});

			Sets.Add("Shaman's Fury", new DDOItemSet
			{
				Name = "Shaman's Fury",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Shaman.27s_Fury",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Power",
								Type = "equipment",
								Value = 55
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Power",
								Type = "equipment",
								Value = 55
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Power",
								Type = "equipment",
								Value = 55
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "equipment",
								Value = 55
							}
						}
					}
				}
			});

			Sets.Add("Greater Shaman's Fury", new DDOItemSet
			{
				Name = "Greater Shaman's Fury",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Greater_Shaman.27s_Fury",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Power",
								Type = "equipment",
								Value = 84
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Power",
								Type = "equipment",
								Value = 84
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Power",
								Type = "equipment",
								Value = 84
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "equipment",
								Value = 84
							}
						}
					}
				}
			});

			Sets.Add("Siren's Ward", new DDOItemSet
			{
				Name = "Siren's Ward",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Siren.27s_Ward",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Fortitude",
								Type = "insight",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Reflex",
								Type = "insight",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Will",
								Type = "insight",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class",
								Type = "insight",
								Value = 6
							}
						}
					}
				}
			});

			Sets.Add("Greater Siren's Ward", new DDOItemSet
			{
				Name = "Greater Siren's Ward",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Greater_Siren.27s_Ward",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Fortitude",
								Type = "insight",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Reflex",
								Type = "insight",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Will",
								Type = "insight",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class",
								Type = "insight",
								Value = 4
							}
						}
					}
				}
			});

			Sets.Add("Vulkoor's Cunning", new DDOItemSet
			{
				Name = "Vulkoor's Cunning",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Vulkoor.27s_Cunning",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Reduction",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Vulkoorim Poison"
							}
						}
					}
				}
			});

			Sets.Add("Greater Vulkoor's Cunning", new DDOItemSet
			{
				Name = "Greater Vulkoor's Cunning",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Greater_Vulkoor.27s_Cunning",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Reduction",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Powerful Vulkoorim Poison"
							}
						}
					}
				}
			});

			Sets.Add("Vulkoor's Might", new DDOItemSet
			{
				Name = "Vulkoor's Might",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Vulkoor.27s_Might",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Generation",
								Value = 20
							}
						}
					}
				}
			});

			Sets.Add("Greater Vulkoor's Might", new DDOItemSet
			{
				Name = "Greater Vulkoor's Might",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Greater_Vulkoor.27s_Might",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Damage",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Generation",
								Value = 20
							}
						}
					}
				}
			});
			#endregion

			#region Wrath of Sora Kell set
			Sets.Add("Wrath of Sora Kell", new DDOItemSet
			{
				Name = "Wrath of Sora Kell",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Wrath_of_Sora_Kell",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Universal Spell Power",
								Type = "equipment",
								Value = 40
							},
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage",
								Type = "artifact",
								Value = 2
							}
						}
					}
				}
			});
			#endregion

			#region Subterrane Raid and Dragontouched sets
			Sets.Add("Glacial Assault", new DDOItemSet
			{
				Name = "Glacial Assault",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Glacial_Assault",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Power",
								Type = "equipment",
								Value = 72
							}
						}
					},
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Power",
								Type = "equipment",
								Value = 78
							}
						}
					}
				}
			});

			Sets.Add("Levik's Defender", new DDOItemSet
			{
				Name = "Levik's Defender",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Levik.27s_Defender",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class",
								Type = "insight",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Generation",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fortitude",
								Type = "insight",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Will",
								Type = "insight",
								Value = 1
							}
						}
					},
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class",
								Type = "insight",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Generation",
								Value = 30
							}
						}
					}
				}
			});

			Sets.Add("Lorikk's Champion", new DDOItemSet
			{
				Name = "Lorik's Champion",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Lorikk.27s_Champion",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "equipment",
								Value = 72
							}
						}
					},
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "equipment",
								Value = 78
							}
						}
					}
				}
			});

			Sets.Add("Tharne's Wrath", new DDOItemSet
			{
				Name = "Tharne's Wrath",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Tharne.27s_Wrath",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Ghost Touch"
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Reduction",
								Value = 20
							}
						}
					},
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Ghost Touch"
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Reduction",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Feather Falling"
							}
						}
					}
				}
			});
			#endregion

			#region Prestige Enhancements sets
			Sets.Add("Dragonmark Heir", new DDOItemSet
			{
				Name = "Dragonmark Heir",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Dragonmark_Heir",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Greater Dragonmark Uses",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Frenzied Berserker", new DDOItemSet
			{
				Name = "Frenzied Berserker",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Frenzied_Berserker",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Rage Uses",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Strength",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Constitution",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Occult Slayer", new DDOItemSet
			{
				Name = "Occult Slayer",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Occult_Slayer",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Will",
								Type = "exceptional",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Reflex",
								Type = "exceptional",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Spell Resistance",
								Type = "enhancement",
								Value = 22
							}
						}
					}
				}
			});

			Sets.Add("Ravager", new DDOItemSet
			{
				Name = "Ravager",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Ravager",
				SetBonuses = new List<DDOItemSetBonus>()
			});

			Sets.Add("Spell Singer", new DDOItemSet
			{
				Name = "Spell Singer",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Spell_Singer",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Bard Song Uses",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Virtuoso", new DDOItemSet
			{
				Name = "Virtuoso",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Virtuoso",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Bard Song Uses",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Extend Spell Point Reduction",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Warchanter", new DDOItemSet
			{
				Name = "Warchanter",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Warchanter",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Bard Song Uses",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Maximize Spell Point Reduction",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Extend Spell Point Reduction",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Exorcist of the Silver Flame", new DDOItemSet
			{
				Name = "Exorcist of the Silver Flame",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Exorcist_of_the_Silver_Flame",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Turn Undead Uses",
								Type = "enhancement",
								Value = 3
							}
						}
					}
				}
			});

			Sets.Add("Radiant Servant", new DDOItemSet
			{
				Name = "Radiant Servant",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Radiant_Servant",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Turn Undead Uses",
								Type = "enhancement",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Turn Undead Effective Level",
								Type = "exceptional",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Warpriest", new DDOItemSet
			{
				Name = "Warpriest",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Warpriest",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Turn Undead Uses",
								Type = "enhancement",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Attack",
								Type = "exceptional",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Kensai", new DDOItemSet
			{
				Name = "Kensai",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Kensai",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Melee Attack",
								Type = "exceptional",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Confirm Critical Hits",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Type = "exceptional",
								Value = 3
							}
						}
					}
				}
			});

			Sets.Add("Purple Dragon Knight", new DDOItemSet
			{
				Name = "Purple Dragon Knight",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Purple_Dragon_Knight",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Generation",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Threat Generation",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Type = "exceptional",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Stalwart Defender", new DDOItemSet
			{
				Name = "Stalwart Defender",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Stalwart_Defender",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Generation",
								Value = 15
							},
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Henshin Mystic", new DDOItemSet
			{
				Name = "Henshin Mystic",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Henshin_Mystic",
				SetBonuses = new List<DDOItemSetBonus>()
			});

			Sets.Add("Ninja Spy", new DDOItemSet
			{
				Name = "Ninja Spy",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Ninja_Spy",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Attack",
								Type = "exceptional",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Damage",
								Type = "exceptional",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Reduction",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Threat Reduction",
								Value = 20
							}
						}
					}
				}
			});

			Sets.Add("Shintao Monk", new DDOItemSet
			{
				Name = "Shintao Monk",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Shintao_Monk",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Type = "exceptional",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage",
								Type = "exceptional",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Generation",
								Value = 15
							}
						}
					}
				}
			});

			Sets.Add("Defender of Siberys", new DDOItemSet
			{
				Name = "Defender of Siberys",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Defender_of_Siberys",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Generation",
								Value = 15
							},
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Hunter of the Dead", new DDOItemSet
			{
				Name = "Hunter of the Dead",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Hunter_of_the_Dead",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Remove Disease Uses",
								Value = 3
							}
						}
					}
				}
			});

			Sets.Add("Knight of the Chalice", new DDOItemSet
			{
				Name = "Knight of the Chalice",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Knight_of_the_Chalice",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Spell Resistance",
								Type = "enhancement",
								Value = 22
							}
						}
					}
				}
			});

			Sets.Add("Arcane Archer", new DDOItemSet
			{
				Name = "Arcane Archer",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Arcane_Archer",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Attack Speed",
								Type = "competnce",
								Value = 10
							}
						}
					}
				}
			});

			Sets.Add("Deepwood Sniper", new DDOItemSet
			{
				Name = "Deepwood Sniper",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Deepwood_Sniper",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Attack Speed",
								Type = "competence",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Tempest", new DDOItemSet
			{
				Name = "Tempest",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Tempest",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Value = 3
							}
						}
					}
				}
			});

			Sets.Add("Assassin", new DDOItemSet
			{
				Name = "Assassin",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Assassin",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Attack",
								Type = "enhancement",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Damage",
								Type = "enhancement",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Reduction",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Threat Reduction",
								Value = 20
							}
						}
					}
				}
			});

			Sets.Add("Mechanic", new DDOItemSet
			{
				Name = "Mechanic",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Mechanic",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Open Lock",
								Type = "competence",
								Value = 15
							},
							new DDOItemSetBonusProperty
							{
								Property = "Disable Device",
								Type = "competence",
								Value = 15
							},
							new DDOItemSetBonusProperty
							{
								Property = "Balance",
								Type = "exceptional",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Hide",
								Type = "exceptional",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Move Silently",
								Type = "exceptional",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Open Lock",
								Type = "exceptional",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Tumble",
								Type = "exceptional",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Thief Acrobat", new DDOItemSet
			{
				Name = "Thief Acrobat",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Thief_Acrobat",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Jump",
								Type = "competence",
								Value = 15
							}
						}
					}
				}
			});

			Sets.Add("Air Savant", new DDOItemSet
			{
				Name = "Air Savant",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Air_Savant",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Critical Chance",
								Type = "artifact",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Earth Savant", new DDOItemSet
			{
				Name = "Earth Savant",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Earth_Savant",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Critical Chance",
								Type = "artifact",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Fire Savant", new DDOItemSet
			{
				Name = "Fire Savant",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Fire_Savant",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Critical Chance",
								Type = "artifact",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Water Savant", new DDOItemSet
			{
				Name = "Water Savant",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Water_Savant",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Critical Chance",
								Type = "artifact",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Archmage", new DDOItemSet
			{
				Name = "Archmage",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Archmage",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Critical Chance",
								Type = "equipment",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Critical Chance",
								Type = "equipment",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Critical Chance",
								Type = "equipment",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Critical Chance",
								Type = "equipment",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Critical Chance",
								Type = "equipment",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Critical Chance",
								Type = "equipment",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Critical Chance",
								Type = "equipment",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Poison Spell Critical Chance",
								Type = "equipment",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Critical Chance",
								Type = "equipment",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Spell Critical Chance",
								Type = "equipment",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sonic Spell Critical Chance",
								Type = "equipment",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Critical Multiplier",
								Type = "equipment",
								Value = 0.5f
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Critical Multiplier",
								Type = "equipment",
								Value = 0.5f
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Critical Multiplier",
								Type = "equipment",
								Value = 0.5f
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Critical Multiplier",
								Type = "equipment",
								Value = 0.5f
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Critical Multiplier",
								Type = "equipment",
								Value = 0.5f
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Critical Multiplier",
								Type = "equipment",
								Value = 0.5f
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Critical Multiplier",
								Type = "equipment",
								Value = 0.5f
							},
							new DDOItemSetBonusProperty
							{
								Property = "Poison Spell Critical Multiplier",
								Type = "equipment",
								Value = 0.5f
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Critical Multiplier",
								Type = "equipment",
								Value = 0.5f
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Spell Critical Multiplier",
								Type = "equipment",
								Value = 0.5f
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sonic Spell Critical Multiplier",
								Type = "equipment",
								Value = 0.5f
							}
						}
					}
				}
			});

			Sets.Add("Pale Master", new DDOItemSet
			{
				Name = "Pale Master",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Pale_Master",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Power",
								Type = "equipment",
								Value = 78
							},
							new DDOItemSetBonusProperty
							{
								Property = "Poison Spell Power",
								Type = "equipment",
								Value = 78
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Power",
								Type = "enhancement",
								Value = 90
							},
							new DDOItemSetBonusProperty
							{
								Property = "Poison Spell Power",
								Type = "enhancement",
								Value = 90
							}
						}
					}
				}
			});

			Sets.Add("Wild Mage", new DDOItemSet
			{
				Name = "Wild Mage",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Wild_Mage",
				SetBonuses = new List<DDOItemSetBonus>()
			});
			#endregion

			#region Secrets of the Artificers sets
			Sets.Add("Tinker's Finesse", new DDOItemSet
			{
				Name = "Tinker's Finesse",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Tinker.27s_Finesse",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "True Seeing"
							},
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Damage",
								Type = "enhancement",
								Value = 8
							}
						}
					}
				}
			});

			Sets.Add("Magewright's Expertise", new DDOItemSet
			{
				Name = "Magewright's Expertise",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Magewright.27s_Expertise",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "equipment",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "equipment",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "equipment",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "equipment",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "equipment",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "equipment",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "equipment",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "equipment",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Universal Spell Power",
								Type = "alchemical",
								Value = 12
							}
						}
					}
				}
			});

			Sets.Add("Fabricator's Ingenuity", new DDOItemSet
			{
				Name = "Fabricator's Ingenuity",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Fabricator.27s_Ingenuity",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Fortification",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage",
								Type = "artifact",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Alchemist's Lore", new DDOItemSet
			{
				Name = "Alchemist's Lore",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Alchemist.27s_Lore",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Empower Spell Point Reduction",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Empower Healing Spell Point Reduction",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Maximize Spell Point Reduction",
								Value = 2
							}
						}
					}
				}
			});
			#endregion

			#region Commendation sets
			Sets.Add("Amaunator's Blessing", new DDOItemSet
			{
				Name = "Amaunator's Blessing",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Amaunator.27s_Blessing",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Spell Point Cost Reduction %",
								Type = "enhancement",
								Value = 10
							}
						}
					}
				}
			});

			Sets.Add("Woodsman's Guile", new DDOItemSet
			{
				Name = "Woodsman's Guile",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Woodsman.27s_Guile",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Attack",
								Type = "enhancement",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Damage",
								Type = "enhancement",
								Value = 6
							}
						}
					}
				}
			});

			Sets.Add("Knight's Loyalty", new DDOItemSet
			{
				Name = "Knight's Loyalty",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Knight.27s_Loyalty",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class",
								Type = "insight natural armor",
								Value = 3
							}
						}
					}
				}
			});

			Sets.Add("Way of the Sun Soul", new DDOItemSet
			{
				Name = "Way of the Sun Soul",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Way_of_the_Sun_Soul",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage",
								Type = "artifact",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Battle Arcanist", new DDOItemSet
			{
				Name = "Battle Arcanist",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Battle_Arcanist",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Spell Point Cost Reduction %",
								Type = "enhancement",
								Value = 10
							}
						}
					}
				}
			});
			#endregion

			#region Planar Focus sets
			Sets.Add("Planar Focus: Erudition", new DDOItemSet
			{
				Name = "Planar Focus: Erudition",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Planar_Focus_Item_Sets",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Spell Penetration",
								Type = "equipment",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Spell Points",
								Type = "enhancement",
								Value = 250
							},
							new DDOItemSetBonusProperty
							{
								Property = "Universal Spell Power",
								Type = "psionic",
								Value = 15
							}
						}
					}
				}
			});

			Sets.Add("Planar Focus: Prowess", new DDOItemSet
			{
				Name = "Planar Focus: Prowess",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Planar_Focus_Item_Sets",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Attack",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "artifact",
								Value = 15
							}
						}
					}
				}
			});

			Sets.Add("Planar Focus: Subterfuge", new DDOItemSet
			{
				Name = "Planar Focus: Subterfuge",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Planar_Focus:_Subterfuge",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Attack",
								Type = "insight",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Damage",
								Type = "insight",
								Value = 8
							},
							new DDOItemSetBonusProperty
							{
								Property = "True Seeing"
							},
							new DDOItemSetBonusProperty
							{
								Property = "Dodge",
								Type = "enhancement",
								Value = 3
							}
						}
					}
				}
			});
			#endregion

			#region Dragonscale Armor sets
			Sets.Add("Draconic Ferocity", new DDOItemSet
			{
				Name = "Draconic Ferocity",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Draconic_Ferocity",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Doublestrike",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Attack",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Damage",
								Type = "artifact",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Draconic Mind", new DDOItemSet
			{
				Name = "Draconic Mind",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Draconic_Mind",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Universal Spell Power",
								Type = "artifact",
								Value = 15
							}
						}
					}
				}
			});

			Sets.Add("Draconic Resilience", new DDOItemSet
			{
				Name = "Draconic Resilience",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Draconic_Resilience",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Hit Points",
								Type = "artifact",
								Value = 50
							}
						}
					}
				}
			});
			#endregion

			#region Iconic Level 15 Reward sets
			Sets.Add("Risk and Reward", new DDOItemSet
			{
				Name = "Risk and Reward",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Risk_and_Reward",
				SetBonuses = new List<DDOItemSetBonus>()
			});
			#endregion

			#region Unbreakable Adamancy set
			Sets.Add("Unbreakable Adamancy", new DDOItemSet
			{
				Name = "Unbreakable Adamancy",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Unbreakable_Adamancy",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "luck",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Magical Resistance Rating",
								Type = "luck",
								Value = 5
							}
						}
					}
				}
			});
			#endregion

			#region The Devil's Gambits sets
			Sets.Add("Captain's Set", new DDOItemSet
			{
				Name = "Captain's Set",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Captain.27s_Set",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Strength",
								Type = "quality",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Epic Captain's Set", new DDOItemSet
			{
				Name = "Epic Captain's Set",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Epic_Captain.27s_Set",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Strength",
								Type = "quality",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Double Helix Set", new DDOItemSet
			{
				Name = "Double Helix Set",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Double_Helix_Set",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "insight",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Magical Resistance Rating",
								Type = "insight",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Epic Double Helix Set", new DDOItemSet
			{
				Name = "Epic Double Helix Set",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Epic_Double_Helix_Set",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "insight",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Magical Resistance Rating",
								Type = "insight",
								Value = 5
							}
						}
					}
				}
			});

			Sets.Add("Griffon Set", new DDOItemSet
			{
				Name = "Griffon Set",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Griffon_Set",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Constitution",
								Type = "quality",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Epic Griffon Set", new DDOItemSet
			{
				Name = "Epic Griffon Set",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Epic_Griffon_Set",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Constitution",
								Type = "quality",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Slice 'n Dice Set", new DDOItemSet
			{
				Name = "Slice 'n Dice Set",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Slice_.27n_Dice_Set",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Confirm Critical Hits",
								Type = "quality",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Critical Hit Damage",
								Type = "quality",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Attack",
								Type = "quality",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Damage",
								Type = "quality",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Epic Slice 'n Dice Set", new DDOItemSet
			{
				Name = "Epic Slice 'n Dice Set",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Epic_Slice_.27n_Dice_Set",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Confirm Critical Hits",
								Type = "quality",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Critical Hit Damage",
								Type = "quality",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Attack",
								Type = "quality",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Damage",
								Type = "quality",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("The Devil's Handiwork", new DDOItemSet
			{
				Name = "The Devil's Handiwork",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#The_Devil.27s_Handiwork",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 5,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Strength",
								Type = "quality",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Dexterity",
								Type = "quality",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Constitution",
								Type = "quality",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Intelligence",
								Type = "quality",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Wisdom",
								Type = "quality",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Charisma",
								Type = "quality",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Epic The Devil's Handiwork", new DDOItemSet
			{
				Name = "Epic The Devil's Handiwork",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Epic_The_Devil.27s_Handiwork",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 5,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Strength",
								Type = "quality",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Dexterity",
								Type = "quality",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Constitution",
								Type = "quality",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Intelligence",
								Type = "quality",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Wisdom",
								Type = "quality",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Charisma",
								Type = "quality",
								Value = 3
							}
						}
					}
				}
			});
			#endregion

			#region Slave Lords Crafting sets
			Sets.Add("Slave Lord's Might", new DDOItemSet
			{
				Name = "Slave Lord's Might",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Slave_Lord.27s_Might",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "artifact competence",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Power",
								Type = "artifact competence",
								Value = 1
							}
						}
					},
					new DDOItemSetBonus
					{
						MinimumItems = 5,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Strength",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Dexterity",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Power",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "artifact competence",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Power",
								Type = "artifact competence",
								Value = 2
							}
						}
					},
				}
			});

			Sets.Add("Legendary Slave Lord's Might", new DDOItemSet
			{
				Name = "Legendary Slave Lord's Might",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Slave_Lord.27s_Might",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Power",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "artifact competence",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Power",
								Type = "artifact competence",
								Value = 2
							}
						}
					},
					new DDOItemSetBonus
					{
						MinimumItems = 5,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Strength",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Dexterity",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Power",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "artifact competence",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Power",
								Type = "artifact competence",
								Value = 4
							}
						}
					},
				}
			});

			Sets.Add("Slave Lord's Sorcery", new DDOItemSet
			{
				Name = "Slave Lord's Sorcery",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Slave_Lord.27s_Sorcery",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Power",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Power",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Power",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Power",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Power",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Power",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Poison Spell Power",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Spell Power",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sonic Spell Power",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 1
							}
						}
					},
					new DDOItemSetBonus
					{
						MinimumItems = 5,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Intelligence",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Wisdom",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Charisma",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Poison Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sonic Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Legendary Slave Lord's Sorcery", new DDOItemSet
			{
				Name = "Legendary Slave Lord's Sorcery",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Slave_Lord.27s_Sorcery",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Poison Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sonic Spell Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 2
							}
						}
					},
					new DDOItemSetBonus
					{
						MinimumItems = 5,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Intelligence",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Wisdom",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Charisma",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Power",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Power",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Power",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Power",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Power",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Power",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Poison Spell Power",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Spell Power",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sonic Spell Power",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 4
							}
						}
					}
				}
			});

			Sets.Add("Slave's Endurance", new DDOItemSet
			{
				Name = "Slave's Endurance",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Slave.27s_Endurance",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Magical Resistance Rating",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fortitude",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Reflex",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Will",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Spell Save",
								Type = "artifact",
								Value = 1
							}
						}
					},
					new DDOItemSetBonus
					{
						MinimumItems = 5,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Constitution",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Magical Resistance Rating",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fortitude",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Reflex",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Will",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Spell Saves",
								Type = "artifact",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Legendary Slave's Endurance", new DDOItemSet
			{
				Name = "Legendary Slave's Endurance",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Slave.27s_Endurance",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Magical Resistance Rating",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fortitude",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Reflex",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Will",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Spell Saves",
								Type = "artifact",
								Value = 2
							}
						}
					},
					new DDOItemSetBonus
					{
						MinimumItems = 5,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Constitution",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Magical Resistance Rating",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fortitude",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Reflex",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Will",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Spell Saves",
								Type = "artifact",
								Value = 4
							}
						}
					}
				}
			});
			#endregion

			#region Ravenloft sets
			Sets.Add("Beacon of Magic", new DDOItemSet
			{
				Name = "Beacon of Magic",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Beacon_of_Magic_Set_.28Heroic.29",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Universal Spell Power",
								Type = "artifact",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Poison Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sonic Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Magical Resistance Rating Cap",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Missile Deflection",
								Type = "artifact",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Legendary Beacon of Magic", new DDOItemSet
			{
				Name = "Legendary Beacon of Magic",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Beacon_of_Magic_Set_.28Legendary.29",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Universal Spell Power",
								Type = "artifact",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Poison Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sonic Spell Power",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Magical Resistance Rating Cap",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Missile Deflection",
								Type = "artifact",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Knight of the Shadows", new DDOItemSet
			{
				Name = "Knight of the Shadows",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Knight_of_the_Shadows",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Magical Resistance Rating",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Generation",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Threat Generation",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class %",
								Type = "artifact",
								Value = 10
							}
						}
					}
				}
			});

			Sets.Add("Legendary Knight of the Shadows", new DDOItemSet
			{
				Name = "Legendary Knight of the Shadows",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Legendary_Knight_of_the_Shadows",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "artifact",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Magical Resistance Rating",
								Type = "artifact",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Generation",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Threat Generation",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class %",
								Type = "artifact",
								Value = 10
							}
						}
					}
				}
			});

			Sets.Add("Crypt Raider", new DDOItemSet
			{
				Name = "Crypt Raider",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Crypt_Raider",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Attack vs Evil",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage vs Evil",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Reduction",
								Type = "artifact",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Threat Reduction",
								Type = "artifact",
								Value = 20
							}
						}
					}
				}
			});

			Sets.Add("Legendary Crypt Raider", new DDOItemSet
			{
				Name = "Legendary Crypt Raider",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Legendary_Crypt_Raider",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Attack vs Evil",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage vs Evil",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "artifact",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Power",
								Type = "artifact",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Reduction",
								Type = "artifact",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Threat Reduction",
								Type = "artifact",
								Value = 20
							}
						}
					}
				}
			});

			Sets.Add("Silent Avenger", new DDOItemSet
			{
				Name = "Silent Avenger",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Silent_Avenger",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Doublestrike",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Doubleshot",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Dice",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fortification Bypass",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage vs Helpless %",
								Type = "artifact",
								Value = 5
							}
						}
					}
				}
			});

			Sets.Add("Legendary Silent Avenger", new DDOItemSet
			{
				Name = "Legendary Silent Avenger",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Legendary_Silent_Avenger",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Doublestrike",
								Type = "artifact",
								Value = 15
							},
							new DDOItemSetBonusProperty
							{
								Property = "Doubleshot",
								Type = "artifact",
								Value = 15
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Dice",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fortification Bypass",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage vs Helpless %",
								Type = "artifact",
								Value = 10
							}
						}
					}
				}
			});

			Sets.Add("Adherent of the Mists", new DDOItemSet
			{
				Name = "Adherent of the Mists",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Adherent_of_the_Mists",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 5,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "profane",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Healing Amplification",
								Type = "profane",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Healing Amplification",
								Type = "profane",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Healing Amplification",
								Type = "profane",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "profane",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Power",
								Type = "profane",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Universal Spell Power",
								Type = "profane",
								Value = 10
							}
						}
					}
				}
			});

			Sets.Add("Legendary Adherent of the Mists", new DDOItemSet
			{
				Name = "Legendary Adherent of the Mists",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Legendary_Adherent_of_the_Mists",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 5,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "profane",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Healing Amplification",
								Type = "profane",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Healing Amplification",
								Type = "profane",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Healing Amplification",
								Type = "profane",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "profane",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Power",
								Type = "profane",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Universal Spell Power",
								Type = "profane",
								Value = 20
							}
						}
					}
				}
			});

			Sets.Add("Pain and Suffering", new DDOItemSet
			{
				Name = "Pain and Suffering",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Pain_and_Suffering",
				SetBonuses = new List<DDOItemSetBonus>()
			});
			#endregion

			#region Disciples of Rage sets
			Sets.Add("Wayward Warrior", new DDOItemSet
			{
				Name = "Wayward Warrior",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Wayward_Warrior",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class",
								Type = "artifact natural armor",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Constitution",
								Type = "artifact",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Wayward Warrior (Legendary)", new DDOItemSet
			{
				Name = "Wayward Warrior (Legendary)",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Wayward_Warrior_.28Legendary.29",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class",
								Type = "artifact natural armor",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Constitution",
								Type = "artifact",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Seasons of Change", new DDOItemSet
			{
				Name = "Seasons of Change",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Seasons_of_Change",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Power",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Seasons of Change (Legendary)", new DDOItemSet
			{
				Name = "Seasons of Change (Legendary)",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Seasons_of_Change_.28Legendary.29",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 2
							}
						}
					}
				}
			});

			Sets.Add("Renegade Champion", new DDOItemSet
			{
				Name = "Renegade Champion",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Renegade_Champion",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Critical Multiplier on 19-20",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Rune Arm DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Healing Amplification",
								Type = "artifact",
								Value = 10
							}
						}
					}
				}
			});

			Sets.Add("Renegade Champion (Legendary)", new DDOItemSet
			{
				Name = "Renegade Champion (Legendary)",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Renegade_Champion_.28Legendary.29",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Critical Multiplier on 19-20",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Rune Arm DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Healing Amplification",
								Type = "artifact",
								Value = 20
							}
						}
					}
				}
			});

			Sets.Add("Heavy Warfare", new DDOItemSet
			{
				Name = "Heavy Warfare",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Heavy_Warfare",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Doublestrike",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Offhand Doublestrike",
								Type = "artifact",
								Value = 5
							}
						}
					}
				}
			});

			Sets.Add("Heavy Warfare (Legendary)", new DDOItemSet
			{
				Name = "Heavy Warfare (Legendary)",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Heavy_Warfare_.28Legendary.29",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "artifact",
								Value = 15
							},
							new DDOItemSetBonusProperty
							{
								Property = "Doublestrike",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Offhand Doublestrike",
								Type = "artifact",
								Value = 10
							}
						}
					}
				}
			});

			Sets.Add("Curse Necromancer", new DDOItemSet
			{
				Name = "Curse Necromancer",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Curse_Necromancer",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Negative Healing Amplification",
								Type = "artifact",
								Value = 5
							}
						}
					}
				}
			});

			Sets.Add("Curse Necromancer (Legendary)", new DDOItemSet
			{
				Name = "Curse Necromancer (Legendary)",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Curse_Necromancer_.28Legendary.29",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Negative Healing Amplification",
								Type = "artifact",
								Value = 15
							}
						}
					}
				}
			});
			#endregion

			#region Killing Time sets
			Sets.Add("Brilliant Crescents", new DDOItemSet
			{
				Name = "Brilliant Crescents",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Brilliant_Crescents",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Offhand Strike Chance",
								Value = 20
							}
						}
					}
				}
			});

			Sets.Add("Mountainskin Set", new DDOItemSet
			{
				Name = "Mountainskin Set",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Mountainskin_Set",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Power",
								Type = "exceptional",
								Value = 30
							},
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Critical Chance",
								Type = "exceptional",
								Value = 15
							}
						}
					}
				}
			});
			#endregion

			#region Masterminds of Sharn sets
			Sets.Add("Arcsteel Battlemage", new DDOItemSet
			{
				Name = "Arcsteel Battlemage",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Arcsteel_Battlemage",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Intelligence",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Legendary Arcsteel Battlemage", new DDOItemSet
			{
				Name = "Legendary Arcsteel Battlemage",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Legendary_Arcsteel_Battlemage",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Repair Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Intelligence",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 3
							}
						}
					}
				}
			});

			Sets.Add("Esoteric Initiate", new DDOItemSet
			{
				Name = "Esoteric Initiate",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Esoteric_Initiate",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Universal Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Intelligence",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Wisdom",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Charisma",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Magical Resistance Rating Cap",
								Value = 10
							}
						}
					}
				}
			});

			Sets.Add("Legendary Esoteric Initiate", new DDOItemSet
			{
				Name = "Legendary Esoteric Initiate",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Legendary_Esoteric_Initiate",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Universal Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Intelligence",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Wisdom",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Charisma",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Magical Resistance Rating Cap",
								Value = 20
							}
						}
					}
				}
			});

			Sets.Add("Flamecleansed Fury", new DDOItemSet
			{
				Name = "Flamecleansed Fury",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Flamecleansed_Fury",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Wisdom",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Charisma",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Legendary Flamecleansed Fury", new DDOItemSet
			{
				Name = "Legendary Flamecleansed Fury",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Legendary_Flamecleansed_Fury",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Wisdom",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Charisma",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 3
							}
						}
					}
				}
			});

			Sets.Add("Guardian of the Gates", new DDOItemSet
			{
				Name = "Guardian of the Gates",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Guardian_of_the_Gates",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class %",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Magical Resistance Rating",
								Type = "artifact",
								Value = 15
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Generation",
								Type = "artifact",
								Value = 75
							},
							new DDOItemSetBonusProperty
							{
								Property = "Acid Absorption",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Absorption",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Absorption",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Absorption",
								Type = "artifact",
								Value = 10
							}
						}
					}
				}
			});

			Sets.Add("Legendary Guardian of the Gates", new DDOItemSet
			{
				Name = "Legendary Guardian of the Gates",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Legendary_Guardian_of_the_Gates",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class %",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "artifact",
								Value = 15
							},
							new DDOItemSetBonusProperty
							{
								Property = "Magical Resistance Rating",
								Type = "artifact",
								Value = 30
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Threat Generation",
								Type = "artifact",
								Value = 75
							},
							new DDOItemSetBonusProperty
							{
								Property = "Acid Absorption",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Absorption",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Absorption",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Absorption",
								Type = "artifact",
								Value = 10
							}
						}
					}
				}
			});

			Sets.Add("Hruit's Influence", new DDOItemSet
			{
				Name = "Hruit's Influence",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Hruit.27s_Influence",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Wisdom",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Legendary Hruit's Influence", new DDOItemSet
			{
				Name = "Legendary Hruit's Influence",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Legendary_Hruit.27s_Influence",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Cold Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Electric Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fire Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Positive Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Wisdom",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 3
							}
						}
					}
				}
			});

			Sets.Add("Part of the Family", new DDOItemSet
			{
				Name = "Part of the Family",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Part_of_the_Family",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Doublestrike",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage vs Helpless %",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fortification Bypass",
								Type = "artifact",
								Value = 5
							}
						}
					}
				}
			});

			Sets.Add("Legendary Part of the Family", new DDOItemSet
			{
				Name = "Legendary Part of the Family",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Legendary_Part_of_the_Family",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Doublestrike",
								Type = "artifact",
								Value = 15
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Damage vs Helpless %",
								Type = "artifact",
								Value = 15
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fortification Bypass",
								Type = "artifact",
								Value = 10
							}
						}
					}
				}
			});

			Sets.Add("Wallwatch", new DDOItemSet
			{
				Name = "Wallwatch",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Wallwatch",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Dice",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fortification Bypass",
								Type = "artifact",
								Value = 15
							},
							new DDOItemSetBonusProperty
							{
								Property = "Doubleshot",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Power",
								Type = "artifact",
								Value = 10
							}
						}
					}
				}
			});

			Sets.Add("Legendary Wallwatch", new DDOItemSet
			{
				Name = "Legendary Wallwatch",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Legendary_Wallwatch",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Dice",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fortification Bypass",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Doubleshot",
								Type = "artifact",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Power",
								Type = "artifact",
								Value = 20
							}
						}
					}
				}
			});

			Sets.Add("Inevitable Balance", new DDOItemSet
			{
				Name = "Inevitable Balance",
				WikiURL = "",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Doublestrike",
								Value = 5
							}
						}
					}
				}
			});

			Sets.Add("Legendary Inevitable Balance", new DDOItemSet
			{
				Name = "Legendary Inevitable Balance",
				WikiURL = "",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Melee Power",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Doublestrike",
								Value = 10
							}
						}
					}
				}
			});
			#endregion

			#region Soul Splitter sets
			Sets.Add("Dreadkeeper", new DDOItemSet
			{
				Name = "Dreadkeeper",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Dreadkeeper",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Intelligence",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Legendary Dreadkeeper", new DDOItemSet
			{
				Name = "Legendary Dreadkeeper",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Legendary_Dreadkeeper",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Negative Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Intelligence",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Physical Resistance Rating",
								Type = "artifact",
								Value = 20
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 3
							}
						}
					}
				}
			});

			Sets.Add("Feywild Dreamer", new DDOItemSet
			{
				Name = "Feywild Dreamer",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Feywild_Dreamer",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sonic Spell Power",
								Type = "artifact",
								Value = 25
							},
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sonic Spell Critical Chance",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Charisma",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Legendary Feywild Dreamer", new DDOItemSet
			{
				Name = "Legendary Feywild Dreamer",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Feywild_Dreamer",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sonic Spell Power",
								Type = "artifact",
								Value = 50
							},
							new DDOItemSetBonusProperty
							{
								Property = "Acid Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Force Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Light Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sonic Spell Critical Chance",
								Type = "artifact",
								Value = 10
							},
							new DDOItemSetBonusProperty
							{
								Property = "Charisma",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "artifact",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "artifact",
								Value = 3
							}
						}
					}
				}
			});

			Sets.Add("Profane Experiment", new DDOItemSet
			{
				Name = "Profane Experiment",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Profane_Experiment",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Dice",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Doublestrike",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Doubleshot",
								Type = "artifact",
								Value = 5
							},
							new DDOItemSetBonusProperty
							{
								Property = "Constitution",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Intelligence",
								Type = "artifact",
								Value = 2
							},
							new DDOItemSetBonusProperty
							{
								Property = "Universal Spell Power",
								Type = "artifact",
								Value = 25
							}
						}
					}
				}
			});

			Sets.Add("Legendary Profane Experiment", new DDOItemSet
			{
				Name = "Legendary Profane Experiment",
				WikiURL = "https://ddowiki.com/page/Named_item_sets#Legendary_Profane_Experiment",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 3,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Dice",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Doublestrike",
								Type = "artifact",
								Value = 15
							},
							new DDOItemSetBonusProperty
							{
								Property = "Doubleshot",
								Type = "artifact",
								Value = 15
							},
							new DDOItemSetBonusProperty
							{
								Property = "Constitution",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Intelligence",
								Type = "artifact",
								Value = 4
							},
							new DDOItemSetBonusProperty
							{
								Property = "Universal Spell Power",
								Type = "artifact",
								Value = 50
							}
						}
					}
				}
			});
			#endregion

			// these aren't really sets, but the easiest way to model them is by defining them as sets
			// that are options for the items that can have them
			#region Thunder-Forged abilities
			Sets.Add("Shadow Caster", new DDOItemSet
			{
				Name = "Shadow Caster",
				WikiURL = "",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 1,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Concentration",
								Type = "profane",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Abjuration Spell DC",
								Type = "profane",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Conjuration Spell DC",
								Type = "profane",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Divination Spell DC",
								Type = "profane",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Enchantment Spell DC",
								Type = "profane",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Evocation Spell DC",
								Type = "profane",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Illusion Spell DC",
								Type = "profane",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Necromancy Spell DC",
								Type = "profane",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Transmutation Spell DC",
								Type = "profane",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Shadow Disciple", new DDOItemSet
			{
				Name = "Shadow Disciple",
				WikiURL = "",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 1,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Concentration",
								Type = "profane",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Trip DC",
								Type = "profane",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Sunder DC",
								Type = "profane",
								Value = 1
							},
							new DDOItemSetBonusProperty
							{
								Property = "Stunning DC",
								Type = "profane",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Shadow Killer", new DDOItemSet
			{
				Name = "Shadow Killer",
				WikiURL = "",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 1,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Sneak Attack Damage",
								Type = "profane",
								Value = 12 // this is really 2d6
							},
							new DDOItemSetBonusProperty
							{
								Property = "Reflex",
								Type = "profane",
								Value = 1
							}
						}
					}
				}
			});

			Sets.Add("Shadow Striker", new DDOItemSet
			{
				Name = "Shadow Striker",
				WikiURL = "",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 1,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Doublestrike",
								Type = "profane",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Melee Attack Speed",
								Type = "enhancement",
								Value = 15
							},
							new DDOItemSetBonusProperty
							{
								Property = "Ranged Attack Speed",
								Type = "enhancement",
								Value = 20
							}
						}
					}
				}
			});

			Sets.Add("Shadow Guardian", new DDOItemSet
			{
				Name = "Shadow Guardian",
				WikiURL = "",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 1,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Damage Reduction",
								Type = "epic",
								Value = 60
							}
						}
					}
				}
			});

			Sets.Add("Shadow Construct", new DDOItemSet
			{
				Name = "Shadow Construct",
				WikiURL = "",
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 1,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Repair Healing Amplification",
								Type = "profane",
								Value = 10
							}
						}
					}
				}
			});
			#endregion

			// there are only two items in the game that use this
			#region Mysterious Effects
			Sets.Add("Mysterious Effect Option 1", new DDOItemSet
			{
				Name = "Mysterious Effect Option 1",
				WikiURL = null,
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Dexterity",
								Type = "enhancement",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fortitude",
								Type = "resistance",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Reflex",
								Type = "resistance",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Will",
								Type = "resistance",
								Value = 3
							}
						}
					}
				}
			});

			Sets.Add("Mysterious Effect Option 2", new DDOItemSet
			{
				Name = "Mysterious Effect Option 2",
				WikiURL = null,
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Charisma",
								Type = "enhancement",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Fortification",
								Type = "enhancement",
								Value = 75
							}
						}
					}
				}
			});

			Sets.Add("Mysterious Effect Option 3", new DDOItemSet
			{
				Name = "Mysterious Effect Option 3",
				WikiURL = null,
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Strength",
								Type = "enhancement",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Armor Class",
								Type = "deflection",
								Value = 3
							}
						}
					}
				}
			});

			Sets.Add("Mysterious Effect Option 4", new DDOItemSet
			{
				Name = "Mysterious Effect Option 4",
				WikiURL = null,
				SetBonuses = new List<DDOItemSetBonus>
				{
					new DDOItemSetBonus
					{
						MinimumItems = 2,
						Bonuses = new List<DDOItemSetBonusProperty>
						{
							new DDOItemSetBonusProperty
							{
								Property = "Constitution",
								Type = "enhancement",
								Value = 3
							},
							new DDOItemSetBonusProperty
							{
								Property = "Spell Penetration",
								Type = "equipment",
								Value = 1
							}
						}
					}
				}
			});
			#endregion

			/*
				Sets.Add("", new DDOItemSet
				{
					Name = "",
					WikiURL = "",
					SetBonuses = new List<DDOItemSetBonus>
					{
						new DDOItemSetBonus
						{
							MinimumItems = 2,
							Bonuses = new List<DDOItemSetBonusProperty>
							{
								new DDOItemSetBonusProperty
								{
								}
							}
						}
					}
				});
			*/
		}

		public void AddItemProperty(string prop, string type, DDOItemData item)
		{
			DDOItemProperty ip;
			if (ItemProperties.ContainsKey(prop)) ip = ItemProperties[prop];
			else
			{
				ip = new DDOItemProperty { Property = prop };
				ItemProperties[prop] = ip;
			}

			if (!string.IsNullOrWhiteSpace(type) && !ip.Types.Contains(type)) ip.Types.Add(type);
			else if (string.IsNullOrWhiteSpace(type) && !ip.Types.Contains("")) ip.Types.Add("");

			if (item != null)
			{
				if (ip.Items.Find(i => i.Name == item.Name) == null) ip.Items.Add(item);

				// property hasn't seen this slot yet
				if ((ip.SlotsFoundOn & item.Slot) == 0)
				{
					// property has seen one slot
					if (Enum.IsDefined(typeof(SlotType), ip.SlotsFoundOn) && ip.SlotsFoundOn != SlotType.None)
					{
						if (SlotExclusiveItemProperties.ContainsKey(SlotType.None)) SlotExclusiveItemProperties[SlotType.None].Add(ip);
						else SlotExclusiveItemProperties[SlotType.None] = new List<DDOItemProperty> { ip };
					}

					if (SlotExclusiveItemProperties.ContainsKey(item.Slot)) SlotExclusiveItemProperties[item.Slot].Add(ip);
					else SlotExclusiveItemProperties[item.Slot] = new List<DDOItemProperty> { ip };

					ip.SlotsFoundOn |= item.Slot;
				}
			}
		}

		public string AddItem(DDOItemData item)
		{
			// add to the slot
			Slots[item.Slot].Items.Add(item);
			// go through all item properties, to include optional ones
			foreach (var ip in item.Properties)
			{
				if (ip.Options != null)
				{
					foreach (var o in ip.Options)
					{
						if (o.Type == "set")
						{
							try { Sets[o.Property].Items.Add(item); }
							catch { return "- " + item.Name + "referenced bad set " + o.Property; }
						}
						else AddItemProperty(o.Property, o.Type, item);
					}
				}
				else if (ip.Type == "set")
				{
					try { Sets[ip.Property].Items.Add(item); }
					catch { return "- " + item.Name + "referenced bad set " + ip.Property; }
				}
				else AddItemProperty(ip.Property, ip.Type, item);
			}

			Items.Add(item);

			return null;
		}
	}
}
