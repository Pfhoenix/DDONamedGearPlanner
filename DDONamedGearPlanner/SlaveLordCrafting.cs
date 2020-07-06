using System;
using System.Collections.Generic;
using System.Xml;

namespace DDONamedGearPlanner
{
	public static class SlaveLordCrafting
	{
		public const string BrokenShackles = "Broken Shackles";
		public const string LegendaryBrokenShackles = "Legendary Broken Shackles";
		public const string ChainLinks = "Chain Links";
		public const string LegendaryChainLinks = "Legendary Chain Links";
		public const string BrokenCollars = "Broken Collars";
		public const string LegendaryBrokenCollars = "Legendary Broken Collars";
		public const string FrayedLeathers = "Frayed Leathers";
		public const string LegendaryFrayedLeathers = "Legendary Frayed Leathers";
		public const string StaffSplinters = "Staff Splinters";
		public const string LegendaryStaffSplinters = "Legendary Staff Splinters";
		public const string ShatteredSymbols = "Shattered Symbols of the Slave Lords";
		public const string LegendaryShatteredSymbols = "Legendary Shattered Symbols of the Slave Lords";
		public const string StatuetteGods = "Statuette of the Gods";
		public const string LegendaryStatuetteGods = "Legendary Statuette of the Gods";
		public const string SlaveMastersBust = "Slave Master's Bust";
		public const string LegendarySlaveMastersBust = "Legendary Slave Master's Bust";

		public enum ESlaveLordItemSlots { Prefix, Suffix, Extra, Bonus, Augment, Set, Mythic }

