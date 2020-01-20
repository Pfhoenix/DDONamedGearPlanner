using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace DDONamedGearPlanner
{
    /// <summary>
    /// Interaction logic for BuildFiltersWindow.xaml
    /// </summary>
    public partial class BuildFiltersWindow : Window
    {
		const string ALL_TYPES = "< All >";
		GearSetBuild CurrentBuild;
		DDODataset Dataset;

		Dictionary<SlotType, ListBox> SlotListBoxes = new Dictionary<SlotType, ListBox>();
		DataTemplate SlotListBoxDT;
		Style ItemContainerStyle;

		public bool FiltersChanged { get; private set; }

        public BuildFiltersWindow()
        {
            InitializeComponent();

			DataContext = this;

			ItemContainerStyle = new Style(typeof(ListBoxItem));
			ItemContainerStyle.Setters.Add(new Setter(AllowDropProperty, true));
			ItemContainerStyle.Setters.Add(new EventSetter(DropEvent, new DragEventHandler(Filters_Drop)));
			lbFiltersGS.ItemContainerStyle = ItemContainerStyle;

			SlotListBoxDT = new DataTemplate();
			FrameworkElementFactory dp = new FrameworkElementFactory(typeof(DockPanel));

			FrameworkElementFactory btn = new FrameworkElementFactory(typeof(Button));
			btn.SetValue(DockPanel.DockProperty, Dock.Left);
			btn.SetValue(WidthProperty, 25.0);
			btn.SetBinding(ContentProperty, new Binding("Priority"));
			btn.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left);
			btn.AddHandler(PreviewMouseDownEvent, new MouseButtonEventHandler(FiltersPriority_PreviewMouseDown));
			dp.AppendChild(btn);

			FrameworkElementFactory cb = new FrameworkElementFactory(typeof(ComboBox));
			cb.SetValue(DockPanel.DockProperty, Dock.Left);
			cb.SetValue(MarginProperty, new Thickness(2, 0, 0, 0));
			cb.SetValue(WidthProperty, 70.0);
			cb.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left);
			cb.SetBinding(ComboBox.SelectedValueProperty, new Binding { Path = new PropertyPath("Include"), Mode = BindingMode.TwoWay });
			cb.SetValue(ComboBox.SelectedValuePathProperty, "Content");
			FrameworkElementFactory cbi = new FrameworkElementFactory(typeof(ComboBoxItem));
			cbi.SetValue(ContentProperty, "Include");
			cb.AppendChild(cbi);
			cbi = new FrameworkElementFactory(typeof(ComboBoxItem));
			cbi.SetValue(ContentProperty, "Exclude");
			cb.AppendChild(cbi);
			dp.AppendChild(cb);

			btn = new FrameworkElementFactory(typeof(Button));
			btn.SetValue(DockPanel.DockProperty, Dock.Right);
			btn.SetValue(WidthProperty, 20.0);
			btn.SetValue(MarginProperty, new Thickness(2, 0, 0, 0));
			btn.AddHandler(Button.ClickEvent, new RoutedEventHandler(FiltersSlotDelete_Clicked));
			btn.SetValue(ContentProperty, "X");
			dp.AppendChild(btn);

			cb = new FrameworkElementFactory(typeof(ComboBox));
			cb.SetValue(MarginProperty, new Thickness(2, 0, 0, 0));
			cb.SetBinding(ComboBox.ItemsSourceProperty, new Binding("AvailableProperties"));
			cb.SetBinding(ComboBox.SelectedItemProperty, new Binding { Path = new PropertyPath("ItemProperty"), Mode = BindingMode.TwoWay });
			cb.SetValue(ComboBox.DisplayMemberPathProperty, "Property");
			cb.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(FilterAvailableProperties_SelectionChanged));
			dp.AppendChild(cb);

			cb = new FrameworkElementFactory(typeof(ComboBox));
			cb.SetValue(MarginProperty, new Thickness(2, 0, 0, 0));
			cb.SetBinding(ComboBox.ItemsSourceProperty, new Binding("AvailableTypes"));
			cb.SetBinding(ComboBox.SelectedItemProperty, new Binding { Path = new PropertyPath("Type"), Mode = BindingMode.TwoWay });
			dp.AppendChild(cb);

			SlotListBoxDT.VisualTree = dp;
		}

		public void Initialize(GearSetBuild build, DDODataset dataset)
		{
			CurrentBuild = build;
			Dataset = dataset;

			foreach (var cb in CurrentBuild.Filters)
			{
				if (cb.Key != SlotType.None) cbFiltersSlotAdd.Items.Add(cb.Key);
			}

			FiltersGSReset_Clicked(null, null);
			FiltersSlotReset_Clicked(null, null);
		}

		void SetAvailableTypes(BuildFilterItemData bfid)
		{
			if (string.IsNullOrWhiteSpace(bfid.ItemProperty?.Property)) return;

			List<string> values = new List<string> { ALL_TYPES };
			foreach (var t in bfid.ItemProperty.Types)
			{
				if (string.IsNullOrWhiteSpace(t)) values.Add("untyped");
				else values.Add(t);
			}
			bfid.AvailableTypes = values;
		}

		private void AddFilterGS_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox cb = sender as ComboBox;
			if (cb.SelectedIndex == -1) return;

			var filter = new BuildFilterItemData { AvailableProperties = new List<DDOItemProperty>(Dataset.SlotExclusiveItemProperties[SlotType.None]), Include = "Include" };

			if (cb.SelectedIndex == 0)
			{
				lbFiltersGS.Items.Insert(0, filter);
				int i = 1;
				foreach (BuildFilterItemData b in lbFiltersGS.Items)
					b.Priority = i++;
			}
			else
			{
				filter.Priority = lbFiltersGS.Items.Count + 1;
				lbFiltersGS.Items.Add(filter);
			}

			Action a = () => cb.Text = "Add Filter";
			Dispatcher.BeginInvoke(a);
		}

		private void FilterAvailableProperties_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox cb = sender as ComboBox;
			BuildFilterItemData bfid = cb.DataContext as BuildFilterItemData;
			if (bfid != null)
			{
				SetAvailableTypes(bfid);
				if (!bfid.AvailableTypes.Contains(bfid.Type)) bfid.Type = ALL_TYPES;
			}
		}

		void SaveFiltersToBuild(SlotType slot, ListBox lb)
		{
			List<BuildFilter> bfs = CurrentBuild.Filters[slot];
			bfs.Clear();
			if (lb == null) return;
			foreach (BuildFilterItemData bfid in lb.Items)
			{
				if (bfid.ItemProperty == null) continue;
				bfs.Add(new BuildFilter { Slot = slot, Property = bfid.ItemProperty.Property, Type = bfid.Type == ALL_TYPES ? null : bfid.Type, Include = bfid.Include == "Include" });
			}
		}

		private void FiltersGSApply_Clicked(object sender, RoutedEventArgs e)
		{
			SaveFiltersToBuild(SlotType.None, lbFiltersGS);
		}

		private void FiltersGSReset_Clicked(object sender, RoutedEventArgs e)
		{
			lbFiltersGS.Items.Clear();

			var bfs = CurrentBuild.Filters[SlotType.None];
			for (int i = 0; i < bfs.Count; i++)
			{
				var filter = new BuildFilterItemData { Priority = i + 1, ItemProperty = Dataset.ItemProperties[bfs[i].Property], AvailableProperties = new List<DDOItemProperty>(Dataset.SlotExclusiveItemProperties[SlotType.None]), Type = string.IsNullOrWhiteSpace(bfs[i].Type) ? ALL_TYPES : bfs[i].Type, Include = bfs[i].Include ? "Include" : "Exclude" };
				SetAvailableTypes(filter);
				lbFiltersGS.Items.Add(filter);
			}
		}

		private void FiltersGSClear_Clicked(object sender, RoutedEventArgs e)
		{
			lbFiltersGS.Items.Clear();
		}

		private void FiltersGSDelete_Clicked(object sender, RoutedEventArgs e)
		{
			BuildFilterItemData bfid = (sender as Button).DataContext as BuildFilterItemData;
			lbFiltersGS.Items.Remove(bfid);
			for (int i = bfid.Priority - 1; i < lbFiltersGS.Items.Count; i++)
				(lbFiltersGS.Items[i] as BuildFilterItemData).Priority = i + 1;
		}

		private void FiltersPriority_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			ListBoxItem lbi = ((((sender as Button).Parent as DockPanel).TemplatedParent as ContentPresenter).Parent as Border).TemplatedParent as ListBoxItem;
			DragDrop.DoDragDrop(lbi, lbi.DataContext, DragDropEffects.Move);
			lbi.IsSelected = true;
			return;
		}

		void Filters_Drop(object sender, DragEventArgs e)
		{
			BuildFilterItemData droppedData = e.Data.GetData(typeof(BuildFilterItemData)) as BuildFilterItemData;
			BuildFilterItemData target = ((ListBoxItem)(sender)).DataContext as BuildFilterItemData;

			if (droppedData == target) return;
			if (droppedData.Slot != target.Slot) return;
			ListBox lb = lbFiltersGS;
			if (droppedData.Slot != SlotType.None) lb = SlotListBoxes[droppedData.Slot];

			if (droppedData.Priority < target.Priority)
			{
				lb.Items.Insert(target.Priority, droppedData);
				lb.Items.RemoveAt(droppedData.Priority - 1);
			}
			else
			{
				lb.Items.RemoveAt(droppedData.Priority - 1);
				lb.Items.Insert(target.Priority - 1, droppedData);
			}

			int i = 1;
			foreach (BuildFilterItemData bfid in lb.Items)
				bfid.Priority = i++;
		}

		bool IsSlotLocked(SlotType slot)
		{
			if (slot == SlotType.None) return false;
			else if (slot == SlotType.Finger) return CurrentBuild.LockedSlots.Contains(EquipmentSlotType.Finger1) || CurrentBuild.LockedSlots.Contains(EquipmentSlotType.Finger2);
			else return CurrentBuild.LockedSlots.Contains((EquipmentSlotType)Enum.Parse(typeof(EquipmentSlotType), slot.ToString()));
		}

		void AddFilterToSlotListBox(BuildFilterItemData filter)
		{
			ListBox lb;
			if (SlotListBoxes.ContainsKey(filter.Slot) && SlotListBoxes[filter.Slot] != null) lb = SlotListBoxes[filter.Slot];
			else
			{
				GroupBox gb = new GroupBox();
				gb.Header = filter.Slot.ToString();
				if (IsSlotLocked(filter.Slot))
				{
					gb.Foreground = Brushes.Red;
					gb.BorderBrush = Brushes.Red;
				}
				lb = new ListBox();
				lb.BorderThickness = new Thickness(0);
				lb.ItemTemplate = SlotListBoxDT;
				lb.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
				lb.ItemContainerStyle = ItemContainerStyle;
				gb.Content = lb;
				SlotListBoxes[filter.Slot] = lb;
				spFiltersSlots.Children.Add(gb);
			}

			filter.Priority = lb.Items.Count + 1;
			lb.Items.Add(filter);
		}

		private void AddFilterSlot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox cb = sender as ComboBox;
			if (cb.SelectedIndex == -1) return;

			SlotType slot = (SlotType)cb.SelectedValue;

			var filter = new BuildFilterItemData { Slot = slot, AvailableProperties = new List<DDOItemProperty>(Dataset.SlotExclusiveItemProperties[slot]), Include = "Include" };
			AddFilterToSlotListBox(filter);

			Action a = () => cb.Text = "Add Filter";
			Dispatcher.BeginInvoke(a);
		}

		private void FiltersSlotDelete_Clicked(object sender, RoutedEventArgs e)
		{
			BuildFilterItemData bfid = (sender as Button).DataContext as BuildFilterItemData;
			ListBox lb = SlotListBoxes[bfid.Slot];
			lb.Items.Remove(bfid);
			if (lb.Items.Count == 0)
			{
				SlotListBoxes[bfid.Slot] = null;
				spFiltersSlots.Children.Remove(lb.Parent as GroupBox);
			}
			else
			{
				for (int i = bfid.Priority - 1; i < lb.Items.Count; i++)
					(lb.Items[i] as BuildFilterItemData).Priority = i + 1;
			}
		}

		private void FiltersSlotApply_Clicked(object sender, RoutedEventArgs e)
		{
			SlotType[] slotvalues = (SlotType[])Enum.GetValues(typeof(SlotType));
			foreach (var st in slotvalues)
				SaveFiltersToBuild(st, null);

			foreach (var slb in SlotListBoxes)
			{
				if (slb.Value != null) SaveFiltersToBuild(slb.Key, slb.Value);
			}
		}

		private void FiltersSlotReset_Clicked(object sender, RoutedEventArgs e)
		{
			FiltersSlotClear_Clicked(null, null);

			foreach (var lbf in CurrentBuild.Filters)
			{
				if (lbf.Key == SlotType.None) continue;

				for (int i = 0; i < lbf.Value.Count; i++)
				{
					var filter = new BuildFilterItemData { Slot = lbf.Key, Priority = i + 1, ItemProperty = Dataset.ItemProperties[lbf.Value[i].Property], AvailableProperties = new List<DDOItemProperty>(Dataset.SlotExclusiveItemProperties[lbf.Key]), Type = string.IsNullOrWhiteSpace(lbf.Value[i].Type) ? ALL_TYPES : lbf.Value[i].Type, Include = lbf.Value[i].Include ? "Include" : "Exclude" };
					SetAvailableTypes(filter);
					AddFilterToSlotListBox(filter);
				}
			}
		}

		private void FiltersSlotClear_Clicked(object sender, RoutedEventArgs e)
		{
			spFiltersSlots.Children.Clear();
			SlotListBoxes.Clear();
		}
	}

	public class BuildFilterItemData : INotifyPropertyChanged
	{
		public SlotType Slot;

		int _Priority;
		public int Priority
		{
			get { return _Priority; }
			set
			{
				_Priority = value;
				NotifyPropertyChanged();
			}
		}

		DDOItemProperty _ItemProperty;
		public DDOItemProperty ItemProperty
		{
			get { return _ItemProperty; }
			set
			{
				_ItemProperty = value;
				NotifyPropertyChanged();
			}
		}

		public List<DDOItemProperty> AvailableProperties { get; set; }

		public string _Type;
		public string Type
		{
			get { return _Type; }
			set
			{
				_Type = value;
				NotifyPropertyChanged();
			}
		}

		public List<string> _AvailableTypes = new List<string>();
		public List<string> AvailableTypes
		{
			get { return _AvailableTypes; }
			set
			{
				_AvailableTypes = value;
				NotifyPropertyChanged();
			}
		}

		public string Include { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
