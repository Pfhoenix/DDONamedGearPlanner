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
		public CustomItemsWindow()
		{
			InitializeComponent();

			tvItems.ContextMenu = tvItems.Resources["EmptyCM"] as ContextMenu;

			SetupTreeView();
		}

		TreeViewItem AddItemToTreeView(CustomItemContainer cic)
		{
			TreeViewItem tvi = null;
			foreach (TreeViewItem i in tvItems.Items)
			{
				if ((SlotType)i.Tag == cic.Item.Slot)
				{
					tvi = i;
					break;
				}
			}
			if (tvi == null)
			{
				tvi = new TreeViewItem();
				TextBlock tb = new TextBlock { Text = cic.Item.Slot.ToString(), FontWeight = FontWeights.Bold };
				tvi.Header = tb;
				tvi.Tag = cic.Item.Slot;
				tvItems.Items.Add(tvi);
			}

			TreeViewItem tvii = new TreeViewItem();
			tvii.Header = cic.Item.Name + " (" + cic.Item.Source.ToString() + ")";
			tvii.Tag = cic;
			tvi.Items.Add(tvii);

			return tvii;
		}

		void SetupTreeView()
		{
			List<CustomItemContainer> customitems = CustomItemsManager.GetItemsFromSource<CustomItemContainer>(ItemDataSource.Custom);
			customitems.Sort((a, b) => string.Compare(a.Name, b.Name, true));

			foreach (var ci in customitems)
				AddItemToTreeView(ci);
		}

		private void Items_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			lvDetails.SetItem(null);
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
					(tvItems.ContextMenu.Items[0] as MenuItem).Header = "New Custom " + (SlotType)tvi.Tag + " Item";
				}
				else
				{
					tvItems.ContextMenu = tvItems.Resources["ItemCM"] as ContextMenu;
					lvDetails.SetItem((tvi.Tag as CustomItemContainer).Item);
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

			ItemDoubleClicked?.Invoke((tvi.Tag as CustomItemContainer).Item);
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

		bool GetSlot(SlotType initial, out SlotType rval)
		{
			rval = SlotType.None;
			ComboBoxWindow cbw = new ComboBoxWindow();
			cbw.Owner = this;
			SlotType[] slots = ((SlotType[])Enum.GetValues(typeof(SlotType))).Where(s => s != SlotType.None).ToArray();
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
				if (!GetSlot(SlotType.None, out slot)) return;
			}

			if (slot == SlotType.None) return;

			CustomItemContainer cic = new CustomItemContainer();
			cic.Item = new DDOItemData(ItemDataSource.Custom, false) { Name = "<Custom Item>", Slot = slot };
			cic.Item.AddProperty("Minimum Level", null, 1, null);
			AddSlotSpecificProperties(cic.Item);

			CustomItemsManager.CustomItems.Add(cic);
			TreeViewItem tvi = AddItemToTreeView(cic);
			tvi.BringIntoView();
			tvi.IsSelected = true;
		}

		private void RenameCustomItem_Click(object sender, RoutedEventArgs e)
		{
			CustomItemContainer cic = (tvItems.SelectedItem as TreeViewItem).Tag as CustomItemContainer;
			TextBoxWindow tbw = new TextBoxWindow();
			tbw.Owner = this;
			tbw.Setup("Input name", cic.Name);
			if (tbw.ShowDialog() == true)
			{
				string name = tbw.Text.Replace('"', '\'').Replace('{', '(').Replace('}', ')').Replace('\\', '/').Replace('|', '/');
				cic.Name = name;
				cic.Item.Name = name;
				(tvItems.SelectedItem as TreeViewItem).Header = name;
			}
		}

		public delegate void CustomItemChangedSlotDelegate(DDOItemData item);
		public event CustomItemChangedSlotDelegate CustomItemChangedSlot;
		private void ChangeSlot_Click(object sender, RoutedEventArgs e)
		{
			TreeViewItem tvi = tvItems.SelectedItem as TreeViewItem;
			CustomItemContainer cic = tvi.Tag as CustomItemContainer;
			SlotType slot = SlotType.None;
			if (!GetSlot(cic.Item.Slot, out slot)) return;
			if (slot == cic.Item.Slot) return;
			tvi.IsSelected = false;
			(tvi.Parent as TreeViewItem).Items.Remove(tvi);
			RemoveSlotSpecificProperties(cic.Item);
			cic.Item.Slot = slot;
			AddSlotSpecificProperties(cic.Item);
			tvi = AddItemToTreeView(cic);
			tvi.IsSelected = true;
			tvi.BringIntoView();
			CustomItemChangedSlot?.Invoke(cic.Item);
		}

		public delegate void CustomItemDeletedDelegate(DDOItemData item);
		public event CustomItemDeletedDelegate CustomItemDeleted;
		private void DeleteCustomItem_Click(object sender, RoutedEventArgs e)
		{
			TreeViewItem tvi = tvItems.SelectedItem as TreeViewItem;
			CustomItemContainer cic = tvi.Tag as CustomItemContainer;
			if (MessageBox.Show("Are you sure you want to delete " + cic.Name + "?", "Confirm deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
			CustomItemsManager.CustomItems.Remove(cic);
			tvi.IsSelected = false;
			(tvi.Parent as TreeViewItem).Items.Remove(tvi);
			CustomItemDeleted?.Invoke(cic.Item);
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
			lvDetails.SetItem(null);
			tvItems.Items.Clear();
			SetupTreeView();
			CustomItemsReloaded?.Invoke();
		}
	}
}
