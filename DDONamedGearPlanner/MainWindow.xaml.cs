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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DDONamedGearPlanner
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<string> ItemsInList = new List<string>();

		public MainWindow()
		{
			InitializeComponent();

			lbItemList.ItemsSource = ItemsInList;

			SetFilter(CustomFilter);
		}

		public void SetFilter(Predicate<object> filter)
		{
			if (lbItemList.Items.CurrentItem != null && !filter(lbItemList.Items.CurrentItem))
			{
				lbItemList.SelectedItem = null;
			}
			lbItemList.Items.Filter = filter;
			if (lbItemList.Items.CurrentItem == null && !lbItemList.Items.IsEmpty)
				lbItemList.Items.MoveCurrentToFirst();
		}

		private bool CustomFilter(object obj)
		{
			if (string.IsNullOrWhiteSpace(txtSearchBox.Text))
			{
				return true;
			}
			else
			{
				return obj.ToString().IndexOf(txtSearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
			}
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void BtnFilterAll_Click(object sender, RoutedEventArgs e)
		{
			tbFilterBack.IsChecked = tbFilterBody.IsChecked = tbFilterEye.IsChecked = tbFilterFeet.IsChecked = tbFilterFinger.IsChecked = tbFilterHand.IsChecked = tbFilterHead.IsChecked = tbFilterNeck.IsChecked = tbFilterOffhand.IsChecked = tbFilterTrinket.IsChecked = tbFilterWaist.IsChecked = tbFilterWeapon.IsChecked = tbFilterWrist.IsChecked = true;
		}

		private void BtnFilterClear_Click(object sender, RoutedEventArgs e)
		{
			tbFilterBack.IsChecked = tbFilterBody.IsChecked = tbFilterEye.IsChecked = tbFilterFeet.IsChecked = tbFilterFinger.IsChecked = tbFilterHand.IsChecked = tbFilterHead.IsChecked = tbFilterNeck.IsChecked = tbFilterOffhand.IsChecked = tbFilterTrinket.IsChecked = tbFilterWaist.IsChecked = tbFilterWeapon.IsChecked = tbFilterWrist.IsChecked = false;
		}

		private void BtnFilterApply_Click(object sender, RoutedEventArgs e)
		{
			ItemsInList.Clear();
			txtSearchBox.Text = null;
		}

		private void TxtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			CollectionViewSource.GetDefaultView(lbItemList.ItemsSource).Refresh();
		}

		private void LbItemList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			// validate user double-clicked over an item

			// get the slot the item belongs in
			//  - special case check for an open finger slot
			//  - if both finger slots are full, check for one not locked

			// check if slot is filled and locked
			//  - display warning and return
		}

		#region Filter Checked Events

		private void TbFilterBack_Checked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Back = true;
		}

		private void TbFilterBack_Unchecked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Back = false;
		}

		private void TbFilterBody_Checked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Body = true;
			ItemFilterSettings.BodyCloth = true;
			ItemFilterSettings.BodyLight = true;
			ItemFilterSettings.BodyMedium = true;
			ItemFilterSettings.BodyHeavy = true;
			ItemFilterSettings.BodyDocent = true;
		}

		private void TbFilterBody_Unchecked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Body = false;
			ItemFilterSettings.BodyCloth = false;
			ItemFilterSettings.BodyLight = false;
			ItemFilterSettings.BodyMedium = false;
			ItemFilterSettings.BodyHeavy = false;
			ItemFilterSettings.BodyDocent = false;
		}

		private void TbFilterEye_Checked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Eye = true;
		}

		private void TbFilterEye_Unchecked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Eye = false;
		}

		private void TbFilterFeet_Checked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Feet = true;
		}

		private void TbFilterFeet_Unchecked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Feet = false;
		}

		private void TbFilterFinger_Checked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Finger = true;
		}

		private void TbFilterFinger_Unchecked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Finger = false;
		}

		private void TbFilterHand_Checked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Hand = true;
		}

		private void TbFilterHand_Unchecked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Hand = false;
		}

		private void TbFilterHead_Checked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Head = true;
		}

		private void TbFilterHead_Unchecked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Head = false;
		}

		private void TbFilterNeck_Checked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Neck = true;
		}

		private void TbFilterNeck_Unchecked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Neck = false;
		}

		private void TbFilterOffhand_Checked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Offhand = true;
			ItemFilterSettings.OffhandShield = true;
			ItemFilterSettings.OffhandShieldBuckler = true;
			ItemFilterSettings.OffhandShieldSmall = true;
			ItemFilterSettings.OffhandShieldLarge = true;
			ItemFilterSettings.OffhandShieldTower = true;
			ItemFilterSettings.OffhandShieldOrb = true;
			ItemFilterSettings.OffhandWeapon = true;
		}

		private void TbFilterOffhand_Unchecked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Offhand = false;
			ItemFilterSettings.OffhandShield = false;
			ItemFilterSettings.OffhandShieldBuckler = false;
			ItemFilterSettings.OffhandShieldSmall = false;
			ItemFilterSettings.OffhandShieldLarge = false;
			ItemFilterSettings.OffhandShieldTower = false;
			ItemFilterSettings.OffhandShieldOrb = false;
			ItemFilterSettings.OffhandWeapon = false;
		}

		private void TbFilterTrinket_Checked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Trinket = true;
		}

		private void TbFilterTrinket_Unchecked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Trinket = false;
		}

		private void TbFilterWaist_Checked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Waist = true;
		}

		private void TbFilterWaist_Unchecked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Waist = false;
		}

		private void TbFilterWeapon_Checked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Weapon = true;
			ItemFilterSettings.WeaponSimple = true;
			ItemFilterSettings.WeaponSimpleClub = true;
			ItemFilterSettings.WeaponSimpleDagger = true;
			ItemFilterSettings.WeaponSimpleHeavyMace = true;
			ItemFilterSettings.WeaponSimpleHeavyXbow = true;
			ItemFilterSettings.WeaponSimpleLightMace = true;
			ItemFilterSettings.WeaponSimpleLightXbow = true;
			ItemFilterSettings.WeaponSimpleMorningstar = true;
			ItemFilterSettings.WeaponSimpleQuarterstaff = true;
			ItemFilterSettings.WeaponSimpleSickle = true;
			ItemFilterSettings.WeaponMartial = true;
			ItemFilterSettings.WeaponMartialBattleAxe = true;
			ItemFilterSettings.WeaponMartialFalchion = true;
			ItemFilterSettings.WeaponMartialGreatAxe = true;
			ItemFilterSettings.WeaponMartialGreatClub = true;
			ItemFilterSettings.WeaponMartialGreatSword = true;
			ItemFilterSettings.WeaponMartialHandaxe = true;
			ItemFilterSettings.WeaponMartialHeavyPick = true;
			ItemFilterSettings.WeaponMartialKukri = true;
			ItemFilterSettings.WeaponMartialLightHammer = true;
			ItemFilterSettings.WeaponMartialLightPick = true;
			ItemFilterSettings.WeaponMartialLongBow = true;
			ItemFilterSettings.WeaponMartialLongSword = true;
			ItemFilterSettings.WeaponMartialMaul = true;
			ItemFilterSettings.WeaponMartialRapier = true;
			ItemFilterSettings.WeaponMartialScimitar = true;
			ItemFilterSettings.WeaponMartialShortBow = true;
			ItemFilterSettings.WeaponMartialShortSword = true;
			ItemFilterSettings.WeaponMartialWarHammer = true;
		}

		private void TbFilterWeapon_Unchecked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Weapon = false;
			ItemFilterSettings.WeaponSimple = false;
			ItemFilterSettings.WeaponSimpleClub = false;
			ItemFilterSettings.WeaponSimpleDagger = false;
			ItemFilterSettings.WeaponSimpleHeavyMace = false;
			ItemFilterSettings.WeaponSimpleHeavyXbow = false;
			ItemFilterSettings.WeaponSimpleLightMace = false;
			ItemFilterSettings.WeaponSimpleLightXbow = false;
			ItemFilterSettings.WeaponSimpleMorningstar = false;
			ItemFilterSettings.WeaponSimpleQuarterstaff = false;
			ItemFilterSettings.WeaponSimpleSickle = false;
			ItemFilterSettings.WeaponMartial = false;
			ItemFilterSettings.WeaponMartialBattleAxe = false;
			ItemFilterSettings.WeaponMartialFalchion = false;
			ItemFilterSettings.WeaponMartialGreatAxe = false;
			ItemFilterSettings.WeaponMartialGreatClub = false;
			ItemFilterSettings.WeaponMartialGreatSword = false;
			ItemFilterSettings.WeaponMartialHandaxe = false;
			ItemFilterSettings.WeaponMartialHeavyPick = false;
			ItemFilterSettings.WeaponMartialKukri = false;
			ItemFilterSettings.WeaponMartialLightHammer = false;
			ItemFilterSettings.WeaponMartialLightPick = false;
			ItemFilterSettings.WeaponMartialLongBow = false;
			ItemFilterSettings.WeaponMartialLongSword = false;
			ItemFilterSettings.WeaponMartialMaul = false;
			ItemFilterSettings.WeaponMartialRapier = false;
			ItemFilterSettings.WeaponMartialScimitar = false;
			ItemFilterSettings.WeaponMartialShortBow = false;
			ItemFilterSettings.WeaponMartialShortSword = false;
			ItemFilterSettings.WeaponMartialWarHammer = false;
		}

		private void TbFilterWrist_Checked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Wrist = true;
		}

		private void TbFilterWrist_Unchecked(object sender, RoutedEventArgs e)
		{
			ItemFilterSettings.Wrist = false;
		}

		#endregion
	}
}
