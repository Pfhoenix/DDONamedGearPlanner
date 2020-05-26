using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DDONamedGearPlanner
{
	/// <summary>
	/// Interaction logic for SlaveLordItemProperties.xaml
	/// </summary>
	public partial class SlaveLordItemProperties : UserControl
	{
		SlaveLordCrafting.SlaveLordItemContainer SLItem;

		public SlaveLordItemProperties()
		{
			InitializeComponent();
		}

		public void SetItem(SlaveLordCrafting.SlaveLordItemContainer item)
		{
			SLItem = null;
			lvSlots.Items.Clear();
			lvCrafting.Items.Clear();
			if (item == null) return;

			if (item.BaseItem.Name.StartsWith("Legendary")) cbML.SelectedIndex = 1;
			else cbML.SelectedIndex = 0;

			SLItem = item;
			SlaveLordCrafting.ESlaveLordItemSlots[] slots = (SlaveLordCrafting.ESlaveLordItemSlots[])Enum.GetValues(typeof(SlaveLordCrafting.ESlaveLordItemSlots));
			foreach (var slot in slots)
			{
				SlaveLordItemPropertyData data = new SlaveLordItemPropertyData(SLItem, slot);
				data.PropertyChanged += Data_PropertyChanged;
				lvSlots.Items.Add(data);
			}

			// we call this to regenerate the DDOItemData potentially being used 
			SLItem.GetItem();

			CalculateCraftingCosts();
		}

		private void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// we call this to regenerate the DDOItemData potentially being used 
			SLItem.GetItem();

			CalculateCraftingCosts();
		}

		void CalculateCraftingCosts()
		{
			lvCrafting.Items.Clear();

			List<CraftingIngredient> ingredients = new List<CraftingIngredient>();
			SlaveLordCrafting.ESlaveLordItemSlots[] slots = (SlaveLordCrafting.ESlaveLordItemSlots[])Enum.GetValues(typeof(SlaveLordCrafting.ESlaveLordItemSlots));
			foreach (var slot in slots)
			{
				CraftedItemProperty cip = SLItem.Slots[(int)slot];
				if (cip?.Cost == null) continue;
				foreach (var ing in cip.Cost)
				{
					bool found = false;
					for (int i = 0; i < ingredients.Count; i++)
					{
						if (ingredients[i].Name == ing.Name)
						{
							CraftingIngredient ci = ingredients[i];
							ci.Amount += ing.Amount;
							ingredients[i] = ci;
							found = true;
							break;
						}
					}
					if (!found) ingredients.Add(ing);
				}
			}

			ingredients.ForEach(i => lvCrafting.Items.Add(i));
		}

		private void LV_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double tw = e.NewSize.Width;
			GridView gv = lvCrafting.View as GridView;
			gv.Columns[1].Width = Math.Min(70, Math.Max(40, tw * 0.2));
			gv.Columns[0].Width = tw - gv.Columns[1].Width - 2;
		}

		private void Slots_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double tw = e.NewSize.Width;
			GridView gv = lvSlots.View as GridView;
			gv.Columns[0].Width = Math.Min(70, Math.Max(40, tw * 0.2));
			gv.Columns[1].Width = tw - gv.Columns[0].Width - 2;
		}

		private void cbML_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (SLItem == null) return;

			string oldml = cbML.SelectedIndex == 1 ? "Heroic " : "Legendary ";
			string newml = cbML.SelectedIndex == 0 ? "Heroic " : "Legendary ";
			string newbin = SLItem.BaseItem.Name.StartsWith("Legendary") ? SLItem.BaseItem.Name.Substring(10) : "Legendary " + SLItem.BaseItem.Name;
			SLItem.BaseItem = DatasetManager.Dataset.Items.Find(i => i.Name == newbin && i.Slot == SLItem.BaseItem.Slot);
			SlaveLordCrafting.ESlaveLordItemSlots[] slots = (SlaveLordCrafting.ESlaveLordItemSlots[])Enum.GetValues(typeof(SlaveLordCrafting.ESlaveLordItemSlots));
			foreach (var slot in slots)
			{
				// get the current index in the oldml list
				int i = SlaveLordCrafting.ItemSlots[oldml + slot].IndexOf(SLItem.Slots[(int)slot]);
				// set the newml list at the old index
				SLItem.Slots[(int)slot] = SlaveLordCrafting.ItemSlots[newml + slot][i];
			}

			SetItem(SLItem);
		}
	}

	public class SlaveLordItemPropertyData : INotifyPropertyChanged
	{
		public SlaveLordCrafting.SlaveLordItemContainer SLItem { get; private set; }
		public SlaveLordCrafting.ESlaveLordItemSlots SLSlot { get; private set; }

		public List<CraftedItemProperty> AvailableProperties { get; private set; }

		public string Property
		{
			get { return SLItem.Slots[(int)SLSlot].Name; }
			set
			{
				SLItem.Slots[(int)SLSlot] = AvailableProperties.Find(p => p.Name == value);
				NotifyPropertyChanged();
			}
		}


		public SlaveLordItemPropertyData(SlaveLordCrafting.SlaveLordItemContainer item, SlaveLordCrafting.ESlaveLordItemSlots slot)
		{
			SLItem = item;
			SLSlot = slot;
			string ml = "Heroic ";
			if (SLItem.BaseItem.Name.StartsWith("Legendary")) ml = "Legendary ";

			AvailableProperties = SlaveLordCrafting.ItemSlots[ml + slot.ToString()];
			//AvailableProperties.Insert(0, new CraftedItemProperty { Name = "- empty -" });
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
