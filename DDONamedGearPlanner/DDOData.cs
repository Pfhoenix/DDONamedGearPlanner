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

	[Flags]
	public enum WeaponType
	{

	}

	public class ItemProperty
	{
		public string Property;
		public string Type;
		public int Value;
	}

	public class DDOData
	{
		public string Name;
		public SlotType Slot;
		public int Category;
		public List<ItemProperty> Properties = new List<ItemProperty>();
	}
}
