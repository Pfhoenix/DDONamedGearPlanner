using System;
using System.Collections.Generic;
using System.Xml;

namespace DDONamedGearPlanner
{
	public static class LGSCrafting
	{
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
			}
		};
	}
}
