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
	class NamedSetInfo
	{
		public string Name { get; set; }
		public int ML { get; set; }
		public string WikiURL { get; set; }
		public DDOItemSet Set;
	}

	/// <summary>
	/// Interaction logic for NamedSetSelectorWindow.xaml
	/// </summary>
	public partial class NamedSetSelectorWindow : Window
	{
		Dictionary<EquipmentSlotType, EquipmentSlotControl> EquipmentSlots;
		int FingerLimit = 2;
		EquipmentSlotControl WeaponSlot;
		EquipmentSlotControl OffhandSlot;
		DDOItemData SelectedWeapon;
		DDOItemData SelectedOffhand;
		List<NamedSetInfo> Sets = new List<NamedSetInfo>();
		List<CheckBox> SelectedItems = new List<CheckBox>();

		public NamedSetSelectorWindow()
		{
			InitializeComponent();

			tbSearchName.Focus();
		}

		public void SetFilter(Predicate<object> filter)
		{
			if (lvSets.Items.CurrentItem != null && !filter(lvSets.Items.CurrentItem))
			{
				lvSets.SelectedItem = null;
			}
			lvSets.Items.Filter = filter;
			if (lvSets.Items.CurrentItem == null && !lvSets.Items.IsEmpty)
				lvSets.Items.MoveCurrentToFirst();
		}


		public void Initialize(Dictionary<EquipmentSlotType, EquipmentSlotControl> es)
		{
			FingerLimit = 2;
			EquipmentSlots = es;
			WeaponSlot = EquipmentSlots[EquipmentSlotType.Weapon];
			OffhandSlot = EquipmentSlots[EquipmentSlotType.Offhand];
			if (EquipmentSlots[EquipmentSlotType.Finger1].IsLocked) FingerLimit--;
			if (EquipmentSlots[EquipmentSlotType.Finger2].IsLocked) FingerLimit--;

			foreach (var set in DatasetManager.Dataset.Sets)
			{
				if (string.IsNullOrWhiteSpace(set.Value.WikiURL)) continue;
				int ml = 0;
				List<DDOAdventurePackData> apds = new List<DDOAdventurePackData>();
				foreach (var item in set.Value.Items)
				{
					if (item.QuestFoundIn.Adpack != null && !apds.Contains(item.QuestFoundIn.Adpack)) apds.Add(item.QuestFoundIn.Adpack);
					if (!QuestSourceManager.IsItemAllowed(item)) continue;
					var mlp = item.Properties.Find(i => i.Property == "Minimum Level");
					if (mlp != null && mlp.Value > ml)
						ml = (int)mlp.Value;
				}

				foreach (var apd in apds)
				{
					if (QuestSourceManager.IsAllowed(apd))
					{
						Sets.Add(new NamedSetInfo{ Name = set.Value.Name, ML = ml, WikiURL = set.Value.WikiURL, Set = set.Value });
						break;
					}
				}
			}

			Sets.Sort((a, b) => a.ML < b.ML ? -1 : (a.ML > b.ML ? 1 : a.Name.CompareTo(b.Name)));

			lvSets.ItemsSource = Sets;
			SetFilter(CustomFilter);
		}

		private bool CustomFilter(object obj)
		{
			if (string.IsNullOrWhiteSpace(tbSearchName.Text)) return true;
			else return (obj as NamedSetInfo).Name.IndexOf(tbSearchName.Text, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		bool MouseRightClicked;

		private void LvSets_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!MouseRightClicked) return;

			if (lvSets.SelectedItem != null) System.Diagnostics.Process.Start((lvSets.SelectedItem as NamedSetInfo).WikiURL);

			MouseRightClicked = false;
		}

		private void LvSets_MouseLeave(object sender, MouseEventArgs e)
		{
			MouseRightClicked = false;
		}

		private void LvSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SlotPanel.Children.Clear();
			SlotPanel.Tag = null;
			SelectedItems.Clear();
			SelectedWeapon = null;
			SelectedOffhand = null;
			btnApply.IsEnabled = false;
			if (lvSets.SelectedItem == null) return;
			DDOItemSet set = (lvSets.SelectedItem as NamedSetInfo).Set;
			SlotPanel.Tag = set;
			foreach (var item in set.Items)
			{
				// don't show items that aren't allowed
				if (!QuestSourceManager.IsItemAllowed(item)) continue;

				string group = item.Slot.ToString() + " Slot";
				bool ro = false;
				// search for an existing groupbox first
				GroupBox gb = null;
				StackPanel sp = null;
				foreach (GroupBox tgb in SlotPanel.Children)
				{
					if ((tgb.Header as Label).Content.ToString() == group)
					{
						gb = tgb;
						if ((gb.Header as Label).Foreground == Brushes.Red) ro = true;
						sp = gb.Content as StackPanel;
						break;
					}
				}
				if (gb == null)
				{
					gb = new GroupBox();
					Label header = new Label { Content = group };
					header.Content = group;
					gb.Header = header;
					sp = new StackPanel();
					gb.Content = sp;
					SlotPanel.Children.Add(gb);
					foreach (var eq in EquipmentSlots)
					{
						if (item.Slot == eq.Value.SlotType)
						{
							if (eq.Value.SlotType != SlotType.Finger && eq.Value.SlotType != SlotType.Weapon && eq.Value.IsLocked)
							{
								ro = true;
								break;
							}
						}
					}

					bool go = false;
					if (item.Slot == SlotType.Finger)
					{
						if (FingerLimit <= 0) ro = true;
						else if (FingerLimit == 1) go = true;
					}
					else if (item.Slot == SlotType.Weapon)
					{
						if (item.Handedness == 1)
						{
							if (WeaponSlot.IsLocked && OffhandSlot.IsLocked) ro = true;
							else if (WeaponSlot.IsLocked || OffhandSlot.IsLocked) go = true;
						}
						else if (item.Handedness == 2)
						{
							if (WeaponSlot.IsLocked) ro = true;
							else if (OffhandSlot.IsLocked)
							{
								if (OffhandSlot.Item == null) ro = true;
								else if (OffhandSlot.Item.Item.Slot == SlotType.Weapon) ro = true;
								else if ((OffhandCategory)OffhandSlot.Item.Item.Category != OffhandCategory.RuneArm) ro = true;
								else if (!DatasetManager.RuneArmCompatibleTwoHandedWeaponTypes.Contains(item.WeaponType)) ro = true;
								else go = true;
							}
						}
					}

					if (ro) header.Foreground = Brushes.Red;
					else if (go) header.Foreground = Brushes.Goldenrod;
				}
				
				CheckBox cb = new CheckBox { Content = item.Name, Tag = item };
				if (ro) cb.IsEnabled = false;
				else
				{
					// we do individual offhand evaluation here
					if (item.Slot == SlotType.Offhand)
					{
						if (WeaponSlot.IsLocked && WeaponSlot.Item != null && WeaponSlot.Item.Item.Handedness == 2)
						{
							if (!DatasetManager.RuneArmCompatibleTwoHandedWeaponTypes.Contains(WeaponSlot.Item.Item.WeaponType) || (OffhandCategory)item.Category != OffhandCategory.RuneArm) cb.IsEnabled = false;
							else cb.Click += ItemCheckBox_Clicked;
						}
						else cb.Click += ItemCheckBox_Clicked;
					}
					else cb.Click += ItemCheckBox_Clicked;
				}
				cb.ToolTip = item.Name;
				cb.MouseRightButtonDown += ItemCheckBox_MouseRightButtonDown;
				sp.Children.Add(cb);
			}

			tcSetBonuses.Items.Clear();
			foreach (var sb in set.SetBonuses)
			{
				TabItem ti = new TabItem { Header = sb.MinimumItems + " Pieces" };
				ListViewItemProperties ip = new ListViewItemProperties();
				ip.SetSetBonuses(sb);
				ti.Content = ip;
				ti.Tag = sb;

				tcSetBonuses.Items.Add(ti);
			}

			tcSetBonuses.SelectedIndex = 0;
		}

		private void ItemCheckBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			CheckBox cb = sender as CheckBox;
			System.Diagnostics.Process.Start((cb.Tag as DDOItemData).WikiURL);
		}

		private void ItemCheckBox_Clicked(object sender, RoutedEventArgs e)
		{
			CheckBox cb = sender as CheckBox;
			if (cb.IsChecked.Value)
			{
				DDOItemData si = cb.Tag as DDOItemData;
				DDOItemData ci;
				int fi = 0;
				for (int i = 0; i < SelectedItems.Count; i++)
				{
					ci = SelectedItems[i].Tag as DDOItemData;
					if (si.Slot == ci.Slot)
					{
						// finger slots don't get unchecked, since there could be two of them
						if (si.Slot == SlotType.Finger) fi++;
						// weapon slot doesn't get checked, since a weapon could be in the offhand
						else if (si.Slot != SlotType.Weapon)
						{
							SelectedItems[i].IsChecked = false;
							SelectedItems.RemoveAt(i);
							if (SelectedOffhand == ci) SelectedOffhand = null;
							break;
						}
					}
				}
				if (si.Slot == SlotType.Finger)
				{
					if (fi >= FingerLimit)
					{
						MessageBox.Show("Can't use more than " + (FingerLimit == 1 ? "one finger item" : "two finger items") + ". Remove one before adding another.", "Finger slots at capacity", MessageBoxButton.OK, MessageBoxImage.Stop);
						cb.IsChecked = false;
					}
					else SelectedItems.Add(cb);
				}
				else if (si.Slot == SlotType.Weapon)
				{
					bool allow = false;
					if (si.Handedness == 1)
					{
						if (WeaponSlot.IsLocked)
						{
							if (WeaponSlot.Item == null || WeaponSlot.Item.Item.Handedness == 1)
							{
								if (SelectedOffhand == null)
								{
									SelectedOffhand = si;
									allow = true;
								}
							}
						}
						else if (SelectedWeapon == null)
						{
							SelectedWeapon = si;
							allow = true;
						}
						else if (SelectedWeapon.Handedness == 1 && !OffhandSlot.IsLocked && SelectedOffhand == null)
						{
							SelectedOffhand = si;
							allow = true;
						}
					}
					else if (si.Handedness == 2)
					{
						if (SelectedWeapon == null)
						{
							if (OffhandSlot.Item == null) allow = true;
							else if (OffhandSlot.IsLocked)
							{
								if (OffhandSlot.Item.Item.Slot == SlotType.Offhand && DatasetManager.CanBeUsedTogether(si, OffhandSlot.Item.Item)) allow = true;
							}
							else if (SelectedOffhand == null) allow = true;
							else if (SelectedOffhand.Slot == SlotType.Offhand && DatasetManager.CanBeUsedTogether(si, SelectedOffhand)) allow = true;

							if (allow) SelectedWeapon = si;
						}
					}

					if (!allow)
					{
						MessageBox.Show("There are no hands available to use the selected weapon. Free up a weapon or offhand item to add another.", "Weapon/Offhand slots at capacity", MessageBoxButton.OK, MessageBoxImage.Stop);
						cb.IsChecked = false;
					}
					else SelectedItems.Add(cb);
				}
				else if (si.Slot == SlotType.Offhand)
				{
					bool allow = false;
					if (SelectedOffhand == null)
					{
						if (SelectedWeapon != null) allow = DatasetManager.CanBeUsedTogether(SelectedWeapon, si);
						else if (WeaponSlot.Item != null) allow = DatasetManager.CanBeUsedTogether(WeaponSlot.Item.Item, si);
						else allow = true;
					}

					if (!allow)
					{
						MessageBox.Show("There are no hands available to use the selected offhand. Free up a weapon item first.", "Weapon/Offhand slots at capacity", MessageBoxButton.OK, MessageBoxImage.Stop);
						cb.IsChecked = false;
					}
					else
					{
						SelectedItems.Add(cb);
						SelectedOffhand = si;
					}
				}
				else SelectedItems.Add(cb);
			}
			// item is being deselected by user
			else
			{
				SelectedItems.Remove(cb);
				DDOItemData id = cb.Tag as DDOItemData;
				if (id.Slot == SlotType.Offhand) SelectedOffhand = null;
				else if (id.Slot == SlotType.Weapon)
				{
					if (SelectedWeapon == id)
					{
						SelectedWeapon = null;
						if (SelectedOffhand != null && SelectedOffhand.Slot == SlotType.Weapon)
						{
							SelectedWeapon = SelectedOffhand;
							SelectedOffhand = null;
						}
					}
					else if (SelectedOffhand == id) SelectedOffhand = null;
				}
			}

			// update minimum item notification for bonuses tabs
			TabItem sti = null;
			foreach (TabItem ti in tcSetBonuses.Items)
			{
				DDOItemSetBonus sb = ti.Tag as DDOItemSetBonus;
				if (sb.MinimumItems <= SelectedItems.Count) sti = ti;
				ti.Header = sb.MinimumItems.ToString() + " Pieces";
			}
			if (sti != null) sti.Header = (sti.Tag as DDOItemSetBonus).MinimumItems.ToString() + " Pieces *";

			btnApply.IsEnabled = SelectedItems.Count > 0;
		}

		private void ListViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			MouseRightClicked = true;
		}

		private void SearchTextChanged(object sender, TextChangedEventArgs e)
		{
			CollectionViewSource.GetDefaultView(lvSets.ItemsSource).Refresh();
		}

		private void Cancel_Clicked(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void Apply_Clicked(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		public List<DDOItemData> GetItems()
		{
			List<DDOItemData> items = new List<DDOItemData>();
			foreach (var item in SelectedItems)
				items.Add(item.Tag as DDOItemData);

			return items;
		}
	}
}
