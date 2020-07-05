using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DDONamedGearPlanner
{
    /// <summary>
    /// Interaction logic for ItemPropertyReportWindow.xaml
    /// </summary>
    public partial class ItemPropertyBrowserWindow : Window
    {
        public ItemPropertyBrowserWindow()
        {
            InitializeComponent();
			Initialize();
        }

		bool ProcessPropertyForSlots(string type, List<string> types, bool first, TextBlock tb)
		{
			if (string.IsNullOrWhiteSpace(type)) type = "untyped";
			if ((type == "untyped" && types.Contains("")) || types.Contains(type))
			{
				if (type == "untyped") types.Remove("");
				else types.Remove(type);
				if (first) first = false;
				else tb.Inlines.Add(", ");
				tb.Inlines.Add(type);
			}

			return first;
		}

		void Initialize()
		{
			List<DDOItemProperty> ips = DatasetManager.Dataset.ItemProperties.Select(ip => ip.Value).OrderBy(ip => ip.Property).ToList();

			// populate by property treeview
			foreach (var ip in ips)
			{
				TreeViewItem tvi = new TreeViewItem();
				TextBlock header = new TextBlock();
				header.Inlines.Add(new Run { Text = ip.Property, FontWeight = FontWeights.Bold });
				if (ip.Items.Count == 0)
				{
					header.Inlines.Add(" (set bonus only)");
				}
				else
				{
					header.Inlines.Add(" (" + ip.SlotsFoundOn + ")");

					tvi.Tag = ip;
					foreach (var ipt in ip.Types)
					{
						string type = ipt;
						if (string.IsNullOrWhiteSpace(ipt)) type = "untyped";
						TreeViewItem tvii = new TreeViewItem();
						SlotType st = SlotType.None;
						bool itemnotallowed = false;
						foreach (var item in ip.Items)
						{
							if (!QuestSourceManager.IsItemAllowed(item))
							{
								itemnotallowed = true;
								continue;
							}
							if (item.Properties.Find(p => p.Property == ip.Property && (p.Type == type || (type == "untyped" && string.IsNullOrWhiteSpace(p.Type)))) != null) st |= item.Slot;
						}
						if (st != SlotType.None)
						{
							tvii.Header = type + " (" + st + ")";
							tvii.Tag = ip;
							tvi.Items.Add(tvii);
						}
						else if (!itemnotallowed)
						{
							tvii.Header = type + " (set bonus only)";
							tvi.Items.Add(tvii);
						}
					}
				}
				if (tvi.HasItems)
				{
					tvi.Header = header;
					tvByProperty.Items.Add(tvi);
				}
			}

			// populate by slot treeview
			// setup dictionary for quicker processing
			string[] slotnames = Enum.GetNames(typeof(SlotType));
			Dictionary<SlotType, TreeViewItem> slots = new Dictionary<SlotType, TreeViewItem>();
			foreach (string s in slotnames)
			{
				if (s == "None") continue;
				TreeViewItem tvi = new TreeViewItem();
				TextBlock tb = new TextBlock();
				tb.Inlines.Add(new Run { Text = s, FontWeight = FontWeights.Bold });
				tvi.Header = tb;
				tvBySlot.Items.Add(tvi);
				SlotType st = (SlotType)Enum.Parse(typeof(SlotType), s);
				slots.Add(st, tvi);
				tvi.Tag = st;
			}

			foreach (var ip in ips)
			{
				Dictionary<SlotType, TreeViewItem> ipslots = new Dictionary<SlotType, TreeViewItem>();
				List<string> types;
				TreeViewItem tvi;
				TextBlock tb;
				foreach (var item in ip.Items)
				{
					if (!QuestSourceManager.IsItemAllowed(item)) continue;

					bool first;
					if (ipslots.ContainsKey(item.Slot))
					{
						tvi = ipslots[item.Slot];
						tb = tvi.Header as TextBlock;
						types = tvi.Tag as List<string>;
						first = false;
					}
					else
					{
						tvi = new TreeViewItem();
						tb = new TextBlock();
						tb.Inlines.Add(new Run { Text = ip.Property, FontWeight = FontWeights.Bold });
						tb.Inlines.Add(" (");
						tvi.Header = tb;
						types = new List<string>(ip.Types);
						tvi.Tag = types;
						slots[item.Slot].Items.Add(tvi);
						ipslots[item.Slot] = tvi;
						first = true;
					}

					foreach (var p in item.Properties)
					{
						if (p.Options != null && !p.HideOptions)
						{
							foreach (var op in p.Options)
							{
								if (op.Property == ip.Property)
									first = ProcessPropertyForSlots(op.Type, types, first, tb);
							}
						}
						else if (p.Property == ip.Property)
							first = ProcessPropertyForSlots(p.Type, types, first, tb);
					}
				}

				// add ")" to the end of each treeviewitem
				foreach (var ipsl in ipslots)
				{
					(ipsl.Value.Header as TextBlock).Inlines.Add(")");
					ipsl.Value.Tag = ip;
				}
			}
		}

		void PopulateItemListForProperty(DDOItemProperty ip, SlotType slot)
		{
			foreach (var item in ip.Items)
			{
				if (!QuestSourceManager.IsItemAllowed(item)) continue;
				if (slot != SlotType.None && item.Slot != slot) continue;

				ListBoxItem lbi = new ListBoxItem();
				TextBlock tb = new TextBlock();
				lbi.Content = tb;
				lbi.Tag = item;
				tb.Inlines.Add(new Run { Text = item.Name, FontWeight = FontWeights.Bold });
				tb.Inlines.Add(" (" + item.Slot + ") (");
				bool first = true;
				foreach (var p in item.Properties)
				{
					if (p.Options != null && !p.HideOptions)
					{
						foreach (var op in p.Options)
						{
							if (op.Property == ip.Property)
							{
								if (first) first = false;
								else tb.Inlines.Add(", ");
								tb.Inlines.Add(string.IsNullOrWhiteSpace(op.Type) ? "untyped" : op.Type);
								tb.Inlines.Add(" " + op.Value);
							}
						}
					}
					else if (p.Property == ip.Property)
					{
						if (first) first = false;
						else tb.Inlines.Add(", ");
						tb.Inlines.Add(string.IsNullOrWhiteSpace(p.Type) ? "untyped" : p.Type);
						tb.Inlines.Add(" " + p.Value);
					}
				}
				tb.Inlines.Add(")");
				lbItems.Items.Add(lbi);
			}
		}

		private void ByPropertySelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (lbItems == null) return;
			lbItems.Items.Clear();
			TreeViewItem selected = tvByProperty.SelectedItem as TreeViewItem;
			if (selected?.Tag == null) return;
			DDOItemProperty ip = selected.Tag as DDOItemProperty;
			if (selected.HasItems)
			{
				PopulateItemListForProperty(ip, SlotType.None);
			}
			else
			{
				string type = selected.Header.ToString();
				type = type.Substring(0, type.IndexOf(" ("));

				foreach (var item in ip.Items)
				{
					ListBoxItem lbi = new ListBoxItem();
					TextBlock tb = new TextBlock();
					lbi.Content = tb;
					lbi.Tag = item;
					tb.Inlines.Add(new Run { Text = item.Name, FontWeight = FontWeights.Bold });
					tb.Inlines.Add(" (" + item.Slot + ") (");
					bool first = true;
					foreach (var p in item.Properties)
					{
						if (p.Options != null && !p.HideOptions)
						{
							foreach (var op in p.Options)
							{
								if (op.Property == ip.Property && (op.Type == type || (type == "untyped" && string.IsNullOrWhiteSpace(op.Type))))
								{
									if (first) first = false;
									else tb.Inlines.Add(", ");
									tb.Inlines.Add(string.IsNullOrWhiteSpace(op.Type) ? "untyped" : op.Type);
									tb.Inlines.Add(" " + op.Value);
								}
							}
						}
						else if (p.Property == ip.Property && (p.Type == type || (type == "untyped" && string.IsNullOrWhiteSpace(p.Type))))
						{
							if (first) first = false;
							else tb.Inlines.Add(", ");
							tb.Inlines.Add(string.IsNullOrWhiteSpace(p.Type) ? "untyped" : p.Type);
							tb.Inlines.Add(" " + p.Value);
						}
					}
					if (!first)
					{
						tb.Inlines.Add(")");
						lbItems.Items.Add(lbi);
					}
				}
			}
		}

		private void BySlotSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			lbItems.Items.Clear();
			if (tvBySlot?.SelectedItem == null) return;
			TreeViewItem tvi = tvBySlot.SelectedItem as TreeViewItem;
			if (tvi.HasItems) return;
			SlotType slot = (SlotType)(tvi.Parent as TreeViewItem).Tag;
			PopulateItemListForProperty(tvi.Tag as DDOItemProperty, slot);
		}

		private void ItemListMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (lbItems.SelectedItem == null) return;
			DDOItemData item = (lbItems.SelectedItem as ListBoxItem).Tag as DDOItemData;
			System.Diagnostics.Process.Start(item.WikiURL);
		}

		public delegate void ItemDoubleClickedDelegate(DDOItemData item);
		public event ItemDoubleClickedDelegate ItemDoubleClicked;

		private void ItemListMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (lbItems.SelectedItem == null) return;
			DDOItemData item = (lbItems.SelectedItem as ListBoxItem).Tag as DDOItemData;
			ItemDoubleClicked?.Invoke(item);
		}

		private void SelectedTabChanged(object sender, SelectionChangedEventArgs e)
		{
			if ((sender as TabControl).SelectedItem == tiByProperty)
			{
				ByPropertySelectionChanged(null, null);
			}
			else
			{
				BySlotSelectionChanged(null, null);
			}
		}
	}
}
