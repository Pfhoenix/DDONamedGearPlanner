using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace DDONamedGearPlanner
{
	public partial class RangeSlider : UserControl, IComponentConnector
	{
		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnMinimumChanged)));
		//public static readonly DependencyProperty LowerValueProperty = DependencyProperty.Register(nameof(LowerValue), typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnLowerValueChanged)));
		//public static readonly DependencyProperty UpperValueProperty = DependencyProperty.Register(nameof(UpperValue), typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnUpperValueChanged)));
		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(RangeSlider), new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnMaximumChanged)));
		public static readonly DependencyProperty DateProperty = DependencyProperty.Register(nameof(Date), typeof(DateTime), typeof(RangeSlider), new UIPropertyMetadata(DateTime.Today, new PropertyChangedCallback(OnDateChanged)));
		//private bool _contentLoaded;

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

		private void LowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			LowerSlider.Value = Math.Min(UpperSlider.Value, LowerSlider.Value);
		}

		private void UpperSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			UpperSlider.Value = Math.Max(UpperSlider.Value, LowerSlider.Value);
		}

		public double Minimum
		{
			get
			{
				return (double)GetValue(MinimumProperty);
			}
			set
			{
				SetValue(MinimumProperty, value);
			}
		}

		public static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RangeSlider rangeSlider = (RangeSlider)d;
			rangeSlider.LowerSlider.Minimum = (double)e.NewValue;
			rangeSlider.UpperSlider.Minimum = (double)e.NewValue;
		}

		/*public double LowerValue
		{
			get
			{
				return (double)GetValue(LowerValueProperty);
			}
			set
			{
				SetValue(LowerValueProperty, value);
			}
		}*/

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

		/*public double UpperValue
		{
			get
			{
				return (double)GetValue(UpperValueProperty);
			}
			set
			{
				SetValue(UpperValueProperty, value);
			}
		}*/

		public static void OnUpperValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RangeSlider)d).UpperSlider.Value = (double)e.NewValue;
		}

		public double Maximum
		{
			get
			{
				return (double)GetValue(MaximumProperty);
			}
			set
			{
				SetValue(MaximumProperty, value);
			}
		}

		public static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RangeSlider rangeSlider = (RangeSlider)d;
			rangeSlider.LowerSlider.Maximum = (double)e.NewValue;
			rangeSlider.UpperSlider.Maximum = (double)e.NewValue;
		}

		public static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RangeSlider rangeSlider = (RangeSlider)d;
			rangeSlider.LowerSlider.Date = (DateTime)e.NewValue;
			rangeSlider.UpperSlider.Date = (DateTime)e.NewValue;
		}

		public DateTime Date
		{
			get
			{
				return (DateTime)GetValue(DateProperty);
			}
			set
			{
				SetValue(DateProperty, value);
			}
		}

		/*[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (_contentLoaded) return;
			_contentLoaded = true;
			Application.LoadComponent(this, new Uri("/RangeSliderWpfApp;component/rangeslider.xaml", UriKind.Relative));
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId != 1)
			{
				if (connectionId == 2)
					UpperSlider = (FormattedSlider)target;
				else
					_contentLoaded = true;
			}
			else
				LowerSlider = (FormattedSlider)target;
		}*/
	}
}
