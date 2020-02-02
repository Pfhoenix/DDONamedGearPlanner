using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DDONamedGearPlanner
{
	/// <summary>
	/// Interaction logic for ListViewCustomItemProperties.xaml
	/// </summary>
	public partial class ListViewCustomItemProperties : UserControl
	{
		static readonly string[] ReadOnlyProperties =
		{
			"Minimum Level"
		};

		static readonly string[] NameTypeProtectedProperties =
		{
			"Handedness"
		};

		public DDOItemData Item { get; private set; }
		List<string> AvailableProperties;

		public ListViewCustomItemProperties()
		{
			InitializeComponent();
			AvailableProperties = DatasetManager.Dataset.ItemProperties.Select(kv => kv.Key).OrderBy(p => p).ToList();
			foreach (var s in ReadOnlyProperties) AvailableProperties.Remove(s);
			foreach (var s in NameTypeProtectedProperties) AvailableProperties.Remove(s);
			foreach (var s in DatasetManager.CategoryProperties) AvailableProperties.Remove(s);
		}

		void AddItemProperty(ItemProperty ip)
		{
			CustomItemPropertyData cipd = new CustomItemPropertyData(ip);
			if (ReadOnlyProperties.Contains(ip.Property)) cipd.IsEditable = false;
			else cipd.IsEditable = true;
			if (NameTypeProtectedProperties.Contains(ip.Property)) cipd.EditProperty = cipd.EditType = false;
			else if (DatasetManager.CategoryProperties.Contains(ip.Property))
			{
				cipd.EditProperty = false;
				cipd.AnyType = false;
			}
			if (!cipd.EditProperty) cipd.AvailableProperties = new List<string> { ip.Property };
			else cipd.AvailableProperties = AvailableProperties;
			if (cipd.EditType) cipd.SetAvailableTypes();
			lvDetails.Items.Add(cipd);
		}

		public void SetItem(DDOItemData item)
		{
			Item = item;
			lvDetails.Items.Clear();
			if (item == null) return;
			foreach (var ip in item.Properties)
				AddItemProperty(ip);
		}

		private void Details_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double tw = e.NewSize.Width;
			GridView gv = lvDetails.View as GridView;
			gv.Columns[2].Width = Math.Min(70, Math.Max(40, tw * 0.2));
			gv.Columns[1].Width = Math.Min(110, Math.Max(60, tw * 0.314));
			gv.Columns[0].Width = tw - gv.Columns[1].Width - gv.Columns[2].Width - 2;
		}

		private void ComboBox_Loaded(object sender, RoutedEventArgs e)
		{
			TextBox innerTextBox = (sender as ComboBox).Template.FindName("PART_EditableTextBox", sender as ComboBox) as TextBox;
			if (innerTextBox != null) innerTextBox.ContextMenu = (ContextMenu)lvDetails.Resources["ItemContextMenu"];
		}

		private void ItemDelete_Click(object sender, RoutedEventArgs e)
		{
			CustomItemPropertyData cipd = lvDetails.SelectedItem as CustomItemPropertyData;
			if (ReadOnlyProperties.Contains(cipd.Property)) return;
			if (NameTypeProtectedProperties.Contains(cipd.Property)) return;
			if (DatasetManager.CategoryProperties.Contains(cipd.Property)) return;
			lvDetails.Items.Remove(cipd);
			Item.Properties.Remove(cipd.ItemProperty);
		}

		ListViewItem GetListViewItemOf(DependencyObject d)
		{
			if (d == null) return null;
			if (d is ListViewItem) return (ListViewItem)d;
			else return GetListViewItemOf(VisualTreeHelper.GetParent(d));
		}

		private void ListView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			ListViewItem lvi = GetListViewItemOf(e.OriginalSource as DependencyObject);
			if (lvi == null)
			{
				if (lvDetails.SelectedItem != null) lvDetails.SelectedItem = null;
			}
			else if (lvDetails.SelectedItem != lvi)
			{
				lvDetails.SelectedItem = null;
				lvDetails.SelectedItem = lvi;
				lvi.IsSelected = true;
			}
		}

		private void AddProperty_Click(object sender, RoutedEventArgs e)
		{
			ItemProperty ip = Item.AddProperty("", null, 0, null);
			AddItemProperty(ip);
		}
	}

	public class CustomItemPropertyData : INotifyPropertyChanged
	{
		public List<string> AvailableProperties { get; set; }

		List<string> _AvailableTypes;
		public List<string> AvailableTypes
		{
			get { return _AvailableTypes; }
			set
			{
				_AvailableTypes = value;
				NotifyPropertyChanged();
			}
		}

		public void SetAvailableTypes()
		{
			if (!AvailableProperties.Contains(Property))
			{
				AvailableTypes = new List<string>();
			}
			else
			{
				AvailableTypes = DatasetManager.Dataset.ItemProperties[Property].Types.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
			}
		}


		public bool EditProperty { get; set; }
		public bool EditType { get; set; }
		public bool AnyType { get; set; }
		bool _IsEditable;
		public bool IsEditable
		{
			get { return _IsEditable; }

			set
			{
				_IsEditable = value;
				EditProperty = value;
				EditType = value;
				AnyType = value;
			}
		}

		public ItemProperty ItemProperty { get; private set; }

		public string Property
		{
			get { return ItemProperty.Property; }
			set
			{
				ItemProperty.Property = value;
				SetAvailableTypes();
				NotifyPropertyChanged();
			}
		}

		public string Type
		{
			get { return ItemProperty.Type; }
			set
			{
				ItemProperty.Type = value;
				if (DatasetManager.CategoryProperties.Contains(Property))
				{
					if (DatasetManager.CategoryProperties[0] == Property) ItemProperty.Owner.Category = (int)(ArmorCategory)Enum.Parse(typeof(ArmorCategory), value);
					else if (DatasetManager.CategoryProperties[1] == Property) ItemProperty.Owner.Category = (int)(OffhandCategory)Enum.Parse(typeof(OffhandCategory), value);
					else if (DatasetManager.CategoryProperties[2] == Property) ItemProperty.Owner.Category = (int)(WeaponCategory)Enum.Parse(typeof(WeaponCategory), value);
				}
				NotifyPropertyChanged();
			}
		}

		public float Value
		{
			get { return ItemProperty.Value; }
			set
			{
				ItemProperty.Value = value;
				NotifyPropertyChanged();
			}
		}

		public CustomItemPropertyData(ItemProperty ip)
		{
			ItemProperty = ip;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
