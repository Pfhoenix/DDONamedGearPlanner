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
		GearSet LeftGS, RightGS;
		PlannerWindow OwnerPW;

		List<BuildItem> SavedItems;

		public GearSetComparisonWindow(PlannerWindow opw)
		{
			InitializeComponent();

			Owner = opw;
			OwnerPW = opw;
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
					if (re > 0) lvRight.Items.Add("");
					lvLeft.Items.Add(LeftGS.Items[l++].Item.Name);
				}
				// if l >= le then proceed r only
				else if (l >= le)
				{
					// we can assume r < re
					if (le > 0) lvLeft.Items.Add("");
					lvRight.Items.Add(RightGS.Items[r++].Item.Name);
				}
				else
				{
					if (RightGS.Items[r].Slot < LeftGS.Items[l].Slot)
					{
						lvRight.Items.Add(RightGS.Items[r++].Item.Name);
						lvLeft.Items.Add("");
					}
					else if (RightGS.Items[r].Slot > LeftGS.Items[l].Slot)
					{
						lvLeft.Items.Add(LeftGS.Items[l++].Item.Name);
						lvRight.Items.Add("");
					}
					else
					{
						lvRight.Items.Add(RightGS.Items[r++].Item.Name);
						lvLeft.Items.Add(LeftGS.Items[l++].Item.Name);
					}
				}
			}
		}
	}
}
