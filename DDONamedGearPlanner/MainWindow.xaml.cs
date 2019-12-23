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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<string> ItemsInList = new List<string>();

		public MainWindow()
		{
			InitializeComponent();

			lbItemList.ItemsSource = ItemsInList;

			SetFilter(CustomFilter);
		}

		public void SetFilter(Predicate<object> filter)
		{
			if (lbItemList.Items.CurrentItem != null && !filter(lbItemList.Items.CurrentItem))
			{
				lbItemList.SelectedItem = null;
			}
			lbItemList.Items.Filter = filter;
			if (lbItemList.Items.CurrentItem == null && !lbItemList.Items.IsEmpty)
				lbItemList.Items.MoveCurrentToFirst();
		}

		private bool CustomFilter(object obj)
		{
			if (string.IsNullOrWhiteSpace(txtSearchBox.Text))
			{
				return true;
			}
			else
			{
				return obj.ToString().IndexOf(txtSearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
			}
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
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

		private void BtnFilterApply_Click(object sender, RoutedEventArgs e)
		{
			ItemsInList.Clear();
			txtSearchBox.Text = null;
		}

		private void TxtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			CollectionViewSource.GetDefaultView(lbItemList.ItemsSource).Refresh();
		}
	}
}
