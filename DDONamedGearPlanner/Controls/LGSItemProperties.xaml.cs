using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DDONamedGearPlanner
{
	/// <summary>
	/// Interaction logic for LGSItemProperties.xaml
	/// </summary>
	public partial class LGSItemProperties : UserControl
	{
		LGSCrafting.LGSItemContainer LGSItem;

		public LGSItemProperties()
		{
			InitializeComponent();
		}

		public void SetItem(LGSCrafting.LGSItemContainer item)
		{
			LGSItem = null;
			lvSlots.Items.Clear();
			tvBreakdown.Items.Clear();
			lvCrafting.Items.Clear();
			if (item == null) return;

			LGSItem = item;
			for (int i = 0; i < LGSCrafting.Tiers; i++)
			{
				LGSItemPropertyData data = new LGSItemPropertyData(item, i);
				data.PropertyChanged += Data_PropertyChanged;
				lvSlots.Items.Add(data);
			}

			// we call this to regenerate the DDOItemData potentially being used 
			LGSItem.GetItem();

			CalculateCraftingCosts();
		}

		private void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// we call this to regenerate the DDOItemData potentially being used 
			LGSItem.GetItem();

			CalculateCraftingCosts();
		}

		void CalculateCraftingCosts()
		{
			tvBreakdown.Items.Clear();
			lvCrafting.Items.Clear();

			List<LGSCrafting.LGSCraftingIngredient> lgsci = new List<LGSCrafting.LGSCraftingIngredient>();
			List<CraftingIngredient> totalci = new List<CraftingIngredient>();
			for (int t = 0; t < LGSCrafting.Tiers; t++)
			{
				LGSCrafting.LGSCraftedItemProperty cip = LGSItem.Slots[t];
				if (cip?.Cost == null) continue;
				foreach (var lgsi in cip.Cost)
				{
					LGSCrafting.LGSCraftingIngredient tlgsci = lgsci.Find(l => l.Name == lgsi.Name);
					if (tlgsci != null) tlgsci.Count++;
					else
					{
						tlgsci = new LGSCrafting.LGSCraftingIngredient { Name = lgsi.Name, Ingredients = lgsi.Ingredients };
						lgsci.Add(tlgsci);
					}

					foreach (var ing in lgsi.Ingredients)
					{
						bool found = false;
						for (int i = 0; i < totalci.Count; i++)
						{
							if (totalci[i].Name == ing.Name)
							{
								CraftingIngredient ci = totalci[i];
								ci.Amount += ing.Amount;
								totalci[i] = ci;
								found = true;
								break;
							}
						}
						if (!found) totalci.Add(ing);
					}
				}
			}

			// populate ingredient breakdown treeview
			foreach (var lgsi in lgsci)
			{
				TreeViewItem litvi = new TreeViewItem();
				litvi.Header = lgsi.Name;
				tvBreakdown.Items.Add(litvi);
				foreach (var li in lgsi.Ingredients)
				{
					TreeViewItem citvi = new TreeViewItem();
					citvi.Header = "(" + (lgsi.Count * li.Amount) + ") " + li.Name;
					litvi.Items.Add(citvi);
				}
			}

			// populate total ingredients list
			totalci.ForEach(i => lvCrafting.Items.Add(i));
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
	}

	public class LGSItemPropertyData : INotifyPropertyChanged
	{
		public LGSCrafting.LGSItemContainer LGSItem { get; private set; }
		public string Slot { get; private set; }
		public int LGSSlot;

		public List<LGSCrafting.LGSCraftedItemProperty> AvailableProperties { get; private set; }

		public string Property
		{
			get { return LGSItem.Slots[LGSSlot].Name; }
			set
			{
				LGSItem.Slots[LGSSlot] = AvailableProperties.Find(p => p.Name == value);
				NotifyPropertyChanged();
			}
		}

		public LGSItemPropertyData(LGSCrafting.LGSItemContainer item, int slot)
		{
			LGSItem = item;
			LGSSlot = slot;
			Slot = "Tier " + (slot + 1);

			AvailableProperties = LGSCrafting.LGSAugments[Slot];
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
