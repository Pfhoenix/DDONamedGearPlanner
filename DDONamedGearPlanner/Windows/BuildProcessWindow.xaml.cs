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

		GearSetBuild Build;
		bool FilterTest;
		bool DoneProcessing;

		public BuildProcessWindow()
		{
			InitializeComponent();

			bdrPhase1Results.Visibility = Visibility.Hidden;
			bdrPhase2.Visibility = Visibility.Hidden;
			bdrFinalResults.Visibility = Visibility.Hidden;
		}

		public void Initialize(GearSetBuild b, bool filtertest)
		{
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
				pbPhase1.Maximum = DatasetManager.Dataset.Items.Count;
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
				ItemPropertyCache = null;
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
					else if (p.Options != null && !p.HideOptions)
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
			for (int i = 0; i < DatasetManager.Dataset.Items.Count; i++)
			{
				if (CancelBuild) return;

				if (i % 100 == 0) (sender as BackgroundWorker).ReportProgress(i);

				DDOItemData item = DatasetManager.Dataset.Items[i];
				if (item.ML < Build.MinimumLevel || item.ML > Build.MaximumLevel) continue;
				if (!QuestSourceManager.IsItemAllowed(item)) continue;

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
			tbPhase1.Text = e.ProgressPercentage + " of " + DatasetManager.Dataset.Items.Count;
		}

		// phase 1 goes over all items and sorts any with an included property via filter
		void Phase1_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			if (CancelBuild)
			{
				CancelAndClose();
				return;
			}

			pbPhase1.Value = pbPhase1.Maximum;
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
			//ulong combos = 1;
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
			}

			tbPhase1Results.Text = string.Format("{0} items across {1} slot{2}", numitems, numslots, numslots > 1 ? "s" : "");

			bdrPhase1Results.Visibility = Visibility.Visible;

			// start phase 2
			pbPhase2.Minimum = 0;
			pbPhase2.Maximum = 1;
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
		/*bool GetFilteredProperties(ItemProperty p, List<BuildFilter> filters, List<ItemProperty> options)
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
		}*/

		/*void BuildGearSet(BackgroundWorker bw, List<BuildItem> items)
		{
			GearSet gs = new GearSet();
			foreach (var item in items)
				if (item != null) gs.AddItem(item);

			Build.AddBuildGearSet(gs);
			if (Build.BuildResults.Count % 250 == 0) bw.ReportProgress(Build.BuildResults.Count);
		}*/

		List<KeyValuePair<EquipmentSlotType, List<ItemProperty>>> ItemPropertyCache;

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

		/*ulong DuplicateBuildBufferContents(ulong start, ulong end)
		{
			ulong newstart = BuildBuffer.Count;
			for (ulong i = start; i <= end; i++)
				BuildBuffer.Add(new List<BuildItem>(BuildBuffer[i]));

			return newstart;
		}*/

		/*public void BuildGearSets(BackgroundWorker bw, int ici)
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
		}*/

		BuildPhase2Progress BP2P;

		List<List<BuildItem>> BuildItemLists(BackgroundWorker bw, int ici)
		{
			List<List<BuildItem>> results = new List<List<BuildItem>>();
			List<List<BuildItem>> tailCombos = null;
			if (ici < ItemPropertyCache.Count - 1)
			{
				tailCombos = BuildItemLists(bw, ici + 1);
				if (CancelBuild) return null;
				results.AddRange(tailCombos);
			}

			List<ItemProperty> properties = ItemPropertyCache[ici].Value;
			for (int i = 0; i < properties.Count; i++)
			{
				if (CancelBuild) return null;
				int e = i;
				List<List<ItemProperty>> proplists = new List<List<ItemProperty>>();

				// scan ahead for properties from the same owner
				for (e = i + 1; e < properties.Count; e++)
				{
					if (properties[e].Owner != properties[i].Owner)
					{
						e--;
						break;
					}
				}
				if (e == properties.Count) e = properties.Count - 1;

				// generate combinations of the same-item properties
				proplists.Add(new List<ItemProperty>());
				List<ItemProperty> optionals = new List<ItemProperty>();
				// setup the first proplist to be stock properties that can't change
				for (int j = i; j <= e; j++)
				{
					if (properties[j].Owner.Properties.Contains(properties[j])) proplists.Last().Add(properties[j]);
					else optionals.Add(properties[j]);
				}
				if (optionals.Count > 0)
				{
					// now we need to create all the possible combinations of the optionals with the stock in proplist[0]
					int comboCount = (int)Math.Pow(2, optionals.Count) - 1;
					for (int h = 1; h < comboCount + 1; h++)
					{
						proplists.Add(new List<ItemProperty>());
						for (int j = 0; j < optionals.Count; j++)
						{
							if ((h >> j) % 2 != 0)
								proplists.Last().Add(optionals[j]);
						}
					}
				}

				// the stock item needs to be included
				if (proplists[0].Count != 0)
				{
					if (++BP2P.BarValue % 250 == 0) bw.ReportProgress(BP2P.BarValue);
					// add just the item as a possible combination
					BuildItem bi = new BuildItem(properties[i].Owner, ItemPropertyCache[ici].Key);
					results.Add(new List<BuildItem> { bi });
					if (tailCombos != null)
					{
						foreach (var tc in tailCombos)
						{
							if (CancelBuild) return null;

							if (++BP2P.BarValue % 250 == 0) bw.ReportProgress(BP2P.BarValue);
							results.Add(new List<BuildItem>(tc));
							results.Last().Add(bi);
						}
					}
				}
				for (int pl = 1; pl < proplists.Count; pl++)
				{
					if (++BP2P.BarValue % 250 == 0) bw.ReportProgress(BP2P.BarValue);
					BuildItem bi = new BuildItem(properties[i].Owner, ItemPropertyCache[ici].Key);
					bi.OptionProperties = proplists[pl];
					results.Add(new List<BuildItem> { bi });
					if (tailCombos != null)
					{
						foreach (var tc in tailCombos)
						{
							if (CancelBuild) return null;

							if (++BP2P.BarValue % 250 == 0) bw.ReportProgress(BP2P.BarValue);
							results.Add(new List<BuildItem>(tc));
							results.Last().Add(bi);
						}
					}
				}

				// this allows us to not process properties from the same owner more than once (this pass)
				i = e;
			}

			return results;
		}

		// returns a list of the equipment slots filled
		void Phase2_ProcessBuildFilters(BackgroundWorker bw, List<KeyValuePair<EquipmentSlotType, List<DDOItemData>>> items, List<BuildFilter> filters)
		{
			decimal maxvalue = -1;
			decimal curoverridevalue = 0;
			List<GearSet> finalgs = new List<GearSet>();

			foreach (var bf in filters)
			{
				BP2P.CurrentFilter++;
				BP2P.BarMin = 0;
				BP2P.BarMax = 1;
				BP2P.BarValue = 0;
				BP2P.Format = "Processing filter {0} of {1}";
				BP2P.Stage = EBuildPhase2Progress.FilterStart;
				bw.ReportProgress(0, BP2P);

				// yes this will slow things down, but it's necessary for memory usage
				GC.Collect();

				// exclude filters are handled long before here
				if (!bf.Include) continue;

				// make a duplicate list of lists of just the item properties that match this filter
				List<KeyValuePair<EquipmentSlotType, List<ItemProperty>>> filteredproperties = new List<KeyValuePair<EquipmentSlotType, List<ItemProperty>>>();
				foreach (var il in items)
				{
					if (CancelBuild) return;
					List<ItemProperty> tfi = new List<ItemProperty>();
					KeyValuePair<EquipmentSlotType, List<ItemProperty>> kvp = new KeyValuePair<EquipmentSlotType, List<ItemProperty>>(il.Key, tfi);
					// each item's relevant properties get stored in tfi for evaluation
					// it's possible for an item to contribute more than one property
					foreach (var item in il.Value)
					{
						foreach (var p in item.Properties)
						{
							if (CancelBuild) return;
							if (bf.Property == p.Property)
							{
								if (bf.Type == null || bf.Type == p.Type || (bf.Type == "untyped" && string.IsNullOrWhiteSpace(p.Type)))
								{
									tfi.Add(p);
								}
							}
							else if (p.Options != null && !p.HideOptions)
							{
								foreach (var op in p.Options)
								{
									if (bf.Property == op.Property)
									{
										if (bf.Type == null || bf.Type == op.Type || (bf.Type == "untyped" && string.IsNullOrWhiteSpace(op.Type)))
										{
											tfi.Add(op);
										}
									}
								}
							}
						}
					}
					filteredproperties.Add(kvp);
				}

				// ensure we have at least one gear set to add to
				if (Build.BuildResults.Count == 0)
				{
					GearSet gs = new GearSet();
					gs.Items.AddRange(Build.LockedSlotItems);
					Build.BuildResults.Add(new GearSetEvaluation(gs, new List<EquipmentSlotType>(Build.LockedSlots)));
				}

				List<GearSet> gearsets = new List<GearSet>();
				// now we process per gear set already started
				// next we cull out properties belonging to items that can't be slotted into the existing gear sets
				foreach (var gse in Build.BuildResults)
				{
					if (CancelBuild) return;
					ItemPropertyCache.Clear();
					// this is the filteredproperties with the addition of any applicable optional properties from the gear set
					List<KeyValuePair<EquipmentSlotType, List<ItemProperty>>> gsfp = new List<KeyValuePair<EquipmentSlotType, List<ItemProperty>>>();
					foreach (var tfi in filteredproperties) gsfp.Add(new KeyValuePair<EquipmentSlotType, List<ItemProperty>>(tfi.Key, new List<ItemProperty>(tfi.Value)));
					bool gearpropertyinfiltered = false;
					foreach (var item in gse.GearSet.Items)
					{
						foreach (var p in item.Item.Properties)
						{
							if (CancelBuild) return;
							if (p.Options == null || p.HideOptions) continue;
							if (p.Options.Any(a => item.OptionProperties.Contains(a))) continue;
							foreach (var op in p.Options)
							{
								if (bf.Property == op.Property)
								{
									if (bf.Type == null || bf.Type == op.Type || (bf.Type == "untyped" && string.IsNullOrWhiteSpace(op.Type)))
									{
										var tfi = gsfp.Find(f => f.Key == item.Slot);
										if (tfi.Key == EquipmentSlotType.None) gsfp.Add(new KeyValuePair<EquipmentSlotType, List<ItemProperty>>(item.Slot, new List<ItemProperty>() { op }));
										else tfi.Value.Add(op);
										gearpropertyinfiltered = true;
									}
								}
							}
						}
					}

					foreach (var tfi in gsfp)
					{
						if (CancelBuild) return;
						// ignore items that fit into an already occupied slot in the gear set when there aren't any gear set item properties being tested
						var gsi = gse.GearSet.Items.Find(f => f.Slot == tfi.Key);
						if (!gearpropertyinfiltered && gsi != null) continue;
						List<ItemProperty> props = null;
						foreach (var ip in tfi.Value)
						{
							if (CancelBuild) return;
							if (gearpropertyinfiltered && gsi != null && ip.Owner != gsi.Item) continue;
							// check that this property's item isn't already slotted in the gear set in a different slot (avoid duplicate rings and duplicate weapons)
							// the same slot indicates that this property is an optional being checked for validity
							var ti = gse.GearSet.Items.Find(f => f.Item == ip.Owner);
							if (ti != null && ti.Slot != tfi.Key) continue;
							// check that 2-handed weapons won't be possibly slotted when the gear set has an offhand already set
							if (tfi.Key == EquipmentSlotType.Weapon)
							{
								if (gse.GearSet.Items.Find(f => f.Slot == EquipmentSlotType.Offhand) != null && ip.Owner.Handedness > 1) continue;
							}
							// check that offhand items won't be possibly slotted when the gear set has a two-handed weapon already set
							else if (tfi.Key == EquipmentSlotType.Offhand)
							{
								if (gse.GearSet.Items.Find(f => f.Slot == EquipmentSlotType.Weapon && f.Item.Handedness > 1) != null) continue;
							}
							//check that there isn't a minor artifact already in the gear set
							if (ip.Owner.MinorArtifact && gse.GearSet.Items.Find(f => f.Item.MinorArtifact == true) != null) continue;
							// check that this property isn't already overridden by slotted gear
							if (!string.IsNullOrWhiteSpace(ip.Type))
							{
								bool overridden = false;
								foreach (var bi in gse.GearSet.Items)
								{
									// check selected optional properties of the gear set item
									if (bi.OptionProperties.Find(f => f.Property == ip.Property && f.Type == ip.Type && f.Value >= ip.Value) != null)
									{
										overridden = true;
										break;
									}
									// check item properties of the gear set item
									if (bi.Item.Properties.Find(f => f.Property == ip.Property && f.Type == ip.Type && f.Value >= ip.Value) != null)
									{
										overridden = true;
										break;
									}
								}
								if (overridden) continue;
							}

							// this is a property that is valid to otherwise be used in the gear set
							if (props == null)
							{
								props = new List<ItemProperty>();
								KeyValuePair<EquipmentSlotType, List<ItemProperty>> kvp = new KeyValuePair<EquipmentSlotType, List<ItemProperty>>(tfi.Key, props);
								ItemPropertyCache.Add(kvp);
							}
							props.Add(ip);
						}
					}

					if (ItemPropertyCache.Count == 0) continue;

					// we now have lists by slot of item properties that satisfy this filter
					// now we figure out maximum values by type for each slot
					BP2P.BarMax = 1;
					foreach (var tfi in ItemPropertyCache)
					{
						if (CancelBuild) return;
						// first sort the item properties
						tfi.Value.Sort((a, b) => string.Compare(a.Type, b.Type) != 0 ? string.Compare(a.Type, b.Type) : (a.Value > b.Value ? -1 : (a.Value < b.Value ? 1 : 0)));
						// now we remove all but the max values for each type, with the exception of untyped (null), as they stack with everything including each other
						string curtype = null;
						float value = 0;
						for (int i = 0; i < tfi.Value.Count; i++)
						{
							if (string.IsNullOrWhiteSpace(tfi.Value[i].Type)) continue;
							else if (curtype == tfi.Value[i].Type && value > tfi.Value[i].Value) tfi.Value.RemoveAt(i--);
							else
							{
								curtype = tfi.Value[i].Type;
								value = tfi.Value[i].Value;
							}
						}
						// now we resort by item name, so properties from the same item end up together
						tfi.Value.Sort((a, b) => string.Compare(a.Owner.Name, b.Owner.Name));

						BP2P.BarMax *= tfi.Value.Count + 1;
					}

					BP2P.BarValue = 0;
					BP2P.Stage = EBuildPhase2Progress.GenerateItemLists;
					BP2P.Format = "Item combos : {0} of {1}";
					bw.ReportProgress(0, BP2P);
					// now we can generate all combinations on the remaining properties
					List<List<BuildItem>> itemcombos = BuildItemLists(bw, 0);
					if (CancelBuild) return;
					BP2P.BarValue = 0;
					BP2P.BarMax = itemcombos.Count;
					BP2P.Stage = EBuildPhase2Progress.BuildGearSets;
					BP2P.Format = "Gear sets : {0} of {1}";
					bw.ReportProgress(0, BP2P);
					// build gear sets
					maxvalue = -1;
					curoverridevalue = 0;
					finalgs.Clear();
					foreach (var bil in itemcombos)
					{
						if (CancelBuild) return;
						if (++BP2P.BarValue % 250 == 0) bw.ReportProgress(BP2P.BarValue);
						GearSet gs = new GearSet();
						// only add items from the gear set that aren't in the additional build
						foreach (var gsi in gse.GearSet.Items)
							if (!bil.Exists(i => i.Item == gsi.Item)) gs.AddItem(gsi.Clone());
						foreach (var bi in bil)
						{
							var ti = gse.GearSet.Items.Find(f => f.Item == bi.Item);
							// the item doesn't exist in the existing gear set, so add it
							if (ti == null) gs.AddItem(bi.Clone());
							// the item does exist, meaning we're adding optional properties to it
							else
							{
								var tbi = ti.Clone();
								tbi.OptionProperties.AddRange(bi.OptionProperties);
								gs.AddItem(tbi);
							}
						}
						gs.ProcessItems();
						// now we test early to find maximum current filtered property values
						GearSetProperty gsp = gs.Properties.Find(p => p.Property == bf.Property);
						if (gsp == null) continue;
						decimal tv = (decimal)gsp.TotalValue;
						if (tv > maxvalue || (tv == maxvalue && (decimal)gsp.OverriddenValue < curoverridevalue))
						{
							maxvalue = tv;
							curoverridevalue = (decimal)gsp.OverriddenValue;
							finalgs.Clear();
							finalgs.Add(gs);
						}
						else if (tv == maxvalue && (decimal)gsp.OverriddenValue == curoverridevalue) finalgs.Add(gs);
					}

					gearsets.AddRange(finalgs);
				}

				if (gearsets.Count == 0) continue;

				// now we have gearsets for each prior existing build result, find the one with the highest totalvalue in its properties that matches the filtered property
				maxvalue = -1;
				curoverridevalue = 0;
				finalgs.Clear();
				foreach (var gs in gearsets)
				{
					if (CancelBuild) return;
					GearSetProperty gsp = gs.Properties.Find(p => p.Property == bf.Property);
					if (gsp == null) continue;
					decimal tv = (decimal)gsp.TotalValue;
					if (tv > maxvalue || (tv == maxvalue && (decimal)gsp.OverriddenValue < curoverridevalue))
					{
						maxvalue = tv;
						curoverridevalue = (decimal)gsp.OverriddenValue;
						finalgs.Clear();
						finalgs.Add(gs);
					}
					else if (tv == maxvalue && (decimal)gsp.OverriddenValue == curoverridevalue) finalgs.Add(gs);
				}
				Build.BuildResults.Clear();
				foreach (var gs in finalgs)
					Build.BuildResults.Add(new GearSetEvaluation(gs, Build.LockedSlots));
			}
		}

		void Phase2_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker bw = sender as BackgroundWorker;
			foreach (var kvp in Build.Filters) BP2P.TotalFilters += kvp.Value.Count;
			ItemPropertyCache = new List<KeyValuePair<EquipmentSlotType, List<ItemProperty>>>();

			List<BuildFilter> gearsetfilters = null;
			foreach (var kvp in Build.Filters)
			{
				if (CancelBuild) return;
				if (kvp.Value.Count == 0) continue;
				if (kvp.Key == SlotType.None) gearsetfilters = kvp.Value;
				else
				{
					if (kvp.Key == SlotType.Finger)
					{
						if (Build.DiscoveredItems.ContainsKey(EquipmentSlotType.Finger1))
						{
							Phase2_ProcessBuildFilters(bw, new List<KeyValuePair<EquipmentSlotType, List<DDOItemData>>>() { Build.DiscoveredItems.First(f => f.Key == EquipmentSlotType.Finger1) }, kvp.Value);
							if (CancelBuild) return;
						}
						if (Build.DiscoveredItems.ContainsKey(EquipmentSlotType.Finger2))
						{
							Phase2_ProcessBuildFilters(bw, new List<KeyValuePair<EquipmentSlotType, List<DDOItemData>>>() { Build.DiscoveredItems.First(f => f.Key == EquipmentSlotType.Finger2) }, kvp.Value);
							if (CancelBuild) return;
						}
					}
					else
					{
						EquipmentSlotType est = kvp.Key.ToEquipmentSlotType();
						if (Build.DiscoveredItems.ContainsKey(est))
						{
							Phase2_ProcessBuildFilters(bw, new List<KeyValuePair<EquipmentSlotType, List<DDOItemData>>>() { Build.DiscoveredItems.First(f => f.Key == est) }, kvp.Value);
							if (CancelBuild) return;
						}
					}
				}
			}

			if (gearsetfilters != null)
			{
				List<KeyValuePair<EquipmentSlotType, List<DDOItemData>>> itemlists = Build.DiscoveredItems.ToList();
				Phase2_ProcessBuildFilters(bw, itemlists, gearsetfilters);
				if (CancelBuild) return;
			}

			// now we need to cull out duplicate builds (only possible where the same two rings or two weapons are used but in different slots
			Dictionary<string, List<GearSetEvaluation>> gss = new Dictionary<string, List<GearSetEvaluation>>();
			// alphabetically sort each gear set and then add the string representation to the dictionary
			foreach (var br in Build.BuildResults)
			{
				br.GearSet.Items.Sort((a, b) => string.Compare(a.Item.Name, b.Item.Name));
				string s = br.GearSet.ToString(false);
				if (gss.ContainsKey(s)) gss[s].Add(br);
				else gss[s] = new List<GearSetEvaluation>() { br };
			}
			// duplicates are now identified, so remove them from the BuildResult
			foreach (var kvp in gss)
			{
				for (int i = 1; i < kvp.Value.Count; i++) Build.BuildResults.Remove(kvp.Value[i]);
			}
		}

		BuildPhase2Progress uibp2p;
		void Phase2_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (e.UserState != null)
			{
				uibp2p = (BuildPhase2Progress)e.UserState;
				pbPhase2.Minimum = uibp2p.BarMin;
				pbPhase2.Maximum = uibp2p.BarMax;
				pbPhase2.Value = uibp2p.BarValue;
				pbFilters.Minimum = 0;
				pbFilters.Maximum = uibp2p.TotalFilters;
				pbFilters.Value = uibp2p.CurrentFilter;
				tbFilters.Text = uibp2p.CurrentFilter + " of " + uibp2p.TotalFilters;
			}

			if (pbPhase2.Maximum < e.ProgressPercentage) pbPhase2.Maximum = e.ProgressPercentage;
			pbPhase2.Value = e.ProgressPercentage;

			switch (uibp2p.Stage)
			{
				case EBuildPhase2Progress.FilterStart:
					tbPhase2.Text = null;
					break;

				case EBuildPhase2Progress.GenerateItemLists:
				case EBuildPhase2Progress.BuildGearSets:
					tbPhase2.Text = string.Format(uibp2p.Format, e.ProgressPercentage, (ulong)pbPhase2.Maximum);
					break;
			}
		}

		void Phase2_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			if (CancelBuild)
			{
				CancelAndClose();
				return;
			}

			pbFilters.Value = pbFilters.Maximum;
			pbPhase2.Value = pbPhase2.Maximum;
			tbPhase2.Text = "Done";

			BuildSW.Stop();
			tbFinalResults.Text = string.Format("Build completed in {0}:{1:D2}:{2:D2}", BuildSW.Elapsed.Hours, BuildSW.Elapsed.Minutes, BuildSW.Elapsed.Seconds);
			bdrFinalResults.Visibility = Visibility.Visible;

			DoneProcessing = true;
		}
		#endregion
	}

	public enum EBuildPhase2Progress { FilterStart, GenerateItemLists, BuildGearSets }
	public struct BuildPhase2Progress
	{
		public int BarValue;
		public double BarMin, BarMax;
		public int CurrentFilter, TotalFilters;
		public string Format;
		public EBuildPhase2Progress Stage;
	}
}
