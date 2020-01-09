﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;


namespace DDONamedGearPlanner
{
	/// <summary>
	/// Interaction logic for PlannerWindow.xaml
	/// </summary>
	public partial class PlannerWindow : Window
	{
		public static DDODataset dataset;

		EquipmentSlotControl[] EquipmentSlots = new EquipmentSlotControl[14];
		EquipmentSlotControl SelectedESC;

		public PlannerWindow()
		{
			InitializeComponent();

			if (!LoadDDODataset())
			{
				Close();
				return;
			}

			BtnFilterApply_Click(null, null);

			lvItemList.ItemsSource = dataset.Items;
			SetFilter(CustomFilter);
		}

		bool LoadDDODataset()
		{
			FileStream fs = new FileStream("ddodata.dat", FileMode.Open);
			try
			{
				BinaryFormatter bf = new BinaryFormatter();
				dataset = (DDODataset)bf.Deserialize(fs);

				return true;
			}
			catch (Exception e)
			{
				MessageBox.Show("Error loading DDO dataset - " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			finally
			{
				fs.Close();
			}
		}

		public void RegisterEquipmentSlot(EquipmentSlotControl esc)
		{
			EquipmentSlots[(int)esc.Slot] = esc;
			esc.EquipmentSlotClicked += EquipmentSlotClicked;
			esc.EquipmentSlotCleared += EquipmentSlotCleared;
		}

		public void SetFilter(Predicate<object> filter)
		{
			if (lvItemList.Items.CurrentItem != null && !filter(lvItemList.Items.CurrentItem))
			{
				lvItemList.SelectedItem = null;
			}
			lvItemList.Items.Filter = filter;
			if (lvItemList.Items.CurrentItem == null && !lvItemList.Items.IsEmpty)
				lvItemList.Items.MoveCurrentToFirst();
		}

		private bool CustomFilter(object obj)
		{
			DDOItemData item = obj as DDOItemData;
			if ((ItemFilterSettings.Slots & item.Slot) == 0) return false;
			if (item.Slot == SlotType.Body)
			{
				if ((ArmorCategory)item.Category == ArmorCategory.Cloth && !ItemFilterSettings.BodyCloth) return false;
				if ((ArmorCategory)item.Category == ArmorCategory.Light && !ItemFilterSettings.BodyLight) return false;
				if ((ArmorCategory)item.Category == ArmorCategory.Medium && !ItemFilterSettings.BodyMedium) return false;
				if ((ArmorCategory)item.Category == ArmorCategory.Heavy && !ItemFilterSettings.BodyHeavy) return false;
				if ((ArmorCategory)item.Category == ArmorCategory.Docent && !ItemFilterSettings.BodyDocent) return false;
			}
			else if (item.Slot == SlotType.Offhand)
			{
				if ((OffhandCategory)item.Category == OffhandCategory.Buckler && !ItemFilterSettings.OffhandShieldBuckler) return false;
				if ((OffhandCategory)item.Category == OffhandCategory.Small && !ItemFilterSettings.OffhandShieldSmall) return false;
				if ((OffhandCategory)item.Category == OffhandCategory.Large && !ItemFilterSettings.OffhandShieldLarge) return false;
				if ((OffhandCategory)item.Category == OffhandCategory.Tower && !ItemFilterSettings.OffhandShieldTower) return false;
				if ((OffhandCategory)item.Category == OffhandCategory.Orb && !ItemFilterSettings.OffhandShieldOrb) return false;
			}
			else if (item.Slot == SlotType.Weapon)
			{
				if ((WeaponCategory)item.Category == WeaponCategory.Simple)
				{
					if (ItemFilterSettings.WeaponSimpleClub && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Club") return false;
					if (ItemFilterSettings.WeaponSimpleDagger && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Dagger") return false;
					if (ItemFilterSettings.WeaponSimpleHeavyMace && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Heavy Mace") return false;
					if (ItemFilterSettings.WeaponSimpleHeavyXbow && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Heavy Crossbow") return false;
					if (ItemFilterSettings.WeaponSimpleLightMace && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Light Mace") return false;
					if (ItemFilterSettings.WeaponSimpleLightXbow && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Light Crossbow") return false;
					if (ItemFilterSettings.WeaponSimpleMorningstar && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Morningstar") return false;
					if (ItemFilterSettings.WeaponSimpleQuarterstaff && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Quarterstaff") return false;
					if (ItemFilterSettings.WeaponSimpleSickle && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Sickle") return false;
				}
				else if ((WeaponCategory)item.Category == WeaponCategory.Martial)
				{
					if (ItemFilterSettings.WeaponMartialBattleAxe && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Battle Axe") return false;
					if (ItemFilterSettings.WeaponMartialFalchion && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Falchion") return false;
					if (ItemFilterSettings.WeaponMartialGreatAxe && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Great Axe") return false;
					if (ItemFilterSettings.WeaponMartialGreatClub && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Great Club") return false;
					if (ItemFilterSettings.WeaponMartialGreatSword && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Great Sword") return false;
					if (ItemFilterSettings.WeaponMartialHandaxe && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Hand Axe") return false;
					if (ItemFilterSettings.WeaponMartialHeavyPick && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Heavy Pick") return false;
					if (ItemFilterSettings.WeaponMartialKukri && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Kukri") return false;
					if (ItemFilterSettings.WeaponMartialLightHammer && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Light Hammer") return false;
					if (ItemFilterSettings.WeaponMartialLightPick && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Light Pick") return false;
					if (ItemFilterSettings.WeaponMartialLongBow && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Long Bow") return false;
					if (ItemFilterSettings.WeaponMartialLongSword && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Long Sword") return false;
					if (ItemFilterSettings.WeaponMartialMaul && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Maul") return false;
					if (ItemFilterSettings.WeaponMartialRapier && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Rapier") return false;
					if (ItemFilterSettings.WeaponMartialScimitar && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Scimitar") return false;
					if (ItemFilterSettings.WeaponMartialShortBow && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Short Bow") return false;
					if (ItemFilterSettings.WeaponMartialShortSword && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Short Sword") return false;
					if (ItemFilterSettings.WeaponMartialWarHammer && item.Properties.Find(i => i.Property == "Weapon Type").Type != "War Hammer") return false;
				}
				else if ((WeaponCategory)item.Category == WeaponCategory.Exotic)
				{
					if (ItemFilterSettings.WeaponExoticBastardSword && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Bastard Sword") return false;
					if (ItemFilterSettings.WeaponExoticDwarvenWarAxe && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Dwarven War Axe") return false;
					if (ItemFilterSettings.WeaponExoticGreatXbow && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Great Crossbow") return false;
					if (ItemFilterSettings.WeaponExoticHandwraps && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Handwraps") return false;
					if (ItemFilterSettings.WeaponExoticKama && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Kama") return false;
					if (ItemFilterSettings.WeaponExoticKhopesh && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Khopesh") return false;
					if (ItemFilterSettings.WeaponExoticRepeatHeavyXbow && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Repeating Heavy Crossbow") return false;
					if (ItemFilterSettings.WeaponExoticRepeatLightXbow && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Repeating Light Crossbow") return false;
					if (ItemFilterSettings.WeaponExoticRuneArm && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Rune Arm") return false;
				}
				else if ((WeaponCategory)item.Category == WeaponCategory.Throwing)
				{
					if (ItemFilterSettings.WeaponThrowingAxe && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Throwing Axe") return false;
					if (ItemFilterSettings.WeaponThrowingDagger && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Throwing Dagger") return false;
					if (ItemFilterSettings.WeaponThrowingDart && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Dart") return false;
					if (ItemFilterSettings.WeaponThrowingHammer && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Throwing Hammer") return false;
					if (ItemFilterSettings.WeaponThrowingShuriken && item.Properties.Find(i => i.Property == "Weapon Type").Type != "Shuriken") return false;
				}
			}

			if (string.IsNullOrWhiteSpace(txtSearchBox.Text)) return true;
			else return item.Name.IndexOf(txtSearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
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

			if (sender != null) CollectionViewSource.GetDefaultView(lvItemList.ItemsSource).Refresh();
		}

		private void TxtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			CollectionViewSource.GetDefaultView(lvItemList.ItemsSource).Refresh();
		}

		private void TbFilterBody_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			cmiFilterBodyCloth.IsChecked = cmiFilterBodyLight.IsChecked = cmiFilterBodyMedium.IsChecked = cmiFilterBodyHeavy.IsChecked = cmiFilterBodyDocent.IsChecked = !tbFilterBody.IsChecked ?? false;
		}

		private void TbFilterOffhand_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			cmiFilterOffhandBuckler.IsChecked = cmiFilterOffhandSmall.IsChecked = cmiFilterOffhandLarge.IsChecked = cmiFilterOffhandTower.IsChecked = cmiFilterOffhandOrb.IsChecked = !tbFilterOffhand.IsChecked ?? false;
		}

		private void FilterMenuItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			mi.IsChecked = !mi.IsChecked;
			foreach (MenuItem cmi in mi.Items)
				cmi.IsChecked = mi.IsChecked;
		}

		private void LvItemList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			// validate user double-clicked over an item
			if (lvItemList.SelectedItem == null) return;

			DDOItemData item = lvItemList.SelectedItem as DDOItemData;
			EquipmentSlotControl esc = null;
			// get the slot the item belongs in
			//  - special case check for an open finger slot
			//  - if both finger slots are full, check for one not locked
			if (item.Slot == SlotType.Finger)
			{
				if (EquipmentSlots[(int)EquipmentSlotType.Finger1].Item == null) esc = EquipmentSlots[(int)EquipmentSlotType.Finger1];
				else if (EquipmentSlots[(int)EquipmentSlotType.Finger2].Item == null) esc = EquipmentSlots[(int)EquipmentSlotType.Finger2];
				else if (!EquipmentSlots[(int)EquipmentSlotType.Finger1].IsLocked) esc = EquipmentSlots[(int)EquipmentSlotType.Finger1];
				else if (!EquipmentSlots[(int)EquipmentSlotType.Finger2].IsLocked) esc = EquipmentSlots[(int)EquipmentSlotType.Finger2];
			}
			else
			{
				esc = EquipmentSlots[(int)(EquipmentSlotType)Enum.Parse(typeof(EquipmentSlotType), item.Slot.ToString())];
			}

			if (esc == null || esc.IsLocked) MessageBox.Show("Can't load an item into a locked slot.", "Slot Locked", MessageBoxButton.OK, MessageBoxImage.Stop);
			else
			{
				esc.SetItem(item);
				// update gearset properties
			}
		}

		private void EquipmentSlotClicked(EquipmentSlotControl esc, MouseButton button)
		{
			if (button == MouseButton.Right)
			{
				if (esc.Item != null) System.Diagnostics.Process.Start(esc.Item.WikiURL);
				return;
			}

			if (SelectedESC == esc)
			{
				OpenPropertiesTab(SelectedESC.Item);
				return;
			}

			if (SelectedESC != null)
			{
				SelectedESC.IsSelected = false;
				SelectedESC.SetSelectBorder(false);
				SelectedESC = null;
			}
			if (esc.Item != null)
			{
				SelectedESC = esc;
				SelectedESC.IsSelected = true;
				SelectedESC.SetSelectBorder(true);
				OpenPropertiesTab(SelectedESC.Item);
			}
		}

		void OpenPropertiesTab(DDOItemData item)
		{
			if (item == null) return;

			// first search for an existing tab for the item
			foreach (TabItem ti in tcPropertyAreas.Items)
			{
				if (ti.Header.ToString() == item.Name)
				{
					tcPropertyAreas.SelectedItem = ti;
					return;
				}
			}

			TabItem nti = new TabItem();
			nti.Header = item.Name;
			nti.MouseRightButtonUp += PropertyTabRightButtonUp;

			tcPropertyAreas.Items.Add(nti);
			tcPropertyAreas.SelectedItem = nti;

			ListViewItemProperties lvip = new ListViewItemProperties();
			nti.Content = lvip;
			lvip.SetItem(item);
		}

		bool ItemRightClicked;
		DDOItemData RightClickedItem;

		private void LvItemList_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (ItemRightClicked && e.RightButton == MouseButtonState.Released)
			{
				ItemRightClicked = false;
				System.Diagnostics.Process.Start(RightClickedItem.WikiURL);
				RightClickedItem = null;
			}
		}

		private void LvItemList_MouseLeave(object sender, MouseEventArgs e)
		{
			ItemRightClicked = false;
		}

		private void ListViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			var item = sender as ListViewItem;
			if (item != null)
			{
				RightClickedItem = item.Content as DDOItemData;
				if (RightClickedItem != null) ItemRightClicked = true;
			}
		}

		private void EquipmentSlotCleared(EquipmentSlotControl esc)
		{
			if (esc.IsSelected)
			{
				esc.IsSelected = false;
				esc.SetSelectBorder(false);
				SelectedESC = null;
			}
		}

		private void PropertyTabRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			TabItem ti = sender as TabItem;
			tcPropertyAreas.Items.Remove(ti);
		}

		private void LvItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			OpenPropertiesTab(lvItemList.SelectedItem as DDOItemData);
		}

		private void UnlockClearAll(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < EquipmentSlots.Length; i++)
			{
				EquipmentSlots[i].SetLockStatus(false);
				EquipmentSlots[i].SetItem(null);
			}

			// wipe calculated gear set
		}

		private void ImportNamedSet(object sender, RoutedEventArgs e)
		{

		}
	}
}