		public static Dictionary<string, List<CraftedItemProperty>> ItemSlots = new Dictionary<string, List<CraftedItemProperty>>
		{
			{
				"Heroic Prefix",
				new List<CraftedItemProperty>
				{
					new CraftedItemProperty
					{
						Name = "- empty -"
					},
					new CraftedItemProperty
					{
						Name = "Strength +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Strength", Type = "enhancement", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenShackles, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Dexterity +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Dexterity", Type = "enhancement", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenShackles, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Constitution +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Constitution", Type = "enhancement", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenShackles, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Intelligence +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Intelligence", Type = "enhancement", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenShackles, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Wisdom +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Wisdom", Type = "enhancement", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenShackles, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Charisma +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Charisma", Type = "enhancement", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenShackles, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "False Life +18",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Hit Points", Type = "enhancement", Value = 18 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenShackles, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Fortification +70%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortification", Type = "enhancement", Value = 70 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenShackles, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Sheltering +12",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Physical Resistance Rating", Type = "enhancement", Value = 12 },
							new ItemProperty { Property = "Magical Resistance Rating", Type = "enhancement", Value = 12 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenShackles, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Wizardy +96",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Spell Points", Type = "enhancement", Value = 96 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenShackles, Amount = 50 }
						}
					},
				}
			},
			{
				"Legendary Prefix",
				new List<CraftedItemProperty>
				{
					new CraftedItemProperty
					{
						Name = "- empty -"
					},
					new CraftedItemProperty
					{
						Name = "Strength +17",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Strength", Type = "enhancement", Value = 17 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Dexterity +17",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Dexterity", Type = "enhancement", Value = 17 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Constitution +17",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Constitution", Type = "enhancement", Value = 17 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Intelligence +17",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Intelligence", Type = "enhancement", Value = 17 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Wisdom +17",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Wisdom", Type = "enhancement", Value = 17 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Charisma +17",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Charisma", Type = "enhancement", Value = 17 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "False Life +68",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Hit Points", Type = "enhancement", Value = 68 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Fortification +185%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortification", Type = "enhancement", Value = 185 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Sheltering +45",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Physical Resistance Rating", Type = "enhancement", Value = 45 },
							new ItemProperty { Property = "Magical Resistance Rating", Type = "enhancement", Value = 45 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Wizardy +371",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Spell Points", Type = "enhancement", Value = 371 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 100 }
						}
					},
				}
			},
			{
				"Heroic Suffix",
				new List<CraftedItemProperty>
				{
					new CraftedItemProperty
					{
						Name = "- empty -"
					},
					new CraftedItemProperty
					{
						Name = "Accuracy +8",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Attack", Type = "enhancement", Value = 8 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Armor Piercing +8",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortification Bypass", Type = "enhancement", Value = 8 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Damage Guards +2d8",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Damage Guards", Type = "", Value = 0 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Deadly +4",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Damage", Type = "enhancement", Value = 4 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Deception +3",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Sneak Attack Attack", Type = "enhancement", Value = 3 },
							new ItemProperty { Property = "Sneak Attack Damage", Type = "enhancement", Value = 4 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Resistance +4",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortitude", Type = "resistance", Value = 4 },
							new ItemProperty { Property = "Reflex", Type = "resistance", Value = 4 },
							new ItemProperty { Property = "Will", Type = "resistance", Value = 4 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Seeker +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Confirm Critical Hits", Type = "enhancement", Value = 5 },
							new ItemProperty { Property = "Critical Strike Damage", Type = "enhancement", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Acid Spell Lore +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Acid Spell Critical Chance", Type = "equipment", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Cold Spell Lore +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Cold Spell Critical Chance", Type = "equipment", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Electric Spell Lore +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Electric Spell Critical Chance", Type = "equipment", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Fire Spell Lore +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fire Spell Critical Chance", Type = "equipment", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Force Spell Lore +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Force Spell Critical Chance", Type = "equipment", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Light Spell Lore +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Light Spell Critical Chance", Type = "equipment", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Negative Spell Lore +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Negative Spell Critical Chance", Type = "equipment", Value = 10 },
							new ItemProperty { Property = "Poison Spell Critical Chance", Type = "equipment", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Positive Spell Lore +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Positive Spell Critical Chance", Type = "equipment", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Repair Spell Lore +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Repair Spell Critical Chance", Type = "equipment", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Sonic Spell Lore +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Sonic Spell Critical Chance", Type = "equipment", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Acid Spell Power +70",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Acid Spell Power", Type = "equipment", Value = 70 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Cold Spell Power +70",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Cold Spell Power", Type = "equipment", Value = 70 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Electric Spell Power +70",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Electric Spell Power", Type = "equipment", Value = 70 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Fire Spell Power +70",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fire Spell Power", Type = "equipment", Value = 70 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Force Spell Power +70",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Force Spell Power", Type = "equipment", Value = 70 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Light Spell Power +70",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Light Spell Power", Type = "equipment", Value = 70 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Negative Spell Power +70",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Negative Spell Power", Type = "equipment", Value = 70 },
							new ItemProperty { Property = "Poison Spell Power", Type = "equipment", Value = 70 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Positive Spell Power +70",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Positive Spell Power", Type = "equipment", Value = 70 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Repair Spell Power +70",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Repair Spell Power", Type = "equipment", Value = 70 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Sonic Spell Power +70",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Sonic Spell Power", Type = "equipment", Value = 70 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ChainLinks, Amount = 50 }
						}
					},
				}
			},
			{
				"Legendary Suffix",
				new List<CraftedItemProperty>
				{
					new CraftedItemProperty
					{
						Name = "- empty -"
					},
					new CraftedItemProperty
					{
						Name = "Accuracy +28",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Attack", Type = "enhancement", Value = 28 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Armor Piercing +28",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortification Bypass", Type = "enhancement", Value = 28 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Damage Guards +8d8",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Damage Guards", Type = "", Value = 0 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Deadly +14",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Damage", Type = "enhancement", Value = 14 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Deception +14",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Sneak Attack Attack", Type = "enhancement", Value = 14 },
							new ItemProperty { Property = "Sneak Attack Damage", Type = "enhancement", Value = 21 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Resistance +14",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortitude", Type = "resistance", Value = 14 },
							new ItemProperty { Property = "Reflex", Type = "resistance", Value = 14 },
							new ItemProperty { Property = "Will", Type = "resistance", Value = 14 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Seeker +17",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Confirm Critical Hits", Type = "enhancement", Value = 17 },
							new ItemProperty { Property = "Critical Strike Damage", Type = "enhancement", Value = 17 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Acid Spell Lore +27",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Acid Spell Critical Chance", Type = "equipment", Value = 27 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Cold Spell Lore +27",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Cold Spell Critical Chance", Type = "equipment", Value = 27 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Electric Spell Lore +27",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Electric Spell Critical Chance", Type = "equipment", Value = 27 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Fire Spell Lore +27",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fire Spell Critical Chance", Type = "equipment", Value = 27 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Force Spell Lore +27",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Force Spell Critical Chance", Type = "equipment", Value = 27 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Light Spell Lore +27",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Light Spell Critical Chance", Type = "equipment", Value = 27 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Negative Spell Lore +27",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Negative Spell Critical Chance", Type = "equipment", Value = 27 },
							new ItemProperty { Property = "Poison Spell Critical Chance", Type = "equipment", Value = 27 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Positive Spell Lore +27",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Positive Spell Critical Chance", Type = "equipment", Value = 27 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Repair Spell Lore +27",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Repair Spell Critical Chance", Type = "equipment", Value = 27 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Sonic Spell Lore +27",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Sonic Spell Critical Chance", Type = "equipment", Value = 27 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Acid Spell Power +185",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Acid Spell Power", Type = "equipment", Value = 185 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Cold Spell Power +185",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Cold Spell Power", Type = "equipment", Value = 185 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Electric Spell Power +185",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Electric Spell Power", Type = "equipment", Value = 185 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Fire Spell Power +185",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fire Spell Power", Type = "equipment", Value = 185 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Force Spell Power +185",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Force Spell Power", Type = "equipment", Value = 185 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Light Spell Power +185",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Light Spell Power", Type = "equipment", Value = 185 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Negative Spell Power +185",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Negative Spell Power", Type = "equipment", Value = 185 },
							new ItemProperty { Property = "Poison Spell Power", Type = "equipment", Value = 185 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Positive Spell Power +185",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Positive Spell Power", Type = "equipment", Value = 185 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Repair Spell Power +185",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Repair Spell Power", Type = "equipment", Value = 185 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Sonic Spell Power +185",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Sonic Spell Power", Type = "equipment", Value = 185 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 100 }
						}
					},
				}
			},
			{
				"Heroic Extra",
				new List<CraftedItemProperty>
				{
					new CraftedItemProperty
					{
						Name = "- empty -"
					},
					new CraftedItemProperty
					{
						Name = "Balance +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Balance", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Bluff +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Bluff", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Concentration +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Concentration", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Diplomacy +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Diplomacy", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Disable Device +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Disable Device", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Haggle +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Haggle", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Heal +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Heal", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Hide +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Hide", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Intimidate +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Intimidate", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Jump +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Jump", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Listen +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Listen", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Move Silently +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Move Silently", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Open Lock +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Open Lock", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Perform +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Perform", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Repair +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Repair", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Search +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Search", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Spellcraft +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Spellcraft", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Spot +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Spot", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Swim +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Swim", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Tumble +10",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Tumble", Type = "enhancement", Value = 10 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Use Magic Device +1",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Use Magic Device", Type = "enhancement", Value = 1 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Shatter +6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Sunder DC", Type = "enhancement", Value = 6 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Spell Focus Mastery +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Abjuration Spell DC", Type = "equipment", Value = 2 },
							new ItemProperty { Property = "Conjuration Spell DC", Type = "equipment", Value = 2 },
							new ItemProperty { Property = "Divination Spell DC", Type = "equipment", Value = 2 },
							new ItemProperty { Property = "Enchantment Spell DC", Type = "equipment", Value = 2 },
							new ItemProperty { Property = "Evocation Spell DC", Type = "equipment", Value = 2 },
							new ItemProperty { Property = "Illusion Spell DC", Type = "equipment", Value = 2 },
							new ItemProperty { Property = "Necromancy Spell DC", Type = "equipment", Value = 2 },
							new ItemProperty { Property = "Transmutation Spell DC", Type = "equipment", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Spell Penetration +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Spell Penetration", Type = "enhancement", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Stunning +6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Stunning DC", Type = "enhancement", Value = 6 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Tendon Slice +4",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Tendon Slice", Type = "enhancement", Value = 4 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Vertigo +6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Trip DC", Type = "enhancement", Value = 6 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					},
				}
			},
			{
				"Legendary Extra",
				new List<CraftedItemProperty>
				{
					new CraftedItemProperty
					{
						Name = "- empty -"
					},
					new CraftedItemProperty
					{
						Name = "Balance +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Balance", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Bluff +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Bluff", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Concentration +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Concentration", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Diplomacy +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Diplomacy", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Disable Device +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Disable Device", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Haggle +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Haggle", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Heal +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Heal", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Hide +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Hide", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Intimidate +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Intimidate", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Jump +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Jump", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Listen +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Listen", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Move Silently +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Move Silently", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Open Lock +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Open Lock", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Perform +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Perform", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Repair +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Repair", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Search +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Search", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Spellcraft +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Spellcraft", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Spot +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Spot", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Swim +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Swim", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Tumble +22",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Tumble", Type = "enhancement", Value = 22 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Use Magic Device +7",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Use Magic Device", Type = "enhancement", Value = 7 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Shatter +20",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Sunder DC", Type = "enhancement", Value = 20 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Spell Focus Mastery +6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Abjuration Spell DC", Type = "equipment", Value = 6 },
							new ItemProperty { Property = "Conjuration Spell DC", Type = "equipment", Value = 6 },
							new ItemProperty { Property = "Divination Spell DC", Type = "equipment", Value = 6 },
							new ItemProperty { Property = "Enchantment Spell DC", Type = "equipment", Value = 6 },
							new ItemProperty { Property = "Evocation Spell DC", Type = "equipment", Value = 6 },
							new ItemProperty { Property = "Illusion Spell DC", Type = "equipment", Value = 6 },
							new ItemProperty { Property = "Necromancy Spell DC", Type = "equipment", Value = 6 },
							new ItemProperty { Property = "Transmutation Spell DC", Type = "equipment", Value = 6 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Spell Penetration +7",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Spell Penetration", Type = "enhancement", Value = 7 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Stunning +20",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Stunning DC", Type = "enhancement", Value = 20 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Tendon Slice +14",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Tendon Slice", Type = "enhancement", Value = 14 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Vertigo +20",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Trip DC", Type = "enhancement", Value = 20 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 100 }
						}
					},
				}
			},
			{
				"Heroic Bonus",
				new List<CraftedItemProperty>
				{
					new CraftedItemProperty
					{
						Name = "- empty -"
					},
					new CraftedItemProperty
					{
						Name = "Quality Strength +1",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Strength", Type = "quality", Value = 1 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Dexterity +1",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Dexterity", Type = "quality", Value = 1 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Constitution +1",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Constitution", Type = "quality", Value = 1 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Intelligence +1",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Intelligence", Type = "quality", Value = 1 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Wisdom +1",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Wisdom", Type = "quality", Value = 1 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Charisma +1",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Charisma", Type = "quality", Value = 1 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality False Life +4",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Hit Points", Type = "quality", Value = 4 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Fortification +17%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortification", Type = "quality", Value = 17 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality MRR +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Magical Resistance Rating", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality PRR +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Physical Resistance Rating", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Balance +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Balance", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Bluff +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Bluff", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Concentration +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Concentration", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Diplomacy +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Diplomacy", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Disable Device +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Disable Device", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Haggle +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Haggle", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Heal +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Heal", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Hide +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Hide", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Intimidate +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Intimidate", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Jump +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Jump", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Listen +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Listen", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Move Silently +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Move Silently", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Open Lock +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Open Lock", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Perform +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Perform", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Repair +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Repair", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Search +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Search", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Spellcraft +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Spellcraft", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Spot +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Spot", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Swim +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Swim", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Tumble +2",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Tumble", Type = "quality", Value = 2 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = FrayedLeathers, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					}
				}
			},
			{
				"Legendary Bonus",
				new List<CraftedItemProperty>
				{
					new CraftedItemProperty
					{
						Name = "- empty -"
					},
					new CraftedItemProperty
					{
						Name = "Quality Strength +4",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Strength", Type = "quality", Value = 4 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Dexterity +4",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Dexterity", Type = "quality", Value = 4 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Constitution +4",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Constitution", Type = "quality", Value = 4 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Intelligence +4",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Intelligence", Type = "quality", Value = 4 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Wisdom +4",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Wisdom", Type = "quality", Value = 4 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Charisma +4",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Charisma", Type = "quality", Value = 4 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality False Life +16",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Hit Points", Type = "quality", Value = 16 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Fortification +45%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortification", Type = "quality", Value = 45 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality MRR +11",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Magical Resistance Rating", Type = "quality", Value = 11 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality PRR +11",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Physical Resistance Rating", Type = "quality", Value = 11 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Balance +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Balance", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Bluff +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Bluff", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Concentration +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Concentration", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Diplomacy +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Diplomacy", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Disable Device +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Disable Device", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Haggle +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Haggle", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Heal +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Heal", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Hide +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Hide", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Intimidate +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Intimidate", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Jump +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Jump", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Listen +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Listen", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Move Silently +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Move Silently", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Open Lock +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Open Lock", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Perform +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Perform", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Repair +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Repair", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Search +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Search", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Spellcraft +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Spellcraft", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Spot +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Spot", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Swim +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Swim", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Quality Tumble +5",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Tumble", Type = "quality", Value = 5 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryFrayedLeathers, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					}
				}
			},
			{
				"Heroic Augment",
				new List<CraftedItemProperty>
				{
					new CraftedItemProperty
					{
						Name = "- empty -"
					},
					new CraftedItemProperty
					{
						Name = "Colorless Augment Slot",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Augment Slot", Type = "colorless", Value = 0 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = StaffSplinters, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Blue Augment Slot",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Augment Slot", Type = "blue", Value = 0 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = StaffSplinters, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Yellow Augment Slot",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Augment Slot", Type = "yellow", Value = 0 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = StaffSplinters, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Green Augment Slot",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Augment Slot", Type = "green", Value = 0 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = StaffSplinters, Amount = 30 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					}
				}
			},
			{
				"Legendary Augment",
				new List<CraftedItemProperty>
				{
					new CraftedItemProperty
					{
						Name = "- empty -"
					},
					new CraftedItemProperty
					{
						Name = "Colorless Augment Slot",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Augment Slot", Type = "colorless", Value = 0 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryStaffSplinters, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Blue Augment Slot",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Augment Slot", Type = "blue", Value = 0 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryStaffSplinters, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Yellow Augment Slot",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Augment Slot", Type = "yellow", Value = 0 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryStaffSplinters, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Green Augment Slot",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Augment Slot", Type = "green", Value = 0 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = StaffSplinters, Amount = 100 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = ChainLinks, Amount = 50 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 50 }
						}
					}
				}
			},
			{
				"Heroic Set",
				new List<CraftedItemProperty>
				{
					new CraftedItemProperty
					{
						Name = "- empty -"
					},
					new CraftedItemProperty
					{
						Name = "Slave Lord's Might",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Slave Lord's Might", Type = "set" }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ShatteredSymbols, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Slave Lord's Sorcery",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Slave Lord's Sorcery", Type = "set" }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ShatteredSymbols, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Slave's Endurance",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Slave's Endurance", Type = "set" }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = ShatteredSymbols, Amount = 20 },
							new CraftingIngredient { Name = BrokenShackles, Amount = 20 },
							new CraftingIngredient { Name = ChainLinks, Amount = 20 },
							new CraftingIngredient { Name = BrokenCollars, Amount = 20 }
						}
					}
				}
			},
			{
				"Legendary Set",
				new List<CraftedItemProperty>
				{
					new CraftedItemProperty
					{
						Name = "- empty -"
					},
					new CraftedItemProperty
					{
						Name = "Legendary Slave Lord's Might",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Legendary Slave Lord's Might", Type = "set" }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryShatteredSymbols, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Legendary Slave Lord's Sorcery",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Legendary Slave Lord's Sorcery", Type = "set" }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryShatteredSymbols, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Legendary Slave's Endurance",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Legendary Slave's Endurance", Type = "set" }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryShatteredSymbols, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenShackles, Amount = 50 },
							new CraftingIngredient { Name = LegendaryChainLinks, Amount = 50 },
							new CraftingIngredient { Name = LegendaryBrokenCollars, Amount = 50 }
						}
					}
				}
			},
			{
				"Heroic Mythic",
				new List<CraftedItemProperty>
				{
					new CraftedItemProperty
					{
						Name = "- empty -"
					},
					new CraftedItemProperty
					{
						Name = "Mythic Boost +1",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Mythic Boost", Type = "", Value = 1 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = StatuetteGods, Amount = 1 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Mythic Boost +3",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Mythic Boost", Type = "", Value = 3 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = SlaveMastersBust, Amount = 1 }
						}
					}
				}
			},
			{
				"Legendary Mythic",
				new List<CraftedItemProperty>
				{
					new CraftedItemProperty
					{
						Name = "- empty -"
					},
					new CraftedItemProperty
					{
						Name = "Mythic Boost +1",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Mythic Boost", Type = "", Value = 1 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendaryStatuetteGods, Amount = 1 }
						}
					},
					new CraftedItemProperty
					{
						Name = "Mythic Boost +3",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Mythic Boost", Type = "", Value = 3 }
						},
						Cost = new List<CraftingIngredient>
						{
							new CraftingIngredient { Name = LegendarySlaveMastersBust, Amount = 1 }
						}
					}
				}
			}
		};

		public class SlaveLordItemContainer : ACustomItemContainer
		{
			public DDOItemData BaseItem;
			DDOItemData GeneratedItem;
			public CraftedItemProperty[] Slots { get; set; }

			public static List<SlotType> DisallowedSlots = new List<SlotType> { SlotType.Back, SlotType.Body, SlotType.Eye, SlotType.Hand, SlotType.Head, SlotType.Offhand, SlotType.Weapon };

			public SlaveLordItemContainer()
			{
				Source = ItemDataSource.SlaveLord;
				WikiURL = "https://ddowiki.com/page/Slave_Lords_Crafting";
				Slots = new CraftedItemProperty[7];
				for (int i = 0; i < 7; i++)
					Slots[i] = ItemSlots["Heroic " + ((ESlaveLordItemSlots)i).ToString()][0];
			}

			public override List<SlotType> GetDisallowedSlots()
			{
				return DisallowedSlots;
			}

			void GenerateItem()
			{
				if (GeneratedItem == null)
				{
					GeneratedItem = new DDOItemData(ItemDataSource.SlaveLord, false)
					{
						Name = Name,
						WikiURL = "https://ddowiki.com/page/Slave_Lords_Crafting",
						Slot = BaseItem.Slot,
						Category = BaseItem.Category,
						QuestFoundIn = BaseItem.QuestFoundIn
					};
				}
				else GeneratedItem.Properties.Clear();

				// we skip the last slot, mythic, as it's not used by the planner for gear
				for (int i = 0; i < 6; i++)
				{
					if (Slots[i]?.AppliedProperties == null) continue;
					foreach (var ip in Slots[i].AppliedProperties)
						GeneratedItem.AddProperty(ip.Property, ip.Type, ip.Value, null);
				}
			}

			public override DDOItemData GetItem()
			{
				GenerateItem();

				return GeneratedItem;
			}

			public override void ToXml(XmlElement xci, XmlDocument doc)
			{
				XmlElement xe = doc.CreateElement("BaseItem");
				xe.InnerText = BaseItem.Name + "|" + BaseItem.Slot;
				xci.AppendChild(xe);
				var slots = (ESlaveLordItemSlots[])Enum.GetValues(typeof(ESlaveLordItemSlots));
				foreach (var slot in slots)
				{
					xe = doc.CreateElement(slot.ToString());
					xe.InnerText = Slots[(int)slot]?.Name;
					xci.AppendChild(xe);
				}
			}

			public override bool FromXml(XmlElement xci)
			{
				try
				{
					string[] baseitem = xci.GetElementsByTagName("BaseItem")[0].InnerText.Split('|');
					SlotType slot = (SlotType)Enum.Parse(typeof(SlotType), baseitem[1]);
					BaseItem = DatasetManager.Dataset.Items.Find(i => i.Name == baseitem[0] && i.Slot == slot);
					string ml = BaseItem.Name.StartsWith("Legendary") ? "Legendary " : "Heroic ";
					var slots = (ESlaveLordItemSlots[])Enum.GetValues(typeof(ESlaveLordItemSlots));
					foreach (var s in slots)
					{
						List<CraftedItemProperty> props = ItemSlots[ml + s.ToString()];
						string p = xci.GetElementsByTagName(s.ToString())[0].InnerText;
						Slots[(int)s] = props.Find(i => i.Name == p);
					}

					return true;
				}
				catch
				{
					return false;
				}
			}
		}
	}
}
