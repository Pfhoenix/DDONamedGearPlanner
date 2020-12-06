using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using Microsoft.Win32;
using CoenM.Encoding;


namespace DDONamedGearPlanner
{
	/// <summary>
	/// Interaction logic for PlannerWindow.xaml
	/// </summary>
	public partial class PlannerWindow : Window
	{
		public static readonly string VERSION = "0.8.6";

		public GearSetBuild CurrentBuild = new GearSetBuild();

		Dictionary<EquipmentSlotType, EquipmentSlotControl> EquipmentSlots = new Dictionary<EquipmentSlotType, EquipmentSlotControl>();
		EquipmentSlotControl SelectedESC;

		// item search stuff
		List<DDOItemData> ItemListCopy;
		string LastSortBy;
		int LastSortDir = 1;

		List<DDOItemProperty> ItemPropertiesCopy;

		public PlannerWindow()
		{
			InitializeComponent();

			Title += VERSION;
			CurrentBuild.AppVersion = VERSION;

			string error = DatasetManager.Load();
			if (error != null)
			{
				MessageBox.Show("Error loading DDO dataset - " + error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				Close();
				return;
			}

			LoadQuestSourceSettings();

			CustomItemsManager.Load();

			BtnFilterApply_Click(null, null);

			ItemListCopy = new List<DDOItemData>(DatasetManager.Dataset.Items);
			lvItemList.ItemsSource = ItemListCopy;
			SetFilter(CustomFilter);

			ItemPropertiesCopy = new List<DDOItemProperty>();
			foreach (var ip in DatasetManager.Dataset.ItemProperties)
				if (ip.Key == "Armor Category" || ip.Key == "Weapon Category" || ip.Key == "Offhand Category") continue;
				else if (ip.Value.Items.Count > 0) ItemPropertiesCopy.Add(ip.Value);
			ItemPropertiesCopy.Sort((a, b) => string.Compare(a.Property, b.Property));
			ItemPropertiesCopy.Insert(0, new DDOItemProperty { Property = "< All >" });
			cbItemPropertyFilter.ItemsSource = ItemPropertiesCopy;
			cbItemPropertyFilter.SelectedIndex = 0;

			txtSearchBox.Focus();
		}

		public List<BuildItem> GetEquippedItems()
		{
			return EquipmentSlots.Where(es => es.Value.Item != null).Select(es => es.Value.Item).ToList();
		}

		void LoadQuestSourceSettings()
		{
			QuestSourceManager.InitializeFromDataset();

			string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "questsources.cfg");
			if (File.Exists(filepath))
			{
				string[] lines = File.ReadAllLines(filepath);
				foreach (var line in lines)
				{
					string[] split = line.Split('=');
					if (split.Length < 2) continue;
					split[0] = split[0].Trim();
					QuestSourceManager.SetQuestSourceAllowed(split[0].Trim(), split[1].Trim() == "yes", false);
				}
				foreach (var tmi in miQuestSources.Items)
				{
					MenuItem mi = tmi as MenuItem;
					if (mi == null) break;
					mi.IsChecked = QuestSourceManager.IsAllowed(mi.Header.ToString());
				}
			}

			QuestSourceManager.SaveSettings();
		}

		public void RegisterEquipmentSlot(EquipmentSlotControl esc)
		{
			EquipmentSlots[esc.Slot] = esc;
			esc.EquipmentSlotClicked += EquipmentSlotClicked;
			esc.EquipmentSlotCleared += EquipmentSlotCleared;
			esc.EquipmentSlotLockChanged += EquipmentSlotLockChanged;
			if (esc.Slot == EquipmentSlotType.Finger1 || esc.Slot == EquipmentSlotType.Finger2) esc.SlotType = SlotType.Finger;
			else esc.SlotType = (SlotType)Enum.Parse(typeof(SlotType), esc.Slot.ToString());
		}

		private void EquipmentSlotLockChanged(EquipmentSlotControl esc)
		{
			CurrentBuild.SetLockStatus(esc.Slot, esc.IsLocked);

			/*
			// if weapon slot and item is two-handed, set the offhand slot lock to the same state
			if (esc.Item != null && esc.Slot == EquipmentSlotType.Weapon)
			{
				if (esc.Item.Item.Handedness == 2)
				{
					EquipmentSlots[EquipmentSlotType.Offhand].SetLockStatus(esc.IsLocked);
					CurrentBuild.SetLockStatus(EquipmentSlotType.Offhand, esc.IsLocked);
				}
			}
			// if offhand slot and weapon slot has two-handed item in it, reset offhand slot lock to weapon slot lock
			else if (esc.Slot == EquipmentSlotType.Offhand)
			{
				var item = EquipmentSlots[EquipmentSlotType.Weapon].Item;
				if (item != null && item.Item.Handedness == 2)
				{
					esc.SetLockStatus(EquipmentSlots[EquipmentSlotType.Weapon].IsLocked);
					CurrentBuild.SetLockStatus(EquipmentSlotType.Offhand, esc.IsLocked);
				}
			}*/
		}

