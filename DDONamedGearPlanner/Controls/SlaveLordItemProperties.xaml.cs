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
	/// Interaction logic for SlaveLordItemProperties.xaml
	/// </summary>
	public partial class SlaveLordItemProperties : UserControl
	{
		SlaveLordCrafting.SlaveLordItemContainer SLItem;

		public SlaveLordItemProperties()
		{
			InitializeComponent();
		}

		public void SetItem(SlaveLordCrafting.SlaveLordItemContainer item)
		{
			SLItem = item;
			lvCrafting.Items.Clear();
			if (item == null) return;
		}

		private void LV_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double tw = e.NewSize.Width;
			GridView gv = lvSlots.View as GridView;
			gv.Columns[1].Width = Math.Min(70, Math.Max(40, tw * 0.2));
			gv.Columns[0].Width = tw - gv.Columns[1].Width - 2;
		}

		private void Slots_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double tw = e.NewSize.Width;
			GridView gv = lvCrafting.View as GridView;
			gv.Columns[0].Width = Math.Min(70, Math.Max(40, tw * 0.2));
			gv.Columns[1].Width = tw - gv.Columns[0].Width - 2;
		}
	}
}
