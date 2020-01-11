using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DDONamedGearPlanner
{
	public class FormattedSlider : Slider
	{
		private ToolTip _autoToolTip;

		public string AutoToolTipFormat { get; set; }
		public DateTime Date { get; set; }

		protected override void OnThumbDragStarted(DragStartedEventArgs e)
		{
			base.OnThumbDragStarted(e);
			FormatAutoToolTipContent();
		}

		protected override void OnThumbDragDelta(DragDeltaEventArgs e)
		{
			base.OnThumbDragDelta(e);
			FormatAutoToolTipContent();
		}

		private void FormatAutoToolTipContent()
		{
			if (Date != DateTime.MinValue)
				AutoToolTip.Content = Date.AddDays(Convert.ToDouble(AutoToolTip.Content)).ToString("dd/MM/yyyy");
			if (string.IsNullOrEmpty(AutoToolTipFormat))
				return;
			AutoToolTip.Content = string.Format(AutoToolTipFormat, AutoToolTip.Content);
		}

		private ToolTip AutoToolTip
		{
			get
			{
				if (_autoToolTip == null)
					_autoToolTip = typeof(Slider).GetField("_autoToolTip", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this) as ToolTip;
				return _autoToolTip;
			}
		}
	}
}
