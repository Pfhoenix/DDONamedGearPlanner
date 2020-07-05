using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DDONamedGearPlanner
{
    /// <summary>
    /// Interaction logic for FilterTestResultsWindow.xaml
    /// </summary>
    public partial class FilterTestResultsWindow : Window
    {
		GearSetBuild Build;

        public FilterTestResultsWindow()
        {
            InitializeComponent();
        }

		bool BuildFilterProperty(TreeViewItem tvi, DDOItemData item, ItemProperty p, SlotType slot)
		{
			string prefix = "(" + (slot == SlotType.None ? "gear set filter" : slot.ToString() + " filter") + ") ";
			bool added = false;
			foreach (BuildFilter bf in Build.TestFilters[slot])
			{
				if (bf.Property == p.Property)
				{
					if (bf.Type == null || bf.Type == p.Type || (bf.Type == "untyped" && string.IsNullOrWhiteSpace(p.Type)))
					{
						if (bf.Include)
						{
							TreeViewItem tvip = new TreeViewItem();
							tvip.Header = prefix + p.Property + ", " + (string.IsNullOrWhiteSpace(p.Type) ? "untyped" : p.Type) + ", " + p.Value;
							tvip.Tag = item;
							tvi.Items.Add(tvip);
							added = true;
						}
					}
				}
				else if (p.Options != null && !p.HideOptions)
				{
					foreach (var op in p.Options)
					{
						if (bf.Property == op.Property)
						{
							if (bf.Type == null || bf.Type == op.Type || (bf.Type == "untyped" && string.IsNullOrWhiteSpace(op.Type)))
							{
								if (bf.Include)
								{
									TreeViewItem tvip = new TreeViewItem();
									tvip.Header = "> " + prefix + op.Property + ", " + (string.IsNullOrWhiteSpace(op.Type) ? "untyped" : op.Type) + ", " + op.Value;
									tvip.Tag = item;
									tvi.Items.Add(tvip);
									added = true;
								}
							}
						}
					}
				}
			}

			return added;
		}

		public void Initialize(GearSetBuild build)
		{
			Build = build;

			foreach (var kv in Build.FilterTestItems)
			{
				//kv.Value.Sort((a, b) => a.ML < b.ML ? -1 : (a.ML > b.ML ? 1 : string.Compare(a.Name, b.Name, true)));
				kv.Value.Sort((a, b) => string.Compare(a.Name, b.Name, true));
				TreeViewItem tvi = new TreeViewItem();
				TextBlock tb = new TextBlock { Text = kv.Key.ToString(), FontWeight = FontWeights.Bold };
				tvi.Header = tb;
				tvi.Tag = kv.Key;
				tvSlots.Items.Add(tvi);
				foreach (var item in kv.Value)
				{
					TreeViewItem tvii = new TreeViewItem();
					tvii.Header = item.Name;
					tvii.Tag = item;
					foreach (var p in item.Properties)
					{
						BuildFilterProperty(tvii, item, p, kv.Key.ToSlotType());
						BuildFilterProperty(tvii, item, p, SlotType.None);
					}
					if (tvii.HasItems) tvi.Items.Add(tvii);
				}
			}
		}

		private void Item_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeViewItem tvi = tvSlots.SelectedItem as TreeViewItem;
			if (tvi == null || tvi.Tag == null)
			{
				lvItem.SetItem(null);
				return;
			}
			DDOItemData item = tvi.Tag as DDOItemData;
			if (item != null)
			{
				EquipmentSlotType est;
				if (tvi.HasItems) est = (EquipmentSlotType)(tvi.Parent as TreeViewItem).Tag;
				else est = (EquipmentSlotType)((tvi.Parent as TreeViewItem).Parent as TreeViewItem).Tag;
				lvItem.SetItem(new BuildItem(tvi.Tag as DDOItemData, est));
			}
			else lvItem.SetItem(null);
		}

		private void Item_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			TreeViewItem tvi = tvSlots.SelectedItem as TreeViewItem;
			if (tvi == null || tvi.Tag == null) return;
			DDOItemData item = tvi.Tag as DDOItemData;
			if (item == null) return;
			System.Diagnostics.Process.Start(item.WikiURL);
		}

		private void Item_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			TreeViewItem tvi = e.Source as TreeViewItem;
			if (tvi == null)
			{
				if (tvSlots.SelectedItem != null) (tvSlots.SelectedItem as TreeViewItem).IsSelected = false;
			}
			else tvi.IsSelected = true;
		}

		private void Item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			TreeViewItem tvi = tvSlots.SelectedItem as TreeViewItem;
			if (tvi == null || tvi.HasItems || tvi.Tag == null) return;
			DDOItemData item = tvi.Tag as DDOItemData;
			if (item == null) return;

			EquipmentSlotType est;
			if (tvi.HasItems) est = (EquipmentSlotType)(tvi.Parent as TreeViewItem).Tag;
			else est = (EquipmentSlotType)((tvi.Parent as TreeViewItem).Parent as TreeViewItem).Tag;
			((PlannerWindow)(Owner.Owner)).SlotItem(new BuildItem(item, est));
			((PlannerWindow)(Owner.Owner)).CalculateGearSet(true);
		}
	}
}
