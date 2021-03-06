﻿using System;
using System.Collections.Generic;
using System.Xml;

namespace DDONamedGearPlanner
{
	public static class LGSCrafting
	{
        public const int Tiers = 3;
		public const string CoV = "Commendation of Valor";
		public const string SmallGlowingArrowhead = "Legendary Small Glowing Arrowhead";
		public const string SmallGnawedBone = "Legendary Small Gnawed Bone";
		public const string SmallTwistedShrapnel = "Legendary Small Twisted Shrapnel";
		public const string SmallInfernalChain = "Legendary Small Length of Infernal Chain";
		public const string SmallSulfurousStone = "Legendary Small Sulfurous Stone";
		public const string SmallDevilScales = "Legendary Small Devil Scales";
		public const string MediumGlowingArrowhead = "Legendary Medium Glowing Arrowhead";
		public const string MediumGnawedBone = "Legendary Medium Gnawed Bone";
		public const string MediumTwistedShrapnel = "Legendary Medium Twisted Shrapnel";
		public const string MediumInfernalChain = "Legendary Medium Length of Infernal Chain";
		public const string MediumSulfurousStone = "Legendary Medium Sulfurous Stone";
		public const string MediumDevilScales = "Legendary Medium Devil Scales";
		public const string LargeGlowingArrowhead = "Legendary Large Glowing Arrowhead";
		public const string LargeGnawedBone = "Legendary Large Gnawed Bone";
		public const string LargeTwistedShrapnel = "Legendary Large Twisted Shrapnel";
		public const string LargeInfernalChain = "Legendary Large Length of Infernal Chain";
		public const string LargeSulfurousStone = "Legendary Large Sulfurous Stone";
		public const string LargeDevilScales = "Legendary Large Devil Scales";
		public const string InferiorFocusAir = "Legendary Inferior Focus of Air";
		public const string InferiorFocusEarth = "Legendary Inferior Focus of Earth";
		public const string InferiorFocusFire = "Legendary Inferior Focus of Fire";
		public const string InferiorFocusWater = "Legendary Inferior Focus of Water";
		public const string InferiorFocusNegative = "Legendary Inferior Focus of Negative Energy";
		public const string InferiorFocusPositive = "Legendary Inferior Focus of Positive Energy";
		public const string DilutedEtherealEssence = "Legendary Diluted Ethereal Essence";
		public const string DilutedMaterialEssence = "Legendary Diluted Material Essence";
		public const string CloudyGemDominion = "Legendary Cloudy Gem of Dominion";
		public const string CloudyGemEscalation = "Legendary Cloudy Gem of Escalation";
		public const string CloudyGemOpposition = "Legendary Cloudy Gem of Opposition";
		public const string FocusAir = "Legendary Focus of Air";
		public const string FocusEarth = "Legendary Focus of Earth";
		public const string FocusFire = "Legendary Focus of Fire";
		public const string FocusWater = "Legendary Focus of Water";
		public const string FocusNegative = "Legendary Focus of Negative Energy";
		public const string FocusPositive = "Legendary Focus of Positive Energy";
		public const string EtherealEssence = "Legendary Ethereal Essence";
		public const string MaterialEssence = "Legendary Material Essence";
		public const string GemDominion = "Legendary Gem of Dominion";
		public const string GemEscalation = "Legendary Gem of Escalation";
		public const string GemOpposition = "Legendary Gem of Opposition";
		public const string SuperiorFocusAir = "Legendary Superior Focus of Air";
		public const string SuperiorFocusEarth = "Legendary Superior Focus of Earth";
		public const string SuperiorFocusFire = "Legendary Superior Focus of Fire";
		public const string SuperiorFocusWater = "Legendary Superior Focus of Water";
		public const string SuperiorFocusNegative = "Legendary Superior Focus of Negative Energy";
		public const string SuperiorFocusPositive = "Legendary Superior Focus of Positive Energy";
		public const string PureEtherealEssence = "Legendary Pure Ethereal Essence";
		public const string PureMaterialEssence = "Legendary Pure Material Essence";
		public const string FlawlessGemDominion = "Legendary Flawless Gem of Dominion";
		public const string FlawlessGemEscalation = "Legendary Flawless Gem of Escalation";
		public const string FlawlessGemOpposition = "Legendary Flawless Gem of Opposition";

