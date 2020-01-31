using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace DDONamedGearPlanner
{
	/// <summary>
	/// Interaction logic for TextBoxWindow.xaml
	/// </summary>
	public partial class TextBoxWindow : Window
	{
		public string Text => TB.Text;

		public TextBoxWindow()
		{
			InitializeComponent();
		}

		public void Setup(string title, string text)
		{
			Title = title;
			TB.Text = text;
			TB.Focus();
			TB.SelectAll();
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

		private void TB_TextChanged(object sender, TextChangedEventArgs e)
		{
			btnOK.IsEnabled = !string.IsNullOrWhiteSpace(TB.Text);
		}

		private void TB_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return && btnOK.IsEnabled) OK_Clicked(null, null);
			if (e.Key == Key.Escape) Cancel_Clicked(null, null);
		}
	}
}
