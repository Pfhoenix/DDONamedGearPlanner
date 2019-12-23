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
			ItemFilterSettings.Back = tbFilterBack.IsChecked ?? false;
			ItemFilterSettings.Body = tbFilterBody.IsChecked ?? false;
			ItemFilterSettings.BodyCloth = ItemFilterSettings.Body ? cmiFilterBodyCloth.IsChecked : false;
			ItemFilterSettings.BodyLight = ItemFilterSettings.Body ? cmiFilterBodyLight.IsChecked : false;
			ItemFilterSettings.BodyMedium = ItemFilterSettings.Body ? cmiFilterBodyMedium.IsChecked : false;
			ItemFilterSettings.BodyHeavy = ItemFilterSettings.Body ? cmiFilterBodyHeavy.IsChecked : false;
			ItemFilterSettings.BodyDocent = ItemFilterSettings.Body ? cmiFilterBodyDocent.IsChecked : false;
			ItemFilterSettings.Eye = tbFilterEye.IsChecked ?? false;
			ItemFilterSettings.Feet = tbFilterFeet.IsChecked ?? false;
			ItemFilterSettings.Finger = tbFilterFinger.IsChecked ?? false;
			ItemFilterSettings.Hand = tbFilterHand.IsChecked ?? false;
			ItemFilterSettings.Head = tbFilterHead.IsChecked ?? false;
			ItemFilterSettings.Neck = tbFilterNeck.IsChecked ?? false;
			ItemFilterSettings.Offhand = tbFilterOffhand.IsChecked ?? false;
			ItemFilterSettings.OffhandShieldBuckler = ItemFilterSettings.Offhand ? cmiFilterOffhandBuckler.IsChecked : false;
			ItemFilterSettings.OffhandShieldSmall = ItemFilterSettings.Offhand ? cmiFilterOffhandSmall.IsChecked : false;
			ItemFilterSettings.OffhandShieldLarge = ItemFilterSettings.Offhand ? cmiFilterOffhandLarge.IsChecked : false;
			ItemFilterSettings.OffhandShieldTower = ItemFilterSettings.Offhand ? cmiFilterOffhandTower.IsChecked : false;
			ItemFilterSettings.OffhandShieldOrb = ItemFilterSettings.Offhand ? cmiFilterOffhandOrb.IsChecked : false;
			ItemFilterSettings.OffhandWeapon = ItemFilterSettings.Offhand ? cmiFilterOffhandWeapon.IsChecked : false;
			ItemFilterSettings.Trinket = tbFilterTrinket.IsChecked ?? false;
			ItemFilterSettings.Waist = tbFilterWaist.IsChecked ?? false;
			ItemFilterSettings.Weapon = tbFilterWeapon.IsChecked ?? false;
			ItemFilterSettings.WeaponSimpleClub = ItemFilterSettings.Weapon ? cmiFilterWeaponSimpleClub.IsChecked : false;
			ItemFilterSettings.WeaponSimpleQuarterstaff = ItemFilterSettings.Weapon ? cmiFilterWeaponSimpleQuarterstaff.IsChecked : false;
			ItemFilterSettings.WeaponSimpleDagger = ItemFilterSettings.Weapon ? cmiFilterWeaponSimpleDagger.IsChecked : false;
			ItemFilterSettings.WeaponSimpleSickle = ItemFilterSettings.Weapon ? cmiFilterWeaponSimpleSickle.IsChecked : false;
			ItemFilterSettings.WeaponSimpleLightMace = ItemFilterSettings.Weapon ? cmiFilterWeaponSimpleLightMace.IsChecked : false;
			ItemFilterSettings.WeaponSimpleHeavyMace = ItemFilterSettings.Weapon ? cmiFilterWeaponSimpleHeavyMace.IsChecked : false;
			ItemFilterSettings.WeaponSimpleMorningstar = ItemFilterSettings.Weapon ? cmiFilterWeaponSimpleMorningstar.IsChecked : false;
			ItemFilterSettings.WeaponSimpleLightXbow = ItemFilterSettings.Weapon ? cmiFilterWeaponSimpleLightXbow.IsChecked : false;
			ItemFilterSettings.WeaponSimpleHeavyXbow = ItemFilterSettings.Weapon ? cmiFilterWeaponSimpleHeavyXbow.IsChecked : false;
			ItemFilterSettings.WeaponMartialHandaxe = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialHandaxe.IsChecked : false;
			ItemFilterSettings.WeaponMartialBattleAxe = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialBattleAxe.IsChecked : false;
			ItemFilterSettings.WeaponMartialGreatAxe = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialGreatAxe.IsChecked : false;
			ItemFilterSettings.WeaponMartialKukri = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialKukri.IsChecked : false;
			ItemFilterSettings.WeaponMartialShortSword = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialShortSword.IsChecked : false;
			ItemFilterSettings.WeaponMartialLongSword = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialLongSword.IsChecked : false;
			ItemFilterSettings.WeaponMartialGreatSword = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialGreatSword.IsChecked : false;
			ItemFilterSettings.WeaponMartialScimitar = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialScimitar.IsChecked : false;
			ItemFilterSettings.WeaponMartialFalchion = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialFalchion.IsChecked : false;
			ItemFilterSettings.WeaponMartialRapier = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialRapier.IsChecked : false;
			ItemFilterSettings.WeaponMartialLightPick = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialLightPick.IsChecked : false;
			ItemFilterSettings.WeaponMartialHeavyPick = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialHeavyPick.IsChecked : false;
			ItemFilterSettings.WeaponMartialLightHammer = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialLightHammer.IsChecked : false;
			ItemFilterSettings.WeaponMartialWarHammer = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialWarHammer.IsChecked : false;
			ItemFilterSettings.WeaponMartialMaul = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialMaul.IsChecked : false;
			ItemFilterSettings.WeaponMartialGreatClub = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialGreatClub.IsChecked : false;
			ItemFilterSettings.WeaponMartialShortBow = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialShortBow.IsChecked : false;
			ItemFilterSettings.WeaponMartialLongBow = ItemFilterSettings.Weapon ? cmiFilterWeaponMartialLongBow.IsChecked : false;
			ItemFilterSettings.WeaponExoticBastardSword = ItemFilterSettings.Weapon ? cmiFilterWeaponExoticBastardSword.IsChecked : false;
			ItemFilterSettings.WeaponExoticDwarvenWarAxe = ItemFilterSettings.Weapon ? cmiFilterWeaponExoticDwarvenWarAxe.IsChecked : false;
			ItemFilterSettings.WeaponExoticKama = ItemFilterSettings.Weapon ? cmiFilterWeaponExoticKama.IsChecked : false;
			ItemFilterSettings.WeaponExoticKhopesh = ItemFilterSettings.Weapon ? cmiFilterWeaponExoticKhopesh.IsChecked : false;
			ItemFilterSettings.WeaponExoticHandwraps = ItemFilterSettings.Weapon ? cmiFilterWeaponExoticHandwraps.IsChecked : false;
			ItemFilterSettings.WeaponExoticRuneArm = ItemFilterSettings.Weapon ? cmiFilterWeaponExoticRuneArm.IsChecked : false;
			ItemFilterSettings.WeaponExoticGreatXbow = ItemFilterSettings.Weapon ? cmiFilterWeaponExoticGreatXbow.IsChecked : false;
			ItemFilterSettings.WeaponExoticRepeatLightXbow = ItemFilterSettings.Weapon ? cmiFilterWeaponExoticRepeatLightXbow.IsChecked : false;
			ItemFilterSettings.WeaponExoticRepeatHeavyXbow = ItemFilterSettings.Weapon ? cmiFilterWeaponExoticRepeatHeavyXbow.IsChecked : false;
			ItemFilterSettings.WeaponThrowingAxe = ItemFilterSettings.Weapon ? cmiFilterWeaponThrowingAxe.IsChecked : false;
			ItemFilterSettings.WeaponThrowingDagger = ItemFilterSettings.Weapon ? cmiFilterWeaponThrowingDagger.IsChecked : false;
			ItemFilterSettings.WeaponThrowingHammer = ItemFilterSettings.Weapon ? cmiFilterWeaponThrowingHammer.IsChecked : false;
			ItemFilterSettings.WeaponThrowingDart = ItemFilterSettings.Weapon ? cmiFilterWeaponThrowingDart.IsChecked : false;
			ItemFilterSettings.WeaponThrowingShuriken = ItemFilterSettings.Weapon ? cmiFilterWeaponThrowingShuriken.IsChecked : false;
			ItemFilterSettings.Wrist = tbFilterWrist.IsChecked ?? false;
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

		private void TbFilterBody_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			cmiFilterBodyCloth.IsChecked = cmiFilterBodyLight.IsChecked = cmiFilterBodyMedium.IsChecked = cmiFilterBodyHeavy.IsChecked = cmiFilterBodyDocent.IsChecked = !tbFilterBody.IsChecked ?? false;
		}

		private void TbFilterOffhand_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			cmiFilterOffhandBuckler.IsChecked = cmiFilterOffhandSmall.IsChecked = cmiFilterOffhandLarge.IsChecked = cmiFilterOffhandTower.IsChecked = cmiFilterOffhandOrb.IsChecked = cmiFilterOffhandWeapon.IsChecked = !tbFilterOffhand.IsChecked ?? false;
		}

		private void FilterMenuItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			mi.IsChecked = !mi.IsChecked;
			foreach (MenuItem cmi in mi.Items)
				cmi.IsChecked = mi.IsChecked;
		}
	}
}
