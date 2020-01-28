using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace DDONamedGearPlanner
{
	/// <summary>
	/// Interaction logic for BuildProcessWindow.xaml
	/// </summary>
	public partial class BuildProcessWindow : Window
	{
		public string Error;
		bool CancelBuild = false;

		Stopwatch BuildSW = new Stopwatch();

		DDODataset Dataset;
		GearSetBuild Build;
		bool FilterTest;
		bool DoneProcessing;

		LargeContainer<List<BuildItem>> BuildBuffer;

		public BuildProcessWindow()
		{
			InitializeComponent();

			bdrPhase1Results.Visibility = Visibility.Hidden;
			bdrPhase2.Visibility = Visibility.Hidden;
			bdrPhase3.Visibility = Visibility.Hidden;
			bdrPhase4.Visibility = Visibility.Hidden;
			bdrFinalResults.Visibility = Visibility.Hidden;
		}

		public void Initialize(DDODataset ds, GearSetBuild b, bool filtertest)
		{
			Dataset = ds;
			Build = b;
			FilterTest = filtertest;
		}

		bool Shown;
		protected override void OnContentRendered(EventArgs e)
		{
			if (!Shown)
			{
				Shown = true;

				BuildSW.Start();

				// start the build process phase 1
				pbPhase1.Minimum = 0;
				pbPhase1.Maximum = Dataset.Items.Count;
				pbPhase1.Value = 0;
				BackgroundWorker bw = new BackgroundWorker();
				bw.WorkerReportsProgress = true;
				bw.DoWork += Phase1_DoWork;
				bw.ProgressChanged += Phase1_ProgressChanged;
				bw.RunWorkerCompleted += Phase1_Completed;

				bw.RunWorkerAsync();
			}

			base.OnContentRendered(e);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (!DoneProcessing)
			{
				if (MessageBox.Show("Are you sure you want to cancel the build?", "Build Cancellation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					CancelBuild = true;
					DialogResult = false;
				}
				e.Cancel = true;
			}
			else
			{
				ItemCache = null;
				BuildBuffer?.Clear();
				BuildBuffer = null;
				DialogResult = !CancelBuild;
				base.OnClosing(e);
			}
		}

		void CancelAndClose()
		{
			BuildSW.Stop();
			Build.DiscoveredItems.Clear();
			Build.BuildResults.Clear();
			DoneProcessing = true;
			Close();
		}

		#region Phase 1
		int ProcessBuildFilters(DDOItemData item, List<BuildFilter> filters)
		{
			foreach (var bf in filters)
			{
				foreach (var p in item.Properties)
				{
					if (bf.Property == p.Property)
					{
						if (bf.Type == null || bf.Type == p.Type || (bf.Type == "untyped" && string.IsNullOrWhiteSpace(p.Type)))
						{
							if (bf.Include) return 1;
							else return -1;
						}
					}
					else if (p.Options != null)
					{
						foreach (var op in p.Options)
						{
							if (bf.Property == op.Property)
							{
								if (bf.Type == null || bf.Type == op.Type || (bf.Type == "untyped" && string.IsNullOrWhiteSpace(op.Type)))
								{
									if (bf.Include) return 1;
									else return -1;
								}
							}
						}
					}
				}
			}

			return 0;
		}

		int ProcessSlotFilter(DDOItemData item, EquipmentSlotType slot)
		{
			SlotType filterslot = slot.ToSlotType();
			int include = ProcessBuildFilters(item, FilterTest ? Build.TestFilters[filterslot] : Build.Filters[filterslot]);
			if (include == 1)
			{
				var items = FilterTest ? Build.FilterTestItems : Build.DiscoveredItems;

				if (!items.ContainsKey(slot)) items[slot] = new List<DDOItemData>();
				if (!items[slot].Contains(item)) items[slot].Add(item);
			}

			return include;
		}

		void Phase1_DoWork(object sender, DoWorkEventArgs e)
		{
			for (int i = 0; i < Dataset.Items.Count; i++)
			{
				if (CancelBuild) return;

				if (i % 100 == 0) (sender as BackgroundWorker).ReportProgress(i);

				DDOItemData item = Dataset.Items[i];
				if (item.ML < Build.MinimumLevel || item.ML > Build.MaximumLevel) continue;

				EquipmentSlotType slot1 = EquipmentSlotType.None, slot2 = EquipmentSlotType.None;

				// check against locked slots first
				if (item.Slot == SlotType.Finger)
				{
					if (Build.LockedSlots.Contains(EquipmentSlotType.Finger1) && Build.LockedSlots.Contains(EquipmentSlotType.Finger2)) continue;
					if (!Build.LockedSlots.Contains(EquipmentSlotType.Finger1)) slot1 = EquipmentSlotType.Finger1;
					if (!Build.LockedSlots.Contains(EquipmentSlotType.Finger2))
					{
						if (slot1 != EquipmentSlotType.None) slot2 = EquipmentSlotType.Finger2;
						else slot1 = EquipmentSlotType.Finger2;
					}
				}
				else if (item.Slot == SlotType.Weapon)
				{
					if (Build.LockedSlots.Contains(EquipmentSlotType.Weapon))
					{
						if (item.Handedness == 2) continue;
						if (Build.LockedSlots.Contains(EquipmentSlotType.Offhand)) continue;
						slot1 = EquipmentSlotType.Offhand;
					}
					else if (Build.LockedSlots.Contains(EquipmentSlotType.Offhand))
					{
						if (item.Handedness == 2) continue;
						slot1 = EquipmentSlotType.Weapon;
					}
					else
					{
						slot1 = EquipmentSlotType.Weapon;
						if (item.Handedness == 1) slot2 = EquipmentSlotType.Offhand;
					}
				}
				else
				{
					slot1 = (EquipmentSlotType)Enum.Parse(typeof(EquipmentSlotType), item.Slot.ToString());
					if (Build.LockedSlots.Contains(slot1)) continue;
				}

				// go through slot filters first
				int include1 = ProcessSlotFilter(item, slot1);
				// item has a definitive filter ruling for slot1 and no slot2, means we're done with the item
				if (include1 < 0 && slot2 == EquipmentSlotType.None) continue;
				int include2 = 0;
				if (slot2 != EquipmentSlotType.None)
				{
					include2 = ProcessSlotFilter(item, slot2);
					// both slot1 and slot2 have a definitive filter ruling
					if (include1 != 0 && include2 != 0) continue;
				}

				// go through gear set filters
				int ginclude = ProcessBuildFilters(item, FilterTest ? Build.TestFilters[SlotType.None] : Build.Filters[SlotType.None]);
				if (ginclude == 1)
				{
					var items = FilterTest ? Build.FilterTestItems : Build.DiscoveredItems;

					if (!items.ContainsKey(slot1)) items[slot1] = new List<DDOItemData>();
					if (!items[slot1].Contains(item)) items[slot1].Add(item);
					if (slot2 != EquipmentSlotType.None)
					{
						if (!items.ContainsKey(slot2)) items[slot2] = new List<DDOItemData>();
						if (!items[slot2].Contains(item)) items[slot2].Add(item);
					}
				}
			}
		}

		void Phase1_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			pbPhase1.Value = e.ProgressPercentage;
			tbPhase1.Text = e.ProgressPercentage + " of " + Dataset.Items.Count;
		}

		// phase 1 goes over all items and sorts any with an included property via filter
		void Phase1_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			if (CancelBuild)
			{
				CancelAndClose();
				return;
			}

			tbPhase1.Text = "Done";

			if (FilterTest)
			{
				DoneProcessing = true;
				Close();
				return;
			}

			bool fingercounted = false;
			bool weaponcounted = false;
			int numitems = 0;
			int numslots = 0;
			ulong combos = 1;
			foreach (var kv in Build.DiscoveredItems)
			{
				// process the slot's items
				bool flagweapon = false;
				foreach (var item in kv.Value)
				{
					if (item.Slot == SlotType.Weapon)
					{
						flagweapon = true;
						if (!weaponcounted) numitems++;
					}
					else if (item.Slot == SlotType.Finger)
					{
						if (!fingercounted) numitems++;
					}
					else numitems++;
				}

				if (kv.Key == EquipmentSlotType.Finger1 || kv.Key == EquipmentSlotType.Finger2)
				{
					if (!fingercounted) numslots++;
					fingercounted = true;
				}
				else
				{
					if (kv.Key == EquipmentSlotType.Weapon || (kv.Key == EquipmentSlotType.Offhand && flagweapon)) weaponcounted = true;
					numslots++;
				}

				combos *= (ulong)(kv.Value.Count + 1);
			}

			combos -= 1;
			tbPhase1Results.Text = string.Format("{0} items across {1} slot{2}, {3} minimum combinations", numitems, numslots, numslots > 1 ? "s" : "", combos);

			bdrPhase1Results.Visibility = Visibility.Visible;

			try
			{
				BuildBuffer = new LargeContainer<List<BuildItem>>(combos);
			}
			catch
			{
				Error = "Insufficient memory to store the expected item combinations";
				CancelAndClose();
			}

			// start phase 2
			pbPhase2.Minimum = 0;
			pbPhase2.Maximum = combos;
			pbPhase2.Value = 0;
			tbPhase2.Text = null;
			bdrPhase2.Visibility = Visibility.Visible;
			BackgroundWorker bw = new BackgroundWorker();
			bw.WorkerReportsProgress = true;
			bw.DoWork += Phase2_DoWork;
			bw.ProgressChanged += Phase2_ProgressChanged;
			bw.RunWorkerCompleted += Phase2_Completed;

			bw.RunWorkerAsync();
		}
		#endregion

		#region Phase 2
		bool GetFilteredProperties(ItemProperty p, List<BuildFilter> filters, List<ItemProperty> options)
		{
			bool stock = false;

			foreach (BuildFilter bf in filters)
			{
				if (bf.Property == p.Property)
				{
					if (bf.Type == null || bf.Type == p.Type || (bf.Type == "untyped" && string.IsNullOrWhiteSpace(p.Type)))
					{
						if (bf.Include) stock = true;
					}
				}
				else if (p.Options != null)
				{
					foreach (var op in p.Options)
					{
						if (bf.Property == op.Property)
						{
							if (bf.Type == null || bf.Type == op.Type || (bf.Type == "untyped" && string.IsNullOrWhiteSpace(op.Type)))
							{
								if (bf.Include) options.Add(op);
							}
						}
					}
				}
			}

			return stock;
		}

		void BuildGearSet(BackgroundWorker bw, List<BuildItem> items)
		{
			GearSet gs = new GearSet();
			foreach (var item in items)
				if (item != null) gs.AddItem(item);

			Build.AddBuildGearSet(gs);
			if (Build.BuildResults.Count % 250 == 0) bw.ReportProgress(Build.BuildResults.Count);
		}

		List<KeyValuePair<EquipmentSlotType, List<DDOItemData>>> ItemCache;

		/*public List<List<BuildItem>> BuildGearSets(BackgroundWorker bw, int ici)
		{
			if (CancelBuild) return null;

			SlotType filterslot = ItemCache[ici].Key.ToSlotType();
			List<List<BuildItem>> result = new List<List<BuildItem>>();

			// this only needs to be called once per list entry
			List<List<BuildItem>> tailCombos = null;
			if (ici < ItemCache.Count - 1)
			{
				tailCombos = BuildGearSets(bw, ici + 1);
				if (CancelBuild) return null;
			}

			// iterate over all items in the head list
			foreach (var item in ItemCache[ici].Value)
			{
				List<ItemProperty> options = new List<ItemProperty>();
				List<List<ItemProperty>> optionsets = new List<List<ItemProperty>>();
				bool stock = false;
				options.Clear();
				// figure out how many passes we need to do
				foreach (var p in item.Properties)
				{
					if (CancelBuild) return null;
					// check slot filters first
					if (GetFilteredProperties(p, Build.Filters[filterslot], options)) stock = true;
					if (GetFilteredProperties(p, Build.Filters[SlotType.None], options)) stock = true;
				}
				// do all combinations of options
				int comboCount = (int)Math.Pow(2, options.Count) - 1;
				for (int h = 1; h < comboCount + 1; h++)
				{
					optionsets.Add(new List<ItemProperty>());
					for (int j = 0; j < options.Count; j++)
					{
						if ((h >> j) % 2 != 0)
							optionsets.Last().Add(options[j]);
					}
				}

				List<List<BuildItem>> added = new List<List<BuildItem>>();

				if (stock)
				{
					// head
					result.Add(new List<BuildItem>());
					result.Last().Add(new BuildItem(item, ItemCache[ici].Key));
					// make a copy and buffer it
					added.Add(new List<BuildItem>(result.Last()));
					BuildGearSet(bw, result.Last());
					if (tailCombos != null)
					{
						foreach (var combo in tailCombos)
						{
							if (CancelBuild) return null;
							// this ensures the previous results propagate into the new result set
							result.Add(new List<BuildItem>(combo));
							// we create another copy of the previous results in order to add our items to them
							List<BuildItem> nc = new List<BuildItem>(combo);
							nc.Add(new BuildItem(item, ItemCache[ici].Key));
							result.Add(nc);
							// make a copy and buffer it
							added.Add(new List<BuildItem>(nc));
							BuildGearSet(bw, result.Last());
						}
					}
				}

				foreach (var os in optionsets)
				{
					if (!stock)
					{
						// head
						result.Add(new List<BuildItem>());
						result.Last().Add(new BuildItem(item, ItemCache[ici].Key) { OptionProperties = os });
						// make a copy and buffer it
						added.Add(new List<BuildItem>(result.Last()));
						BuildGearSet(bw, result.Last());
						if (tailCombos != null)
						{
							foreach (var combo in tailCombos)
							{
								if (CancelBuild) return null;
								// this ensures the previous results propagate into the new result set
								result.Add(new List<BuildItem>(combo));
								// we create another copy of the previous results in order to add our items to them
								List<BuildItem> nc = new List<BuildItem>(combo);
								nc.Add(new BuildItem(item, ItemCache[ici].Key) { OptionProperties = os });
								result.Add(nc);
								// make a copy and buffer it
								added.Add(new List<BuildItem>(nc));
								BuildGearSet(bw, nc);
							}
						}
						stock = true;
					}
					else
					{
						// update each added with the optionset
						foreach (var a in added)
						{
							// each list in added has this calls' item at the end
							// create a new item so we aren't modifying an existing, used build item
							a[a.Count - 1] = new BuildItem(item, ItemCache[ici].Key) { OptionProperties = os };
							result.Add(new List<BuildItem>(a));
							BuildGearSet(bw, result.Last());
						}
					}
				}
			}

			return result;
		}*/

		ulong DuplicateBuildBufferContents(ulong start, ulong end)
		{
			ulong newstart = BuildBuffer.Count;
			for (ulong i = start; i <= end; i++)
				BuildBuffer.Add(new List<BuildItem>(BuildBuffer[i]));

			return newstart;
		}

		public void BuildGearSets(BackgroundWorker bw, int ici)
		{
			if (CancelBuild) return;

			SlotType filterslot = ItemCache[ici].Key.ToSlotType();

			if (ici < ItemCache.Count - 1)
			{
				BuildGearSets(bw, ici + 1);
				if (CancelBuild) return;
			}

			ulong origend = BuildBuffer.Count - 1;

			// iterate over all items in the head list
			foreach (var item in ItemCache[ici].Value)
			{
				List<ItemProperty> options = new List<ItemProperty>();
				List<List<ItemProperty>> optionsets = new List<List<ItemProperty>>();
				bool stock = false;
				options.Clear();
				// figure out how many passes we need to do
				foreach (var p in item.Properties)
				{
					if (CancelBuild) return;
					// check slot filters first
					if (GetFilteredProperties(p, Build.Filters[filterslot], options)) stock = true;
					if (GetFilteredProperties(p, Build.Filters[SlotType.None], options)) stock = true;
				}
				// do all combinations of options
				int comboCount = (int)Math.Pow(2, options.Count) - 1;
				for (int h = 1; h < comboCount + 1; h++)
				{
					List<ItemProperty> ips = new List<ItemProperty>();
					optionsets.Add(ips);
					for (int j = 0; j < options.Count; j++)
					{
						if ((h >> j) % 2 != 0)
							ips.Add(options[j]);
					}
				}

				if (stock)
				{
					List<BuildItem> items = new List<BuildItem>();
					items.Add(new BuildItem(item, ItemCache[ici].Key));
					BuildBuffer.Add(items);
					BuildGearSet(bw, items);
					if (ici < ItemCache.Count - 1)
					{
						ulong start = DuplicateBuildBufferContents(0, origend);
						ulong end = BuildBuffer.Count - 1;
						for (; start <= end; start++)
						{
							if (CancelBuild) return;

							BuildBuffer[start].Add(new BuildItem(item, ItemCache[ici].Key));
							BuildGearSet(bw, BuildBuffer[start]);
						}
					}
				}

				foreach (var os in optionsets)
				{
					List<BuildItem> items = new List<BuildItem>();
					items.Add(new BuildItem(item, ItemCache[ici].Key) { OptionProperties = os });
					BuildBuffer.Add(items);
					BuildGearSet(bw, items);
					if (ici < ItemCache.Count - 1)
					{
						ulong start = DuplicateBuildBufferContents(0, origend);
						ulong end = BuildBuffer.Count - 1;
						for (; start <= end; start++)
						{
							if (CancelBuild) return;

							BuildBuffer[start].Add(new BuildItem(item, ItemCache[ici].Key) { OptionProperties = os });
							BuildGearSet(bw, BuildBuffer[start]);
						}
					}
				}
			}
		}

		// phase 2 builds every possible combination of gear sets
		void Phase2_DoWork(object sender, DoWorkEventArgs e)
		{
			ItemCache = Build.DiscoveredItems.ToList();
			if (ItemCache.Count == 0)
			{
				CancelBuild = true;
				return;
			}

			BuildGearSets(sender as BackgroundWorker, 0);
		}

		void Phase2_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			//if (e.ProgressPercentage % 250000 == 0) GC.Collect();
			if (e.ProgressPercentage > pbPhase2.Maximum) pbPhase2.Maximum = e.ProgressPercentage;
			pbPhase2.Value = e.ProgressPercentage;
			tbPhase2.Text = e.ProgressPercentage + " generated";
		}

		void Phase2_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			if (CancelBuild)
			{
				CancelAndClose();
				return;
			}

			pbPhase2.Value = pbPhase2.Maximum;
			tbPhase2.Text = Build.BuildResults.Count + " generated";

			// start phase 3
			pbPhase3.Minimum = 0;
			pbPhase3.Maximum = Build.BuildResults.Count;
			pbPhase3.Value = 0;
			tbPhase3.Text = null;
			bdrPhase3.Visibility = Visibility.Visible;
			BackgroundWorker bw = new BackgroundWorker();
			bw.WorkerReportsProgress = true;
			bw.DoWork += Phase3_DoWork;
			bw.ProgressChanged += Phase3_ProgressChanged;
			bw.RunWorkerCompleted += Phase3_Completed;

			bw.RunWorkerAsync();
		}
		#endregion

		#region Phase 3
		bool IsPropertyFiltered(string property)
		{
			foreach (var kv in Build.Filters)
			{
				foreach (var bf in kv.Value)
				{
					if (bf.Include && bf.Property == property) return true;
				}
			}

			return false;
		}

		// Phase 3 rates all gear sets
		void Phase3_DoWork(object sender, DoWorkEventArgs e)
		{
			for (int i = 0; i <Build.BuildResults.Count; i++)
			{
				if (CancelBuild) return;

				if (i % 250 == 0) (sender as BackgroundWorker).ReportProgress(i);
				Build.BuildResults[i].GearSet.ProcessItems(Dataset);

				foreach (var gsp in Build.BuildResults[i].GearSet.Properties)
				{
					if (!IsPropertyFiltered(gsp.Property)) continue;

					if (gsp.IsGroup)
					{
						Build.BuildResults[i].Rating += 5;
						continue;
					}

					string lasttype = null;
					foreach (var ip in gsp.ItemProperties)
					{
						if (string.IsNullOrWhiteSpace(lasttype) || lasttype != ip.Type)
						{
							Build.BuildResults[i].Rating += (int)Math.Max(1, Math.Abs((ip.Value > 0 && ip.Value < 1) ? ip.Value * 10 : ip.Value));
							lasttype = ip.Type;
						}
						else Build.BuildResults[i].Penalty += (int)Math.Max(1, Math.Abs((ip.Value > 0 && ip.Value < 1) ? ip.Value * 10 : ip.Value));
					}
				}
			}
		}

		void Phase3_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			pbPhase3.Value = e.ProgressPercentage;
			tbPhase3.Text = e.ProgressPercentage + " of " + Build.BuildResults.Count;
		}

		Timer DotTimer;
		Stopwatch Phase4SW;
		void Phase3_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			if (CancelBuild)
			{
				CancelAndClose();
				return;
			}

			pbPhase3.Value = pbPhase3.Maximum;
			tbPhase3.Text = "Done";

			// start phase 4
			bdrPhase4.Visibility = Visibility.Visible;

			DotTimer = new Timer(1000);
			DotTimer.AutoReset = true;
			DotTimer.Elapsed += (obj, args) =>
			{
				Dispatcher.BeginInvoke((Action)(() => Phase4ProgressUpdate()));
			};
			DotTimer.Start();
			Phase4SW = new Stopwatch();
			Phase4SW.Start();

			BackgroundWorker bw = new BackgroundWorker();
			bw.WorkerReportsProgress = true;
			bw.DoWork += Phase4_DoWork;
			bw.RunWorkerCompleted += Phase4_Completed;

			bw.RunWorkerAsync();
		}
		#endregion

		#region Phase 4
		void Phase4_DoWork(object sender, DoWorkEventArgs e)
		{
			// sort buildresults by rating and penalty
			Build.BuildResults.Sort((a, b) => a.Penalty == 0 && b.Penalty == 0 ? (a.Rating > b.Rating ? -1 : (a.Rating < b.Rating ? 1 : 0)) : 
				(a.Penalty == 0 ? -1 : 
					(b.Penalty == 0 ? 1 : 
						(a.Rating > b.Rating ? -1 : 
							(a.Rating < b.Rating ? 1 : 
								(a.Penalty < b.Penalty ? -1 : 
									(a.Penalty > b.Penalty ? 1 : 0)
								)
							)
						)
					)
				)
			);
			// sort all item lists in each buildresult
			for (int i = 0; i < Build.BuildResults.Count; i++)
			{
				if (CancelBuild) return;
				Build.BuildResults[i].GearSet.Items.Sort((a, b) => string.Compare(a.Item.Name, b.Item.Name));
			}
			// do a pass to remove redundant gear sets
			for (int i = 0; i < Build.BuildResults.Count; i++)
			{
				if (i == Build.BuildResults.Count - 1) break;

				// dupe check
				if (Build.BuildResults[i].Rating != Build.BuildResults[i + 1].Rating) continue;
				if (Build.BuildResults[i].Penalty != Build.BuildResults[i + 1].Penalty) continue;
				if (Build.BuildResults[i].GearSet.Items.Count != Build.BuildResults[i + 1].GearSet.Items.Count) continue;
				bool dupe = false;
				for (int j = 0; j < Build.BuildResults[i].GearSet.Items.Count; j++)
				{
					if (Build.BuildResults[i].GearSet.Items[j].Item != Build.BuildResults[i + 1].GearSet.Items[j].Item) break;
					if (Build.BuildResults[i].GearSet.Items[j].OptionProperties.Count != Build.BuildResults[i + 1].GearSet.Items[j].OptionProperties.Count) continue;
					if (Build.BuildResults[i].GearSet.Items[j].OptionProperties.Count == 0)
					{
						dupe = true;
						break;
					}
					else
					{
						bool diff = false;
						for (int p = 0; p < Build.BuildResults[i].GearSet.Items[j].OptionProperties.Count; p++)
						{
							if (!Build.BuildResults[i + 1].GearSet.Items[j].OptionProperties.Contains(Build.BuildResults[i].GearSet.Items[j].OptionProperties[p]))
							{
								diff = true;
								break;
							}
						}

						if (!diff)
						{
							dupe = true;
							break;
						}
					}
				}

				if (dupe) Build.BuildResults.RemoveAt(i--);
			}
		}

		void Phase4ProgressUpdate()
		{
			if (tbPhase4.Text.Length > 45) tbPhase4.Text = "Sorting and removing duplicate gear sets";
			else tbPhase4.Text += ".";
		}

		void Phase4_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			if (CancelBuild)
			{
				CancelAndClose();
				return;
			}
			Phase4SW.Stop();
			BuildSW.Stop();
			DotTimer.Stop();

			tbPhase4.Text = string.Format("Sorting and removing duplicate gear sets took {0:F3} seconds", Phase4SW.ElapsedMilliseconds / 500.0);

			tbFinalResults.Text = string.Format("Build completed in {0}:{1:D2}:{2:D2}", BuildSW.Elapsed.Hours, BuildSW.Elapsed.Minutes, BuildSW.Elapsed.Seconds);
			bdrFinalResults.Visibility = Visibility.Visible;

			DoneProcessing = true;
		}
		#endregion
	}
}
