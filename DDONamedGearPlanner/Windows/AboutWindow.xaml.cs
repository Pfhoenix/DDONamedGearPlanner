using System.Windows;
using System.Windows.Documents;

namespace DDONamedGearPlanner
{
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow : Window
	{
		public AboutWindow()
		{
			InitializeComponent();
			txtVersion.Text = PlannerWindow.VERSION;
		}

		private void Hyperlink_Clicked(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start((sender as Hyperlink).NavigateUri.AbsoluteUri);
		}
	}
}
