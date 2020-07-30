using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	/// Interaction logic for GearSetComparisonWindow.xaml
	/// </summary>
	public partial class GearSetComparisonWindow : Window
	{
		readonly string[] PropertiesToIgnore =
		{
			"Armor Category",
			"Augment Slot",
			"Feat",
			"Weapon Category",
			"Weapon Type"
		};

		GearSet LeftGS, RightGS;
		ScrollViewer LeftSV, RightSV;
		PlannerWindow OwnerPW;

		List<BuildItem> SavedItems;

		public GearSetComparisonWindow(PlannerWindow opw)
		{
			InitializeComponent();

			Owner = opw;
			OwnerPW = opw;

			//RightSV = (VisualTreeHelper.GetChild(lvRight, 0) as Decorator).Child as ScrollViewer;
			//LeftSV = (VisualTreeHelper.GetChild(lvLeft, 0) as Decorator).Child as ScrollViewer;
		}

		void LoadGearsetFromPlanner(bool left)
		{
			GearSet gs = OwnerPW.CalculateGearSet(false);
			if (left) LeftGS = gs;
			else RightGS = gs;

			CompareGearsets();
		}

		void SaveEquippedItems()
		{
			SavedItems = OwnerPW.GetEquippedItems();
		}

		void RestoreEquippedItems()
		{
			OwnerPW.UnlockClearAll(null, null);

			foreach (var bi in SavedItems)
				OwnerPW.SlotItem(bi, true);

			OwnerPW.CalculateGearSet(false);
		}

		void LoadGearsetFromFile(bool left)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Gear Set file (*.gearset)|*.gearset";
			if (ofd.ShowDialog() == true)
			{
				SaveEquippedItems();
				GearSet gs = OwnerPW.UnserializeGearset(File.ReadAllText(ofd.FileName), false, true);
				RestoreEquippedItems();
				if (gs == null)
				{
					MessageBox.Show("Invalid file format!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				if (left) LeftGS = gs;
				else RightGS = gs;

				CompareGearsets();
			}
		}

		void LoadGearsetFromClipboard(bool left)
		{
			SaveEquippedItems();
			GearSet gs = OwnerPW.UnserializeGearset(Clipboard.GetData(DataFormats.Text).ToString(), false, true);
			RestoreEquippedItems();
			if (gs == null)
			{
				MessageBox.Show("No valid gearset data in the clipboard!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (left) LeftGS = gs;
			else RightGS = gs;

			CompareGearsets();
		}

		void ClearGearset(bool left)
		{
			if (left) LeftGS = null;
			else RightGS = null;

			CompareGearsets();
		}

		private void LoadGearset_Clicked(object sender, RoutedEventArgs e)
		{
			LoadGearsetFromPlanner(sender == btnGSLeft);
		}

		private void LoadFile_Clicked(object sender, RoutedEventArgs e)
		{
			LoadGearsetFromFile(sender == btnFileLeft);
		}

		private void LoadClipboard_Clicked(object sender, RoutedEventArgs e)
		{
			LoadGearsetFromClipboard(sender == btnClipLeft);
		}

		private void ClearGearset_Clicked(object sender, RoutedEventArgs e)
		{
			ClearGearset(sender == btnClearLeft);
		}

		public class CompareListItem
		{
			public string Text { get; set; }
			public BuildItem BI { get; set; }
			public GearSetProperty Prop { get; set; }
			public Color BackgroundColor { get; set; } = Colors.Transparent;

			public bool HasTooltip => Prop != null ? true : (BI != null ? (BI.OptionProperties != null && BI.OptionProperties.Count > 0) : false);
		}

		void AddItemToList(BuildItem bi, ListView lv)
		{
			lv.Items.Add(new CompareListItem { Text = bi.ToString(), BI = bi });
		}

		void AddTextToList(string t, Color c, ListView lv)
		{
			lv.Items.Add(new CompareListItem { Text = t, BackgroundColor = c });
		}

		void AddPropertyToList(GearSetProperty p, Color c, ListView lv)
		{
			lv.Items.Add(new CompareListItem { Text = p.Property + (p.TotalValue != 0 ? " " + p.TotalValue : ""), Prop = p, BackgroundColor = c });
		}

		bool IsPropertyAllowed(GearSetProperty p)
		{
			if (PropertiesToIgnore.Contains(p.Property)) return false;
			if (p.ItemProperties[0].Type == "set") return false;

			return true;
		}

		bool ignoreLeftScroll, ignoreRightScroll;

		private void Scroll_Scrolled(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
		{
			ignoreLeftScroll = ignoreRightScroll = true;
			LeftSV.ScrollToVerticalOffset(e.NewValue);
			RightSV.ScrollToVerticalOffset(e.NewValue);
		}

		private void Scroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (ignoreRightScroll || ignoreLeftScroll) return;

			ignoreLeftScroll = ignoreRightScroll = true;
			LeftSV.ScrollToVerticalOffset(e.NewValue);
			RightSV.ScrollToVerticalOffset(e.NewValue);
		}

		private void LV_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (LeftSV == null) LeftSV = (VisualTreeHelper.GetChild(lvLeft, 0) as Decorator).Child as ScrollViewer;
			if (RightSV == null) RightSV = (VisualTreeHelper.GetChild(lvRight, 0) as Decorator).Child as ScrollViewer;

			if (sender == lvLeft)
			{
				if (ignoreLeftScroll) ignoreLeftScroll = false;
				else
				{
					ignoreRightScroll = true;
					RightSV.ScrollToVerticalOffset(e.VerticalOffset);
					sbScroll.Maximum = e.ExtentHeight - e.ViewportHeight;
					sbScroll.Value = e.VerticalOffset;
				}
			}
			else if (sender == lvRight)
			{
				if (ignoreRightScroll) ignoreRightScroll = false;
				else
				{
					ignoreLeftScroll = true;
					LeftSV.ScrollToVerticalOffset(e.VerticalOffset);
					sbScroll.Maximum = e.ExtentHeight - e.ViewportHeight;
					sbScroll.Value = e.VerticalOffset;
				}
			}
		}

		private void ItemToolTip_Opening(object sender, ToolTipEventArgs e)
		{
			ListViewItem lvi = sender as ListViewItem;
			CompareListItem cli = lvi.Content as CompareListItem;
			ToolTip tt = lvi.ToolTip as ToolTip;

			StackPanel sp = new StackPanel { Orientation = Orientation.Vertical };
			tt.Content = sp;

			if (cli.Prop != null)
			{
				string lasttype = null;
				foreach (var p in cli.Prop.ItemProperties)
				{
					string source = p.Owner?.Name;
					if (source == null)
					{
						source = p.SetBonusOwner + " set";
					}
					TextBlock tb = new TextBlock();
					string l = null;
					if (cli.Prop.Property == "Damage Reduction") l = ((int)p.Value).ToString() + "/" + p.Type;
					else if (p.Type == "set") l = p.Owner.Name;
					else
					{
						if (string.IsNullOrWhiteSpace(p.Type) && p.Value == 0) l = source;
						else
						{
							l += (string.IsNullOrWhiteSpace(p.Type) ? "untyped" : p.Type) + " ";
							l += p.Value + " (" + source + ")";
						}
						if (!cli.Prop.IsGroup && !string.IsNullOrWhiteSpace(lasttype) && p.Type == lasttype)
						{
							tb.Foreground = Brushes.Red;
						}
						lasttype = p.Type;
					}
					tb.Text = l;
					sp.Children.Add(tb);
				}
			}
			else if (cli.BI != null)
			{
				foreach (var op in cli.BI.OptionProperties)
				{
					TextBlock tb = new TextBlock();
					string t = op.ToString();
					tb.Text = t.Substring(1, t.Length - 2);
					sp.Children.Add(tb);
				}
			}
		}

		void CompareGearsets()
		{
			lvLeft.Items.Clear();
			lvRight.Items.Clear();

			if (LeftGS != null) LeftGS.Items.Sort((a, b) => a.Slot < b.Slot ? -1 : (a.Slot > b.Slot ? 1 : 0));
			if (RightGS != null) RightGS.Items.Sort((a, b) => a.Slot < b.Slot ? -1 : (a.Slot > b.Slot ? 1 : 0));

			int r = 0, l = 0;
			int re = RightGS?.Items.Count ?? 0;
			int le = LeftGS?.Items.Count ?? 0;
			while (r < re || l < le)
			{
				// if r >= re then proceed l only
				if (r >= re)
				{
					// we can assume l < le
					if (re > 0) AddTextToList("-", Colors.Transparent, lvRight);
					AddItemToList(LeftGS.Items[l++], lvLeft);
				}
				// if l >= le then proceed r only
				else if (l >= le)
				{
					// we can assume r < re
					if (le > 0) AddTextToList("-", Colors.Transparent, lvLeft);
					AddItemToList(RightGS.Items[r++], lvRight);
				}
				else
				{
					if (RightGS.Items[r].Slot < LeftGS.Items[l].Slot)
					{
						AddItemToList(RightGS.Items[r++], lvRight);
						AddTextToList("-", Colors.Transparent, lvLeft);
					}
					else if (RightGS.Items[r].Slot > LeftGS.Items[l].Slot)
					{
						AddItemToList(LeftGS.Items[l++], lvLeft);
						AddTextToList("-", Colors.Transparent, lvRight);
					}
					else
					{
						AddItemToList(RightGS.Items[r++], lvRight);
						AddItemToList(LeftGS.Items[l++], lvLeft);
					}
				}
			}

			AddTextToList("", Colors.Transparent, lvRight);
			AddTextToList("", Colors.Transparent, lvLeft);

			r = l = 0;
			re = RightGS?.Properties.Count ?? 0;
			le = LeftGS?.Properties.Count ?? 0;
			while (r < re || l < le)
			{
				// if r >= re then proceed l only
				if (r >= re)
				{
					if (!IsPropertyAllowed(LeftGS.Properties[l]))
					{
						l++;
						continue;
					}
					// we can assume l < le
					if (re > 0) AddTextToList("", Colors.Transparent, lvRight);
					AddPropertyToList(LeftGS.Properties[l++], Colors.LightGreen, lvLeft);
				}
				// if l >= le then proceed r only
				else if (l >= le)
				{
					if (!IsPropertyAllowed(RightGS.Properties[r]))
					{
						r++;
						continue;
					}
					// we can assume r < re
					if (le > 0) AddTextToList("", Colors.Transparent, lvLeft);
					AddPropertyToList(RightGS.Properties[r++], Colors.LightGreen, lvRight);
				}
				else
				{
					int sc = string.Compare(RightGS.Properties[r].Property, LeftGS.Properties[l].Property);

					if (sc < 0)
					{
						if (!IsPropertyAllowed(RightGS.Properties[r]))
						{
							r++;
							continue;
						}

						AddTextToList("", Colors.Transparent, lvLeft);
						AddPropertyToList(RightGS.Properties[r++], Colors.LightGreen, lvRight);
					}
					else if (sc > 0)
					{
						if (!IsPropertyAllowed(LeftGS.Properties[l]))
						{
							l++;
							continue;
						}

						AddTextToList("", Colors.Transparent, lvRight);
						AddPropertyToList(LeftGS.Properties[l++], Colors.LightGreen, lvLeft);
					}
					else
					{
						if (!IsPropertyAllowed(RightGS.Properties[r]))
						{
							r++;
							l++;
							continue;
						}

						Color lc = Colors.Transparent, rc = Colors.Transparent;
						if (RightGS.Properties[r].TotalValue < LeftGS.Properties[l].TotalValue)
						{
							rc = Colors.LightSalmon;
							lc = Colors.LightGreen;
						}
						else if (RightGS.Properties[r].TotalValue > LeftGS.Properties[l].TotalValue)
						{
							rc = Colors.LightGreen;
							lc = Colors.LightSalmon;
						}

						AddPropertyToList(RightGS.Properties[r++], rc, lvRight);
						AddPropertyToList(LeftGS.Properties[l++], lc, lvLeft);
					}
				}
			}
		}

		static ListViewItem VisualUpwardSearch(DependencyObject source)
		{
			while (source != null && !(source is ListViewItem))
				source = VisualTreeHelper.GetParent(source);

			return source as ListViewItem;
		}

		private void LV_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			ListViewItem item = VisualUpwardSearch(e.OriginalSource as DependencyObject);
			CompareListItem cli = item?.Content as CompareListItem;

			if (cli != null)
			{
				if (cli.BI != null)
				{
					System.Diagnostics.Process.Start(cli.BI.Item.WikiURL);
				}
				e.Handled = true;
			}
		}
	}
}