		bool IsMinorArtifactSlotted()
		{
			foreach (var es in EquipmentSlots)
			{
				if (es.Value.Item == null) continue;
				if (es.Value.Item.Item.MinorArtifact) return true;
			}

			return false;
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
			if (!QuestSourceManager.IsItemAllowed(item)) return false;

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

			if (ItemFilterSettings.SearchProperty != null && !item.Properties.Exists(i => i.Property == ItemFilterSettings.SearchProperty || (i.Options != null && i.Options.Exists(o => o.Property == ItemFilterSettings.SearchProperty)))) return false;

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

		void UpdateItemSearchResults()
		{
			CollectionViewSource.GetDefaultView(lvItemList.ItemsSource).Refresh();
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

			if (sender != null) UpdateItemSearchResults();
		}

		private void TxtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateItemSearchResults();
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

		void RenderGearSet(GearSet gs)
		{
			TreeView tv = new TreeView();
			tciGearSet.Content = tv;
			foreach (var ip in gs.Properties)
			{
				if (DatasetManager.CategoryProperties.Contains(ip.Property)) continue;

				TreeViewItem tvi = new TreeViewItem();
				if (ip.IsGroup)
				{
					if (ip.ItemProperties[0].Type == "set")
					{
						DDOItemSet set = DatasetManager.Dataset.Sets[ip.Property];
						DDOItemSetBonus sb = set.GetSetBonuses(ip.ItemProperties);
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
							l += (string.IsNullOrWhiteSpace(p.Type) ? "untyped" : p.Type) + " ";
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
		}

		public GearSet CalculateGearSet(bool render)
		{
			GearSet gs = new GearSet();
			foreach (var kv in EquipmentSlots)
				if (kv.Value.Item != null) gs.AddItem(kv.Value.Item);
			gs.ProcessItems();

			if (render) RenderGearSet(gs);

			return gs;
		}

		EquipmentSlotType SlotItem(DDOItemData item, SlotType slot)
		{
			return SlotItem(new BuildItem(item, EquipmentSlotType.None), slot);
		}

		// this finds a slot to put the item into
		EquipmentSlotType SlotItem(BuildItem item, SlotType slot)
		{
			EquipmentSlotControl esc = null;

			// get the slot the item belongs in
			//  - special case check for an open finger slot
			//  - if both finger slots are full, check for one not locked
			if (item.Item.Slot == SlotType.Finger)
			{
				if (EquipmentSlots[EquipmentSlotType.Finger1].Item == null) esc = EquipmentSlots[EquipmentSlotType.Finger1];
				else if (EquipmentSlots[EquipmentSlotType.Finger2].Item == null) esc = EquipmentSlots[EquipmentSlotType.Finger2];
				else if (!EquipmentSlots[EquipmentSlotType.Finger1].IsLocked) esc = EquipmentSlots[EquipmentSlotType.Finger1];
				else if (!EquipmentSlots[EquipmentSlotType.Finger2].IsLocked) esc = EquipmentSlots[EquipmentSlotType.Finger2];
			}
			// one-handed weapons can be placed in the offhand slot if the weapon slot is locked
			else if (item.Item.Slot == SlotType.Weapon)
			{
				// this special case bypasses sanity checking
				// the assumption is that this isn't passed without validation having already been done
				if (slot == SlotType.Offhand && item.Item.Handedness == 1)
				{
					esc = EquipmentSlots[EquipmentSlotType.Offhand];
				}
				else
				{
					if (EquipmentSlots[EquipmentSlotType.Weapon].IsLocked)
					{
						if (item.Item.Handedness == 1 && (EquipmentSlots[EquipmentSlotType.Weapon].Item == null || EquipmentSlots[EquipmentSlotType.Weapon].Item.Item.Handedness == 1))
						{
							if (EquipmentSlots[EquipmentSlotType.Offhand].IsLocked)
							{
								MessageBox.Show("Can't load a weapon into a locked weapon nor offhand slot.", "Slot Locked", MessageBoxButton.OK, MessageBoxImage.Stop);
								return EquipmentSlotType.None;
							}
							else esc = EquipmentSlots[EquipmentSlotType.Offhand];
						}
					}
					else if (item.Item.Handedness == 2)
					{
						if (EquipmentSlots[EquipmentSlotType.Offhand].Item != null)
						{
							if (DatasetManager.CanBeUsedTogether(item.Item, EquipmentSlots[EquipmentSlotType.Offhand].Item.Item)) esc = EquipmentSlots[EquipmentSlotType.Weapon];
							else
							{
								MessageBox.Show("Can't load a two-handed weapon with an incompatible offhand item slotted.", "Item Incompatibility Detected", MessageBoxButton.OK, MessageBoxImage.Stop);
								return EquipmentSlotType.None;
							}
						}
						else esc = EquipmentSlots[EquipmentSlotType.Weapon];
					}
					else esc = EquipmentSlots[EquipmentSlotType.Weapon];
				}
			}
			else if (item.Item.Slot == SlotType.Offhand)
			{
				BuildItem w = EquipmentSlots[EquipmentSlotType.Weapon].Item;
				if (w != null && w.Item.Handedness == 2)
				{
					if (DatasetManager.CanBeUsedTogether(w.Item, item.Item)) esc = EquipmentSlots[EquipmentSlotType.Offhand];
					else
					{
						MessageBox.Show("Can't load an offhand item when a two-handed weapon is slotted.", "Item Incompatibility Detected", MessageBoxButton.OK, MessageBoxImage.Stop);
						return EquipmentSlotType.None;
					}
				}
				else esc = EquipmentSlots[EquipmentSlotType.Offhand];
			}
			else
			{
				esc = EquipmentSlots[(EquipmentSlotType)Enum.Parse(typeof(EquipmentSlotType), item.Item.Slot.ToString())];
			}

			if (esc == null || esc.IsLocked)
			{
				MessageBox.Show("Can't load an item into a locked slot.", "Slot Locked", MessageBoxButton.OK, MessageBoxImage.Stop);
				return EquipmentSlotType.None;
			}
			else if (item.Item.MinorArtifact && IsMinorArtifactSlotted())
			{
				MessageBox.Show("Can't equip more than one minor artifact.", "Minor Artifact Limit", MessageBoxButton.OK, MessageBoxImage.Stop);
				return EquipmentSlotType.None;
			}
			else
			{
				esc.SetItem(item);
				// slotting a two-handed weapon means ensuring the offhand slot is empty
				/*if (item.Item.Handedness == 2)
				{
					if (!DatasetManager.RuneArmCompatibleTwoHandedWeaponTypes.Contains(item.Item.WeaponType) || (EquipmentSlots[EquipmentSlotType.Offhand].Item != null && (OffhandCategory)EquipmentSlots[EquipmentSlotType.Offhand].Item.Item.Category != OffhandCategory.RuneArm))
						EquipmentSlots[EquipmentSlotType.Offhand].SetItem(null);
				}*/
				return esc.Slot;
			}
		}

		// this is for slotting a build item into a particular (set) slot
		public bool SlotItem(BuildItem item, bool suppressErrors = false)
		{
			if (item.Slot == EquipmentSlotType.None || EquipmentSlots[item.Slot].IsLocked)
			{
				if (!suppressErrors) MessageBox.Show("Can't load an item into a locked slot.", "Slot Locked", MessageBoxButton.OK, MessageBoxImage.Stop);
				return false;
			}

			if (item.Slot == EquipmentSlotType.Weapon)
			{
				if (item.Item.Handedness == 2)
				{
					if (EquipmentSlots[EquipmentSlotType.Offhand].IsLocked)
					{
						if (!suppressErrors) MessageBox.Show("Can't load a two-handed weapon with a locked offhand slot.", "Slot Locked", MessageBoxButton.OK, MessageBoxImage.Stop);
						return false;
					}
					else EquipmentSlots[EquipmentSlotType.Offhand].SetItem(null);
				}
			}
			else if (item.Slot == EquipmentSlotType.Offhand)
			{
				if (EquipmentSlots[EquipmentSlotType.Weapon].Item != null)
				{
					if (EquipmentSlots[EquipmentSlotType.Weapon].Item.Item.Handedness == 2)
					{
						if (EquipmentSlots[EquipmentSlotType.Weapon].IsLocked)
						{
							if (!suppressErrors) MessageBox.Show("Can't load an offhand item with a locked two-handed weapon locked.", "Slot Locked", MessageBoxButton.OK, MessageBoxImage.Stop);
							return false;
						}
						else EquipmentSlots[EquipmentSlotType.Weapon].SetItem(null);
					}
				}
			}

			if (item.Item.MinorArtifact && IsMinorArtifactSlotted())
			{
				if (!suppressErrors) MessageBox.Show("Can't equip more than one minor artifact.", "Minor Artifact Limit", MessageBoxButton.OK, MessageBoxImage.Stop);
				return false;
			}

			EquipmentSlots[item.Slot].SetItem(item);
			return true;
		}

		private void LvItemList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			// validate user double-clicked over an item
			if (lvItemList.SelectedItem == null) return;

			DDOItemData item = lvItemList.SelectedItem as DDOItemData;
			if (SlotItem(item, SlotType.None) != EquipmentSlotType.None)
			{
				CalculateGearSet(true);
			}
		}

		void IPBWindow_ItemDoubleClicked(DDOItemData item)
		{
			if (SlotItem(item, SlotType.None) != EquipmentSlotType.None)
			{
				CalculateGearSet(true);
			}
		}

		private void EquipmentSlotClicked(EquipmentSlotControl esc, MouseButton button)
		{
			if (button == MouseButton.Right)
			{
				if (esc.Item == null) return;
				if (esc.Item.Item.Source == ItemDataSource.Dataset) System.Diagnostics.Process.Start(esc.Item.Item.WikiURL);
				else
				{
					ManageCustomItems(null, null);
					CIWindow.SelectItem(esc.Item.Item.Name, esc.Item.Item.Slot.ToString());
				}
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

		TabItem CreateItemPropertiesTab(BuildItem bi)
		{
			TabItem nti = new TabItem();
			nti.MouseRightButtonUp += PropertyTabRightButtonUp;
			tcPropertyAreas.Items.Add(nti);
			tcPropertyAreas.SelectedItem = nti;

			ListViewItemProperties lvip = new ListViewItemProperties();
			nti.Content = lvip;
			lvip.SetItem(bi);
			lvip.MouseDoubleClick += ItemPropertyTab_MouseDoubleClicked;

			return nti;
		}

		private void ItemPropertyTab_MouseDoubleClicked(object sender, MouseButtonEventArgs e)
		{
			ListViewItemProperties lvip = sender as ListViewItemProperties;
			if (lvip.Item == null) return;
			if (!lvip.Item.InUse) return;
			ListViewItem lvi = lvip.lvDetails.SelectedItem as ListViewItem;
			if (lvi == null) return;
			ItemProperty ip = lvi.Tag as ItemProperty;
			if (ip == null) return;

			// this is an optional property that the user wants to unset
			if (lvip.Item.OptionProperties.Contains(ip))
			{
				lvip.Item.OptionProperties.Remove(ip);
			}
			// this is an optional property that the user wants to set
			else
			{
				lvip.Item.OptionProperties.Add(ip);
			}

			lvip.SetItem(lvip.Item);
			if (lvip.Item.InUse) CalculateGearSet(true);
		}

		void OpenPropertiesTab(BuildItem bi)
		{
			if (bi == null) return;

			bool optionals = bi.OptionProperties.Count > 0 || bi.Item.Properties.Exists(p => p.Options != null && !p.HideOptions);

			// first search for an existing tab for the item
			foreach (TabItem ti in tcPropertyAreas.Items)
			{
				if (optionals && ti.Tag == bi || (!optionals && ti.Header.ToString() == bi.Item.Name))
				{
					tcPropertyAreas.SelectedItem = ti;
					return;
				}
			}

			TabItem nti = CreateItemPropertiesTab(bi);
			nti.Tag = bi;
			nti.Header = bi.Item.Name + (optionals ? " *" : "");
		}

		void OpenPropertiesTab(DDOItemData item)
		{
			if (item == null) return;

			bool optionals = item.Properties.Exists(p => p.Options != null && !p.HideOptions);

			foreach (TabItem ti in tcPropertyAreas.Items)
			{
				if (optionals && ti.Tag != null) continue;
				if (ti.Header.ToString() == item.Name)
				{
					tcPropertyAreas.SelectedItem = ti;
					return;
				}
			}

			TabItem nti = CreateItemPropertiesTab(new BuildItem(item, EquipmentSlotType.None));
			nti.Header = item.Name;
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

		public void UnlockClearAll(object sender, RoutedEventArgs e)
		{
			foreach (var kv in EquipmentSlots)
			{
				kv.Value.SetLockStatus(false);
				kv.Value.SetItem(null);
			}

			CurrentBuild.LockedSlots.Clear();

			CalculateGearSet(true);
		}

		private void ImportNamedSet(object sender, RoutedEventArgs e)
		{
			NamedSetSelectorWindow sw = new NamedSetSelectorWindow();
			sw.Owner = this;
			sw.Initialize(EquipmentSlots);
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

			UpdateItemSearchResults();
		}

		private void MLRangeChanged(RangeSlider slider, double oldvalue, double newvalue)
		{
			if (slider == rsML)
			{
				ItemFilterSettings.MinimumLevel = (int)rsML.LowerValue;
				ItemFilterSettings.MaximumLevel = (int)rsML.UpperValue;
				grpML.Header = "ML Range: " + ItemFilterSettings.MinimumLevel + " to " + ItemFilterSettings.MaximumLevel;
				UpdateItemSearchResults();
			}
		}

		string EncodeString(string raw)
		{
			using (MemoryStream output = new MemoryStream())
			{
				using (DeflateStream gzip = new DeflateStream(output, CompressionMode.Compress))
				{
					using (StreamWriter writer = new StreamWriter(gzip, Encoding.UTF8))
					{
						writer.Write(raw);
					}
				}
				return Z85Extended.Encode(output.ToArray());
			}
		}

		string DecodeString(string cdata)
		{
			byte[] input = Z85Extended.Decode(cdata);

			using (MemoryStream inputStream = new MemoryStream(input))
			{
				using (DeflateStream gzip = new DeflateStream(inputStream, CompressionMode.Decompress))
				{
					using (StreamReader reader = new StreamReader(gzip, Encoding.UTF8))
					{
						return reader.ReadToEnd();
					}
				}
			}
		}

		/*string EncodeGearset()
		{
			string raw = "";
			foreach (var es in EquipmentSlots)
			{
				if (es.Value.Item != null)
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(es.Value.Item.Item.Name);
					foreach (var op in es.Value.Item.OptionProperties)
					{
						sb.Append("{");
						sb.Append(op.Property + ";" + (string.IsNullOrWhiteSpace(op.Type) ? "untyped" : op.Type) + ";" + op.Value);
					}
					if (raw == "") raw = sb.ToString();
					else raw += "|" + sb.ToString();
				}
			}

			return EncodeString(raw);
		}*/

		private void SendGearsetToClipboard(object sender, RoutedEventArgs e)
		{
			Clipboard.Clear();
			Clipboard.SetData(DataFormats.Text, SerializeGearset());
		}

		GearSet DecodeGearset(string cdata, bool render)
		{
			try
			{
				cdata = DecodeString(cdata);

				UnlockClearAll(null, null);
				string[] split = cdata.Split('|');
				foreach (var s in split)
				{
					string[] itemsplit = s.Split('{');
					DDOItemData item = DatasetManager.Dataset.Items.Find(i => i.Name == itemsplit[0]);
					// attempt to find the item as a loaded custom item
					if (item == null)
					{
						ACustomItemContainer cic = CustomItemsManager.CustomItems.Find(i => i.Name == itemsplit[0]);
						if (cic != null) item = cic.GetItem();
					}
					if (item != null)
					{
						BuildItem bi = new BuildItem(item, EquipmentSlotType.None);
						for (int i = 1; i < itemsplit.Length; i++)
						{
							string[] ps = itemsplit[i].Split(';');
							var optionals = item.Properties.Where(p => p.Options != null && !p.HideOptions).ToList();
							foreach (var op in optionals)
							{
								ItemProperty ot = op.Options.Find(o => o.Property == ps[0] && (o.Type == ps[1] || (string.IsNullOrWhiteSpace(o.Type) && ps[1] == "untyped")));
								if (ot != null)
								{
									bi.OptionProperties.Add(ot);
									break;
								}
							}
						}
						if (item.Slot == SlotType.Weapon && EquipmentSlots[EquipmentSlotType.Weapon].Item != null) SlotItem(bi, SlotType.Offhand);
						else SlotItem(bi, SlotType.None);
					}
				}

				return CalculateGearSet(render);
			}
			catch (Exception ex)
			{
				//MessageBox.Show("There was an error decoding the data. Check the source and try again.");
				return null;
			}
		}

		private void GetGearsetFromClipboard(object sender, RoutedEventArgs e)
		{
			UnserializeGearset(Clipboard.GetData(DataFormats.Text).ToString());
		}

		private void ItemPropertyFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cbItemPropertyFilter.SelectedIndex == 0) ItemFilterSettings.SearchProperty = null;
			else ItemFilterSettings.SearchProperty = (cbItemPropertyFilter.SelectedItem as DDOItemProperty).Property;

			UpdateItemSearchResults();
		}

		ItemPropertyBrowserWindow IPBWindow;
		private void ViewItemPropertyBrowser(object sender, RoutedEventArgs e)
		{
			if (IPBWindow == null || !IPBWindow.IsActive)
			{
				IPBWindow = new ItemPropertyBrowserWindow();
				IPBWindow.Owner = this;
				IPBWindow.ItemDoubleClicked += IPBWindow_ItemDoubleClicked;
			}
			
			IPBWindow.Show();
		}

		private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
		{
			AboutWindow aw = new AboutWindow();
			aw.Owner = this;
			aw.ShowDialog();
		}

		private void BuildFilters_Click(object sender, RoutedEventArgs e)
		{
			CurrentBuild.SetupLockedSlots(EquipmentSlots);
			BuildFiltersWindow bfw = new BuildFiltersWindow();
			bfw.Owner = this;
			bfw.Initialize(CurrentBuild, EquipmentSlots);
			bfw.ShowDialog();

			if (bfw.FiltersChanged)
			{
				if (CurrentBuild.BuildResults.Count != 0)
				{
					CurrentBuild.FiltersResultsMismatch = true;
					MessageBox.Show("By changing the build filters, the current build results will not match the new filter settings. If you save this build, the current build results will not be included in the save.", "Filters and results mismatch", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}

			btnStartBuild.IsEnabled = miSaveBuildFilters.IsEnabled = CurrentBuild.ValidateFilters(false);
			if (!btnStartBuild.IsEnabled) MessageBox.Show("A build cannot be started without a gear set or slot filter that includes an item property.", "Missing include filter", MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		void ResetBuildResultsUI()
		{
			rsBuildML.LowerValue = 1;
			rsBuildML.UpperValue = 30;
			btnStartBuild.IsEnabled = false;
			tbTotalGearSets.Text = "Gear Sets: 0";
			btnPreviousGS.IsEnabled = false;
			tbCurrentGS.Text = null;
			btnNextGS.IsEnabled = false;
			//tbCurrentGSRating.Text = "Rating:";
			//tbCurrentGSPenalty.Text = "Penalty:";
		}

		void SetBuildResult(int cbr)
		{
			UnlockClearAll(null, null);

			if (cbr < 0 || cbr >= CurrentBuild.BuildResults.Count) return;

			GearSetEvaluation br = CurrentBuild.BuildResults[cbr];
			tbTotalGearSets.Text = "Gear Sets: " + CurrentBuild.BuildResults.Count;
			btnPreviousGS.IsEnabled = cbr > 0;
			tbCurrentGS.Text = (cbr + 1).ToString();
			btnNextGS.IsEnabled = cbr < (CurrentBuild.BuildResults.Count - 1);
			//tbCurrentGSRating.Text = "Rating: " + br.Rating;
			//tbCurrentGSPenalty.Text = "Penalty: " + br.Penalty;

			foreach (BuildItem bi in br.GearSet.Items)
				SlotItem(bi);

			foreach (var esc in br.LockedSlots)
				EquipmentSlots[esc].SetLockStatus(true);

			RenderGearSet(br.GearSet);
		}

		private void PreviousGearSet_Click(object sender, RoutedEventArgs e)
		{
			if (CurrentBuild.CurrentBuildResult < 1) (sender as Button).IsEnabled = false;
			SetBuildResult(--CurrentBuild.CurrentBuildResult);
		}

		private void NextGearSet_Click(object sender, RoutedEventArgs e)
		{
			if (CurrentBuild.CurrentBuildResult >= CurrentBuild.BuildResults.Count - 2) (sender as Button).IsEnabled = false;
			SetBuildResult(++CurrentBuild.CurrentBuildResult);
		}

		private void BuildMLRangeChanged(RangeSlider slider, double oldvalue, double newvalue)
		{
			CurrentBuild.MinimumLevel = (int)slider.LowerValue;
			CurrentBuild.MaximumLevel = (int)slider.UpperValue;
		}

		private void ExportGearsetToFile(object sender, RoutedEventArgs e)
		{
			// bring up a save file dialog
			SaveFileDialog sfd = new SaveFileDialog();// { InitialDirectory = AppDomain.CurrentDomain.BaseDirectory };
			sfd.Filter = "Gear Set file (*.gearset)|*.gearset";
			sfd.AddExtension = true;
			if (sfd.ShowDialog() == true)
			{
				File.WriteAllText(sfd.FileName, SerializeGearset());
			}
		}

		private void ImportGearsetFromFile(object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();// { InitialDirectory = AppDomain.CurrentDomain.BaseDirectory };
			ofd.Filter = "Gear Set file (*.gearset)|*.gearset";
			if (ofd.ShowDialog() == true)
			{
				UnserializeGearset(File.ReadAllText(ofd.FileName));
			}
		}

		private void StartBuild_Click(object sender, RoutedEventArgs e)
		{
			if (CurrentBuild.BuildResults.Count > 0 && MessageBox.Show("This will overwrite the current build results. Are you sure?", "Overwrite Results", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
			{
				return;
			}

			GC.Collect();

			CurrentBuild.FiltersResultsMismatch = false;
			CurrentBuild.SetupBuildProcess(EquipmentSlots);

			miSaveBuildResults.IsEnabled = false;

			BuildProcessWindow bpw = new BuildProcessWindow();
			bpw.Initialize(CurrentBuild, false);
			bpw.Owner = this;
			if (bpw.ShowDialog() == true)
			{
				miSaveBuildResults.IsEnabled = true;

				CurrentBuild.CurrentBuildResult = 0;
				SetBuildResult(0);
			}

			GC.Collect();
		}

		string WriteXmlToString(XmlDocument doc)
		{
			using (var stringWriter = new StringWriter())
			using (var xmlTextWriter = XmlWriter.Create(stringWriter))
			{
				doc.WriteTo(xmlTextWriter);
				xmlTextWriter.Flush();
				return stringWriter.GetStringBuilder().ToString();
			}
		}

		void SaveBuild(bool filters, bool results)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "Build file (*.build)|*.build";
			sfd.AddExtension = true;
			if (sfd.ShowDialog() == false) return;

			CurrentBuild.SetupLockedSlots(EquipmentSlots);

			XmlDocument doc = CurrentBuild.ToXml(filters, results);

			string raw = WriteXmlToString(doc);
			File.WriteAllText(sfd.FileName, EncodeString(raw));
		}

		void LoadBuild(bool filters, bool results)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Build file (*.build)|*.build";
			if (ofd.ShowDialog() == false) return;

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(DecodeString(File.ReadAllText(ofd.FileName)));

			GearSetBuild gsb = GearSetBuild.FromXml(doc, filters, results);
			if (results)
			{
				if (!filters && gsb.BuildResults.Count > 0) CurrentBuild.FiltersResultsMismatch = true;
				CurrentBuild.BuildResults = gsb.BuildResults;
				if (CurrentBuild.BuildResults.Count == 0) ResetBuildResultsUI();
				else
				{
					CurrentBuild.CurrentBuildResult = 0;
					SetBuildResult(CurrentBuild.CurrentBuildResult);
				}
			}
			if (filters)
			{
				if (!results && CurrentBuild.BuildResults.Count > 0) CurrentBuild.FiltersResultsMismatch = true;
				rsBuildML.LowerValue = CurrentBuild.MinimumLevel = gsb.MinimumLevel;
				rsBuildML.UpperValue = CurrentBuild.MaximumLevel = gsb.MaximumLevel;
				CurrentBuild.Filters = gsb.Filters;
				btnStartBuild.IsEnabled = CurrentBuild.ValidateFilters(false);
			}
			if (filters && results) CurrentBuild.FiltersResultsMismatch = false;

			return;
		}

		private void NewBuild_Click(object sender, RoutedEventArgs e)
		{
			ResetBuildResultsUI();
			CurrentBuild.Clear();
			GC.Collect();
		}

		private void LoadBuild_Click(object sender, RoutedEventArgs e)
		{
			LoadBuild(true, true);
		}

		private void SaveBuild_Click(object sender, RoutedEventArgs e)
		{
			SaveBuild(true, !CurrentBuild.FiltersResultsMismatch);
		}

		private void LoadBuildFilters_Click(object sender, RoutedEventArgs e)
		{
			LoadBuild(true, false);
		}

		private void SaveBuildFilters_Click(object sender, RoutedEventArgs e)
		{
			SaveBuild(true, false);
		}

		private void LoadBuildResults_Click(object sender, RoutedEventArgs e)
		{
			LoadBuild(false, true);
		}

		private void SaveBuildResults_Click(object sender, RoutedEventArgs e)
		{
			SaveBuild(false, true);
		}

		private string SerializeGearset()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var es in EquipmentSlots)
			{
				if (es.Value.Item != null) sb.AppendLine(es.Value.Item.ToString(true));
			}

			// get the treeview control from tciGearSet
			// iterate over all treeviewitems in the treeview
			TreeView tv = tciGearSet.Content as TreeView;
			if (tv != null)
			{
				if (tv.HasItems)
				{
					if (sb.Length > 0) sb.AppendLine();
					foreach (TreeViewItem tvi in tv.Items)
					{
						string h = tvi.Header.ToString();
						if (h.Contains(" set (")) continue;
						sb.AppendLine(h);
					}
				}
			}

			return sb.ToString();
		}

		public GearSet UnserializeGearset(string text, bool render = true, bool suppressErrors = false)
		{
			UnlockClearAll(null, null);

			string[] gearset = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

			// first try to decode the old format, for backwards compatibility
			if (gearset.Length == 1) return DecodeGearset(gearset[0], render);

			foreach (string line in gearset)
			{
				string[] entry = line.Split(':');
				// we've hit a line that isn't a proper entry, so stop processing
				if (entry.Length < 2) break;
				EquipmentSlotType slot;
				try
				{
					slot = (EquipmentSlotType)Enum.Parse(typeof(EquipmentSlotType), entry[0]);
				}
				catch
				{
					break;
				}
				entry = entry[1].Split('{');
				DDOItemData item = DatasetManager.Dataset.Items.Find(i => i.Name == entry[0]);
				// attempt to find the item as a loaded custom item
				if (item == null)
				{
					item = CustomItemsManager.CustomItems.Find(i => i.Name == entry[0])?.GetItem();
					if (item != null) SlotItem(new BuildItem(item, slot), suppressErrors);
				}
				else
				{
					BuildItem bi = new BuildItem(item, slot);
					if (entry.Length > 1)
					{
						// go through all optional properties on item
						var optionals = item.Properties.Where(p => p.Options != null && !p.HideOptions).ToList();
						foreach (var ops in optionals)
						{
							foreach (var op in ops.Options)
							{
								bool found = false;
								string s = op.ToString().Substring(1);
								for (int e = 1; e < entry.Length; e++)
								{
									if (entry[e] == s)
									{
										entry[e] = null;
										found = true;
										break;
									}
								}
								if (found)
								{
									bi.OptionProperties.Add(op);
									break;
								}
							}
						}
					}

					SlotItem(bi, suppressErrors);
				}
			}

			return CalculateGearSet(render);
		}

		private void LockFilledSlots(object sender, RoutedEventArgs e)
		{
			CurrentBuild.LockedSlots.Clear();
			foreach (var kv in EquipmentSlots)
			{
				if (kv.Value.Item != null)
				{
					CurrentBuild.LockedSlots.Add(kv.Key);
					kv.Value.SetLockStatus(true);
				}
				else kv.Value.SetLockStatus(false);
			}
		}

		CustomItemsWindow CIWindow;
		private void ManageCustomItems(object sender, RoutedEventArgs e)
		{
			if (CIWindow == null || !CIWindow.IsActive)
			{
				CIWindow = new CustomItemsWindow();
				CIWindow.Owner = this;
				CIWindow.ItemDoubleClicked += CIWindow_ItemDoubleClicked;
				CIWindow.RefreshGearSet += RefreshGearSet;
				CIWindow.CustomItemsReloaded += CustomItemsReloaded;
				CIWindow.CustomItemChangedSlot += UnslotCustomItem;
				CIWindow.CustomItemDeleted += UnslotCustomItem;
			}

			CIWindow.Show();
		}

		void CIWindow_ItemDoubleClicked(DDOItemData item)
		{
			SlotItem(item, item.Slot);
			CalculateGearSet(true);
		}

		void RefreshGearSet()
		{
			CalculateGearSet(true);
		}

		void CustomItemsReloaded()
		{
			bool recalc = false;
			foreach (var kv in EquipmentSlots)
			{
				if (kv.Value.Item == null) continue;
				if (kv.Value.Item.Item.Source != ItemDataSource.Dataset)
				{
					// we match by name and source
					// it's up to the user to not have duplicate item names if they reload while using an item
					DDOItemData ni = CustomItemsManager.CustomItems.Find(i => i.Name == kv.Value.Item.Item.Name && i.Source == kv.Value.Item.Item.Source)?.GetItem();
					kv.Value.SetItem(new BuildItem(ni, EquipmentSlotType.None));
					recalc = true;
				}
			}

			if (recalc) CalculateGearSet(true);
		}

		void UnslotCustomItem(DDOItemData item)
		{
			foreach (var kv in EquipmentSlots)
			{
				if (kv.Value.Item == null) continue;
				if (kv.Value.Item.Item == item)
				{
					kv.Value.SetItem(null);
					CalculateGearSet(true);
					return;
				}
			}
		}

		void QuestSourceItem_Clicked(object sender, RoutedEventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			QuestSourceManager.SetQuestSourceAllowed(mi.Header.ToString(), mi.IsChecked, true);

			UpdateItemSearchResults();
		}

		private void ManageQuestSources_Clicked(object sender, RoutedEventArgs e)
		{
			QuestSourcesWindow qsw = new QuestSourcesWindow();
			qsw.Owner = this;
			qsw.ShowDialog();

			UpdateItemSearchResults();
		}

		private void CompareGearsets(object sender, RoutedEventArgs e)
		{
			GearSetComparisonWindow gsw = new GearSetComparisonWindow(this);
			gsw.Show();
		}
	}
}
