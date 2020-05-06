using System;
using System.Windows.Controls;

namespace DDONamedGearPlanner
{
	/// <summary>
	/// Interaction logic for ListViewItemProperties.xaml
	/// </summary>
	public partial class ListViewItemProperties : UserControl
	{
		public BuildItem Item { get; private set; }

		public ListViewItemProperties()
		{
			InitializeComponent();
		}

		public void SetItem(BuildItem item)
		{
			Item = item;
			lvDetails.Items.Clear();
			if (item == null) return;

			if (item.Item.QuestFoundIn != null && item.Item.QuestFoundIn.IsRaid)
			{
				ListViewItem lvi = new ListViewItem();
				lvi.Content = new { Property = "Raid Drop", Type = "", Value = "" };
				lvDetails.Items.Add(lvi);
			}

			foreach (var p in item.Item.Properties)
			{
				if (p.Options != null)
				{
					bool found = false;
					if (item.OptionProperties != null)
					{
						foreach (var op in item.OptionProperties)
						{
							if (p.Options.Contains(op))
							{
								ListViewItem lvi = new ListViewItem();
								lvi.Content = new { Property = "* " + op.Property, op.Type, op.Value };
								lvi.Tag = op;
								lvDetails.Items.Add(lvi);
								found = true;
								break;
							}
						}
					}

					if (!found)
					{
						ListViewItem lvi = new ListViewItem();
						lvi.Content = new { p.Property, p.Type, p.Value };
						lvDetails.Items.Add(lvi);
						foreach (var ip in p.Options)
						{
							lvi = new ListViewItem();
							lvi.Content = new { Property = "> " + ip.Property, ip.Type, ip.Value };
							lvi.Tag = ip;
							lvDetails.Items.Add(lvi);
						}
					}
				}
				else
				{
					ListViewItem lvi = new ListViewItem();
					lvi.Content = new { p.Property, p.Type, p.Value };
					lvDetails.Items.Add(lvi);
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

		private void Details_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
		{
			double tw = e.NewSize.Width;
			GridView gv = lvDetails.View as GridView;
			gv.Columns[2].Width = Math.Min(70, Math.Max(40, tw * 0.2));
			gv.Columns[1].Width = Math.Min(110, Math.Max(60, tw * 0.257));
			gv.Columns[0].Width = tw - gv.Columns[1].Width - gv.Columns[2].Width - 2;
		}
	}
}
