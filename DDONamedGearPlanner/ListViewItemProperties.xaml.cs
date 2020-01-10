using System.Windows.Controls;

namespace DDONamedGearPlanner
{
	/// <summary>
	/// Interaction logic for ListViewItemProperties.xaml
	/// </summary>
	public partial class ListViewItemProperties : UserControl
	{
		public ListViewItemProperties()
		{
			InitializeComponent();
		}

		public void SetItem(DDOItemData item)
		{
			lvDetails.Items.Clear();
			foreach (var p in item.Properties)
			{
				lvDetails.Items.Add(new { p.Property, p.Type, p.Value });
				if (p.Options != null)
				{
					foreach (var ip in p.Options)
						lvDetails.Items.Add(new { Property = "> " + ip.Property, ip.Type, ip.Value });
				}
			}
		}

		public void SetSetBonuses(DDOItemSetBonus sb)
		{
			lvDetails.Items.Clear();
			foreach (var p in sb.Bonuses)
			{
				lvDetails.Items.Add(new { p.Property, p.Type, p.Value });
			}
		}
	}
}
