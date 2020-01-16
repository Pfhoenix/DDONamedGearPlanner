using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using CoenM.Encoding;


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

		// item search stuff
		List<DDOItemData> ItemListCopy;
		string LastSortBy;
		int LastSortDir = 1;

		List<DDOItemProperty> ItemPropertiesCopy;

		public PlannerWindow()
		{
			InitializeComponent();

			if (!LoadDDODataset())
			{
				Close();
				return;
			}

			BtnFilterApply_Click(null, null);

			ItemListCopy = new List<DDOItemData>(dataset.Items);
			lvItemList.ItemsSource = ItemListCopy;
			SetFilter(CustomFilter);

			ItemPropertiesCopy = new List<DDOItemProperty>();
			foreach (var ip in dataset.ItemProperties)
				if (ip.Value.Items.Count > 0) ItemPropertiesCopy.Add(ip.Value);
			ItemPropertiesCopy.Sort((a, b) => string.Compare(a.Property, b.Property));
			ItemPropertiesCopy.Insert(0, new DDOItemProperty { Property = "< All >" });
			cbItemPropertyFilter.ItemsSource = ItemPropertiesCopy;
			cbItemPropertyFilter.SelectedIndex = 0;

			txtSearchBox.Focus();
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
			esc.EquipmentSlotLockChanged += EquipmentSlotLockChanged;
			if (esc.Slot == EquipmentSlotType.Finger1 || esc.Slot == EquipmentSlotType.Finger2) esc.SlotType = SlotType.Finger;
			else esc.SlotType = (SlotType)Enum.Parse(typeof(SlotType), esc.Slot.ToString());
		}

		private void EquipmentSlotLockChanged(EquipmentSlotControl esc)
		{
			// if weapon slot and item is two-handed, set the offhand slot lock to the same state
			if (esc.Item != null && esc.Slot == EquipmentSlotType.Weapon)
			{
				if (esc.Item.Handedness == 2)
					EquipmentSlots[(int)EquipmentSlotType.Offhand].SetLockStatus(esc.IsLocked);
			}
			// if offhand slot and weapon slot has two-handed item in it, reset offhand slot lock to weapon slot lock
			else if (esc.Slot == EquipmentSlotType.Offhand)
			{
				var item = EquipmentSlots[(int)EquipmentSlotType.Weapon].Item;
				if (item != null && item.Handedness == 2)
					esc.SetLockStatus(EquipmentSlots[(int)EquipmentSlotType.Weapon].IsLocked);
			}
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
			if (ItemFilterSettings.MinimumLevel > item.ML) return false;
			if (ItemFilterSettings.MaximumLevel < item.ML) return false;
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
				if ((OffhandCategory)item.Category == OffhandCategory.RuneArm && !ItemFilterSettings.OffhandRuneArm) return false;
			}
			else if (item.Slot == SlotType.Weapon)
			{
				if ((WeaponCategory)item.Category == WeaponCategory.Simple)
				{
					if (item.WeaponType == "Club" && !ItemFilterSettings.WeaponSimpleClub) return false;
					if (item.WeaponType == "Dagger" && !ItemFilterSettings.WeaponSimpleDagger) return false;
					if (item.WeaponType == "Heavy Mace" && !ItemFilterSettings.WeaponSimpleHeavyMace) return false;
					if (item.WeaponType == "Heavy Crossbow" && !ItemFilterSettings.WeaponSimpleHeavyXbow) return false;
					if (item.WeaponType == "Light Mace" && !ItemFilterSettings.WeaponSimpleLightMace) return false;
					if (item.WeaponType == "Light Crossbow" && !ItemFilterSettings.WeaponSimpleLightXbow) return false;
					if (item.WeaponType == "Morningstar" && !ItemFilterSettings.WeaponSimpleMorningstar) return false;
					if (item.WeaponType == "Quarterstaff" && !ItemFilterSettings.WeaponSimpleQuarterstaff) return false;
					if (item.WeaponType == "Sickle" && !ItemFilterSettings.WeaponSimpleSickle) return false;
				}
				else if ((WeaponCategory)item.Category == WeaponCategory.Martial)
				{
					if (item.WeaponType == "Battle Axe" && !ItemFilterSettings.WeaponMartialBattleAxe) return false;
					if (item.WeaponType == "Falchion" && !ItemFilterSettings.WeaponMartialFalchion) return false;
					if (item.WeaponType == "Great Axe" && !ItemFilterSettings.WeaponMartialGreatAxe) return false;
					if (item.WeaponType == "Great Club" && !ItemFilterSettings.WeaponMartialGreatClub) return false;
					if (item.WeaponType == "Great Sword" && !ItemFilterSettings.WeaponMartialGreatSword) return false;
					if (item.WeaponType == "Hand Axe" && !ItemFilterSettings.WeaponMartialHandaxe) return false;
					if (item.WeaponType == "Heavy Pick" && !ItemFilterSettings.WeaponMartialHeavyPick) return false;
					if (item.WeaponType == "Kukri" && !ItemFilterSettings.WeaponMartialKukri) return false;
					if (item.WeaponType == "Light Hammer" && !ItemFilterSettings.WeaponMartialLightHammer) return false;
					if (item.WeaponType == "Light Pick" && !ItemFilterSettings.WeaponMartialLightPick) return false;
					if (item.WeaponType == "Long Bow" && !ItemFilterSettings.WeaponMartialLongBow) return false;
					if (item.WeaponType == "Long Sword" && !ItemFilterSettings.WeaponMartialLongSword) return false;
					if (item.WeaponType == "Maul" && !ItemFilterSettings.WeaponMartialMaul) return false;
					if (item.WeaponType == "Rapier" && !ItemFilterSettings.WeaponMartialRapier) return false;
					if (item.WeaponType == "Scimitar" && !ItemFilterSettings.WeaponMartialScimitar) return false;
					if (item.WeaponType == "Short Bow" && !ItemFilterSettings.WeaponMartialShortBow) return false;
					if (item.WeaponType == "Short Sword" && !ItemFilterSettings.WeaponMartialShortSword) return false;
					if (item.WeaponType == "War Hammer" && !ItemFilterSettings.WeaponMartialWarHammer) return false;
				}
				else if ((WeaponCategory)item.Category == WeaponCategory.Exotic)
				{
					if (item.WeaponType == "Bastard Sword" && !ItemFilterSettings.WeaponExoticBastardSword) return false;
					if (item.WeaponType == "Dwarven War Axe" && !ItemFilterSettings.WeaponExoticDwarvenWarAxe) return false;
					if (item.WeaponType == "Great Crossbow" && !ItemFilterSettings.WeaponExoticGreatXbow) return false;
					if (item.WeaponType == "Handwraps" && !ItemFilterSettings.WeaponExoticHandwraps) return false;
					if (item.WeaponType == "Kama" && !ItemFilterSettings.WeaponExoticKama) return false;
					if (item.WeaponType == "Khopesh" && !ItemFilterSettings.WeaponExoticKhopesh) return false;
					if (item.WeaponType == "Repeating Heavy Crossbow" && !ItemFilterSettings.WeaponExoticRepeatHeavyXbow) return false;
					if (item.WeaponType == "Repeating Light Crossbow" && !ItemFilterSettings.WeaponExoticRepeatLightXbow) return false;
				}
				else if ((WeaponCategory)item.Category == WeaponCategory.Throwing)
				{
					if (item.WeaponType == "Throwing Axe" && !ItemFilterSettings.WeaponThrowingAxe) return false;
					if (item.WeaponType == "Throwing Dagger" && !ItemFilterSettings.WeaponThrowingDagger) return false;
					if (item.WeaponType == "Dart" && !ItemFilterSettings.WeaponThrowingDart) return false;
					if (item.WeaponType == "Throwing Hammer" && !ItemFilterSettings.WeaponThrowingHammer) return false;
					if (item.WeaponType == "Shuriken" && !ItemFilterSettings.WeaponThrowingShuriken) return false;
				}
			}

			if (ItemFilterSettings.SearchProperty != null && !item.Properties.Exists(i => i.Property == ItemFilterSettings.SearchProperty)) return false;

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
			ItemFilterSettings.OffhandRuneArm = tbFilterOffhand.IsChecked.Value ? cmiFilterOffhandRuneArm.IsChecked : false;
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
			MenuItem mi = e.Source as MenuItem;
			bool flip = !mi.IsChecked;
			if (mi.HasItems) mi.IsChecked = !mi.IsChecked;
			foreach (MenuItem cmi in mi.Items)
				cmi.IsChecked = flip;
		}

		GearSet CalculateGearSet(bool render)
		{
			GearSet gs = new GearSet();
			for (int i = 0; i < EquipmentSlots.Length; i++)
				if (EquipmentSlots[i].Item != null) gs.AddItem(EquipmentSlots[i].Item, null);
			gs.ProcessItems(dataset);

			if (!render) return gs;

			TreeView tv = new TreeView();
			tciGearSet.Content = tv;
			foreach (var ip in gs.Properties)
			{
				TreeViewItem tvi = new TreeViewItem();
				if (ip.IsGroup)
				{
					if (ip.ItemProperties[0].Type == "set")
					{
						DDOItemSet set = dataset.Sets[ip.Property];
						DDOItemSetBonus sb = null;
						foreach (var sbs in set.SetBonuses)
						{
							if (sbs.MinimumItems > ip.ItemProperties.Count) break;
							sb = sbs;
						}
						// no need to see a set listed that we're not getting anything from
						if (sb == null) tvi.Header = ip.Property + " set (no bonuses)";
						else tvi.Header = ip.Property + " set (" + sb.MinimumItems + " pieces)";
					}
					else tvi.Header = ip.Property;
				}
				else tvi.Header = ip.Property + (ip.TotalValue != 0 ? " " + ip.TotalValue : "");
				tv.Items.Add(tvi);
				TreeViewItem tvii;
				string lasttype = null;
				foreach (var p in ip.ItemProperties)
				{
					string source = p.Owner?.Name;
					if (source == null)
					{
						source = p.SetBonusOwner + " set";
						tvi.Background = Brushes.DeepSkyBlue;
					}
					tvii = new TreeViewItem();
					string l = null;
					if (ip.Property == "Damage Reduction") l = ((int)p.Value).ToString() + "/" + p.Type;
					else if (ip.Property == "Augment Slot") l = p.Type + " (" + source + ")";
					else if (p.Type == "set") l = p.Owner.Name;
					else
					{
						if (string.IsNullOrWhiteSpace(p.Type) && p.Value == 0) l = source;
						else
						{
							if (!string.IsNullOrWhiteSpace(p.Type)) l += p.Type + " ";
							l += p.Value + " (" + source + ")";
						}
						if (!ip.IsGroup && !string.IsNullOrWhiteSpace(lasttype) && p.Type == lasttype)
						{
							tvii.Foreground = Brushes.Red;
							tvi.Foreground = Brushes.Red;
						}
						lasttype = p.Type;
					}
					tvii.Header = l;

					tvi.Items.Add(tvii);
				}
			}

			return gs;
		}

		bool SlotItem(DDOItemData item, SlotType slot)
		{
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
			// one-handed weapons can be placed in the offhand slot if the weapon slot is locked
			else if (item.Slot == SlotType.Weapon)
			{
				// this special case bypasses sanity checking
				// the assumption is that this isn't passed without validation having already been done
				if (slot == SlotType.Offhand && item.Handedness == 1)
				{
					esc = EquipmentSlots[(int)EquipmentSlotType.Offhand];
				}
				else
				{
					if (EquipmentSlots[(int)EquipmentSlotType.Weapon].IsLocked)
					{
						if (item.Handedness == 1)
						{
							if (EquipmentSlots[(int)EquipmentSlotType.Offhand].IsLocked)
							{
								MessageBox.Show("Can't load a weapon into a locked weapon nor offhand slot.", "Slot Locked", MessageBoxButton.OK, MessageBoxImage.Stop);
								return false;
							}
							else esc = EquipmentSlots[(int)EquipmentSlotType.Offhand];
						}
					}
					else if (item.Handedness == 2)
					{
						if (EquipmentSlots[(int)EquipmentSlotType.Offhand].IsLocked)
						{
							MessageBox.Show("Can't load a two-handed weapon with a locked offhand slot.", "Slot Locked", MessageBoxButton.OK, MessageBoxImage.Stop);
							return false;
						}
						else esc = EquipmentSlots[(int)EquipmentSlotType.Weapon];
					}
					else esc = EquipmentSlots[(int)EquipmentSlotType.Weapon];
				}
			}
			else
			{
				esc = EquipmentSlots[(int)(EquipmentSlotType)Enum.Parse(typeof(EquipmentSlotType), item.Slot.ToString())];
			}

			if (esc == null || esc.IsLocked)
			{
				MessageBox.Show("Can't load an item into a locked slot.", "Slot Locked", MessageBoxButton.OK, MessageBoxImage.Stop);
				return false;
			}
			else
			{
				esc.SetItem(item);
				// slotting a two-handed weapon means ensuring the offhand slot is empty
				if (item.Handedness == 2) EquipmentSlots[(int)EquipmentSlotType.Offhand].SetItem(null);
				return true;
			}
		}

		private void LvItemList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			// validate user double-clicked over an item
			if (lvItemList.SelectedItem == null) return;

			DDOItemData item = lvItemList.SelectedItem as DDOItemData;
			if (SlotItem(item, SlotType.None))
			{
				CalculateGearSet(true);
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

			CalculateGearSet(true);
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

			CalculateGearSet(true);
		}

		private void ImportNamedSet(object sender, RoutedEventArgs e)
		{
			NamedSetSelectorWindow sw = new NamedSetSelectorWindow();
			sw.Owner = this;
			sw.Initialize(dataset, EquipmentSlots);
			if (sw.ShowDialog().Value)
			{
				List<DDOItemData> items = sw.GetItems();
				bool w = false;
				foreach (var item in items)
				{
					if (item.Slot == SlotType.Weapon && w) SlotItem(item, SlotType.Offhand);
					else
					{
						SlotItem(item, SlotType.None);
						if (item.Slot == SlotType.Weapon) w = true;
					}
				}

				CalculateGearSet(true);
			}
		}

		private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Right click any item to open up the wiki page for it (same for named sets).");
			sb.AppendLine("Double-click an item in the list to assign it to an appropriate slot.");
			sb.AppendLine("Lock an equipment slot to prevent it from being modified.");
			sb.AppendLine("Two-handed weapons will lock/unlock the offhand slot.");

			MessageBox.Show(sb.ToString());
		}

		private void ItemList_HeaderClick(object sender, RoutedEventArgs e)
		{
			string header = (e.OriginalSource as GridViewColumnHeader)?.Content.ToString();
			if (header == null) return;
			if (header != LastSortBy)
			{
				LastSortBy = header;
				LastSortDir = 1;
			}
			else LastSortDir *= -1;
			if (header == "Name") ItemListCopy.Sort((a, b) => string.Compare(a.Name, b.Name) * LastSortDir);
			else if (header == "Slot") ItemListCopy.Sort((a, b) => string.Compare(a.Slot.ToString(), b.Slot.ToString()) == 0 ? string.Compare(a.Name, b.Name) : string.Compare(a.Slot.ToString(), b.Slot.ToString()));
			else if (header == "ML") ItemListCopy.Sort((a, b) => a.ML < b.ML ? -1 * LastSortDir : (a.ML > b.ML ? 1 * LastSortDir : string.Compare(a.Name, b.Name)));

			CollectionViewSource.GetDefaultView(lvItemList.ItemsSource).Refresh();
		}

		private void MLRangeChanged(RangeSlider slider, double oldvalue, double newvalue)
		{
			if (slider == rsML)
			{
				ItemFilterSettings.MinimumLevel = (int)rsML.LowerValue;
				ItemFilterSettings.MaximumLevel = (int)rsML.UpperValue;
				grpML.Header = "ML Range: " + ItemFilterSettings.MinimumLevel + " to " + ItemFilterSettings.MaximumLevel;
				CollectionViewSource.GetDefaultView(lvItemList.ItemsSource).Refresh();
			}
		}

		private void EncodeGearsetToClipboard(object sender, RoutedEventArgs e)
		{
			string raw = "";
			foreach (var es in EquipmentSlots)
			{
				if (es.Item != null)
				{
					if (raw == "") raw = es.Item.Name;
					else raw += "|" + es.Item.Name;
				}
			}

			using (MemoryStream output = new MemoryStream())
			{
				using (DeflateStream gzip = new DeflateStream(output, CompressionMode.Compress))
				{
					using (StreamWriter writer = new StreamWriter(gzip, Encoding.UTF8))
					{
						writer.Write(raw);
					}
				}
				raw = Z85Extended.Encode(output.ToArray());
			}

			Clipboard.Clear();
			Clipboard.SetData(DataFormats.Text, raw);
		}

		private void DecodeGearsetFromClipboard(object sender, RoutedEventArgs e)
		{
			try
			{
				string cdata = Clipboard.GetData(DataFormats.Text).ToString();
				byte[] input = Z85Extended.Decode(cdata);

				using (MemoryStream inputStream = new MemoryStream(input))
				{
					using (DeflateStream gzip = new DeflateStream(inputStream, CompressionMode.Decompress))
					{
						using (StreamReader reader = new StreamReader(gzip, Encoding.UTF8))
						{
							cdata = reader.ReadToEnd();
						}
					}
				}

				UnlockClearAll(null, null);
				string[] split = cdata.Split('|');
				foreach (var s in split)
				{
					DDOItemData item = dataset.Items.Find(i => i.Name == s);
					if (item != null) SlotItem(item, SlotType.None);
				}
				CalculateGearSet(true);
			}
			catch (Exception ex)
			{
				MessageBox.Show("There was an error with the data in the clipboard. Copy the code and try again.");
			}
		}

		private void ItemPropertyFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cbItemPropertyFilter.SelectedIndex == 0) ItemFilterSettings.SearchProperty = null;
			else ItemFilterSettings.SearchProperty = (cbItemPropertyFilter.SelectedItem as DDOItemProperty).Property;

			CollectionViewSource.GetDefaultView(lvItemList.ItemsSource).Refresh();
		}

		private void ViewItemPropertyReport(object sender, RoutedEventArgs e)
		{
			// check for item property report window already open
			//   if so, bring it to foreground/focus
			//   if not, create it
		}
	}
}
