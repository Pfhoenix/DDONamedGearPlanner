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
	/// Interaction logic for EquipmentSlotControl.xaml
	/// </summary>
	public partial class EquipmentSlotControl : UserControl
	{
		public bool IsLocked = false;
		public DDOItemData Item;

		MainWindow ParentWindow;

		protected static BitmapImage[] SlotBGImages;
		protected static Dictionary<string, BitmapImage> SlotFillImages;

		public static readonly DependencyProperty EquipmentSlotProperty =
				DependencyProperty.Register
				(
					"Slot",
					typeof(EquipmentSlotType),
					typeof(EquipmentSlotControl),
					new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSlotChanged))
				);

		private static void OnSlotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (SlotBGImages == null)
			{
				string[] names = Enum.GetNames(typeof(EquipmentSlotType));
				SlotBGImages = new BitmapImage[names.Length];
				for (int i = 0; i < names.Length; i++)
					SlotBGImages[i] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_bg_" + names[i].ToLower() + ".png"));
			}

			if (SlotFillImages == null)
			{
				SlotFillImages = new Dictionary<string, BitmapImage>();
				SlotFillImages["back"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_back.png"));
				SlotFillImages["body_cloth"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_body_cloth.png"));
				SlotFillImages["body_light"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_body_light.png"));
				SlotFillImages["body_medium"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_body_medium.png"));
				SlotFillImages["body_heavy"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_body_heavy.png"));
				SlotFillImages["body_docent"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_body_docent.png"));
				SlotFillImages["eye"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_eye.png"));
				SlotFillImages["feet"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_feet.png"));
				SlotFillImages["finger"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_finger.png"));
				SlotFillImages["hand"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_hand.png"));
				SlotFillImages["head"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_head.png"));
				SlotFillImages["neck"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_neck.png"));
				SlotFillImages["offhand_buckler"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_offhand_buckler.png"));
				SlotFillImages["offhand_small"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_offhand_small.png"));
				SlotFillImages["offhand_large"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_offhand_large.png"));
				SlotFillImages["offhand_tower"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_offhand_tower.png"));
				SlotFillImages["offhand_orb"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_offhand_orb.png"));
				SlotFillImages["trinket"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_trinket.png"));
				SlotFillImages["waist"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_waist.png"));
				SlotFillImages["weapon"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_weapon.png"));
				SlotFillImages["wrist"] = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_fill_wrist.png"));
			}

			EquipmentSlotControl esc = d as EquipmentSlotControl;
			if (SlotBGImages != null) esc.imgIcon.Source = SlotBGImages[(int)esc.Slot];
		}

		static Brush LockBrushOn = new SolidColorBrush(Colors.Red);
		static Brush LockBrushOff = new SolidColorBrush(SystemColors.ControlDarkColor);
		static Brush SelectBrushOn = new SolidColorBrush(Colors.CornflowerBlue);
		static Brush SelectBrushOff = new SolidColorBrush(SystemColors.ControlDarkColor);

		public EquipmentSlotType Slot
		{
			get { return (EquipmentSlotType)GetValue(EquipmentSlotProperty); }
			set
			{
				SetValue(EquipmentSlotProperty, value);
				if (value == EquipmentSlotType.Finger1 || value == EquipmentSlotType.Finger2) SlotType = SlotType.Finger;
				else SlotType = (SlotType)Enum.Parse(typeof(SlotType), value.ToString());
			}
		}
		public SlotType SlotType;

		public delegate void EquipmentSlotClickedDelegate(EquipmentSlotControl esc, MouseButton button);
		public event EquipmentSlotClickedDelegate EquipmentSlotClicked;

		public delegate void EquipmentSlotClearedDelegate(EquipmentSlotControl esc);
		public event EquipmentSlotClearedDelegate EquipmentSlotCleared;

		public bool IsSelected;
		bool MouseLeftClicked;
		bool MouseRightClicked;

		public EquipmentSlotControl()
		{
			InitializeComponent();

			Loaded += EquipmentSlotControl_Loaded;
		}

		private void EquipmentSlotControl_Loaded(object sender, RoutedEventArgs e)
		{
			DependencyObject dobj = Parent;
			while (!(dobj is MainWindow))
			{
				if (dobj == null) break;
				dobj = VisualTreeHelper.GetParent(dobj);
			}
			if (dobj != null)
			{
				ParentWindow = dobj as MainWindow;
				ParentWindow.RegisterEquipmentSlot(this);
			}
		}

		void UpdateLockStatus()
		{
			if (tbLock.IsChecked.Value)
			{
				IsLocked = true;
				tbLock.Content = "Unlock";
				LockBorder.BorderBrush = LockBrushOn;
			}
			else
			{
				IsLocked = false;
				tbLock.Content = "Lock";
				LockBorder.BorderBrush = LockBrushOff;
			}
		}

		private void TbLock_Click(object sender, RoutedEventArgs e)
		{
			UpdateLockStatus();
		}

		public void SetSelectBorder(bool on)
		{
			if (on) SelectBorder.BorderBrush = SelectBrushOn;
			else SelectBorder.BorderBrush = SelectBrushOff;
		}

		public void SetItem(DDOItemData item)
		{
			Item = item;
			if (Item == null)
			{
				imgIcon.Source = SlotBGImages[(int)Slot];
				ttTT.Content = null;
			}
			else
			{
				switch (Item.Slot)
				{
					case SlotType.Body:
						if (Item.Category == (int)ArmorCategory.Cloth) imgIcon.Source = SlotFillImages["body_cloth"];
						else if (Item.Category == (int)ArmorCategory.Light) imgIcon.Source = SlotFillImages["body_light"];
						else if (Item.Category == (int)ArmorCategory.Medium) imgIcon.Source = SlotFillImages["body_medium"];
						else if (Item.Category == (int)ArmorCategory.Heavy) imgIcon.Source = SlotFillImages["body_heavy"];
						else if (Item.Category == (int)ArmorCategory.Docent) imgIcon.Source = SlotFillImages["body_docent"];
						break;

					case SlotType.Offhand:
						if (Item.Category == (int)OffhandCategory.Buckler) imgIcon.Source = SlotFillImages["offhand_buckler"];
						else if (Item.Category == (int)OffhandCategory.Small) imgIcon.Source = SlotFillImages["offhand_small"];
						else if (Item.Category == (int)OffhandCategory.Large) imgIcon.Source = SlotFillImages["offhand_large"];
						else if (Item.Category == (int)OffhandCategory.Tower) imgIcon.Source = SlotFillImages["offhand_tower"];
						else if (Item.Category == (int)OffhandCategory.Orb) imgIcon.Source = SlotFillImages["offhand_orb"];
						break;

					default:
						imgIcon.Source = SlotFillImages[Item.Slot.ToString().ToLower()];
						break;
				}

				ttTT.Content = Item.Name;
			}
		}

		private void UserControl_MouseEnter(object sender, MouseEventArgs e)
		{
			if (!IsSelected) SetSelectBorder(true);
		}

		private void UserControl_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!IsSelected) SetSelectBorder(false);
			MouseLeftClicked = MouseRightClicked = false;
		}

		private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !MouseRightClicked)
				MouseLeftClicked = true;
			if (e.RightButton == MouseButtonState.Pressed && !MouseLeftClicked)
				MouseRightClicked = true;
		}

		private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!MouseLeftClicked && !MouseRightClicked) return;

			EquipmentSlotClicked(this, MouseLeftClicked ? MouseButton.Left : MouseButton.Right);

			MouseLeftClicked = MouseRightClicked = false;
		}

		private void BtnClear_Click(object sender, RoutedEventArgs e)
		{
			if (IsLocked)
			{
				if (MessageBox.Show("Slot is locked. Are you sure you want to clear it? This will unlock the slot as well.", "Slot Locked", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;
				tbLock.IsChecked = false;
				UpdateLockStatus();
			}

			SetItem(null);
			EquipmentSlotCleared(this);
		}
	}
}
