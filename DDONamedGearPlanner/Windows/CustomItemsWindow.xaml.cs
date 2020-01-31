using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

			CustomItemsManager.CustomItems.Sort((a, b) => string.Compare(a.Item.Name, b.Item.Name, true));

			foreach (var ci in CustomItemsManager.CustomItems)
				AddItemToTreeView(ci);

			tvItems.ContextMenu = tvItems.Resources["EmptyCM"] as ContextMenu;
		}

		void AddItemToTreeView(CustomItem ci)
		{
			TreeViewItem tvi = null;
			foreach (TreeViewItem i in tvItems.Items)
			{
				if ((SlotType)i.Tag == ci.Item.Slot)
				{
					tvi = i;
					break;
				}
			}
			if (tvi == null)
			{
				tvi = new TreeViewItem();
				TextBlock tb = new TextBlock { Text = ci.Item.Slot.ToString(), FontWeight = FontWeights.Bold };
				tvi.Header = tb;
				tvi.Tag = ci.Item.Slot;
				tvItems.Items.Add(tvi);
			}

			TreeViewItem tvii = new TreeViewItem();
			tvii.Header = ci.Item.Name + " (" + ci.Source.ToString() + ")";
			tvii.Tag = ci;
			tvi.Items.Add(tvii);
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
					lvDetails.SetItem(new BuildItem((tvi.Tag as CustomItem).Item, EquipmentSlotType.None));
				}
			}
		}

		public delegate void ItemDoubleClickedDelegate(CustomItem item);
		public event ItemDoubleClickedDelegate ItemDoubleClicked;

		private void Items_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if ((e.OriginalSource as Grid) != null) return;
			if (tvItems.SelectedItem == null) return;
			TreeViewItem tvi = tvItems.SelectedItem as TreeViewItem;
			if (tvi.HasItems) return;

			ItemDoubleClicked?.Invoke(tvi.Tag as CustomItem);
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

		private void NewCustomItem_Click(object sender, RoutedEventArgs e)
		{
			SlotType slot = SlotType.None;
			if (tvItems.SelectedItem != null)
			{
				slot = (SlotType)(tvItems.SelectedItem as TreeViewItem).Tag;
			}
			else
			{
				ComboBoxWindow cbw = new ComboBoxWindow();
				cbw.Owner = this;
				SlotType[] slots = ((SlotType[])Enum.GetValues(typeof(SlotType))).Where(s => s != SlotType.None).ToArray();
				cbw.Setup("Select slot", slots);
				if (cbw.ShowDialog() == true) slot = (SlotType)cbw.SelectedItem;
			}

			if (slot == SlotType.None) return;

			DDOItemData item = new DDOItemData { Name = "<Custom Item>", Slot = slot };
			item.AddProperty("Minimum Level", null, 1, null);
			if (item.Slot == SlotType.Weapon) item.AddProperty("Handedness", null, 1, null);
			CustomItem ci = new CustomItem { Item = item, Source = CustomItemSource.Custom };
			CustomItemsManager.CustomItems.Add(ci);
			AddItemToTreeView(ci);
		}

		private void RenameCustomItem_Click(object sender, RoutedEventArgs e)
		{
			CustomItem ci = (tvItems.SelectedItem as TreeViewItem).Tag as CustomItem;
			TextBoxWindow tbw = new TextBoxWindow();
			tbw.Owner = this;
			tbw.Setup("Input name", ci.Item.Name);
			if (tbw.ShowDialog() == true)
			{
				string name = tbw.Text.Replace('"', '\'').Replace('{', '(').Replace('}', ')').Replace('\\', '/').Replace('|', '/');
				ci.Item.Name = name;
				(tvItems.SelectedItem as TreeViewItem).Header = name;
				//TODO: invoke the custom item changed event
			}
		}

		private void ChangeSlot_Click(object sender, RoutedEventArgs e)
		{

		}

		private void DeleteCustomItem_Click(object sender, RoutedEventArgs e)
		{
			TreeViewItem tvi = tvItems.SelectedItem as TreeViewItem;
			CustomItem ci = tvi.Tag as CustomItem;
			if (MessageBox.Show("Are you sure you want to delete " + ci.Item.Name + "?", "Confirm deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
			CustomItemsManager.CustomItems.Remove(ci);
			tvi.IsSelected = false;
			(tvi.Parent as TreeViewItem).Items.Remove(tvi);
		}
	}
}
