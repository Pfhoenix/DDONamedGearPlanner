using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace DDONamedGearPlanner
{
    /// <summary>
    /// Interaction logic for BuildProcessWindow.xaml
    /// </summary>
    public partial class BuildProcessWindow : Window
    {
		DDODataset Dataset;
		GearSetBuild Build;
		bool DoneProcessing;

        public BuildProcessWindow()
        {
            InitializeComponent();

			bdrPhase1Results.Visibility = Visibility.Hidden;
			bdrPhase2.Visibility = Visibility.Hidden;
			bdrPhase3.Visibility = Visibility.Hidden;
			bdrPhase4.Visibility = Visibility.Hidden;
			bdrFinalResults.Visibility = Visibility.Hidden;
        }

		public void Initialize(DDODataset ds, GearSetBuild b)
		{
			Dataset = ds;
			Build = b;

			Build.DiscoveredItems.Clear();
			Build.BuildResults.Clear();
		}

		bool Shown;
		protected override void OnContentRendered(EventArgs e)
		{
			if (!Shown)
			{
				Shown = true;
				// start the build process
			}

			base.OnContentRendered(e);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (!DoneProcessing)
			{
				if (MessageBox.Show("Are you sure you want to cancel the build?", "Build Cancellation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					base.OnClosing(e);
				}
				else e.Cancel = true;
			}
			else base.OnClosing(e);
		}
	}
}