		public class LGSCraftingIngredient
		{
			public string Name;
			public int Count = 1;
			public List<CraftingIngredient> Ingredients;
		}

		public static Dictionary<string, LGSCraftingIngredient> LGSCraftables = new Dictionary<string, LGSCraftingIngredient>
		{
			#region Tier 1 Craftables
			{
				InferiorFocusAir, new LGSCraftingIngredient
				{
					Name = InferiorFocusAir,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = SmallGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = SmallGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = SmallTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = SmallSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 25 }
					}
				}
			},
			{
				InferiorFocusEarth, new LGSCraftingIngredient
				{
					Name = InferiorFocusEarth,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = SmallGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = SmallGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = SmallTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = SmallInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 25 }
					}
				}
			},
			{
				InferiorFocusFire, new LGSCraftingIngredient
				{
					Name = InferiorFocusFire,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = SmallGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = SmallGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = SmallTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = SmallDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 25 }
					}
				}
			},
			{
				InferiorFocusWater, new LGSCraftingIngredient
				{
					Name = InferiorFocusWater,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = SmallGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = SmallGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = SmallInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = SmallDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 25 }
					}
				}
			},
			{
				InferiorFocusNegative, new LGSCraftingIngredient
				{
					Name = InferiorFocusNegative,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = SmallGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = SmallTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = SmallInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = SmallSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 25 }
					}
				}
			},
			{
				InferiorFocusPositive, new LGSCraftingIngredient
				{
					Name = InferiorFocusPositive,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = SmallGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = SmallGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = SmallSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = SmallDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 25 }
					}
				}
			},
			{
				DilutedEtherealEssence, new LGSCraftingIngredient
				{
					Name = DilutedEtherealEssence,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = SmallGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = SmallInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = SmallSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = SmallDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 25 }
					}
				}
			},
			{
				DilutedMaterialEssence, new LGSCraftingIngredient
				{
					Name = DilutedMaterialEssence,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = SmallGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = SmallTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = SmallSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = SmallDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 25 }
					}
				}
			},
			{
				CloudyGemDominion, new LGSCraftingIngredient
				{
					Name = CloudyGemDominion,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = SmallGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = SmallTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = SmallInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = SmallDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 25 }
					}
				}
			},
			{
				CloudyGemEscalation, new LGSCraftingIngredient
				{
					Name = CloudyGemEscalation,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = SmallTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = SmallInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = SmallSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = SmallDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 25 }
					}
				}
			},
			{
				CloudyGemOpposition, new LGSCraftingIngredient
				{
					Name = CloudyGemOpposition,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = SmallGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = SmallInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = SmallSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = SmallDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 25 }
					}
				}
			},
			#endregion
			#region Tier 2 Craftables
			{
				FocusAir, new LGSCraftingIngredient
				{
					Name = FocusAir,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = MediumGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = MediumGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = MediumTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = MediumSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 50 }
					}
				}
			},
			{
				FocusEarth, new LGSCraftingIngredient
				{
					Name = FocusEarth,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = MediumGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = MediumGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = MediumTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = MediumInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 50 }
					}
				}
			},
			{
				FocusFire, new LGSCraftingIngredient
				{
					Name = FocusFire,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = MediumGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = MediumGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = MediumTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = MediumDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 50 }
					}
				}
			},
			{
				FocusWater, new LGSCraftingIngredient
				{
					Name = FocusWater,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = MediumGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = MediumGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = MediumInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = MediumDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 50 }
					}
				}
			},
			{
				FocusNegative, new LGSCraftingIngredient
				{
					Name = FocusNegative,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = MediumGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = MediumTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = MediumInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = MediumSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 50 }
					}
				}
			},
			{
				FocusPositive, new LGSCraftingIngredient
				{
					Name = FocusPositive,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = MediumGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = MediumGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = MediumSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = MediumDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 50 }
					}
				}
			},
			{
				EtherealEssence, new LGSCraftingIngredient
				{
					Name = EtherealEssence,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = MediumGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = MediumInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = MediumSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = MediumDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 50 }
					}
				}
			},
			{
				MaterialEssence, new LGSCraftingIngredient
				{
					Name = MaterialEssence,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = MediumGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = MediumTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = MediumSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = MediumDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 50 }
					}
				}
			},
			{
				GemDominion, new LGSCraftingIngredient
				{
					Name = GemDominion,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = MediumGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = MediumTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = MediumInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = MediumDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 50 }
					}
				}
			},
			{
				GemEscalation, new LGSCraftingIngredient
				{
					Name = GemEscalation,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = MediumTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = MediumInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = MediumSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = MediumDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 50 }
					}
				}
			},
			{
				GemOpposition, new LGSCraftingIngredient
				{
					Name = GemOpposition,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = MediumGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = MediumInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = MediumSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = MediumDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 50 }
					}
				}
			},
			#endregion
			#region Tier 3 Craftables
			{
				SuperiorFocusAir, new LGSCraftingIngredient
				{
					Name = SuperiorFocusAir,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = LargeGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = LargeGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = LargeTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = LargeSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 100 }
					}
				}
			},
			{
				SuperiorFocusEarth, new LGSCraftingIngredient
				{
					Name = SuperiorFocusEarth,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = LargeGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = LargeGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = LargeTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = LargeInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 100 }
					}
				}
			},
			{
				SuperiorFocusFire, new LGSCraftingIngredient
				{
					Name = SuperiorFocusFire,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = LargeGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = LargeGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = LargeTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = LargeDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 100 }
					}
				}
			},
			{
				SuperiorFocusWater, new LGSCraftingIngredient
				{
					Name = SuperiorFocusWater,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = LargeGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = LargeGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = LargeInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = LargeDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 100 }
					}
				}
			},
			{
				SuperiorFocusNegative, new LGSCraftingIngredient
				{
					Name = SuperiorFocusNegative,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = LargeGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = LargeTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = LargeInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = LargeSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 100 }
					}
				}
			},
			{
				SuperiorFocusPositive, new LGSCraftingIngredient
				{
					Name = SuperiorFocusPositive,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = LargeGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = LargeGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = LargeSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = LargeDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 100 }
					}
				}
			},
			{
				PureEtherealEssence, new LGSCraftingIngredient
				{
					Name = PureEtherealEssence,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = LargeGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = LargeInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = LargeSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = LargeDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 100 }
					}
				}
			},
			{
				PureMaterialEssence, new LGSCraftingIngredient
				{
					Name = PureMaterialEssence,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = LargeGlowingArrowhead, Amount = 1 },
						new CraftingIngredient { Name = LargeTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = LargeSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = LargeDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 100 }
					}
				}
			},
			{
				FlawlessGemDominion, new LGSCraftingIngredient
				{
					Name = FlawlessGemDominion,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = LargeGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = LargeTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = LargeInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = LargeDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 100 }
					}
				}
			},
			{
				FlawlessGemEscalation, new LGSCraftingIngredient
				{
					Name = FlawlessGemEscalation,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = LargeTwistedShrapnel, Amount = 1 },
						new CraftingIngredient { Name = LargeInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = LargeSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = LargeDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 100 }
					}
				}
			},
			{
				FlawlessGemOpposition, new LGSCraftingIngredient
				{
					Name = FlawlessGemOpposition,
					Ingredients = new List<CraftingIngredient>
					{
						new CraftingIngredient { Name = LargeGnawedBone, Amount = 1 },
						new CraftingIngredient { Name = LargeInfernalChain, Amount = 1 },
						new CraftingIngredient { Name = LargeSulfurousStone, Amount = 1 },
						new CraftingIngredient { Name = LargeDevilScales, Amount = 1 },
						new CraftingIngredient { Name = CoV, Amount = 100 }
					}
				}
			},
			#endregion
		};

		public class LGSCraftedItemProperty
		{
			public string Name;
			public List<ItemProperty> AppliedProperties;
			public List<LGSCraftingIngredient> Cost;

			public override string ToString()
			{
				return Name;
			}
		}

		public static Dictionary<string, List<LGSCraftedItemProperty>> LGSAugments = new Dictionary<string, List<LGSCraftedItemProperty>>
		{
			{
				"Tier 1", new List<LGSCraftedItemProperty>
				{
					new LGSCraftedItemProperty
					{
						Name = "- empty -"
					},
					new LGSCraftedItemProperty
					{
						Name = "Electric Spell Critical Damage 20%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Electric Spell Critical Damage", Type = "enhancement", Value = 20 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusAir],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Acid Spell Critical Damage 20%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Acid Spell Critical Damage", Type = "enhancement", Value = 20 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusEarth],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Fire Spell Critical Damage 20%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fire Spell Critical Damage", Type = "enhancement", Value = 20 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusFire],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Cold Spell Critical Damage 20%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Cold Spell Critical Damage", Type = "enhancement", Value = 20 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusWater],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Negative Spell Critical Damage 20%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Negative Spell Critical Damage", Type = "enhancement", Value = 20 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusNegative],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Positive Spell Critical Damage 20%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Positive Spell Critical Damage", Type = "enhancement", Value = 20 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusPositive],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Electric Damage on Being Hit 8d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Electric Damage on Being Hit", Type = "enhancement", Value = 8 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusAir],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Acid Damage on Being Hit 8d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Acid Damage on Being Hit", Type = "enhancement", Value = 8 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusEarth],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Fire Damage on Being Hit 8d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fire Damage on Being Hit", Type = "enhancement", Value = 8 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusFire],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Cold Damage on Being Hit 8d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Cold Damage on Being Hit", Type = "enhancement", Value = 8 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusWater],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Evil Damage on Being Hit 8d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Evil Damage on Being Hit", Type = "enhancement", Value = 8 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusNegative],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Good Damage on Being Hit 8d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Good Damage on Being Hit", Type = "enhancement", Value = 8 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusPositive],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+22 Charisma Skills/+6 UMD, +151 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Bluff", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Diplomacy", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Haggle", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Intimidate", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Perform", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Use Magic Device", Type = "competence", Value = 6 },
							new ItemProperty { Property = "Spell Points", Type = "profane", Value = 151 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusAir],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+22 Wisdom Skills, +151 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Heal", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Listen", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Spot", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Spell Points", Type = "profane", Value = 151 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusEarth],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+22 Intelligence Skills, +151 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Disable Device", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Repair", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Search", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Spellcraft", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Spell Points", Type = "profane", Value = 151 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusFire],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+22 Wisdom Skills, +151 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Heal", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Listen", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Spot", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Spell Points", Type = "profane", Value = 151 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusWater],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+22 Intelligence Skills, +151 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Disable Device", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Repair", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Search", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Spellcraft", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Spell Points", Type = "profane", Value = 151 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusNegative],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+22 Charisma Skills/+6 UMD, +151 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Bluff", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Diplomacy", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Haggle", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Intimidate", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Perform", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Use Magic Device", Type = "competence", Value = 6 },
							new ItemProperty { Property = "Spell Points", Type = "profane", Value = 151 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusPositive],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+22 Dexterity Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Balance", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Hide", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Move Silently", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Open Lock", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Tumble", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Hit Points", Type = "profane", Value = 28 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusAir],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+22 Constitution Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Concentration", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Hit Points", Type = "profane", Value = 28 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusEarth],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+22 Dexterity Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Balance", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Hide", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Move Silently", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Open Lock", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Tumble", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Hit Points", Type = "profane", Value = 28 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusFire],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+22 Strength Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Jump", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Swim", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Hit Points", Type = "profane", Value = 28 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusWater],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+22 Strength Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Jump", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Swim", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Hit Points", Type = "profane", Value = 28 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusNegative],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+22 Constitution Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Concentration", Type = "competence", Value = 22 },
							new ItemProperty { Property = "Hit Points", Type = "profane", Value = 28 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusPositive],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+13 Reflex Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Reflex", Type = "resistance", Value = 13 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusAir],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+13 Fortitude Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortitude", Type = "resistance", Value = 13 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusEarth],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+13 Reflex Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Reflex", Type = "resistance", Value = 13 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusFire],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+13 Will Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Will", Type = "resistance", Value = 13 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusWater],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+13 Fortitude Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortitude", Type = "resistance", Value = 13 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusNegative],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+13 Will Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Will", Type = "resistance", Value = 13 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusPositive],
							LGSCraftables[DilutedEtherealEssence],
							LGSCraftables[CloudyGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+50 Electric Resistance",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Electric Resistance", Type = "enhancement", Value = 50 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusAir],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+50 Acid Resistance",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Acid Resistance", Type = "enhancement", Value = 50 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusEarth],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+50 Fire Resistance",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fire Resistance", Type = "enhancement", Value = 50 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusFire],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+50 Cold Resistance",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Cold Resistance", Type = "enhancement", Value = 50 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusWater],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+4 Fortitude Save vs Disease, Blindness Immunity",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortitude vs Disease", Type = "insight", Value = 4 },
							new ItemProperty { Property = "Blindness Immunity" }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusNegative],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+128 Unconsciousness Range, 16 healing every 10 seconds",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Unconsciousness Range", Value = 128 },
							new ItemProperty { Property = "Healing every 10 seconds", Value = 16 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[InferiorFocusPositive],
							LGSCraftables[DilutedMaterialEssence],
							LGSCraftables[CloudyGemOpposition]
						}
					},
				}
			},
			{
				"Tier 2", new List<LGSCraftedItemProperty>
				{
					new LGSCraftedItemProperty
					{
						Name = "- empty -"
					},
					new LGSCraftedItemProperty
					{
						Name = "Electric Spell Critical Damage 10%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Electric Spell Critical Damage", Type = "insight", Value = 20 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusAir],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Acid Spell Critical Damage 10%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Acid Spell Critical Damage", Type = "insight", Value = 20 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusEarth],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Fire Spell Critical Damage 10%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fire Spell Critical Damage", Type = "insight", Value = 20 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusFire],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Cold Spell Critical Damage 10%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Cold Spell Critical Damage", Type = "insight", Value = 20 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusWater],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Negative Spell Critical Damage 10%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Negative Spell Critical Damage", Type = "insight", Value = 20 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusNegative],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Positive Spell Critical Damage 10%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Positive Spell Critical Damage", Type = "insight", Value = 20 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusPositive],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Electric Damage on Being Hit 10d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Electric Damage on Being Hit", Value = 10 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusAir],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Acid Damage on Being Hit 10d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Acid Damage on Being Hit", Value = 10 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusEarth],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Fire Damage on Being Hit 10d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fire Damage on Being Hit", Value = 10 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusFire],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Cold Damage on Being Hit 10d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Cold Damage on Being Hit", Value = 10 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusWater],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Evil Damage on Being Hit 10d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Evil Damage on Being Hit", Value = 10 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusNegative],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Good Damage on Being Hit 10d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Good Damage on Being Hit", Value = 10 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusPositive],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+11 Charisma Skills/+3 UMD, +151 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Bluff", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Diplomacy", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Haggle", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Intimidate", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Perform", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Use Magic Device", Type = "insight", Value = 3 },
							new ItemProperty { Property = "Spell Points", Type = "insight", Value = 151 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusAir],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+11 Wisdom Skills, +151 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Heal", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Listen", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Spot", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Spell Points", Type = "insight", Value = 151 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusEarth],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+11 Intelligence Skills, +151 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Disable Device", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Repair", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Search", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Spellcraft", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Spell Points", Type = "insight", Value = 151 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusFire],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+11 Wisdom Skills, +151 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Heal", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Listen", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Spot", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Spell Points", Type = "insight", Value = 151 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusWater],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+11 Intelligence Skills, +151 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Disable Device", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Repair", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Search", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Spellcraft", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Spell Points", Type = "insight", Value = 151 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusNegative],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+11 Charisma Skills/+3 UMD, +151 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Bluff", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Diplomacy", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Haggle", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Intimidate", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Perform", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Use Magic Device", Type = "insight", Value = 3 },
							new ItemProperty { Property = "Spell Points", Type = "insight", Value = 151 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusPositive],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+11 Dexterity Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Balance", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Hide", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Move Silently", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Open Lock", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Tumble", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Hit Points", Type = "insight", Value = 28 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusAir],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+11 Constitution Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Concentration", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Hit Points", Type = "insight", Value = 28 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusEarth],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+11 Dexterity Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Balance", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Hide", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Move Silently", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Open Lock", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Tumble", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Hit Points", Type = "insight", Value = 28 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusFire],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+11 Strength Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Jump", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Swim", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Hit Points", Type = "insight", Value = 28 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusWater],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+11 Strength Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Jump", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Swim", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Hit Points", Type = "insight", Value = 28 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusNegative],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+11 Constitution Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Concentration", Type = "insight", Value = 11 },
							new ItemProperty { Property = "Hit Points", Type = "insight", Value = 28 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusPositive],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+7 Reflex Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Reflex", Type = "insight", Value = 7 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusAir],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+7 Fortitude Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortitude", Type = "insight", Value = 7 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusEarth],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+7 Reflex Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Reflex", Type = "insight", Value = 7 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusFire],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+7 Will Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Will", Type = "insight", Value = 7 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusWater],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+7 Fortitude Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortitude", Type = "insight", Value = 7 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusNegative],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+7 Will Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Will", Type = "insight", Value = 7 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusPositive],
							LGSCraftables[EtherealEssence],
							LGSCraftables[GemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+25 Electric Resistance",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Electric Resistance", Type = "insight", Value = 25 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusAir],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+25 Acid Resistance",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Acid Resistance", Type = "insight", Value = 25 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusEarth],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+25 Fire Resistance",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fire Resistance", Type = "insight", Value = 25 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusFire],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+25 Cold Resistance",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Cold Resistance", Type = "insight", Value = 25 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusWater],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+4 Fortitude Save vs Poison, Fear Immunity",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortitude vs Poison", Type = "insight", Value = 4 },
							new ItemProperty { Property = "Fear Immunity" }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusNegative],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+64 Unconsciousness Range, 8 healing every 10 seconds",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Unconsciousness Range", Value = 64 },
							new ItemProperty { Property = "Healing every 10 seconds", Value = 8 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[FocusPositive],
							LGSCraftables[MaterialEssence],
							LGSCraftables[GemOpposition]
						}
					},
				}
			},
			{
				"Tier 3", new List<LGSCraftedItemProperty>
				{
					new LGSCraftedItemProperty
					{
						Name = "- empty -"
					},
					new LGSCraftedItemProperty
					{
						Name = "Electric Spell Critical Damage 5%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Electric Spell Critical Damage", Type = "quality", Value = 5 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusAir],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Acid Spell Critical Damage 5%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Acid Spell Critical Damage", Type = "quality", Value = 5 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusEarth],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Fire Spell Critical Damage 5%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fire Spell Critical Damage", Type = "quality", Value = 5 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusFire],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Cold Spell Critical Damage 5%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Cold Spell Critical Damage", Type = "quality", Value = 5 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusWater],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Negative Spell Critical Damage 5%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Negative Spell Critical Damage", Type = "quality", Value = 5 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusNegative],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Positive Spell Critical Damage 5%",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Positive Spell Critical Damage", Type = "quality", Value = 5 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusPositive],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Electric Damage on Being Hit 15d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Electric Damage on Being Hit", Value = 15 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusAir],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Acid Damage on Being Hit 15d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Acid Damage on Being Hit", Value = 15 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusEarth],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Fire Damage on Being Hit 15d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fire Damage on Being Hit", Value = 15 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusFire],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Cold Damage on Being Hit 15d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Cold Damage on Being Hit", Value = 15 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusWater],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Evil Damage on Being Hit 15d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Evil Damage on Being Hit", Value = 15 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusNegative],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "Good Damage on Being Hit 15d6",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Good Damage on Being Hit", Value = 15 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusPositive],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemDominion]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+6 Charisma Skills/+1 UMD, +75 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Bluff", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Diplomacy", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Haggle", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Intimidate", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Perform", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Use Magic Device", Type = "quality", Value = 1 },
							new ItemProperty { Property = "Spell Points", Type = "quality", Value = 75 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusAir],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+6 Wisdom Skills, +75 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Heal", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Listen", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Spot", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Spell Points", Type = "quality", Value = 75 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusEarth],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+6 Intelligence Skills, +75 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Disable Device", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Repair", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Search", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Spellcraft", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Spell Points", Type = "quality", Value = 75 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusFire],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+6 Wisdom Skills, +75 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Heal", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Listen", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Spot", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Spell Points", Type = "quality", Value = 75 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusWater],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+6 Intelligence Skills, +75 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Disable Device", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Repair", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Search", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Spellcraft", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Spell Points", Type = "quality", Value = 75 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusNegative],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+6 Charisma Skills/+1 UMD, +75 Spell Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Bluff", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Diplomacy", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Haggle", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Intimidate", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Perform", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Use Magic Device", Type = "quality", Value = 1 },
							new ItemProperty { Property = "Spell Points", Type = "quality", Value = 75 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusPositive],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+6 Dexterity Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Balance", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Hide", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Move Silently", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Open Lock", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Tumble", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Hit Points", Type = "quality", Value = 14 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusAir],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+6 Constitution Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Concentration", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Hit Points", Type = "quality", Value = 14 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusEarth],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+6 Dexterity Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Balance", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Hide", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Move Silently", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Open Lock", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Tumble", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Hit Points", Type = "quality", Value = 14 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusFire],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+6 Strength Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Jump", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Swim", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Hit Points", Type = "quality", Value = 14 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusWater],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+6 Strength Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Jump", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Swim", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Hit Points", Type = "quality", Value = 14 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusNegative],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+6 Constitution Skills, +28 Hit Points",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Concentration", Type = "quality", Value = 6 },
							new ItemProperty { Property = "Hit Points", Type = "quality", Value = 14 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusPositive],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemEscalation]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+2 Reflex Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Reflex", Type = "quality", Value = 2 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusAir],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+2 Fortitude Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortitude", Type = "quality", Value = 2 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusEarth],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+2 Reflex Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Reflex", Type = "quality", Value = 2 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusFire],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+2 Will Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Will", Type = "quality", Value = 2 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusWater],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+2 Fortitude Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fortitude", Type = "quality", Value = 2 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusNegative],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+2 Will Save",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Will", Type = "quality", Value = 2 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusPositive],
							LGSCraftables[PureEtherealEssence],
							LGSCraftables[FlawlessGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+17 Electric Resistance",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Electric Resistance", Type = "competence", Value = 17 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusAir],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+17 Acid Resistance",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Acid Resistance", Type = "competence", Value = 17 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusEarth],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+17 Fire Resistance",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Fire Resistance", Type = "competence", Value = 17 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusFire],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+17 Cold Resistance",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Cold Resistance", Type = "competence", Value = 17 },
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusWater],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+25 Negative Resistance, Deathblock",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Negative Resistance", Type = "enhancement", Value = 25 },
							new ItemProperty { Property = "Deathblock" }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusNegative],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemOpposition]
						}
					},
					new LGSCraftedItemProperty
					{
						Name = "+32 Unconsciousness Range, 4 healing every 10 seconds",
						AppliedProperties = new List<ItemProperty>
						{
							new ItemProperty { Property = "Unconsciousness Range", Value = 32 },
							new ItemProperty { Property = "Healing every 10 seconds", Value = 4 }
						},
						Cost = new List<LGSCraftingIngredient>
						{
							LGSCraftables[SuperiorFocusPositive],
							LGSCraftables[PureMaterialEssence],
							LGSCraftables[FlawlessGemOpposition]
						}
					},
				}
			},
		};

        public class LGSItemContainer : ACustomItemContainer
        {
            public DDOItemData BaseItem;
            DDOItemData GeneratedItem;
            public LGSCraftedItemProperty[] Slots { get; set; }

            public static List<SlotType> DisallowedSlots = new List<SlotType> { SlotType.Finger, SlotType.Offhand, SlotType.Trinket, SlotType.Weapon };

            public LGSItemContainer()
            {
                Source = ItemDataSource.LegendaryGreenSteel;
				WikiURL = "https://ddowiki.com/page/Legendary_Green_Steel_items";
                Slots = new LGSCraftedItemProperty[Tiers];
                for (int i = 0; i < Tiers; i++)
                    Slots[i] = LGSAugments["Tier " + (i + 1).ToString()][0];
            }

            public override List<SlotType> GetDisallowedSlots()
            {
                return DisallowedSlots;
            }

            void GenerateItem()
            {
                if (GeneratedItem == null)
                {
					GeneratedItem = new DDOItemData(ItemDataSource.LegendaryGreenSteel, false)
					{
						Name = Name,
						IconName = BaseItem.IconName,
                        WikiURL = "https://ddowiki.com/page/Legendary_Green_Steel_items",
                        Slot = BaseItem.Slot,
                        Category = BaseItem.Category,
                        QuestFoundIn = BaseItem.QuestFoundIn
                    };
                }
                else GeneratedItem.Properties.Clear();

                for (int i = 0; i < Tiers; i++)
                {
                    if (Slots[i]?.AppliedProperties == null) continue;
                    foreach (var ap in Slots[i].AppliedProperties)
                        GeneratedItem.AddProperty(ap.Property, ap.Type, ap.Value, null);
                }

                // add in custom LGS set bonus
                ItemProperty ip = GeneratedItem.AddProperty("Legendary Green Steel", "set", 0, null);
				ip.HideOptions = true;
				ip.Options = new List<ItemProperty>();
				for (int i = 0; i < Tiers; i++)
				{
					if (Slots[i]?.AppliedProperties == null) continue;
					foreach (var c in Slots[i].Cost)
						ip.Options.Add(new ItemProperty { Property = c.Name, Type = Slots[i].Name, Value = c.Count });
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
                xe.InnerText = BaseItem.Name;
                xci.AppendChild(xe);
                for (int i = 0; i < Tiers; i++)
                {
                    xe = doc.CreateElement("Tier" + (i + 1));
                    xe.InnerText = Slots[i]?.Name;
                    xci.AppendChild(xe);
                }
            }

            public override bool FromXml(XmlElement xci)
            {
                try
                {
                    string baseitem = xci.GetElementsByTagName("BaseItem")[0].InnerText;
                    BaseItem = DatasetManager.Dataset.Items.Find(i => i.Name == baseitem);
                    for (int i = 0; i < Tiers; i++)
                    {
                        List<LGSCraftedItemProperty> props = LGSAugments["Tier " + (i + 1)];
                        string p = xci.GetElementsByTagName("Tier" + (i + 1))[0].InnerText;
                        Slots[i] = props.Find(pr => pr.Name == p);
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
