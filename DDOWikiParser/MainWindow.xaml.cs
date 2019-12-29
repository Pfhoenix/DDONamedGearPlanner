using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;
using Sgml;
using DDONamedGearPlanner;

namespace DDOWikiParser
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		string[] files;
		string ErrorFile = "errors.log";

		public MainWindow()
		{
			InitializeComponent();
			File.Delete(ErrorFile);
			LogError("Creating error log on " + DateTime.Now);
		}

		void LogError(string msg)
		{
			var file = File.AppendText(ErrorFile);
			file.WriteLine(msg);
			file.Flush();
			file.Close();
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
			DialogResult dr = fbd.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK)
			{
				files = Directory.GetFiles(fbd.SelectedPath);
				pbProgressBar.Minimum = 0;
				pbProgressBar.Maximum = files.Length;
				pbProgressBar.Value = 0;
				BackgroundWorker bw = new BackgroundWorker();
				bw.WorkerReportsProgress = true;
				bw.DoWork += worker_DoWork;
				bw.ProgressChanged += worker_ProgressChanged;
				bw.RunWorkerCompleted += worker_Completed;

				bw.RunWorkerAsync();
			}
		}

		private void worker_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			// go through treeview and generate item counts
			foreach (TreeViewItem cat in tvList.Items)
			{
				int sum = 0;
				foreach (TreeViewItem tvi in cat.Items)
				{
					if (tvi.Items.Count == 0) sum++;
					else
					{
						tvi.Header += " (" + tvi.Items.Count + ")";
						sum += tvi.Items.Count;
					}
				}

				cat.Header += " (" + sum + ")";
			}

			tbStatusBarText.Text = "Done";
		}

		void SetTreeViewItemAtPath(string path, DDOItemData data)
		{
			string[] nodes = path.Split(',');
			ItemCollection ic = tvList.Items;
			TreeViewItem found = null;
			for (int i = 0; i < nodes.Length; i++)
			{
				found = null;
				foreach (TreeViewItem tvi in ic)
				{
					if (tvi.Header.ToString() == nodes[i])
					{
						found = tvi;
						ic = tvi.Items;
						break;
					}
				}

				if (found == null)
				{
					found = new TreeViewItem { Header = nodes[i] };
					ic.Add(found);
					ic = found.Items;
				}
			}

			found.Tag = data;
		}

		void ParseMinimumLevel(DDOItemData data, XmlElement row)
		{
			int ml = 0;
			string[] split = row.InnerText.Split('\n');
			int.TryParse(split[1], out ml);
			data.AddProperty("Minimum Level", null, ml);
		}

		void ParseEnhancements(DDOItemData data, XmlElement row)
		{

		}

		void ParseArmor(DDOItemData data, XmlNodeList rows)
		{
			data.Slot = SlotType.Body;

			foreach (XmlElement r in rows)
			{
				if (r.InnerText.StartsWith("Feat Requirement"))
				{
					string[] split = r.ChildNodes[1].InnerText.Split(' ');
					split[0] = split[0].Replace("\n", "");
					data.Category = (int)(ArmorCategory)Enum.Parse(typeof(ArmorCategory), split[0]);
					Dispatcher.Invoke(new Action(() => { SetTreeViewItemAtPath("Armor," + split[0] + "," + data.Name, data); }));
				}
				else if (r.InnerText.StartsWith("Minimum Level"))
				{
					ParseMinimumLevel(data, r);
				}
				else if (r.InnerText.StartsWith("Armor Bonus"))
				{
					int j = 0;
					string a = r.InnerText.Substring(11).Replace("+", "").Replace("\n", "");
					if (data.Category == (int)ArmorCategory.Docent)
					{
						// docents require additional parsing : "Armor BonusAdamantine Body:+17Mithral Body:+8Composite Plating:+5"
						//   first chance to implement optional values based on conditions (heavy, medium, light armor options for docents)
						a = a.Replace("Adamantine Body:", "").Replace("Mithral Body", "").Replace("Composite Plating", "");
						string[] split = a.Split(':');
						try
						{
							int.TryParse(split[0], out int ac);
							data.AddProperty("Adamantine Body", "armor", ac);
							int.TryParse(split[1], out ac);
							data.AddProperty("Mithril Body", "armor", ac);
							if (split.Length > 2)
							{
								int.TryParse(split[2], out ac);
								data.AddProperty("Composite Plating", "armor", ac);
							}
							else data.AddProperty("Composite Plating", "armor", 0);
						}
						catch
						{
							// log a parsing error with item data.Name and Armor Bonus
							Dispatcher.Invoke(new Action(() => LogError("- parsing error with docent armor bonus for item " + data.Name)));
						}
					}
					else
					{
						int.TryParse(a, out int ac);
						data.AddProperty("Armor Class", "armor", ac);
					}
				}
				else if (r.InnerText.StartsWith("Enchantments"))
				{
					ParseEnhancements(data, r);
				}
			}
		}

		void ParseShield(DDOItemData data, XmlNodeList rows)
		{
			data.Slot = SlotType.Offhand;

			foreach (XmlElement r in rows)
			{
				if (r.InnerText.StartsWith("Shield Type"))
				{
					string[] split = r.ChildNodes[1].InnerText.Split(' ');
					split[0] = split[0].Replace("\n", "");
					data.Category = (int)(OffhandCategory)Enum.Parse(typeof(OffhandCategory), split[0]);
					Dispatcher.Invoke(new Action(() => { SetTreeViewItemAtPath("Offhand," + split[0] + "," + data.Name, data); }));
				}
				else if (r.InnerText.StartsWith("Minimum Level"))
				{
					ParseMinimumLevel(data, r);
				}
				else if (r.InnerText.StartsWith("Shield Bonus"))
				{
					string a = r.InnerText.Substring(12).Replace("+", "").Replace("\n", "");
					int.TryParse(a, out int ac);
					data.AddProperty("Armor Class", "shield", ac);
				}
				else if (r.InnerText.StartsWith("Enhancements"))
				{
					ParseEnhancements(data, r);
				}
			}
		}

		void ParseWeapon(DDOItemData data, XmlNodeList rows)
		{
			data.Slot = SlotType.Weapon;

			foreach (XmlElement r in rows)
			{

			}
		}

		void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			for (int i = 0; i < files.Length; i++)
			{
				(sender as BackgroundWorker).ReportProgress(i);

				// sgml reader to help format and process html into xml
				SgmlReader sgmlReader = new SgmlReader();
				sgmlReader.DocType = "HTML";
				sgmlReader.WhitespaceHandling = WhitespaceHandling.None;
				sgmlReader.CaseFolding = CaseFolding.ToLower;
				sgmlReader.InputStream = File.OpenText(files[i]);

				// create xml document
				XmlDocument doc = new XmlDocument();
				doc.PreserveWhitespace = false;
				doc.XmlResolver = null;
				doc.Load(sgmlReader);

				// get the item name from the title element
				var tableNodes = doc.GetElementsByTagName("title");
				if (tableNodes.Count == 0) continue;
				string itemName = tableNodes[0].InnerText.Replace(" - DDO wiki", "");
				itemName = itemName.Substring(itemName.IndexOf(':') + 1);

				DDOItemData data = new DDOItemData { Name = itemName };

				// reconstruct the original URL
				var linkNodes = doc.GetElementsByTagName("link");
				data.WikiURL = "https://ddowiki.com";
				foreach (XmlElement ln in linkNodes)
				{
					if (ln.GetAttribute("rel") == "edit")
					{
						data.WikiURL += ln.GetAttribute("href").Replace("edit", "page");
						break;
					}
				}

				// attempt to find the table element
				tableNodes = doc.GetElementsByTagName("table");
				foreach (XmlNode tn in tableNodes)
				{
					var trs = ((XmlElement)tn).GetElementsByTagName("tr");
					XmlElement tr = (XmlElement)trs[0];

					if (tr.InnerText.StartsWith("Armor Type")) ParseArmor(data, trs);
					else if (tr.InnerText.StartsWith("Shield Type")) ParseShield(data, trs);
					else if (tr.InnerText.StartsWith("Proficiency Class")) ParseWeapon(data, trs);
					else
					{ }
				}
			}
		}

		void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			tbStatusBarText.Text = Path.GetFileName(files[e.ProgressPercentage]);
			tbProgressText.Text = (e.ProgressPercentage + 1).ToString() + " of " + files.Length;
			pbProgressBar.Value = e.ProgressPercentage;
		}

		private void TvList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeViewItem tvi = tvList.SelectedItem as TreeViewItem;
			if (tvi.Tag == null) return;
			DDOItemData data = tvi.Tag as DDOItemData;
			lvDetails.Items.Clear();
			lvDetails.Items.Add(new { Property = "Name", Value = data.Name });
			foreach (var p in data.Properties)
				lvDetails.Items.Add(new { Property = p.Property, Value = p.Value });
		}

		private void ViewErrorLog_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(ErrorFile);
		}
	}
}
