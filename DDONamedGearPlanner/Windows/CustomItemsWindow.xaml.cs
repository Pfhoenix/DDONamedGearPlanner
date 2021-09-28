using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DDONamedGearPlanner
{
	/// <summary>
	/// Interaction logic for CustomItemsWindow.xaml
	/// </summary>
	public partial class CustomItemsWindow : Window
	{
		ListViewCustomItemProperties CustomIP;
		SlaveLordItemProperties SlaveLordIP;
		LGSItemProperties LGSIP;
		string WikiURL;

		public CustomItemsWindow()
		{
			InitializeComponent();

			tvItems.ContextMenu = tvItems.Resources["EmptyCM"] as ContextMenu;

			SetupTreeView();

			CustomIP = lvDetails;
		}

		public void SelectItem(string name, string slot)
		{
			foreach (TreeViewItem tvi in tvItems.Items)
			{
				if ((tvi.Header as TextBlock).Text == slot)
				{
					foreach (TreeViewItem tvii in tvi.Items)
					{
						ACustomItemContainer cic = tvii.Tag as ACustomItemContainer;
						if (cic?.Name == name)
						{
							tvii.BringIntoView();
							tvii.IsSelected = true;
							return;
						}
					}
				}
			}
		}

		TreeViewItem AddItemToTreeView(ACustomItemContainer cic)
		{
			TreeViewItem tvi = null;
			foreach (TreeViewItem i in tvItems.Items)
			{
				if ((SlotType)i.Tag == cic.GetItem().Slot)
				{
					tvi = i;
					break;
				}
			}
			if (tvi == null)
			{
				tvi = new TreeViewItem();
				TextBlock tb = new TextBlock { Text = cic.GetItem().Slot.ToString(), FontWeight = FontWeights.Bold };
				tvi.Header = tb;
				tvi.Tag = cic.GetItem().Slot;
				tvItems.Items.Add(tvi);
			}

			TreeViewItem tvii = new TreeViewItem();
			tvii.Header = cic.Name + " (" + cic.Source.ToString() + ")";
			tvii.Tag = cic;
			tvi.Items.Add(tvii);

			return tvii;
		}

		void SetupTreeView()
		{
			List<ACustomItemContainer> customitems = CustomItemsManager.CustomItems;
			customitems.Sort((a, b) => string.Compare(a.Name, b.Name, true));

			foreach (var ci in customitems)
				AddItemToTreeView(ci);
		}

		private void Items_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (ItemPropertiesArea.Child == SlaveLordIP) SlaveLordIP.SetItem(null);
			else if (ItemPropertiesArea.Child == CustomIP) CustomIP.SetItem(null);
			else if (ItemPropertiesArea.Child == LGSIP) LGSIP.SetItem(null);

			if (tvItems.SelectedItem == null)
			{
				tvItems.ContextMenu = tvItems.Resources["EmptyCM"] as ContextMenu;
			}
			else
			{
				TreeViewItem tvi = tvItems.SelectedItem as TreeViewItem;
				if (tvi.Tag is SlotType)
				{
					tvItems.ContextMenu = tvItems.Resources["SlotCM"] as ContextMenu;
					SlotType st = (SlotType)tvi.Tag;
					if (LGSCrafting.LGSItemContainer.DisallowedSlots.Contains(st)) (tvItems.ContextMenu.Items[0] as MenuItem).Visibility = Visibility.Collapsed;
					else
					{
						(tvItems.ContextMenu.Items[0] as MenuItem).Visibility = Visibility.Visible;
						(tvItems.ContextMenu.Items[0] as MenuItem).Header = "New Legendary Green Steel " + (SlotType)tvi.Tag + " Item";
					}
					if (SlaveLordCrafting.SlaveLordItemContainer.DisallowedSlots.Contains(st)) (tvItems.ContextMenu.Items[1] as MenuItem).Visibility = Visibility.Collapsed;
					else
					{
						(tvItems.ContextMenu.Items[1] as MenuItem).Visibility = Visibility.Visible;
						(tvItems.ContextMenu.Items[1] as MenuItem).Header = "New Slave Lord " + (SlotType)tvi.Tag + " Item";
					}
					(tvItems.ContextMenu.Items[2] as MenuItem).Header = "New Custom " + (SlotType)tvi.Tag + " Item";
				}
				else
				{
					tvItems.ContextMenu = tvItems.Resources["ItemCM"] as ContextMenu;
					ACustomItemContainer cic = tvi.Tag as ACustomItemContainer;
					switch (cic.Source)
					{
						case ItemDataSource.LegendaryGreenSteel:
							if (LGSIP == null) LGSIP = new LGSItemProperties();
							ItemPropertiesArea.Child = LGSIP;
							LGSIP.SetItem((LGSCrafting.LGSItemContainer)cic);
							(tvItems.ContextMenu.Items[3] as MenuItem).IsEnabled = true;
							WikiURL = cic.WikiURL;
							break;

						case ItemDataSource.SlaveLord:
							if (SlaveLordIP == null) SlaveLordIP = new SlaveLordItemProperties();
							ItemPropertiesArea.Child = SlaveLordIP;
							SlaveLordIP.SetItem((SlaveLordCrafting.SlaveLordItemContainer)cic);
							(tvItems.ContextMenu.Items[3] as MenuItem).IsEnabled = true;
							WikiURL = cic.WikiURL;
							break;

						case ItemDataSource.Custom:
							ItemPropertiesArea.Child = CustomIP;
							CustomIP.SetItem(cic.GetItem());
							(tvItems.ContextMenu.Items[3] as MenuItem).IsEnabled = false;
							break;
					}
				}
			}
		}

		public delegate void ItemDoubleClickedDelegate(DDOItemData item);
		public event ItemDoubleClickedDelegate ItemDoubleClicked;

		private void Items_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if ((e.OriginalSource as Grid) != null) return;
			if (tvItems.SelectedItem == null) return;
			TreeViewItem tvi = tvItems.SelectedItem as TreeViewItem;
			if (tvi.HasItems) return;

			ItemDoubleClicked?.Invoke((tvi.Tag as ACustomItemContainer).GetItem());
		}

		private void Items_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

			if (treeViewItem != null)
			{
				treeViewItem.Focus();
				e.Handled = true;
			}
			else if (tvItems.SelectedItem != null)
			{
				(tvItems.SelectedItem as TreeViewItem).IsSelected = false;
				e.Handled = true;
			}
		}

		static TreeViewItem VisualUpwardSearch(DependencyObject source)
		{
			while (source != null && !(source is TreeViewItem))
				source = VisualTreeHelper.GetParent(source);

			return source as TreeViewItem;
		}

		bool GetSlot(SlotType initial, List<SlotType> disallow, out SlotType rval)
		{
			rval = SlotType.None;
			ComboBoxWindow cbw = new ComboBoxWindow();
			cbw.Owner = this;
			SlotType[] slots = null;
			if (disallow == null) slots = ((SlotType[])Enum.GetValues(typeof(SlotType))).Where(s => s != SlotType.None).ToArray();
			else slots = ((SlotType[])Enum.GetValues(typeof(SlotType))).Where(s => s != SlotType.None && !disallow.Contains(s)).ToArray();
			cbw.Setup("Select slot", slots, initial);
			if (cbw.ShowDialog() == true)
			{
				rval = (SlotType)cbw.SelectedItem;
				return true;
			}
			else return false;
		}

		void AddSlotSpecificProperties(DDOItemData item)
		{
			switch (item.Slot)
			{
				case SlotType.Body:
					item.AddProperty("Armor Category", ArmorCategory.Cloth.ToString(), 0, null, 1);
					item.Category = (int)ArmorCategory.Cloth;
					break;

				case SlotType.Offhand:
					item.AddProperty("Offhand Category", OffhandCategory.Orb.ToString(), 0, null, 1);
					item.Category = (int)OffhandCategory.Orb;
					break;

				case SlotType.Weapon:
					item.AddProperty("Handedness", null, 1, null, 1);
					item.AddProperty("Weapon Category", WeaponCategory.Simple.ToString(), 0, null, 1);
					item.Category = (int)WeaponCategory.Simple;
					break;
			}
		}

		void RemoveSlotSpecificProperties(DDOItemData item)
		{
			switch (item.Slot)
			{
				case SlotType.Body:
					item.Properties.Remove(item.Properties.Find(p => p.Property == "Armor Category"));
					break;

				case SlotType.Offhand:
					item.Properties.Remove(item.Properties.Find(p => p.Property == "Offhand Category"));
					break;

				case SlotType.Weapon:
					item.Properties.Remove(item.Properties.Find(p => p.Property == "Weapon Category"));
					item.Properties.Remove(item.Properties.Find(p => p.Property == "Handedness"));
					break;
			}
		}

		private void NewCustomItem_Click(object sender, RoutedEventArgs e)
		{
			SlotType slot = SlotType.None;
			if (tvItems.SelectedItem != null)
			{
				slot = (SlotType)(tvItems.SelectedItem as TreeViewItem).Tag;
			}
			else
			{
				if (!GetSlot(SlotType.None, null, out slot)) return;
			}

			if (slot == SlotType.None) return;

			CustomItemContainer cic = new CustomItemContainer { Name = "<Custom Item>" };
			cic.Item = new DDOItemData(ItemDataSource.Custom, false) { Name = cic.Name, Slot = slot };
			cic.Item.AddProperty("Minimum Level", null, 1, null);
			AddSlotSpecificProperties(cic.Item);

			CustomItemsManager.CustomItems.Add(cic);
			TreeViewItem tvi = AddItemToTreeView(cic);
			tvi.BringIntoView();
			tvi.IsSelected = true;
		}

		private void NewSlaveLordItem_Click(object sender, RoutedEventArgs e)
		{
			SlotType slot = SlotType.None;
			if (tvItems.SelectedItem != null)
			{
				slot = (SlotType)(tvItems.SelectedItem as TreeViewItem).Tag;
			}
			else
			{
				if (!GetSlot(SlotType.None, SlaveLordCrafting.SlaveLordItemContainer.DisallowedSlots, out slot)) return;
			}

			if (slot == SlotType.None) return;

			SlaveLordCrafting.SlaveLordItemContainer slic = new SlaveLordCrafting.SlaveLordItemContainer { Name = "<Custom Item>" };

			string baseitemname = null;
			switch (slot)
			{
				case SlotType.Feet:
				case SlotType.Wrist:
					baseitemname = "Shackles";
					break;

				case SlotType.Finger:
				case SlotType.Trinket:
					baseitemname = "Five Rings";
					break;

				case SlotType.Neck:
				case SlotType.Waist:
					baseitemname = "Chains";
					break;
			}
			slic.BaseItem = DatasetManager.Dataset.Items.Find(i => i.Name == baseitemname && i.Slot == slot);

			CustomItemsManager.CustomItems.Add(slic);
			TreeViewItem tvi = AddItemToTreeView(slic);
			tvi.BringIntoView();
			tvi.IsSelected = true;
		}

		private void NewLGSItem_Click(object sender, RoutedEventArgs e)
		{
			SlotType slot = SlotType.None;
			if (tvItems.SelectedItem != null)
			{
				slot = (SlotType)(tvItems.SelectedItem as TreeViewItem).Tag;
			}
			else
			{
				if (!GetSlot(SlotType.None, LGSCrafting.LGSItemContainer.DisallowedSlots, out slot)) return;
			}

			if (slot == SlotType.None) return;

			LGSCrafting.LGSItemContainer lic = new LGSCrafting.LGSItemContainer { Name = "<Custom Item>" };

			lic.BaseItem = DatasetManager.Dataset.Items.Find(i => i.Name.StartsWith("Legendary Green Steel") && i.Slot == slot);

			CustomItemsManager.CustomItems.Add(lic);
			TreeViewItem tvi = AddItemToTreeView(lic);
			tvi.BringIntoView();
			tvi.IsSelected = true;
		}

		private void RenameCustomItem_Click(object sender, RoutedEventArgs e)
		{
			ACustomItemContainer cic = (tvItems.SelectedItem as TreeViewItem).Tag as ACustomItemContainer;
			TextBoxWindow tbw = new TextBoxWindow();
			tbw.Owner = this;
			tbw.Setup("Input name", cic.Name);
			if (tbw.ShowDialog() == true)
			{
				string name = tbw.Text.Replace('"', '\'').Replace('{', '(').Replace('}', ')').Replace('\\', '/').Replace('|', '/');
				cic.Name = name;
				cic.GetItem().Name = name;
				(tvItems.SelectedItem as TreeViewItem).Header = name + " (" + cic.Source.ToString() + ")";
			}
		}

		public delegate void CustomItemChangedSlotDelegate(DDOItemData item);
		public event CustomItemChangedSlotDelegate CustomItemChangedSlot;
		private void ChangeSlot_Click(object sender, RoutedEventArgs e)
		{
			TreeViewItem tvi = tvItems.SelectedItem as TreeViewItem;
			ACustomItemContainer cic = tvi.Tag as ACustomItemContainer;
			DDOItemData item = cic.GetItem();
			SlotType slot = SlotType.None;
			if (!GetSlot(item.Slot, cic.GetDisallowedSlots(), out slot)) return;
			if (slot == item.Slot) return;
			tvi.IsSelected = false;
			(tvi.Parent as TreeViewItem).Items.Remove(tvi);
			if (ItemPropertiesArea.Child == CustomIP) RemoveSlotSpecificProperties(item);
			item.Slot = slot;
			if (ItemPropertiesArea.Child == CustomIP) AddSlotSpecificProperties(item);
			tvi = AddItemToTreeView(cic);
			tvi.IsSelected = true;
			tvi.BringIntoView();
			CustomItemChangedSlot?.Invoke(item);
		}

		public delegate void CustomItemDeletedDelegate(DDOItemData item);
		public event CustomItemDeletedDelegate CustomItemDeleted;
		private void DeleteCustomItem_Click(object sender, RoutedEventArgs e)
		{
			TreeViewItem tvi = tvItems.SelectedItem as TreeViewItem;
			ACustomItemContainer cic = tvi.Tag as ACustomItemContainer;
			if (MessageBox.Show("Are you sure you want to delete " + cic.Name + "?", "Confirm deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
			CustomItemsManager.CustomItems.Remove(cic);
			tvi.IsSelected = false;
			(tvi.Parent as TreeViewItem).Items.Remove(tvi);
			CustomItemDeleted?.Invoke(cic.GetItem());
		}

		public delegate void RefreshGearSetDelegate();
		public event RefreshGearSetDelegate RefreshGearSet;
		private void RefreshGearSet_Click(object sender, RoutedEventArgs e)
		{
			RefreshGearSet?.Invoke();
		}

		private void SaveCustomItems_Click(object sender, RoutedEventArgs e)
		{
			CustomItemsManager.Save();
		}

		public delegate void CustomItemsReloadedDelegate();
		public event CustomItemsReloadedDelegate CustomItemsReloaded;
		private void LoadCustomItems_Click(object sender, RoutedEventArgs e)
		{
			if (!CustomItemsManager.Load())
			{
				MessageBox.Show("There was an error loading custom items!", "Loading error", MessageBoxButton.OK, MessageBoxImage.Stop);
				return;
			}
			SlaveLordIP?.SetItem(null);
			CustomIP?.SetItem(null);
			tvItems.Items.Clear();
			SetupTreeView();
			CustomItemsReloaded?.Invoke();
		}

		private void OpenWikiURL_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(WikiURL);
		}
	}
}
