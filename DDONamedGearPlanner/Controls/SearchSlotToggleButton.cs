using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace DDONamedGearPlanner
{
	/// <summary>
	/// Interaction logic for SearchSlotToggleButton.xaml
	/// </summary>
	public partial class SearchSlotToggleButton : ToggleButton
	{
		public static readonly DependencyProperty SlotProperty =
				DependencyProperty.Register
				(
					"Slot",
					typeof(SlotType),
					typeof(SearchSlotToggleButton),
					new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSlotChanged))
				);
		public SlotType Slot
		{
			get { return (SlotType)GetValue(SlotProperty); }
			set { SetValue(SlotProperty, value); UpdateImage(); }
		}

		BitmapImage UnclickedImage;
		BitmapImage ClickedImage;

		public SearchSlotToggleButton()
		{
			Checked += Toggle_Checked;
			Unchecked += Toggle_Checked;
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			BorderThickness = new Thickness(1);

			Image img = new Image();
			img.Margin = new Thickness(2);
			Content = img;

			UpdateImage();
		}

		void UpdateImage()
		{
			if (ClickedImage == null || UnclickedImage == null || Content == null) return;
			(Content as Image).Source = IsChecked.Value ? ClickedImage : UnclickedImage;
		}

		private static void OnSlotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SearchSlotToggleButton tb = d as SearchSlotToggleButton;
			string slotname = ((SlotType)e.NewValue).ToString().ToLower();
			if (slotname == "finger") slotname = "finger1";
			tb.UnclickedImage = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_bg_" + slotname + ".png"));
			tb.ClickedImage = new BitmapImage(new Uri("pack://application:,,,/Resources/slot_bg_green_" + slotname + ".png"));
			tb.UpdateImage();
		}

		private void Toggle_Checked(object sender, RoutedEventArgs e)
		{
			UpdateImage();
		}
	}
}
