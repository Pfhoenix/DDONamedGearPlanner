using System;
using System.Collections.Generic;
using System.Linq;

namespace DDONamedGearPlanner
{
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
		Weapon = 32
	}

	[Flags]
	public enum WeaponCategory
	{
		Simple = 1,
		Martial = 2,
		Exotic = 4,
		Throwing = 8
	}

	public class ItemProperty
	{
		public string Property;
		public string Type;
		public float Value;
		public List<ItemProperty> Options;
	}

	public class DDOItemData
	{
		public string Name;
		public string WikiURL;
		public SlotType Slot;
		public int Category;
		public List<ItemProperty> Properties = new List<ItemProperty>();

		public ItemProperty AddProperty(string prop, string type, float value, List<ItemProperty> options)
		{
			ItemProperty ip = new ItemProperty { Property = prop, Type = type, Value = value, Options = options };
			Properties.Add(ip);
			return ip;
		}
	}

	public class DDOSlot
	{
		public SlotType Slot;
		public List<DDOItemData> Items = new List<DDOItemData>();
		public Type CategoryEnumType;
	}

	public class DDOItemProperty
	{
		public string Property;
		public string Type;
		public List<DDOItemData> Items = new List<DDOItemData>();
	}

	public class DDOItemSetBonusProperty
	{
		public string Property;
		public string Type;
		public float Value;
	}

	public class DDOItemSetBonus
	{
		public int MinimumItems;
		public List<DDOItemSetBonusProperty> Bonuses;
	}

	public class DDOItemSet
	{
		public string Name;
		public string WikiURL;
		public List<DDOItemSetBonus> SetBonuses;
		public List<DDOItemData> Items = new List<DDOItemData>();
	}

	public class DDODataset
	{
		public Dictionary<SlotType, DDOSlot> Slots = new Dictionary<SlotType, DDOSlot>();
		public List<DDOItemProperty> ItemProperties = new List<DDOItemProperty>();
		public List<DDOItemData> Items = new List<DDOItemData>();
		public Dictionary<string, DDOItemSet> Sets = new Dictionary<string, DDOItemSet>();

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
		}
	}
}
