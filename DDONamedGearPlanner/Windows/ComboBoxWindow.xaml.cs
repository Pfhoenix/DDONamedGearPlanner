using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DDONamedGearPlanner
{
	/// <summary>
	/// Interaction logic for ComboBoxWindow.xaml
	/// </summary>
	public partial class ComboBoxWindow : Window
	{
		public object SelectedItem => CB.SelectedItem;

		public ComboBoxWindow()
		{
			InitializeComponent();
		}

		public void Setup(string title, IEnumerable source, object initial = null)
		{
			Title = title;
			CB.ItemsSource = source;
			CB.SelectedItem = initial;
		}

		private void OK_Clicked(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void Cancel_Clicked(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			btnOK.IsEnabled = true;
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				e.Handled = true;
				Cancel_Clicked(null, null);
			}
			else base.OnKeyUp(e);
		}
	}
}
