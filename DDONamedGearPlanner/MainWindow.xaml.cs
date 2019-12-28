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
			ItemFilterSettings.Slots = SlotType.None;
			ItemFilterSettings.Slots |= tbFilterBack.IsChecked.Value ? SlotType.Back : 0;
			ItemFilterSettings.Slots |= tbFilterBody.IsChecked.Value ? SlotType.Body : 0;
			ItemFilterSettings.BodyCloth = tbFilterBody.IsChecked.Value ? cmiFilterBodyCloth.IsChecked : false;
			ItemFilterSettings.BodyLight = tbFilterBody.IsChecked.Value ? cmiFilterBodyLight.IsChecked : false;
			ItemFilterSettings.BodyMedium = tbFilterBody.IsChecked.Value ? cmiFilterBodyMedium.IsChecked : false;
			ItemFilterSettings.BodyHeavy = tbFilterBody.IsChecked.Value ? cmiFilterBodyHeavy.IsChecked : false;
			ItemFilterSettings.BodyDocent = tbFilterBody.IsChecked.Value ? cmiFilterBodyDocent.IsChecked : false;
			ItemFilterSettings.Slots |= tbFilterEye.IsChecked.Value ? SlotType.Eye : 0;
			ItemFilterSettings.Slots |= tbFilterFeet.IsChecked.Value ? SlotType.Feet : 0;
			ItemFilterSettings.Slots |= tbFilterFinger.IsChecked.Value ? SlotType.Finger : 0;
			ItemFilterSettings.Slots |= tbFilterHand.IsChecked.Value ? SlotType.Hand : 0;
			ItemFilterSettings.Slots |= tbFilterHead.IsChecked.Value ? SlotType.Head : 0;
			ItemFilterSettings.Slots |= tbFilterNeck.IsChecked.Value ? SlotType.Neck : 0;
			ItemFilterSettings.Slots |= tbFilterOffhand.IsChecked.Value ? SlotType.Offhand : 0;
			ItemFilterSettings.OffhandShieldBuckler = tbFilterOffhand.IsChecked.Value ? cmiFilterOffhandBuckler.IsChecked : false;
			ItemFilterSettings.OffhandShieldSmall = tbFilterOffhand.IsChecked.Value ? cmiFilterOffhandSmall.IsChecked : false;
			ItemFilterSettings.OffhandShieldLarge = tbFilterOffhand.IsChecked.Value ? cmiFilterOffhandLarge.IsChecked : false;
			ItemFilterSettings.OffhandShieldTower = tbFilterOffhand.IsChecked.Value ? cmiFilterOffhandTower.IsChecked : false;
			ItemFilterSettings.OffhandShieldOrb = tbFilterOffhand.IsChecked.Value ? cmiFilterOffhandOrb.IsChecked : false;
			ItemFilterSettings.OffhandWeapon = tbFilterOffhand.IsChecked.Value ? cmiFilterOffhandWeapon.IsChecked : false;
			ItemFilterSettings.Slots |= tbFilterTrinket.IsChecked.Value ? SlotType.Trinket : 0;
			ItemFilterSettings.Slots |= tbFilterWaist.IsChecked.Value ? SlotType.Waist : 0;
			ItemFilterSettings.Slots |= tbFilterWeapon.IsChecked.Value ? SlotType.Weapon : 0;
			ItemFilterSettings.WeaponSimpleClub = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponSimpleClub.IsChecked : false;
			ItemFilterSettings.WeaponSimpleQuarterstaff = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponSimpleQuarterstaff.IsChecked : false;
			ItemFilterSettings.WeaponSimpleDagger = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponSimpleDagger.IsChecked : false;
			ItemFilterSettings.WeaponSimpleSickle = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponSimpleSickle.IsChecked : false;
			ItemFilterSettings.WeaponSimpleLightMace = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponSimpleLightMace.IsChecked : false;
			ItemFilterSettings.WeaponSimpleHeavyMace = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponSimpleHeavyMace.IsChecked : false;
			ItemFilterSettings.WeaponSimpleMorningstar = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponSimpleMorningstar.IsChecked : false;
			ItemFilterSettings.WeaponSimpleLightXbow = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponSimpleLightXbow.IsChecked : false;
			ItemFilterSettings.WeaponSimpleHeavyXbow = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponSimpleHeavyXbow.IsChecked : false;
			ItemFilterSettings.WeaponMartialHandaxe = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialHandaxe.IsChecked : false;
			ItemFilterSettings.WeaponMartialBattleAxe = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialBattleAxe.IsChecked : false;
			ItemFilterSettings.WeaponMartialGreatAxe = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialGreatAxe.IsChecked : false;
			ItemFilterSettings.WeaponMartialKukri = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialKukri.IsChecked : false;
			ItemFilterSettings.WeaponMartialShortSword = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialShortSword.IsChecked : false;
			ItemFilterSettings.WeaponMartialLongSword = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialLongSword.IsChecked : false;
			ItemFilterSettings.WeaponMartialGreatSword = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialGreatSword.IsChecked : false;
			ItemFilterSettings.WeaponMartialScimitar = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialScimitar.IsChecked : false;
			ItemFilterSettings.WeaponMartialFalchion = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialFalchion.IsChecked : false;
			ItemFilterSettings.WeaponMartialRapier = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialRapier.IsChecked : false;
			ItemFilterSettings.WeaponMartialLightPick = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialLightPick.IsChecked : false;
			ItemFilterSettings.WeaponMartialHeavyPick = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialHeavyPick.IsChecked : false;
			ItemFilterSettings.WeaponMartialLightHammer = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialLightHammer.IsChecked : false;
			ItemFilterSettings.WeaponMartialWarHammer = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialWarHammer.IsChecked : false;
			ItemFilterSettings.WeaponMartialMaul = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialMaul.IsChecked : false;
			ItemFilterSettings.WeaponMartialGreatClub = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialGreatClub.IsChecked : false;
			ItemFilterSettings.WeaponMartialShortBow = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialShortBow.IsChecked : false;
			ItemFilterSettings.WeaponMartialLongBow = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponMartialLongBow.IsChecked : false;
			ItemFilterSettings.WeaponExoticBastardSword = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponExoticBastardSword.IsChecked : false;
			ItemFilterSettings.WeaponExoticDwarvenWarAxe = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponExoticDwarvenWarAxe.IsChecked : false;
			ItemFilterSettings.WeaponExoticKama = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponExoticKama.IsChecked : false;
			ItemFilterSettings.WeaponExoticKhopesh = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponExoticKhopesh.IsChecked : false;
			ItemFilterSettings.WeaponExoticHandwraps = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponExoticHandwraps.IsChecked : false;
			ItemFilterSettings.WeaponExoticRuneArm = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponExoticRuneArm.IsChecked : false;
			ItemFilterSettings.WeaponExoticGreatXbow = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponExoticGreatXbow.IsChecked : false;
			ItemFilterSettings.WeaponExoticRepeatLightXbow = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponExoticRepeatLightXbow.IsChecked : false;
			ItemFilterSettings.WeaponExoticRepeatHeavyXbow = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponExoticRepeatHeavyXbow.IsChecked : false;
			ItemFilterSettings.WeaponThrowingAxe = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponThrowingAxe.IsChecked : false;
			ItemFilterSettings.WeaponThrowingDagger = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponThrowingDagger.IsChecked : false;
			ItemFilterSettings.WeaponThrowingHammer = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponThrowingHammer.IsChecked : false;
			ItemFilterSettings.WeaponThrowingDart = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponThrowingDart.IsChecked : false;
			ItemFilterSettings.WeaponThrowingShuriken = tbFilterWeapon.IsChecked.Value ? cmiFilterWeaponThrowingShuriken.IsChecked : false;
			ItemFilterSettings.Slots |= tbFilterWrist.IsChecked.Value ? SlotType.Wrist : 0;
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
