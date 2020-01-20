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
	/// Interaction logic for FixedComboBox.xaml
	/// </summary>
	public partial class FixedComboBox : ComboBox
	{
		protected override Size MeasureOverride(Size constraint)
		{
			Panel p = Parent as Panel;
			if (p == null || double.IsNaN(p.Width)) return base.MeasureOverride(constraint);
			double tw = p.Width - Margin.Left - Margin.Right;
			foreach (Control c in p.Children)
				if (c != this) tw -= c.Width + c.Margin.Left + c.Margin.Right;

			return base.MeasureOverride(new Size(tw, constraint.Height));
		}
	}
}
