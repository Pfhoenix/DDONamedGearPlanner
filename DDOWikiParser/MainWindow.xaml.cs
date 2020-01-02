using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
		string[] TwoHandedWeaponTypes =
		{
			"Quarterstaff",
			"Falchion",
			"Great Axe",
			"Great Club",
			"Maul",
			"Great Sword",
			"Handwraps"
		};
		char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
		string[] numerals = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", "XIII", "XIV", "XV", "XVI", "XVII", "XVIII", "XIX", "XX" };
		char[] spacers = { ' ', ',', '.', '(', ')', '-', '+', '=', '*', '%', '#' };
		string[] Abilities =
		{
			"Strength",
			"Dexterity",
			"Constitution",
			"Intelligence",
			"Wisdom",
			"Charisma"
		};
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
			string[] nodes = path.Split('|');
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
			data.AddProperty("Minimum Level", null, ml, null);
		}

		int ParseNumber(string s)
		{
			int c = s.IndexOfAny(numbers);
			if (c > -1)
			{
				int ce = s.LastIndexOfAny(numbers, Math.Min(c + 5, s.Length - 1));
				string n = s.Substring(c, ce - c + 1);
				int.TryParse(n, out c);

				return c;
			}

			return 0;
		}

		bool ParseEnhancement(DDOItemData data, XmlNode enh)
		{
			string trimmed = enh.InnerText.Trim();
			string xml = enh.InnerXml.Trim();

			if (trimmed.Contains(" Augment Slot"))
			{
				string[] split = trimmed.Split(' ');
				data.AddProperty("Augment Slot", split[1].ToLower(), 0, null);

				return true;
			}
			else if (!trimmed.StartsWith("Mythic "))
			{
				int c = trimmed.IndexOf(':');
				string p = c > -1 ? trimmed.Substring(0, c).Trim() : trimmed;
				string v = c > -1 ? trimmed.Substring(c + 1).Trim() : "";
				int vi = 0;

				if (p.StartsWith("DRDamage Reduction "))
				{
					v = p.Substring(19);
					string[] split = v.Split('/');
					int.TryParse(split[0].Trim(), out vi);
					p = "Damage Reduction";
					v = split[1].Trim().ToLower();
				}
				else if (p.StartsWith("Arcane Casting Dexterity"))
				{
					vi = ParseNumber(p);
					p = "Arcane Spell Failure";
					v = "";
				}
				else if (p.StartsWith("Twilight") || p.StartsWith("Greater Twilight"))
				{
					vi = ParseNumber(p);
					p = "Arcane Spell Failure";
					v = "";
				}
				else
				{
					// attempt to find a numerical value to use as a stopping point for the property name
					c = p.IndexOfAny(numbers);
					if (c > -1)
					{
						int ce = p.LastIndexOfAny(numbers, Math.Min(c + 5, p.Length - 1));
						string n = p.Substring(c, ce - c + 1);
						int.TryParse(n, out vi);
						if (p[c - 1] == '+') p = p.Substring(0, c - 1).Trim();
						else p = p.Substring(0, c).Trim();

						// standardize property names
						if (p.EndsWith("Armor Bonus")) p = "Armor Class";
						else if (p.EndsWith("Wizardry")) p = "Spell Points";
						else if (p.EndsWith("Protection")) p = "Armor Class";
						else if (p.StartsWith("Natural Armor"))
						{
							p = "Armor Class";
							v = "natural armor";
						}
						else if (p.StartsWith("Hardened Exterior")) p = "Armor Class";
						else if (p.StartsWith("Shield")) p = "Armor Class";
						else if (p.EndsWith("Physical Sheltering")) p = "Physical Resistance Rating";
						else if (p.EndsWith("Magical Sheltering")) p = "Magical Resistance Rating";
						else if (p.StartsWith("Melee Alacrity")) p = "Melee Attack Speed";
						else if (p.StartsWith("Ranged Alacrity")) p = "Ranged Attack Speed";
						else if (p.StartsWith("Striding"))
						{
							p = "Move Speed";
							v = "enhancement";
						}
						else if (p.StartsWith("Magical Efficiency")) p = "Spell Point Cost %";
						else if (p.EndsWith("False Life")) p = "Hit Points";
						else if (p.StartsWith("Vitality")) p = "Hit Points";
						else if (p.EndsWith("Cold Resistance")) p = "Cold Resistance";
						else if (p.EndsWith("Fire Resistance")) p = "Fire Resistance";
						else if (p.EndsWith("Electric Resistance")) p = "Electric Resistance";
						else if (p.EndsWith("Acid Resistance")) p = "Acid Resistance";
						else if (p.EndsWith("Sonic Resistance")) p = "Sonic Resistance";
						else if (p.EndsWith("Spell Focus")) p = "Spell DCs";
						else if (p.EndsWith("Spell Focus Mastery")) p = "Spell DCs";
						else if (p.EndsWith("Corrosion")) p = "Acid Spell Power";
						else if (p.EndsWith("Glaciation")) p = "Cold Spell Power";
						else if (p.EndsWith("Magnetism")) p = "Electric Spell Power";
						else if (p.EndsWith("Combustion")) p = "Fire Spell Power";
						else if (p.EndsWith("Radiance")) p = "Light Spell Power";
						else if (p.EndsWith("Devotion")) p = "Positive Spell Power";
						else if (p.EndsWith("Acid Lore")) p = "Acid Spell Critical Chance";
						else if (p.EndsWith("Fire Lore")) p = "Fire Spell Critical Chance";
						else if (p.EndsWith("Ice Lore")) p = "Cold Spell Critical Chance";
						else if (p.EndsWith("Lightning Lore")) p = "Electric Spell Critical Chance";
						else if (p.EndsWith("Healing Lore")) p = "Positive Spell Critical Chance";
						else if (p.EndsWith("Kinetic Lore")) p = "Force Spell Critical Chance";
						else if (p.EndsWith("Radiance Lore")) p = "Light Spell Critical Chance";
						else if (p.EndsWith("Repair Lore")) p = "Repair Spell Critical Chance";
						else if (p.EndsWith("Sonic Lore")) p = "Sonic Spell Critical Chance";
						else if (p.EndsWith("Spellcasting Implement")) p = "Universal Spell Power";
						else if (p.EndsWith("Distant Diversion"))
						{
							p = "Ranged Threat Reduction";
							v = "";
						}
						else if (p.EndsWith("Diversion"))
						{
							p = "Melee Threat Reduction";
							v = "";
						}
						else if (p.EndsWith("Open Lock")) p = "Open Lock";
						else if (p == "Greater Elemental Energy")
						{
							p = "Hit Points";
							v = "greater elemental energy";
							vi = 20;
						}
						else if (p == "Greater Elemental Spell Power")
						{
							p = "Spell Points";
							v = "greater elemental spell power";
							vi = 100;
						}
					}
					else
					{
						// no numbers means either roman numerals or just a name
						// bypass processing the innertext by finding the first <a> element and grabbing its inner text

						// first try to drill down to the first child with no children
						XmlNode child = enh.FirstChild;
						while (child.HasChildNodes) child = child.FirstChild;
						p = child.InnerText.Trim();
						c = p.LastIndexOf(' ');
						if (c > -1)
						{
							string rn = p.Substring(c + 1).ToUpper();
							for (int rni = 0; rni < numerals.Length; rni++)
							{
								if (numerals[rni] == rn)
								{
									vi = rni + 1;
									break;
								}
							}

							if (vi > 0)
							{
								p = p.Substring(0, c);
								// we flag this for special handling later
								if (p == "Parrying") v = numerals[vi - 1];
							}
						}

						// standardize property names
						if (p.EndsWith("Wizardry"))
						{
							p = "Spell Points";
							if (data.Name.EndsWith("Cunning Trinket"))
							{
								v = "pirate cleverness";
								vi = 50;
							}
							else
							{
								v = "enhancement";
								vi *= 25;
							}
						}
						else if (p == "Magi")
						{
							p = "Spell Points";
							v = "enhancement";
							vi = 100;
						}
						else if (p == "Archmagi")
						{
							p = "Spell Points";
							v = "enhancement";
							vi = 200;
						}
						else if (p == "Dusk")
						{
							p = "Concealment";
							v = "enhancement";
							vi = 10;
						}
						else if (p == "Smoke Screen")
						{
							p = "Concealment";
							v = "enhancement";
							vi = 20;
						}
						else if (p == "Blurry")
						{
							p = "Concealment";
							v = "enhancement";
							vi = 20;
						}
						else if (p == "Lesser Displacement")
						{
							p = "Concealment";
							v = "enhancement";
							vi = 25;
						}
						else if (p == "Power")
						{
							p = "Spell Points";
							v = "enhancement";
							vi *= 10;
						}
					}
				}

				// attempt to find a type to the bonus/value
				c = v.IndexOf(" bonus to ", StringComparison.InvariantCultureIgnoreCase);
				if (c == -1) c = v.IndexOf(" bonuses to ", StringComparison.InvariantCultureIgnoreCase);
				if (c > -1)
				{
					int ce = v.LastIndexOf(' ', c - 1);
					v = Regex.Replace(v.Substring(ce + 1, c - ce), @"\W+", "");
					v = Regex.Replace(v, @"^\d+", "");
					v = v.ToLower();

					// we found a bonus type, let's try to clean up a redundant reference in the property name
					if (p.IndexOf(v, StringComparison.InvariantCultureIgnoreCase) == 0)
						p = p.Substring(p.IndexOf(' ') + 1).Trim();
				}

				// special case check for weapon or armor base enhancement
				if (string.IsNullOrWhiteSpace(p) && v == "enhancement")
				{
					if (data.Slot == SlotType.Body || data.Slot == SlotType.Offhand) p = "Armor Class";
					else if (data.Slot == SlotType.Weapon) p = "Attack and Damage";
				}

				// some enhancements have multiple effects, and we want to capture them individually
				if (p == "Attack and Damage")
				{
					data.AddProperty("Attack", v, vi, null);
					data.AddProperty("Damage", v, vi, null);
				}
				else if (p == "Good Luck")
				{
					data.AddProperty("Fortitude", v, vi, null);
					data.AddProperty("Reflex", v, vi, null);
					data.AddProperty("Will", v, vi, null);
					data.AddProperty("Skill Checks", v, vi, null);
				}
				else if (p == "Resistance")
				{
					data.AddProperty("Fortitude", v, vi, null);
					data.AddProperty("Reflex", v, vi, null);
					data.AddProperty("Will", v, vi, null);
				}
				else if (p == "Parrying")
				{
					if (v == "I") vi = 1;
					else if (v == "IV") vi = 2;
					else if (v == "VIII") vi = 4;

					data.AddProperty("Armor Class", "insight", vi, null);
					data.AddProperty("Fortitude", "insight", vi, null);
					data.AddProperty("Reflex", "insight", vi, null);
					data.AddProperty("Will", "insight", vi, null);
				}
				else if (p == "Sheltering")
				{
					data.AddProperty("Physical Resistance Rating", v, vi, null);
					data.AddProperty("Magical Resistance Rating", v, vi, null);
				}
				else if (p == "Riposte")
				{
					data.AddProperty("Armor Class", "insight", vi, null);
					data.AddProperty("Fortitude", "insight", vi, null);
					data.AddProperty("Reflex", "insight", vi, null);
					data.AddProperty("Will", "insight", vi, null);
				}
				else if (p == "Improved Deception")
				{
					data.AddProperty(p, v, vi, null);
					data.AddProperty("Bluff", "enhancement", 5, null);
				}
				else if (p == "Well Rounded")
				{
					data.AddProperty("Strength", v, vi, null);
					data.AddProperty("Dexterity", v, vi, null);
					data.AddProperty("Constitution", v, vi, null);
					data.AddProperty("Intelligence", v, vi, null);
					data.AddProperty("Wisdom", v, vi, null);
					data.AddProperty("Charisma", v, vi, null);
				}
				else if (p == "Speed")
				{
					data.AddProperty("Move Speed", v, vi, null);
					data.AddProperty("Melee Attack Speed", v, vi / 2, null);
					data.AddProperty("Ranged Attack Speed", v, vi / 2, null);
				}
				else if (p == "Axeblock")
				{
					data.AddProperty("Damage Reduction", "pierce", vi * 2, null);
					data.AddProperty("Damage Reduction", "bludgeon", vi * 2, null);
				}
				else if (p == "Spearblock")
				{
					data.AddProperty("Damage Reduction", "slash", vi * 2, null);
					data.AddProperty("Damage Reduction", "bludgeon", vi * 2, null);
				}
				else if (p == "Hammerblock")
				{
					data.AddProperty("Damage Reduction", "pierce", vi * 2, null);
					data.AddProperty("Damage Reduction", "slash", vi * 2, null);
				}
				else if (p == "Potency")
				{
					data.AddProperty("Acid Spell Power", v, vi, null);
					data.AddProperty("Cold Spell Power", v, vi, null);
					data.AddProperty("Electric Spell Power", v, vi, null);
					data.AddProperty("Fire Spell Power", v, vi, null);
					data.AddProperty("Force Spell Power", v, vi, null);
					data.AddProperty("Light Spell Power", v, vi, null);
					data.AddProperty("Negative Spell Power", v, vi, null);
					data.AddProperty("Poison Spell Power", v, vi, null);
					data.AddProperty("Positive Spell Power", v, vi, null);
					data.AddProperty("Repair Spell Power", v, vi, null);
					data.AddProperty("Sonic Spell Power", v, vi, null);
				}
				else if (p.EndsWith("Spell Lore"))
				{
					data.AddProperty("Acid Spell Critical Chance", v, vi, null);
					data.AddProperty("Cold Spell Critical Chance", v, vi, null);
					data.AddProperty("Electric Spell Critical Chance", v, vi, null);
					data.AddProperty("Fire Spell Critical Chance", v, vi, null);
					data.AddProperty("Force Spell Critical Chance", v, vi, null);
					data.AddProperty("Light Spell Critical Chance", v, vi, null);
					data.AddProperty("Negative Spell Critical Chance", v, vi, null);
					data.AddProperty("Poison Spell Critical Chance", v, vi, null);
					data.AddProperty("Positive Spell Critical Chance", v, vi, null);
					data.AddProperty("Repair Spell Critical Chance", v, vi, null);
					data.AddProperty("Sonic Spell Critical Chance", v, vi, null);
				}
				else if (p == "Void Lore")
				{
					data.AddProperty("Negative Spell Critical Chance", v, vi, null);
					data.AddProperty("Poison Spell Critical Chance", v, vi, null);
				}
				else if (p == "Frozen Thunderstorm Lore")
				{
					data.AddProperty("Cold Spell Critical Chance", v, vi, null);
					data.AddProperty("Lightning Spell Critical Chance", v, vi, null);
					data.AddProperty("Sonic Spell Critical Chance", v, vi, null);
				}
				else if (p == "Power of the Frozen Thunderstorm")
				{
					data.AddProperty("Cold Spell Power", v, vi, null);
					data.AddProperty("Electric Spell Power", v, vi, null);
					data.AddProperty("Sonic Spell Power", v, vi, null);
				}
				else if (p.EndsWith("Nullification"))
				{
					data.AddProperty("Negative Spell Power", v, vi, null);
					data.AddProperty("Poison Spell Power", v, vi, null);
				}
				else if (p.EndsWith("Seeker"))
				{
					data.AddProperty("Confirm Critical Hits", v, vi, null);
					data.AddProperty("Critical Hit Damage", v, vi, null);
				}
				else if (p.EndsWith("Deception"))
				{
					data.AddProperty("Sneak Attack Attack", v, vi, null);
					data.AddProperty("Sneak Attack Damage", v, (int)Math.Round(vi * 1.5f), null);
				}
				else data.AddProperty(p, v, vi, null);

				return true;
			}
			else return false;
		}

		void ParseEnhancements(DDOItemData data, XmlElement row)
		{
			try
			{
				var ul = row.GetElementsByTagName("ul");
				if (ul.Count == 0) return;
				foreach (XmlElement e in ul[0].ChildNodes)
				{
					List<ItemProperty> options = null;

					string trimmed = e.InnerText.Trim();
					if (string.IsNullOrWhiteSpace(trimmed)) continue;
					string xml = e.InnerXml.Trim();

					if (e.InnerText.StartsWith("Nearly Finished") || e.InnerText.StartsWith("Almost There"))
					{
						options = new List<ItemProperty>();

						var lis = e.GetElementsByTagName("li");
						if (lis.Count == 0)
						{
							// check for a hyperlink to Named item sets
							var aa = e.GetElementsByTagName("a");
							foreach (XmlElement a in aa)
							{
								if (a.GetAttribute("href").IndexOf("/page/Named_item_sets") > -1)
								{
									string p = a.InnerText.Trim();
									options.Add(new ItemProperty { Property = p, Type = "set" });
									break;
								}
							}

							if (options.Count == 0) options = null;
						}
						else
						{
							foreach (XmlNode sul in e.ChildNodes)
							{
								if (sul.Name != "ul") continue;

								foreach (XmlNode li in sul.ChildNodes)
								{
									string tli = li.InnerText.Trim();
									if (tli.IndexOf("One of the following", StringComparison.InvariantCultureIgnoreCase) > -1 || tli.IndexOf("Random effect", StringComparison.InvariantCultureIgnoreCase) > -1)
									{
										foreach (XmlNode cul in li.ChildNodes)
										{
											if (cul.Name != "ul") continue;
											foreach (XmlNode cil in cul.ChildNodes)
											{
												if (ParseEnhancement(data, cil))
												{
													ItemProperty ip = data.Properties[data.Properties.Count - 1];
													data.Properties.Remove(ip);
													options.Add(ip);
													if (Abilities.Contains(ip.Property) && string.IsNullOrWhiteSpace(ip.Type))
													{
														ip.Type = "enhancement";
													}
												}
											}
										}
									}
									else if (ParseEnhancement(data, li))
									{
										ItemProperty ip = data.Properties[data.Properties.Count - 1];
										data.Properties.Remove(ip);
										options.Add(ip);
										if (Abilities.Contains(ip.Property))
										{
											ip.Type = "enhancement";
											options.Add(ip);
										}
										else if (ip.Property == "Ability")
										{
											// all abilities
											ip.Property = "Strength";
											if (string.IsNullOrWhiteSpace(ip.Type)) ip.Type = "enhancement";
											options.Add(new ItemProperty { Property = "Dexterity", Type = ip.Type, Value = ip.Value });
											options.Add(new ItemProperty { Property = "Constitution", Type = ip.Type, Value = ip.Value });
											options.Add(new ItemProperty { Property = "Intelligence", Type = ip.Type, Value = ip.Value });
											options.Add(new ItemProperty { Property = "Wisdom", Type = ip.Type, Value = ip.Value });
											options.Add(new ItemProperty { Property = "Charisma", Type = ip.Type, Value = ip.Value });
										}
										else if (li.InnerText.Contains("mind related ability statistics"))
										{
											// Intelligence, Wisdom, Charisma
											ip.Property = "Intelligence";
											if (string.IsNullOrWhiteSpace(ip.Type)) ip.Type = "enhancement";
											options.Add(new ItemProperty { Property = "Wisdom", Type = ip.Type, Value = ip.Value });
											options.Add(new ItemProperty { Property = "Charisma", Type = ip.Type, Value = ip.Value });
										}
										else if (li.InnerText.Contains("body related ability statistics"))
										{
											// Strength, Dexterity, Constitution
											ip.Property = "Strength";
											if (string.IsNullOrWhiteSpace(ip.Type)) ip.Type = "enhancement";
											options.Add(new ItemProperty { Property = "Dexterity", Type = ip.Type, Value = ip.Value });
											options.Add(new ItemProperty { Property = "Constitution", Type = ip.Type, Value = ip.Value });
										}
									}
								}
							}
						}

						if (e.InnerText.StartsWith("Nearly Finished"))
							data.AddProperty("Nearly Finished", null, 0, options);
						else data.AddProperty("Almost There", null, 0, options);
					}
					else if (e.InnerText.StartsWith("Upgrades"))
					{
						options = new List<ItemProperty>();
						var aa = e.GetElementsByTagName("a");
						foreach (XmlElement a in aa)
						{
							if (a.GetAttribute("href").IndexOf("/page/Named_item_sets") > -1)
							{
								int s = a.InnerText.IndexOf(" Set", StringComparison.InvariantCultureIgnoreCase);
								string p;
								if (s > -1) p = a.InnerText.Substring(0, s).Trim();
								else p = a.InnerText;
								if (options.Find(m => m.Property == p) == null)
									options.Add(new ItemProperty { Property = p, Type = "set" });
							}
						}

						data.AddProperty("Upgradeable", "set", 0, options);
					}
					else if (e.InnerText.StartsWith("Upgradeable - Primary Augment"))
					{
						options = new List<ItemProperty>();
						options.Add(new ItemProperty { Property = "Augment Slot", Type = "yellow" });
						options.Add(new ItemProperty { Property = "Augment Slot", Type = "blue" });
						if (data.Slot == SlotType.Weapon || data.Slot == SlotType.Offhand)
							options.Add(new ItemProperty { Property = "Augment Slot", Type = "red" });

						data.AddProperty("Upgradeable", "primary augment", 0, options);
					}
					else if (e.InnerText.StartsWith("Upgradeable - Secondary Augment"))
					{
						options = new List<ItemProperty>();
						options.Add(new ItemProperty { Property = "Augment Slot", Type = "green" });
						if (data.Slot == SlotType.Weapon || data.Slot == SlotType.Offhand)
						{
							options.Add(new ItemProperty { Property = "Augment Slot", Type = "orange" });
							options.Add(new ItemProperty { Property = "Augment Slot", Type = "purple" });
						}

						data.AddProperty("Upgradeable", "secondary augment", 0, options);
					}
					else if (e.InnerText.StartsWith("One of the following"))
					{
						options = new List<ItemProperty>();

						var lis = e.GetElementsByTagName("li");
						foreach (XmlElement li in lis)
						{
							if (ParseEnhancement(data, li))
							{
								ItemProperty ip = data.Properties[data.Properties.Count - 1];
								data.Properties.Remove(ip);
								options.Add(ip);
							}
						}

						data.AddProperty("Random", null, 0, options);
					}
					else if (e.InnerXml.IndexOf("/page/Named_item_sets") > -1)
					{
						var aa = e.GetElementsByTagName("a");
						foreach (XmlElement a in aa)
						{
							if (a.GetAttribute("href").IndexOf("/page/Named_item_sets") > -1)
							{
								string p = a.InnerText;
								data.AddProperty(p, "set", 0, null);
								break;
							}
						}
					}
					else if (e.InnerXml.StartsWith("Against the Slave Lords Set Bonus"))
					{
						options = new List<ItemProperty>();
						string d = data.Name.StartsWith("Legendary ") ? "Legendary " : "";
						options.Add(new ItemProperty { Property = d + "Slave Lord's Might", Type = "set" });
						options.Add(new ItemProperty { Property = d + "Slave Lord's Sorcery", Type = "set" });
						options.Add(new ItemProperty { Property = d + "Slave's Endurance", Type = "set" });

						data.AddProperty("Against the Slave Lords Set Bonus", null, 0, options);
					}
					else ParseEnhancement(data, e);
				}
			}
			catch (Exception ex)
			{
				LogError("- parsing error with enhancements for item " + data.Name + Environment.NewLine + ex.Message);
			}
		}

		string ParseArmor(DDOItemData data, XmlNodeList rows)
		{
			string tvpath = null;

			data.Slot = SlotType.Body;

			foreach (XmlElement r in rows)
			{
				if (r.InnerText.StartsWith("Feat Requirement"))
				{
					string[] split = r.ChildNodes[1].InnerText.Split(' ');
					split[0] = split[0].Replace("\n", "");
					data.Category = (int)(ArmorCategory)Enum.Parse(typeof(ArmorCategory), split[0]);
					tvpath = "Armor|" + split[0] + "|" + data.Name;
				}
				else if (r.InnerText.StartsWith("Minimum Level"))
				{
					ParseMinimumLevel(data, r);
				}
				else if (r.InnerText.StartsWith("Armor Bonus"))
				{
					string a = r.InnerText.Substring(11).Replace("+", "").Replace("\n", "");
					if (data.Category == (int)ArmorCategory.Docent)
					{
						// docents require additional parsing : "Armor BonusAdamantine Body:+17Mithral Body:+8Composite Plating:+5"
						//   first chance to implement optional values based on conditions (heavy, medium, light armor options for docents)
						a = a.Replace("Adamantine Body:", "").Replace("Mithral Body", "").Replace("Composite Plating", "").Replace(",", "").Replace(";", "");
						string[] split = a.Split(':');
						try
						{
							int.TryParse(split[0].Trim(), out int ac);
							data.AddProperty("Adamantine Body", "armor", ac, null);
							int.TryParse(split[1].Trim(), out ac);
							data.AddProperty("Mithril Body", "armor", ac, null);
							if (split.Length > 2)
							{
								int.TryParse(split[2].Trim(), out ac);
								data.AddProperty("Composite Plating", "armor", ac, null);
							}
							else data.AddProperty("Composite Plating", "armor", 0, null);
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
						if (ac > 0) data.AddProperty("Armor Class", "armor", ac, null);
					}
				}
				else if (r.InnerText.StartsWith("Enchantments"))
				{
					ParseEnhancements(data, r);
				}
				else if (r.InnerText.IndexOf("drops on death", StringComparison.InvariantCultureIgnoreCase) > -1)
				{
					return null;
				}
			}

			return tvpath;
		}

		string ParseShield(DDOItemData data, XmlNodeList rows)
		{
			string tvpath = null;

			data.Slot = SlotType.Offhand;

			foreach (XmlElement r in rows)
			{
				if (r.InnerText.StartsWith("Shield Type"))
				{
					string[] split = r.ChildNodes[1].InnerText.Split(' ');
					split[0] = split[0].Replace("\n", "");
					data.Category = (int)(OffhandCategory)Enum.Parse(typeof(OffhandCategory), split[0]);
					tvpath = "Offhand|" + split[0] + "|" + data.Name;
				}
				else if (r.InnerText.StartsWith("Minimum Level"))
				{
					ParseMinimumLevel(data, r);
				}
				else if (r.InnerText.StartsWith("Shield Bonus"))
				{
					string a = r.InnerText.Substring(12).Replace("+", "").Replace("\n", "");
					int.TryParse(a, out int ac);
					data.AddProperty("Armor Class", "shield", ac, null);
				}
				else if (r.InnerText.StartsWith("Enhancements"))
				{
					ParseEnhancements(data, r);
				}
				else if (r.InnerText.IndexOf("drops on death", StringComparison.InvariantCultureIgnoreCase) > -1)
				{
					return null;
				}
			}

			return tvpath;
		}

		string ParseWeapon(DDOItemData data, XmlNodeList rows)
		{
			string tvpath = null;

			data.Slot = SlotType.Weapon;

			foreach (XmlElement r in rows)
			{
				if (r.InnerText.StartsWith("Proficiency Class"))
				{
					try
					{
						string[] split = r.InnerText.Split('\n');
						split = split[1].Split(' ');
						data.Category = (int)(WeaponCategory)Enum.Parse(typeof(WeaponCategory), split[0]);
					}
					catch
					{
						LogError("- parse error with proficiency class for weapon named " + data.Name);
					}
				}
				else if (r.InnerText.StartsWith("Weapon Type"))
				{
					try
					{
						string[] split = r.InnerText.Split('\n');
						split = split[1].Split('/');
						ItemProperty ip = data.AddProperty("Weapon Type", split[0].Trim(), 0, null);
						data.AddProperty("Handedness", null, TwoHandedWeaponTypes.Contains(ip.Type) ? 2 : 1, null);
						if (split[1].IndexOf("throw", StringComparison.OrdinalIgnoreCase) > -1)
						{
							data.Category = (int)WeaponCategory.Throwing;
						}
						tvpath = "Weapon|" + (WeaponCategory)data.Category + "|" + data.Name;
					}
					catch
					{
						LogError("- parse error with weapon type for weapon named " + data.Name);
					}
				}
				else if (r.InnerText.StartsWith("Minimum Level"))
				{
					ParseMinimumLevel(data, r);
				}
				else if (r.InnerText.StartsWith("Enchantments"))
				{
					ParseEnhancements(data, r);
				}
				else if (r.InnerText.IndexOf("drops on death", StringComparison.InvariantCultureIgnoreCase) > -1)
				{
					return null;
				}
			}

			return tvpath;
		}

		string ParseItem(DDOItemData data, XmlNodeList rows)
		{
			string tvpath = null;

			foreach (XmlElement r in rows)
			{
				if (r.InnerText.StartsWith("Slot"))
				{
					try
					{
						//string[] split = r.InnerText.Split('\n');
						//split = split[1].Split(' ');
						data.Slot = (SlotType)Enum.Parse(typeof(SlotType), r.InnerText.Replace("\n", "").Substring(4));
						tvpath = data.Slot + "|" + data.Name;
					}
					catch
					{
						LogError("- parse error with slot for item named " + data.Name);
					}
				}
				else if (r.InnerText.StartsWith("Minimum level"))
				{
					ParseMinimumLevel(data, r);
				}
				else if (r.InnerText.StartsWith("Enchantments"))
				{
					ParseEnhancements(data, r);
				}
				else if (r.InnerText.IndexOf("drops on death", StringComparison.InvariantCultureIgnoreCase) > -1)
				{
					return null;
				}
			}

			return tvpath;
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

					string tvpath = null;
					if (tr.InnerText.StartsWith("Armor Type")) tvpath = ParseArmor(data, trs);
					else if (tr.InnerText.StartsWith("Shield Type")) tvpath = ParseShield(data, trs);
					else if (tr.InnerText.StartsWith("Proficiency Class")) tvpath = ParseWeapon(data, trs);
					else tvpath = ParseItem(data, trs);

					if (tvpath != null) Dispatcher.Invoke(new Action(() => { SetTreeViewItemAtPath(tvpath, data); }));
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
			foreach (var p in data.Properties)
			{
				lvDetails.Items.Add(new { p.Property, p.Type, p.Value });
				if (p.Options != null)
				{
					foreach (var ip in p.Options)
						lvDetails.Items.Add(new { Property = "> " + ip.Property, ip.Type, ip.Value });
				}
			}
		}

		private void ViewErrorLog_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(ErrorFile);
		}
	}
}
