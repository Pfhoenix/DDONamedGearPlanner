using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace DDONamedGearPlanner
{
	public partial class RangeSlider : UserControl, IComponentConnector
	{
		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnMinimumChanged)));
		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(RangeSlider), new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnMaximumChanged)));
		public static readonly DependencyProperty DateProperty = DependencyProperty.Register(nameof(Date), typeof(DateTime), typeof(RangeSlider), new UIPropertyMetadata(DateTime.Today, new PropertyChangedCallback(OnDateChanged)));
		public static readonly DependencyProperty TooltipDisplayProperty = DependencyProperty.Register(nameof(TooltipDisplay), typeof(AutoToolTipPlacement), typeof(RangeSlider), new UIPropertyMetadata(AutoToolTipPlacement.TopLeft, new PropertyChangedCallback(OnTooltipDisplayChanged)));

		public delegate void RangeSliderValueChangedDelegate(RangeSlider slider, double oldvalue, double newvalue);
		public event RangeSliderValueChangedDelegate LowerValueChanged, UpperValueChanged;

		public RangeSlider()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(Slider_Loaded);
		}

		private void Slider_Loaded(object sender, RoutedEventArgs e)
		{
			LowerSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(LowerSlider_ValueChanged);
			UpperSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(UpperSlider_ValueChanged);
		}

		bool TriggerValueChangeCompleted = false;
		private void LowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			double old = LowerSlider.Value;
			LowerSlider.Value = Math.Min(UpperSlider.Value, LowerSlider.Value);
			LowerValueChanged?.Invoke(this, old, LowerSlider.Value);
		}

		private void UpperSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			double old = UpperSlider.Value;
			UpperSlider.Value = Math.Max(UpperSlider.Value, LowerSlider.Value);
			UpperValueChanged?.Invoke(this, old, UpperSlider.Value);
		}

		public double Minimum
		{
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}

		public static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RangeSlider rangeSlider = (RangeSlider)d;
			rangeSlider.LowerSlider.Minimum = (double)e.NewValue;
			rangeSlider.UpperSlider.Minimum = (double)e.NewValue;
		}

		public double LowerValue
		{
			get { return LowerSlider.Value; }
			set { LowerSlider.Value = value; }
		}

		public static void OnLowerValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RangeSlider)d).LowerSlider.Value = (double)e.NewValue;
		}

		public double UpperValue
		{
			get { return UpperSlider.Value; }
			set { UpperSlider.Value = value; }
		}

		public static void OnUpperValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RangeSlider)d).UpperSlider.Value = (double)e.NewValue;
		}

		public double Maximum
		{
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}

		public static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RangeSlider rangeSlider = (RangeSlider)d;
			rangeSlider.LowerSlider.Maximum = (double)e.NewValue;
			rangeSlider.UpperSlider.Maximum = (double)e.NewValue;
		}

		public DateTime Date
		{
			get { return (DateTime)GetValue(DateProperty); }
			set { SetValue(DateProperty, value); }
		}

		public static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RangeSlider rangeSlider = (RangeSlider)d;
			rangeSlider.LowerSlider.Date = (DateTime)e.NewValue;
			rangeSlider.UpperSlider.Date = (DateTime)e.NewValue;
		}

		public AutoToolTipPlacement TooltipDisplay
		{
			get { return (AutoToolTipPlacement)GetValue(TooltipDisplayProperty); }
			set { SetValue(TooltipDisplayProperty, value); }
		}

		public static void OnTooltipDisplayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RangeSlider rs = (RangeSlider)d;
			rs.LowerSlider.AutoToolTipPlacement = (AutoToolTipPlacement)e.NewValue;
			rs.UpperSlider.AutoToolTipPlacement = (AutoToolTipPlacement)e.NewValue;
		}
	}
}
