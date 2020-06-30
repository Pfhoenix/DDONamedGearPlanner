using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;
using Sgml;
using DDONamedGearPlanner;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

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
			"Handwraps",
			"Heavy Crossbow",
			"Light Crossbow",
			"Long Bow",
			"Short Bow",
			"Repeating Heavy Crossbow",
			"Repeating Light Crossbow",
		};
		string[] RuneArmCompatibleTwoHandedWeaponTypes =
		{
			"Heavy Crossbow",
			"Light Crossbow",
			"Repeating Heavy Crossbow",
			"Repeating Light Crossbow"
		};
		string[] MinorArtifacts =
		{
			"Band of Diani ir'Wynarn",
			"Brand of Kalok Shash",
			"Brightlord, the Sigil of the Flame",
			"Charune, Grand Burden of Fury",
			"Doctor LeRoux's Curious Implant",
			"Doctor Vulcana's Broken Wristwatch",
			"Epic Voice of the Master",
			"Gauntlet of the Iron Council",
			"Gilded Gloves of Sanctity",
			"Ir'Kesslan's Most Prescient Lens",
			"Key of Rhukaan Draal",
			"Radiant Ring of Taer Valaestas",
			"Sigil of Regalport",
			"Sigil of the Triumvirate",
			"Stolen Signet of ir'Wynarn",
			"The Shrouded Steps of the Beyond",
			"The Zarash'ak Ward"
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

		List<DDOItemData> ItemsCache = new List<DDOItemData>();
		Dictionary<string, List<DDOItemData>> QuestLookup = new Dictionary<string, List<DDOItemData>>();
		Dictionary<string, DDOAdventurePackData> AdpackLookup = new Dictionary<string, DDOAdventurePackData>();

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
			fbd.SelectedPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\..\\DDOWikiCrawler\\bin\\x64\\debug\\HtmlCache\\");
			if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				files = Directory.GetFiles(fbd.SelectedPath);
				pbProgressBar.Minimum = 0;
				pbProgressBar.Maximum = files.Length;
				pbProgressBar.Value = 0;
				BackgroundWorker bw = new BackgroundWorker();
				bw.WorkerReportsProgress = true;
				bw.DoWork += InitialLoad_DoWork;
				bw.ProgressChanged += InitialLoad_ProgressChanged;
				bw.RunWorkerCompleted += InitialLoad_Completed;

				bw.RunWorkerAsync();
			}
		}

		private void InitialLoad_Completed(object sender, RunWorkerCompletedEventArgs e)
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

			pbProgressBar.Minimum = 0;
			pbProgressBar.Maximum = QuestLookup.Count;
			pbProgressBar.Value = 0;

			BackgroundWorker bw = new BackgroundWorker();
			bw.WorkerReportsProgress = true;
			bw.DoWork += QuestParse_DoWork;
			bw.ProgressChanged += QuestParse_ProgressChanged;
			bw.RunWorkerCompleted += QuestParse_Completed;

			bw.RunWorkerAsync();
		}

		void QuestParse_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			tbProgressText.Text = null;
			tbStatusBarText.Text = "Done";

			foreach (var item in ItemsCache)
			{
				if (item.QuestFoundIn == null) LogError("Null quest for item " + item.Name + ", " + item.WikiURL);
			}
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
			int ml = 1;
			string[] split = row.InnerText.Split('\n');
			if (!int.TryParse(split[1], out ml)) ml = 1;
			data.AddProperty("Minimum Level", null, ml, null);
		}

		string ParseLocation(DDOItemData data, XmlElement row)
		{
			var a = row.GetElementsByTagName("a");
			if (a.Count > 0)
			{
				string href = (a[0] as XmlElement)?.GetAttribute("href");
				if (string.IsNullOrWhiteSpace(href)) return null;

				if (href == "/page/Epic_Crafting")
				{
					// this will signal the process further on to look for 
					href = row.InnerText.Replace("Location\n", "");
				}

				if (QuestLookup.ContainsKey(href)) QuestLookup[href].Add(data);
				else QuestLookup[href] = new List<DDOItemData>() { data };

				return href;
			}

			return null;
		}

		int ParseNumber(string s, int start = 0)
		{
			int c = s.IndexOfAny(numbers, start);
			if (c > -1)
			{
				int e = c;
				for (int i = e; i < s.Length; i++)
				{
					if (!numbers.Contains(s[i]))
					{
						e = i;
						break;
					}
				}
				if (e == c) e = s.Length - 1;
				string n = s.Substring(c, e - c);
				int.TryParse(n, out c);

				return c;
			}

			return 0;
		}

		string ParseSetName(XmlElement e)
		{
			var aa = e.GetElementsByTagName("a");
			foreach (XmlElement a in aa)
			{
				if (a.GetAttribute("href").IndexOf("/page/Named_item_sets") > -1)
				{
					return a.InnerText.Trim();
				}
			}

			return null;
		}

		string ParseAugmentSlot(string text)
		{
			string[] split = text.Split(' ');
			for (int i = 0; i < split.Length - 1; i++)
			{
				if (split[i] == "Augment" && split[i + 1].StartsWith("Slot")) return split[i - 1].ToLower();
			}

			return null;
		}

		string ParseFirstChildText(XmlNode e)
		{
			XmlNode child = e.FirstChild;
			while (child.HasChildNodes) child = child.FirstChild;
			return child.InnerText.Trim();
		}

		bool ParseEnhancement(DDOItemData data, XmlNode enh)
		{
			string trimmed = enh.InnerText.Replace((char)160, ' ').Replace("  ", " ").Trim();

			if (trimmed.StartsWith("If at least ", StringComparison.InvariantCultureIgnoreCase)) return false;
			else if (trimmed.StartsWith("Bound to Character", StringComparison.InvariantCultureIgnoreCase) || trimmed.StartsWith("Bound to Account", StringComparison.InvariantCultureIgnoreCase)) return false;
			else if (trimmed.StartsWith("Craftable (")) return false;
			else if (trimmed.StartsWith("Better Offhanded")) return false;
			else if (trimmed.StartsWith("Mythic ")) return false;
			else if (trimmed.StartsWith("Unverified")) return false;
			else if (trimmed.StartsWith("Echoes of 2006"))
			{
				data.AddProperty("Echoes of 2006", null, 0, null);
				return true;
			}
			else if (trimmed.StartsWith("Tet-zik, The Enlightened Change"))
			{
				data.AddProperty("Tet-zik, The Enlightened Change", null, 0, null);
				return true;
			}
			else if (trimmed.StartsWith("Shield Bonus"))
			{
				string a = trimmed.Substring(12).Replace("+", "").Replace("\n", "");
				int.TryParse(a, out int ac);
				data.AddProperty("Armor Class", "shield", ac, null);
				return true;
			}
			else if (trimmed.Contains(" Augment Slot"))
			{
				data.AddProperty("Augment Slot", ParseAugmentSlot(trimmed), 0, null);
				return true;
			}
			else if (trimmed.StartsWith("Alarphon's Staff: Spell Selection"))
			{
				data.AddProperty("Alarphon's Staff: Spell Selection", null, 0, null);
				return true;
			}
			else if (trimmed.StartsWith("Alchemical (Prototype)"))
			{
				data.AddProperty("Alchemical (Prototype)", null, 0, null);
				return true;
			}
			else if (trimmed.StartsWith("Feat:"))
			{
				int f = trimmed.IndexOf("Feat:", 5);
				data.AddProperty("Feat", trimmed.Substring(5, f - 5).Trim(), 0, null);
				return true;
			}
			else if (trimmed.StartsWith("Absorb Enchantment Spells"))
			{
				data.AddProperty("Absorb Enchantment Spells", null, 0, null);
				return true;
			}
			else if (trimmed.StartsWith("Lesser Absorption of Beholder and Doomsphere Spells"))
			{
				data.AddProperty("Absorb Beholder and Doomsphere Spells", null, 0, null);
				return true;
			}
			else if (trimmed.StartsWith("Absorb Beholder Spells"))
			{
				data.AddProperty("Absorb Beholder Spells", null, 0, null);
				return true;
			}
			else if (trimmed.StartsWith("Absorbs Magic Missiles -"))
			{
				data.AddProperty("Absorbs Magic Missiles", null, ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Upgradeable - Tier ")) return false;
			else if (trimmed.StartsWith("Strength -"))
			{
				data.AddProperty("Strength", null, -ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Dexterity -"))
			{
				data.AddProperty("Dexterity", null, -ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Constitution -"))
			{
				data.AddProperty("Constitution", null, -ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Intelligence -"))
			{
				data.AddProperty("Intelligence", null, -ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Wisdom -"))
			{
				data.AddProperty("Wisdom", null, -ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Charisma -"))
			{
				data.AddProperty("Charisma", null, -ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Spell Absorption -"))
			{
				int a = ParseNumber(trimmed);
				data.AddProperty("Acid Absorption", null, a, null);
				data.AddProperty("Cold Absorption", null, a, null);
				data.AddProperty("Electric Absorption", null, a, null);
				data.AddProperty("Fire Absorption", null, a, null);
				data.AddProperty("Force Absorption", null, a, null);
				data.AddProperty("Light Absorption", null, a, null);
				data.AddProperty("Negative Absorption", null, a, null);
				data.AddProperty("Poison Absorption", null, a, null);
				data.AddProperty("Positive Absorption", null, a, null);
				data.AddProperty("Repair Absorption", null, a, null);
				data.AddProperty("Sonic Absorption", null, a, null);

				data.AddProperty("Spell Absorption", null, ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Negative Energy Absorption -"))
			{
				data.AddProperty("Negative Absorption", null, ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Whirlwind Absorption -"))
			{
				data.AddProperty("Whirlwind Absorption", null, ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Absorb Doomsphere Spells -"))
			{
				data.AddProperty("Absorb Doomsphere Spells", null, ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Percussive Maintenance"))
			{
				data.AddProperty("Percussive Maintenance", null, 0, null);
				return true;
			}
			else if (trimmed.StartsWith("Thunder-Forged (Tier"))
			{
				data.AddProperty(trimmed.Substring(0, trimmed.IndexOf(')') + 1), null, 0, null);
				return true;
			}
			else if (trimmed.StartsWith("Magestar Absorption"))
			{
				data.AddProperty("Magestar Absorption", null, ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Absorbs Magic Missiles"))
			{
				data.AddProperty("Absorbs Magic Missiles", null, ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Scarab of Protection Ward"))
			{
				data.AddProperty("Absorbs Negative Energy, Death Effects, and Energy Drain", null, ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Absorb Petrification"))
			{
				data.AddProperty("Absorbs Petrification", null, ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("A Mysterious Effect"))
			{
				int count = data.Name == "Mysterious Ring" ? 2 : 1;
				for (int i = 0; i < count; i++)
					data.AddProperty("Mysterious Effect", null, 1, new List<ItemProperty>
					{
						new ItemProperty { Property = "Mysterious Effect Option 1", Type = "set" },
						new ItemProperty { Property = "Mysterious Effect Option 2", Type = "set" },
						new ItemProperty { Property = "Mysterious Effect Option 3", Type = "set" },
						new ItemProperty { Property = "Mysterious Effect Option 4", Type = "set" },
					});

				return true;
			}
			else if (trimmed.StartsWith("Brilliant Silver Scales"))
			{
				data.AddProperty("Cold Resistance", "enhancement", 83, null);
				return true;
			}
			else if (trimmed.StartsWith("Chitinous Covering"))
			{
				data.AddProperty("Fire Absorption", "enhancement", ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Confounding Enchantment"))
			{
				if (data.Name == "Kormor's Ring") data.AddProperty("Confounding Enchantment", null, 1, new List<ItemProperty>
				{
					new ItemProperty { Property = "Strength", Type = "exceptional", Value = 1},
					new ItemProperty { Property = "Dexterity", Type = "exceptional", Value = 1},
					new ItemProperty { Property = "Constitution", Type = "exceptional", Value = 1},
					new ItemProperty { Property = "Intelligence", Type = "exceptional", Value = 1},
					new ItemProperty { Property = "Wisdom", Type = "exceptional", Value = 1},
					new ItemProperty { Property = "Charisma", Type = "exceptional", Value = 1}
				});
				else data.AddProperty("Confounding Enchantment", null, 0, null);

				return true;
			}
			else if (trimmed.StartsWith("DestructionEnemies struck "))
			{
				data.AddProperty("Destruction", null, 0, null);
				return true;
			}
			else if (trimmed.StartsWith("Exceptional Resistance +2 (Reflex)"))
			{
				data.AddProperty("Reflex", "exceptional resistance +2 (reflex)", 2, null);
				return true;
			}
			else if (trimmed.StartsWith("Legendary Freezing Ice"))
			{
				data.AddProperty("Legendary Freezing Ice", null, 0, null);
				return true;
			}
			else if (trimmed.StartsWith("Lesser Arcane Sigil"))
			{
				data.AddProperty("Arcane Spell Failure Reduction", null, ParseNumber(trimmed), null);
				return true;
			}
			else if (trimmed.StartsWith("Minor Spell Penetration VII"))
			{
				data.AddProperty("Spell Penetration VII", "equipment", 1, null);
				return true;
			}
			else if (trimmed.StartsWith("Shield Spikes"))
			{
				data.AddProperty("Shield Spikes", null, 0, null);
				return true;
			}
			else if (trimmed.StartsWith("Shining Silver Scales"))
			{
				data.AddProperty("Cold Absorption", "enhancement", 51, null);
				return true;
			}
			else if (trimmed.StartsWith("Thorny Crown of Madness"))
			{
				data.AddProperty("Thorny Crown of Madness", null, 0, null);
				return true;
			}
			else if (trimmed.StartsWith("Transmuted Platinum"))
			{
				data.AddProperty("Transmuted Platinum", null, 0, null);
				return true;
			}
			else
			{
				// check for spell charges first
				int c = trimmed.IndexOf("Caster level:", StringComparison.InvariantCultureIgnoreCase);
				string p;
				string v;
				int vi = 0;
				if (c > -1)
				{
					p = "Spell";
					v = ParseFirstChildText(enh);
					c = trimmed.IndexOfAny(numbers, trimmed.IndexOf("Charges:"));
					for (int i = c + 1; i < trimmed.Length; i++)
					{
						if (!numbers.Contains(trimmed[i]))
						{
							int.TryParse(trimmed.Substring(c, i - c), out vi);
							data.AddProperty(p, v, vi, null);
							return true;
						}
					}
				}
				
				c = trimmed.IndexOf(':');
				p = c > -1 ? trimmed.Substring(0, c).Trim() : trimmed;
				v = c > -1 ? trimmed.Substring(c + 1).Trim() : "";
				string origv = v;

				if (p == "Superior Stability")
				{
					v = null;
				}
				else if (p == "Greater Elemental Energy")
				{
					p = "Hit Points";
					vi = ParseNumber(v);
					v = p.ToLower();
				}
				else if (p == "Greater Elemental Spell Power")
				{
					p = "Spell Points";
					vi = ParseNumber(v);
					v = p.ToLower();
				}
				else if (p.StartsWith("DRDamage Reduction "))
				{
					v = p.Substring(19);
					string[] split = v.Split('/');
					int.TryParse(split[0].Trim(), out vi);
					p = "Damage Reduction";
					v = split[1].Trim().ToLower();
				}
				else if (p.StartsWith("Arcane Casting Dexterity") || p.StartsWith("Lesser Arcane Casting Dexterity") || p.StartsWith("Greater Arcane Casting Dexterity"))
				{
					vi = ParseNumber(p);
					p = "Arcane Spell Failure Reduction";
					v = null;
				}
				else if (p.StartsWith("Twilight") || p.StartsWith("Greater Twilight"))
				{
					vi = ParseNumber(p);
					p = "Arcane Spell Failure Reduction";
					v = null;
				}
				else if (p.StartsWith("Efficient Metamagic - "))
				{
					string[] split = p.Split(' ');
					for (int i = 0; i < split.Length; i++)
					{
						if (numerals.Contains(split[i]))
						{
							c = p.IndexOf(" " + split[i]);
							p = p.Substring(22, c - 22) + " Spell Point Reduction";
							break;
						}
					}
					vi = ParseNumber(v);
					v = "enhancement";
				}
				else if (p.StartsWith("Improved Metamagic"))
				{
					string[] split = v.Split(':');
					v = "enhancement";
					vi = ParseNumber(split[2]);
					p = split[1].Trim() + " Spell Point Reduction";
				}
				else if (p.Contains("Healing Amplification"))
				{
					if (p.EndsWith("Healing Amplification")) p = "Positive Healing Amplification";
					else p = "Healing Amplification";
					vi = ParseNumber(v);
					c = v.IndexOf('(');
					v = v.Substring(c + 1, v.IndexOf(' ', c) - c - 1).ToLower();
				}
				else if (p.StartsWith("Curse of "))
				{
					c = v.IndexOf("Penalty to ") + 11;
					p = v.Substring(c, v.IndexOf('.', c) - c);
					vi = -ParseNumber(v);
					v = null;
				}
				else if (p.StartsWith("Hard hitting secret")) return false;
				else if (p.StartsWith("Required Trait")) return false;
				else if (p.Contains("Arcane Augmentation"))
				{
					string[] split = p.Split(' ');
					if (split[0] == "Arcane") data.AddProperty("Effective Arcane Caster Level " + split[2], "arcane", 2, null);
					else if (split[1] == "Arcane" && split[0] == "Lesser") data.AddProperty("Effective Arcane Caster Level " + split[3], "arcane", 1, null);
					else if (split[1] == "Arcane" && split[0] == "Improved") data.AddProperty("Effective Arcane Caster Level " + split[3], "arcane", 3, null);
					return true;
				}
				else if (p.Contains("Divine Augmentation"))
				{
					string[] split = p.Split(' ');
					if (split[0] == "Divine") data.AddProperty("Effective Divine Caster Level " + split[2], "divine", 2, null);
					else if (split[1] == "Divine" && split[0] == "Lesser") data.AddProperty("Effective Divine Caster Level " + split[3], "divine", 1, null);
					else if (split[1] == "Divine" && split[0] == "Improved") data.AddProperty("Effective Divine Caster Level " + split[3], "divine", 3, null);
					return true;
				}
				else if (p.Contains("Evocation Augmentation"))
				{
					string[] split = p.Split(' ');
					if (split[0] == "Evocation") data.AddProperty("Effective Evocation Caster Level " + split[2], "equipment", 2, null);
					else if (split[1] == "Evocation" && split[0] == "Lesser") data.AddProperty("Effective Evocation Caster Level " + split[3], "equipment", 1, null);
					else if (split[1] == "Evocation" && split[0] == "Improved") data.AddProperty("Effective Evocation Caster Level " + split[3], "equipment", 3, null);
					return true;
				}
				/*else if (p.Contains(" (spell)"))
				{
					LogError("- found \" (spell)\" in " + data.Name);
					return true;
				}*/
				else
				{
					#region Enhancement parsing with a numerical value in the name
					// attempt to find a numerical value to use as a stopping point for the property name
					c = p.IndexOfAny(numbers);
					if (c > -1)
					{
						vi = ParseNumber(p);
						if (p[c - 1] == '+') p = p.Substring(0, c - 1).Trim();
						else if (p[c - 1] == '-')
						{
							p = p.Substring(0, c - 1).Trim();
							vi = -vi;
						}
						else p = p.Substring(0, c).Trim();

						// standardize property names
						if (p.StartsWith("Creeping Dust Lore")) p = "Creeping Dust Lore";
						else if (p.StartsWith("Power of Creeping Dust")) p = "Power of Creeping Dust";
						else if (p.EndsWith("Armor Bonus")) p = "Armor Class";
						else if (p.EndsWith("Accuracy")) p = "Attack";
						else if (p.EndsWith("Deadly")) p = "Damage";
						else if (p.StartsWith("Damage Bonus")) p = "Damage";
						else if (p == "Shatter") p = "Sunder DC";
						else if (p.EndsWith("Stunning")) p = "Stunning DC";
						else if (p.EndsWith("Wizardry")) p = "Spell Points";
						else if (p.EndsWith("Protection")) p = "Armor Class";
						else if (p.StartsWith("Natural Armor"))
						{
							p = "Armor Class";
							v = "natural armor";
						}
						else if (p.StartsWith("Hardened Exterior")) p = "Armor Class";
						else if (p.StartsWith("Heightened Awareness")) p = "Armor Class";
						else if (p == "Shield") p = "Armor Class";
						else if (p.EndsWith("Physical Sheltering")) p = "Physical Resistance Rating";
						else if (p.EndsWith("Magical Sheltering")) p = "Magical Resistance Rating";
						else if (p.StartsWith("Melee Alacrity")) p = "Melee Attack Speed";
						else if (p.StartsWith("Ranged Alacrity")) p = "Ranged Attack Speed";
						else if (p.StartsWith("Striding"))
						{
							p = "Move Speed";
							v = "enhancement";
						}
						else if (p.StartsWith("Magical Efficiency"))
						{
							p = "Spell Point Cost Reduction %";
							v = "enhancement";
						}
						else if (p.EndsWith("False Life")) p = "Hit Points";
						else if (p.StartsWith("Vitality")) p = "Hit Points";
						else if (p.EndsWith("Cold Resistance") || p.Trim().EndsWith("Cold Resistance -")) p = "Cold Resistance";
						else if (p.EndsWith("Fire Resistance") || p.Trim().EndsWith("Fire Resistance -")) p = "Fire Resistance";
						else if (p.EndsWith("Electric Resistance") || p.Trim().EndsWith("Electric Resistance -")) p = "Electric Resistance";
						else if (p.EndsWith("Acid Resistance") || p.Trim().EndsWith("Acid Resistance -")) p = "Acid Resistance";
						else if (p.EndsWith("Sonic Resistance") || p.Trim().EndsWith("Sonic Resistance -")) p = "Sonic Resistance";
						else if (p.EndsWith("Light Resistance") || p.Trim().EndsWith("Light Resistance -")) p = "Light Resistance";
						else if (p.EndsWith("Spell Focus")) p = "Spell Focus";
						else if (p.EndsWith("Spell Focus Mastery")) p = "Spell Focus";
						else if (p.EndsWith("Corrosion")) p = "Acid Spell Power";
						else if (p.EndsWith("Glaciation")) p = "Cold Spell Power";
						else if (p.EndsWith("Magnetism")) p = "Electric Spell Power";
						else if (p.EndsWith("Combustion")) p = "Fire Spell Power";
						else if (p.EndsWith("Radiance")) p = "Light Spell Power";
						else if (p.EndsWith("Devotion")) p = "Positive Spell Power";
						else if (p.EndsWith("Impulse")) p = "Force Spell Power";
						else if (p.EndsWith("Resonance")) p = "Sonic Spell Power";
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
						else if (p.EndsWith("Distant Diversion")) p = "Ranged Threat Reduction";
						else if (p.EndsWith("Mystic Diversion")) p = "Magic Threat Reduction";
						else if (p.EndsWith("Diversion")) p = "Melee Threat Reduction";
						else if (p.EndsWith("Open Lock")) p = "Open Lock";
						else if (p == "Rough Hide")
						{
							p = "Armor Class";
							vi = ParseNumber(v);
							v = "primal natural armor";
						}
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
						else if (p.StartsWith("Shield Bashing "))
						{
							p = "Automatic Secondary Shield Bash";
							v = "equipment";
						}
						else if (p.EndsWith("Negative Amplification"))
						{
							string[] split = p.Split(' ');
							if (split.Length == 2) v = "enhancement";
							else v = split[0].ToLower();
							p = "Negative Healing Amplification";
						}
						else if (p.EndsWith("Repair Amplification"))
						{
							if (p.StartsWith("Insightful")) v = "insight";
							else v = "enhancement";
							p = "Repair Healing Amplification";
						}
						else if (p.EndsWith("Light Guard")) p = "Light Guard";
						else if (p.EndsWith("Good Guard")) p = "Good Guard";
						else if (p.EndsWith("Evil Guard")) p = "Evil Guard";
						else if (p.EndsWith("Negative Guard")) p = "Negative Guard";
						else if (p == "Hallowed") p = "Turn Undead Maximum Hit Dice";
						else if (p == "Sacred") p = "Turn Undead Effective Level";
						else if (p.StartsWith("Construct Fortification "))
						{
							p = "Fortification";
							v = "insight";
						}
						else if (p.StartsWith("Exceptional Fortification"))
						{
							p = "Fortification";
							v = "insight";
						}
						else if (p.StartsWith("Unwieldy "))
						{
							p = "Dexterity";
							vi = -vi;
						}
						else if (p.EndsWith("Enhanced Ki"))
						{
							p = "Ki Generation";
							v = "enhanced ki";
						}
						else if (p.EndsWith("Armor-Piercing -")) p = "Fortification Bypass";
						else if (p.EndsWith("Assassinate") || p.EndsWith("Assassination")) p = "Assassinate DC";
						else if (p == "Attack Bonus") p = "Attack";
						else if (p.StartsWith("Coalesced Flame"))
						{
							p = "Coalesced Flame";
							vi = 0;
						}
						else if (p.StartsWith("Constitution Skills -")) p = "Concentration";
						else if (p.EndsWith("Abjuration Focus")) p = "Abjuration Spell DC";
						else if (p.EndsWith("Breath Weapon Focus")) p = "Breath Weapon Spell DC";
						else if (p.EndsWith("Conjuration Focus")) p = "Conjuration Spell DC";
						else if (p.EndsWith("Enchantment Focus")) p = "Enchantment Spell DC";
						else if (p.EndsWith("Evocation Focus")) p = "Evocation Spell DC";
						else if (p.EndsWith("Illusion Focus")) p = "Illusion Spell DC";
						else if (p.EndsWith("Necromancy Focus")) p = "Necromancy Spell DC";
						else if (p.EndsWith("Rune Arm Focus")) p = "Rune Arm DC";
						else if (p.EndsWith("Transmutation Focus")) p = "Transmutation Spell DC";
						else if (p == "Enhanced Balance") p = "Balance";
						else if (p == "Enhanced Concentration") p = "Concentration";
						else if (p == "Enhanced Disable Device") p = "Disable Device";
						else if (p == "Enhanced Haggle") p = "Haggle";
						else if (p == "Enhanced Hide") p = "Hide";
						else if (p == "Enhanced Intimidate") p = "Intimidate";
						else if (p == "Enhanced Jump") p = "Jump";
						else if (p == "Enhanced Move Silently") p = "Move Silently";
						else if (p == "Enhanced Perform") p = "Perform";
						else if (p == "Enhanced Search") p = "Search";
						else if (p == "Enhanced Spot") p = "Spot";
						else if (p == "Enhanced Use Magic Device") p = "Use Magic Device";
						else if (p.EndsWith("Fortitude Save")) p = "Fortitude";
						else if (p.EndsWith("Will Save")) p = "Will";
						else if (p.EndsWith("Reflex Save")) p = "Reflex";
						else if (p.StartsWith("Fortification Penalty")) p = "Fortification";
						else if (p.StartsWith("Negative Energy Absorption")) p = "Negative Absorption";
						else if (p.EndsWith("Vertigo")) p = "Trip DC";
						//else if (NullTypeProperties.Contains(p)) v = null;
					}
					#endregion
					#region Enhancement parsing with a roman numeral or nothing else in the name
					else
					{
						// no numbers means either roman numerals or just a name
						// bypass processing the innertext by finding the first <a> element and grabbing its inner text

						// first try to drill down to the first child with no children
						p = ParseFirstChildText(enh);
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
								p = p.Substring(0, c).Trim();
								// we flag this for special handling later
								if (p == "Parrying" || p.EndsWith("Deception") || p == "Speed" || p == "Riposte") v = numerals[vi - 1];
							}
						}

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
						else if (p == "Elemental Spell Power")
						{
							p = "Spell Points";
							vi = ParseNumber(v);
							v = "elemental spell power";
						}
						else if (p == "Dusk")
						{
							p = "Concealment";
							v = "enhancement";
							vi = 10;
						}
						else if (p == "Adamantine Lined") p = "Adamantine";
						else if (p == "Accuracy") p = "Attack";
						else if (p.StartsWith("Rune Arm Charge Rate")) p = "Rune Arm Charge Rate";
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
						else if (p == "Silver Flame")
						{
							p = "Turn Undead Total Hit Dice";
							v = null;
							vi = 6;
						}
						else if (p == "Proficiency")
						{
							v = v.Substring(0, v.IndexOf("Proficiency"));
							v = p + ": " + v;
							p = "Feat";
						}
						else if (p.EndsWith("Lightning Resistance"))
						{
							p = "Electric Resistance";
							vi = ParseNumber(v);
							v = "enhancement";
						}
						else if (p.EndsWith("Cold Resistance"))
						{
							p = "Cold Resistance";
							vi = ParseNumber(v);
							v = "enhancement";
						}
						else if (p.EndsWith("Fire Resistance"))
						{
							p = "Fire Resistance";
							vi = ParseNumber(v);
							v = "enhancement";
						}
						else if (p.EndsWith("Acid Resistance"))
						{
							p = "Acid Resistance";
							vi = ParseNumber(v);
							v = "enhancement";
						}
						else if (p.EndsWith("Sonic Resistance"))
						{
							p = "Sonic Resistance";
							vi = ParseNumber(v);
							v = "enhancement";
						}
						else if (p.EndsWith("Light Resistance"))
						{
							p = "Light Resistance";
							vi = ParseNumber(v);
							v = "enhancement";
						}
						else if (p.EndsWith("Acid Lore"))
						{
							p = "Acid Spell Critical Chance";
							vi = ParseNumber(v);
							v = "equipment";
						}
						else if (p.EndsWith("Fire Lore"))
						{
							p = "Fire Spell Critical Chance";
							vi = ParseNumber(v);
							v = "equipment";
						}
						else if (p.EndsWith("Ice Lore"))
						{
							p = "Cold Spell Critical Chance";
							vi = ParseNumber(v);
							v = "equipment";
						}
						else if (p.EndsWith("Lightning Lore"))
						{
							p = "Electric Spell Critical Chance";
							vi = ParseNumber(v);
							v = "equipment";
						}
						else if (p.EndsWith("Radiance Lore"))
						{
							p = "Light Spell Critical Chance";
							vi = ParseNumber(v);
							v = "equipment";
						}
						else if (p.EndsWith("Healing Lore"))
						{
							p = "Positive Spell Critical Chance";
							vi = ParseNumber(v);
							v = "equipment";
						}
						else if (p.EndsWith("Kinetic Lore"))
						{
							p = "Force Spell Critical Chance";
							vi = ParseNumber(v);
							v = "equipment";
						}
						else if (p.EndsWith("Radiance Lore"))
						{
							p = "Light Spell Critical Chance";
							vi = ParseNumber(v);
							v = "equipment";
						}
						else if (p.EndsWith("Repair Lore"))
						{
							p = "Repair Spell Critical Chance";
							vi = ParseNumber(v);
							v = "equipment";
						}
						else if (p.EndsWith("Sonic Lore"))
						{
							p = "Sonic Spell Critical Chance";
							vi = ParseNumber(v);
							v = "equipment";
						}
						else if (p.EndsWith("Abjuration Focus")) p = "Abjuration Spell DC";
						else if (p.EndsWith("Conjuration Focus")) p = "Conjuration Spell DC";
						else if (p.EndsWith("Divination Focus")) p = "Divination Spell DC";
						else if (p.EndsWith("Enchantment Focus")) p = "Enchantment Spell DC";
						else if (p.EndsWith("Evocation Focus")) p = "Evocation Spell DC";
						else if (p.EndsWith("Illusion Focus")) p = "Illusion Spell DC";
						else if (p.EndsWith("Necromancy Focus")) p = "Necromancy Spell DC";
						else if (p.EndsWith("Transmutation Focus")) p = "Transmutation Spell DC";
						else if (p.IndexOf("Hidden effect", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							if (v == "Increases threat generated from spells by 25%")
							{
								p = "Magic Threat Generation";
								v = "enhancement";
								vi = 25;
							}
							else if (v.StartsWith("Demonic Drain"))
							{
								p = "Demonic Drain";
								v = null;
								vi = 0;
							}
							else if (v.StartsWith("Cacophonic Guard"))
							{
								p = "Cacophonic Guard";
								v = null;
								vi = 0;
							}
							else if (v == "around 5% on-being-hit chance to be Crippled and gain DR 20/- for 20 seconds")
							{
								p = "Defiance";
								v = null;
								vi = 0;
							}
							else if (v.StartsWith("Stone Paws"))
							{
								p = "Stone Paws";
								v = null;
								vi = 0;
							}
							else if (v.StartsWith("Will Save "))
							{
								p = "Will";
								vi = ParseNumber(v);
								c = v.IndexOf(vi.ToString());
								if (v[c - 1] == '-') vi = -vi;
								c = v.IndexOf(" bonus to ", StringComparison.InvariantCultureIgnoreCase);
								if (c > -1)
								{
									int ce = v.LastIndexOf(' ', c - 1);
									v = Regex.Replace(v.Substring(ce + 1, c - ce), @"\W+", "");
									v = Regex.Replace(v, @"^\d+", "");
									v = v.ToLower();
								}
								else v = "resistance";
							}
							else if (v.StartsWith("Madstone Reaction"))
							{
								p = "Madstone Reaction";
								v = null;
								vi = 0;
							}
							else if (v.StartsWith("Increases spellcasting, melee, and ranged threat by "))
							{
								vi = ParseNumber(v);
								data.AddProperty("Melee Threat Generation", "enhancement", vi, null);
								data.AddProperty("Ranged Threat Generation", "enhancement", vi, null);
								data.AddProperty("Magic Threat Generation", "enhancement", vi, null);
								return true;
							}
							else if (v == "On critical hits, this weapon saps the target") return false;
						}
						else if (p.EndsWith("Reinforced Fists"))
						{
							if (p == "Reinforced Fists") data.AddProperty("Reinforced Fists", "enhancement", 0.5f, null);
							else if (p.StartsWith("Greater")) data.AddProperty("Reinforced Fists", "enhancement", 1, null);
							else if (p.StartsWith("Superior")) data.AddProperty("Reinforced Fists", "enhancement", 1.5f, null);
							return true;
						}
						else if (p.EndsWith("Vorpal"))
						{
							if (p.StartsWith("Improved ")) vi = 2;
							else if (p.StartsWith("Greater ")) vi = 3;
							else if (p.StartsWith("Sovereign")) vi = 4;
							else vi = 1;
							p = "Vorpal";
							v = "enhancement";
						}
						else if (p == "Proof Against Poison")
						{
							p = "Poison Immunity";
							v = null;
						}
						else if (p.EndsWith("False Life"))
						{
							p = "Hit Points";
							vi = ParseNumber(v);
							v = null;
						}
						else if (p == "Single-Mindedness")
						{
							p = "Enchantment Spell DC";
							v = null;
							vi = -2;
						}
						else if (p == "Invulnerability")
						{
							p = "Damage Reduction";
							v = "magic";
							vi = 5;
						}
						else if (p == "Bashing" && vi > 0)
						{
							p = "Automatic Secondary Shield Bash";
							v = "enhancement";
						}
						else if (p == "Calamitous Blows")
						{
							p = "Doublestrike";
							vi = ParseNumber(v);
						}
						else if (p == "Rune-fueled Warding")
						{
							p = "Damage Reduction";
							v = "-";
							vi = 30;
						}
						else if (p == "Blunt Trauma")
						{
							p = "Critical Multiplier on 19-20";
							v = "melee";
							vi = 1;
						}
						else if (p == "Elasticity")
						{
							p = "Critical Multiplier on 19-20";
							v = "ranged";
							vi = 1;
						}
						else if (p == "Mind Drain")
						{
							p = "Maximum Spell Points %";
							vi = -5;
						}
						else if (p == "Power Store")
						{
							p = "Spell Point Cost %";
							vi = 10;
						}
						else if (p == "Lesser Turning")
						{
							p = "Turn Undead Uses";
							vi = 2;
						}
						else if (p == "Faith")
						{
							p = "Damage Reduction";
							v = "evil";
							vi = 5;
						}
						else if (p == "Inspiring Echoes")
						{
							p = "Bard Song Regeneration";
							vi = ParseNumber(v);
						}
						else if (p.EndsWith("Fire Augmentation"))
						{
							v = numerals[vi - 1];
							if (p.StartsWith("Improved")) vi = 3;
							else vi = 1;
							p = "Fire Spell Caster Level";
						}
						else if (p == "Fleshmaker")
						{
							vi = ParseNumber(v);
							p = "Positive Healing Amplification";
							v = "enhancement";
						}
						else if (p == "Power Drain")
						{
							vi = -ParseNumber(v);
							p = "Spell Points";
						}
						else if (p == "Minor Turning")
						{
							p = "Turn Undead Uses";
							vi = ParseNumber(v);
							v = "enhancement";
						}
						else if (p == "Augment Summoning")
						{
							p = "Feat";
							v = "Augment Summoning";
						}
						else if (p.EndsWith("Action Boost Enhancement"))
						{
							vi = ParseNumber(v);
							if (vi == 0)
							{
								if (p.StartsWith("Minor")) vi = 1;
								else if (p.StartsWith("Lesser")) vi = 2;
							}
							p = "Action Boost Uses";
							v = "enhancement";
						}
						else if (p == "Shield Proficiency: Tower Shield")
						{
							v = p;
							p = "Feat";
						}
						else if (p == "Proficiency: Greatclub")
						{
							v = p;
							p = "Feat";
						}
						else if (p == "Magical Null")
						{
							vi = ParseNumber(v);
							p = "Arcane Spell Failure Increase";
						}
						else if (p == "Diehard")
						{
							v = p;
							p = "Feat";
						}
						else if (p == "Class Required:") p = "Class Required";
						else if (p == "Mind Turbulence")
						{
							p = "Concentration";
							vi = -10;
						}
						else if (p == "Treason")
						{
							p = "Melee Threat Reduction";
							vi = ParseNumber(v);
						}
						else if (p == "Holding On")
						{
							p = "Range of Unconsciousness";
							vi = ParseNumber(v);
						}
						else if (p == "Weapon Focus: Falchion")
						{
							v = "Weapon Focus: Slashing";
							p = "Feat";
						}
						else if (p == "Anger")
						{
							p = "Rage Uses";
							v = "enhancement";
							vi = 3;
						}
						else if (p == "Maximum Charge Tier")
						{
							for (int i = 0; i < numerals.Length; i++)
							{
								if (numerals[i] == v)
								{
									vi = i + 1;
									break;
								}
							}
						}
						else if (p == "Rune Arm Imbue")
						{
							v = v.Substring(0, v.IndexOf("Rune Arm Imbue"));
							string[] split = v.Split(' ');
							for (int i = 0; i < numerals.Length; i++)
							{
								if (numerals[i] == split[split.Length - 1])
								{
									vi = i + 1;
									break;
								}
							}
							v = v.Substring(0, v.Length - split[split.Length - 1].Length).Trim();
						}
						else if (p.StartsWith("Magical Resistance:"))
						{
							p = "Magical Resistance Rating";
							vi = ParseNumber(v);
						}
						else if (p == "Permanent Efficacy")
						{
							p = "Universal Spell Power";
							vi = ParseNumber(v);
						}
						else if (p == "Pirate Vitality")
						{
							p = "Hit Points";
							vi = ParseNumber(v);
							v = "pirate vitality";
						}
						else if (p == "Sacred")
						{
							p = "Turn Undead Effective Level";
							vi = ParseNumber(v);
						}
						else if (p == "Silent Moves")
						{
							p = "Move Silently";
							v = "competence";
							vi = 5;
						}
						else if (p == "Silver, Alchemical") p = "Silver";
						else if (p == "Sundering")
						{
							p = "Sunder DC";
							vi += 5;
							v = "enhancement";
						}
						//else if (NullTypeProperties.Contains(p)) v = null;
					}
					#endregion
				}

				// attempt to find a type to the bonus/value
				if (v != null)
				{
					c = v.IndexOf(" bonus to ", StringComparison.InvariantCultureIgnoreCase);
					if (c == -1) c = v.IndexOf(" bonus on ", StringComparison.InvariantCultureIgnoreCase);
					if (c == -1) c = v.IndexOf(" bonuses to ", StringComparison.InvariantCultureIgnoreCase);
					if (c == -1) c = v.IndexOf(" bonuses on ", StringComparison.InvariantCultureIgnoreCase);
					if (c > -1)
					{
						int ce = v.LastIndexOf(' ', c - 1);
						v = Regex.Replace(v.Substring(ce + 1, c - ce), @"\W+", "");
						v = Regex.Replace(v, @"^\d+", "");
						v = v.ToLower();

						// we found a bonus type, let's try to clean up a redundant reference in the property name
						if (p != "Armor Class" && !string.IsNullOrWhiteSpace(v))
						{
							if (p.IndexOf(v, StringComparison.InvariantCultureIgnoreCase) == 0)
								p = p.Substring(p.IndexOf(' ') + 1).Trim();
						}
					}
				}

				if (string.IsNullOrWhiteSpace(p))
				{
					// special case check for weapon or armor base enhancement
					if (v == "enhancement")
					{
						if (data.Slot == SlotType.Body)
						{
							p = "Armor Class";
							v = "armor enhancement";
						}
						else if (data.Slot == SlotType.Offhand)
						{
							p = "Armor Class";
							v = "shield enhancement";
						}
						else if (data.Slot == SlotType.Weapon) p = "Attack and Damage";
					}
					else if (v != "orb")
					{
						// this may require further refinement in the future if new items are added with properties that are called "+X whatever"
						data.AddProperty("Attack vs Evil", "enhancement", vi, null);
						data.AddProperty("Damage vs Evil", "enhancement", vi, null);
						return true;
					}
					else return false;
				}

				// some enhancements have multiple effects, and we want to capture them individually
				// others require a final processing
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
					data.AddProperty("Balance", v, vi, null);
					data.AddProperty("Bluff", v, vi, null);
					data.AddProperty("Concentration", v, vi, null);
					data.AddProperty("Diplomacy", v, vi, null);
					data.AddProperty("Disable Device", v, vi, null);
					data.AddProperty("Haggle", v, vi, null);
					data.AddProperty("Heal", v, vi, null);
					data.AddProperty("Hide", v, vi, null);
					data.AddProperty("Intimidate", v, vi, null);
					data.AddProperty("Jump", v, vi, null);
					data.AddProperty("Listen", v, vi, null);
					data.AddProperty("Move Silently", v, vi, null);
					data.AddProperty("Open Lock", v, vi, null);
					data.AddProperty("Perform", v, vi, null);
					data.AddProperty("Repair", v, vi, null);
					data.AddProperty("Search", v, vi, null);
					data.AddProperty("Spellcraft", v, vi, null);
					data.AddProperty("Spot", v, vi, null);
					data.AddProperty("Swim", v, vi, null);
					data.AddProperty("Tumble", v, vi, null);
					data.AddProperty("Use Magic Device", v, vi, null);
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
					int ac = vi;
					int s = vi;
					if (numerals.Contains(v))
					{
						ac = (int)Math.Ceiling(vi / 2.0);
						s = (int)Math.Floor(vi / 2.0);
					}
					data.AddProperty("Armor Class", "insight", ac, null);
					data.AddProperty("Fortitude", "insight", s, null);
					data.AddProperty("Reflex", "insight", s, null);
					data.AddProperty("Will", "insight", s, null);
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
					if (numerals.Contains(v))
					{
						v = "enhancement";
						data.AddProperty("Move Speed", v, Math.Min(vi * 5, 30), null);
						data.AddProperty("Melee Attack Speed", v, vi, null);
						data.AddProperty("Ranged Attack Speed", v, vi, null);
					}
					else
					{
						data.AddProperty("Move Speed", v, vi, null);
						data.AddProperty("Melee Attack Speed", v, vi / 2, null);
						data.AddProperty("Ranged Attack Speed", v, vi / 2, null);
					}
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
					if (origv == v) v = "equipment";
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
					if (numerals.Contains(v))
					{
						data.AddProperty("Sneak Attack Attack", "enhancement", vi, null);
						data.AddProperty("Sneak Attack Damage", "enhancement", vi * 2, null);
					}
					else
					{
						if (origv == v) v = "enhancement";
						data.AddProperty("Sneak Attack Attack", v, vi, null);
						data.AddProperty("Sneak Attack Damage", v, (int)Math.Round(vi * 1.5f), null);
					}
				}
				else if (p == "Command")
				{
					data.AddProperty("Bluff", v, vi, null);
					data.AddProperty("Diplomacy", v, vi, null);
					data.AddProperty("Haggle", v, vi, null);
					data.AddProperty("Hide", null, -6, null);
					data.AddProperty("Intimidate", v, vi, null);
					data.AddProperty("Perform", v, vi, null);
					data.AddProperty("Use Magic Device", v, vi, null);
				}
				else if (p == "Fortification")
				{
					if (vi == 0) vi = ParseNumber(v);
					if (vi != 0) data.AddProperty(p, "enhancement", vi, null);
				}
				else if (p.EndsWith(" Threat Reduction"))
				{
					if (vi == 0)
					{
						vi = ParseNumber(v);
						v = null;
					}
					else if (v.StartsWith("This item")) v = "enhancement";

					data.AddProperty(p, v, vi, null);
				}
				else if (p == "Stealth Strike")
				{
					data.AddProperty("Ranged Threat Reduction", null, 15, null);
					data.AddProperty("Magic Threat Reduction", null, 15, null);
				}
				else if (p == "Adamantine")
				{
					data.AddProperty(p, null, 0, null);
					if (!v.StartsWith("Adamantine weapons "))
					{
						vi = ParseNumber(v);
						c = v.IndexOf(vi.ToString());
						if (c > -1)
						{
							int si = v.IndexOf('/', c);
							v = v.Substring(si + 1, v.IndexOf('.', si) - si - 1);

							data.AddProperty("Damage Reduction", v, vi, null);
						}
					}
				}
				else if (p == "Alchemical Conservation")
				{
					data.AddProperty("Ki Generation", "enhanced ki", 1, null);
					data.AddProperty("Action Boost Uses", null, 1, null);
					data.AddProperty("Turn Undead Uses", null, 1, null);
					data.AddProperty("Bard Song Uses", null, 1, null);
				}
				else if (p == "Sneak Attack Bonus")
				{
					data.AddProperty("Sneak Attack Attack", origv == v ? "enhancement" : v, vi, null);
					data.AddProperty("Sneak Attack Damage", origv == v ? "enhancement" : v, (int)Math.Round(vi * 1.5, MidpointRounding.AwayFromZero), null);
				}
				else if (p == "Improved Bashing")
				{
					if (vi > 0)
					{
						string[] split = v.Split(':');
						vi = ParseNumber(split[split.Length - 1]);
						data.AddProperty("Automatic Secondary Shield Bash", "enhancement", vi, null);
					}

					data.AddProperty(p, null, 0, null);
				}
				else if (p == "Flametouched Iron")
				{
					data.AddProperty("Fortitude vs Evil Outsiders", "enhancement", 1, null);
					data.AddProperty("Reflex vs Evil Outsiders", "enhancement", 1, null);
					data.AddProperty("Will vs Evil Outsiders", "enhancement", 1, null);
				}
				else if (p == "Anchoring")
				{
					data.AddProperty("Immunity to Haste", null, 0, null);
					data.AddProperty("Immunity to Banishment", null, 0, null);
				}
				else if (p == "Overfocus")
				{
					data.AddProperty("Search", null, -10, null);
					data.AddProperty("Spot", null, -10, null);
				}
				else if (p == "Dazing")
				{
					data.AddProperty(p, null, vi, null);
					data.AddProperty("Stunning DC", "enhancement", vi * 2, null);
				}
				else if (p == "Linguistics")
				{
					data.AddProperty("Bluff Cooldown", "enhancement", vi, null);
					data.AddProperty("Diplomacy Cooldown", "enhancement", vi, null);
					data.AddProperty("Intimidate Cooldown", "enhancement", vi, null);
				}
				else if (p == "Extra Smites")
				{
					data.AddProperty("Smite Evil Uses", null, vi, null);
					data.AddProperty("Smite Evil Regeneration", null, 10, null);
				}
				else if (p == "Occultation")
				{
					vi = ParseNumber(v);
					data.AddProperty("Melee Threat Reduction", "enhancement", vi, null);
					data.AddProperty("Ranged Threat Reduction", "enhancement", vi, null);
					data.AddProperty("Magic Threat Reduction", "enhancement", vi, null);
				}
				else if (p == "Blood")
				{
					data.AddProperty("Positive Healing Amplification", "enhancement", 20, null);
					data.AddProperty("Fortification", null, -10, null);
				}
				else if (p == "Eternal Faith")
				{
					data.AddProperty("Turn Undead Check", null, 2, null);
					data.AddProperty("Turn Undead Maximum Hit Dice", null, 2, null);
					data.AddProperty("Turn Undead Total Hit Dice", null, 4, null);
				}
				else if (p.EndsWith("Elemental Resistance -"))
				{
					data.AddProperty("Acid Resistance", v, vi, null);
					data.AddProperty("Cold Resistance", v, vi, null);
					data.AddProperty("Electric Resistance", v, vi, null);
					data.AddProperty("Fire Resistance", v, vi, null);
				}
				else if (p == "Marksmanship")
				{
					data.AddProperty("Ranged Attack", null, 2, null);
					data.AddProperty("Ranged Damage", null, 1, null);
				}
				else if (p == "Flawed Shadowscale Armor")
				{
					List<ItemProperty> options = new List<ItemProperty>();
					options.Add(new ItemProperty { Property = "Shadow Caster", Type = "set" });
					options.Add(new ItemProperty { Property = "Shadow Disciple", Type = "set" });
					if (data.Category >= (int)ArmorCategory.Light) options.Add(new ItemProperty { Property = "Shadow Killer", Type = "set" });
					if (data.Category >= (int)ArmorCategory.Medium) options.Add(new ItemProperty { Property = "Shadow Striker", Type = "set" });
					if (data.Category >= (int)ArmorCategory.Heavy) options.Add(new ItemProperty { Property = "Shadow Guardian", Type = "set" });
					if (data.Category >= (int)ArmorCategory.Docent) options.Add(new ItemProperty { Property = "Shadow Construct", Type = "set" });

					data.AddProperty(p, null, 1, options);
				}
				else if (p == "Block Elements")
				{
					vi = ParseNumber(v);
					data.AddProperty("Acid Absorption", null, vi, null);
					data.AddProperty("Cold Absorption", null, vi, null);
					data.AddProperty("Electric Absorption", null, vi, null);
					data.AddProperty("Fire Absorption", null, vi, null);
					data.AddProperty("Sonic Absorption", null, vi, null);
				}
				else if (p == "Rebellion")
				{
					vi = ParseNumber(v);
					v = "enhancement";
					data.AddProperty("Melee Threat Reduction", v, vi, null);
					data.AddProperty("Ranged Threat Reduction", v, vi, null);
					data.AddProperty("Magic Threat Reduction", v, vi, null);
				}
				else if (p == "Elemental Absorption")
				{
					data.AddProperty("Acid Absorption", v, vi, null);
					data.AddProperty("Cold Absorption", v, vi, null);
					data.AddProperty("Electric Absorption", v, vi, null);
					data.AddProperty("Fire Absorption", v, vi, null);
				}
				else if (p == "Frozen Storm Lore")
				{
					vi = ParseNumber(v);
					v = "equipment";
					data.AddProperty("Cold Spell Critical Chance", v, vi, null);
					data.AddProperty("Electric Spell Critical Chance", v, vi, null);
				}
				else if (p == "Arcane Lore")
				{
					data.AddProperty("Acid Spell Critical Chance", "equipment", 6, null);
					data.AddProperty("Cold Spell Critical Chance", "equipment", 6, null);
					data.AddProperty("Electric Spell Critical Chance", "equipment", 6, null);
					data.AddProperty("Fire Spell Critical Chance", "equipment", 6, null);
					data.AddProperty("Force Spell Critical Chance", "equipment", 6, null);
					data.AddProperty("Light Spell Critical Chance", "equipment", 6, null);
					data.AddProperty("Negative Spell Critical Chance", "equipment", 6, null);
					data.AddProperty("Poison Spell Critical Chance", "equipment", 6, null);
					data.AddProperty("Positive Spell Critical Chance", "equipment", 6, null);
					data.AddProperty("Repair Spell Critical Chance", "equipment", 6, null);
					data.AddProperty("Sonic Spell Critical Chance", "equipment", 6, null);
					data.AddProperty("Acid Spell Critical Multiplier", "equipment", 0.25f, null);
					data.AddProperty("Cold Spell Critical Multiplier", "equipment", 0.25f, null);
					data.AddProperty("Electric Spell Critical Multiplier", "equipment", 0.25f, null);
					data.AddProperty("Fire Spell Critical Multiplier", "equipment", 0.25f, null);
					data.AddProperty("Force Spell Critical Multiplier", "equipment", 0.25f, null);
					data.AddProperty("Light Spell Critical Multiplier", "equipment", 0.25f, null);
					data.AddProperty("Negative Spell Critical Multiplier", "equipment", 0.25f, null);
					data.AddProperty("Poison Spell Critical Multiplier", "equipment", 0.25f, null);
					data.AddProperty("Positive Spell Critical Multiplier", "equipment", 0.25f, null);
					data.AddProperty("Repair Spell Critical Multiplier", "equipment", 0.25f, null);
					data.AddProperty("Sonic Spell Critical Multiplier", "equipment", 0.25f, null);
				}
				else if (p == "Purifying Flame Lore")
				{
					data.AddProperty("Fire Spell Critical Chance", "equipment", vi, null);
					data.AddProperty("Light Spell Critical Chance", "equipment", vi, null);
				}
				else if (p == "Power of the Flames of Purity")
				{
					data.AddProperty("Fire Spell Power", v, vi, null);
					data.AddProperty("Light Spell Power", v, vi, null);
				}
				else if (p == "Elemental Manipulation" || p == "Elemental Resonance")
				{
					data.AddProperty("Acid Spell Power", v, vi, null);
					data.AddProperty("Cold Spell Power", v, vi, null);
					data.AddProperty("Electric Spell Power", v, vi, null);
					data.AddProperty("Fire Spell Power", v, vi, null);
				}
				else if (p == "Elemental Resistance")
				{
					data.AddProperty("Acid Resistance", v, vi, null);
					data.AddProperty("Cold Resistance", v, vi, null);
					data.AddProperty("Electric Resistance", v, vi, null);
					data.AddProperty("Fire Resistance", v, vi, null);
					data.AddProperty("Sonic Resistance", v, vi, null);
				}
				else if (p == "Spell Focus")
				{
					data.AddProperty("Abjuration Spell DC", v, vi, null);
					data.AddProperty("Conjuration Spell DC", v, vi, null);
					data.AddProperty("Divination Spell DC", v, vi, null);
					data.AddProperty("Enchantment Spell DC", v, vi, null);
					data.AddProperty("Evocation Spell DC", v, vi, null);
					data.AddProperty("Illusion Spell DC", v, vi, null);
					data.AddProperty("Necromancy Spell DC", v, vi, null);
					data.AddProperty("Transmutation Spell DC", v, vi, null);
				}
				else if (p.EndsWith("Alluring Skills Bonus"))
				{
					data.AddProperty("Bluff", v, vi, null);
					data.AddProperty("Diplomacy", v, vi, null);
					data.AddProperty("Haggle", v, vi, null);
					data.AddProperty("Intimidate", v, vi, null);
					data.AddProperty("Perform", v, vi, null);
				}
				else if (p.EndsWith("Astute Skills Bonus"))
				{
					data.AddProperty("Disable Device", v, vi, null);
					data.AddProperty("Repair", v, vi, null);
					data.AddProperty("Search", v, vi, null);
					data.AddProperty("Spellcraft", v, vi, null);
				}
				else if (p.EndsWith("Prudent Skills Bonus") || p.StartsWith("Wisdom Skills -"))
				{
					data.AddProperty("Heal", v, vi, null);
					data.AddProperty("Listen", v, vi, null);
					data.AddProperty("Spot", v, vi, null);
				}
				else if (p.StartsWith("Charisma Skills -"))
				{
					data.AddProperty("Bluff", v, vi, null);
					data.AddProperty("Diplomacy", v, vi, null);
					data.AddProperty("Haggle", v, vi, null);
					data.AddProperty("Intimidate", v, vi, null);
					data.AddProperty("Perform", v, vi, null);
					data.AddProperty("Use Magic Device", v, vi, null);
				}
				else if (p.StartsWith("Dexterity Skills -") || p.EndsWith("Nimble Skills Bonus"))
				{
					data.AddProperty("Balance", v, vi, null);
					data.AddProperty("Hide", v, vi, null);
					data.AddProperty("Move Silently", v, vi, null);
					data.AddProperty("Open Lock", v, vi, null);
					data.AddProperty("Tumble", v, vi, null);
				}
				else if (p.StartsWith("Strength Skills -"))
				{
					data.AddProperty("Jump", v, vi, null);
					data.AddProperty("Swim", v, vi, null);
				}
				else if (p.StartsWith("Intelligence Skills -"))
				{
					data.AddProperty("Disable Device", v, vi, null);
					data.AddProperty("Repair", v, vi, null);
					data.AddProperty("Search", v, vi, null);
					data.AddProperty("Spellcraft", v, vi, null);
				}
				else if (p.EndsWith("Combat Mastery"))
				{
					data.AddProperty("Trip DC", v, vi, null);
					data.AddProperty("Sunder DC", v, vi, null);
					data.AddProperty("Stunning DC", v, vi, null);
				}
				else if (p == "Finesse")
				{
					data.AddProperty("Feat", "Weapon Finesse", 0, null);
					data.AddProperty("Dexterity", "enhancement", 2, null);
				}
				else if (p == "Lifesealed")
				{
					data.AddProperty("Deathblock", null, 0, null);
					data.AddProperty("Negative Absorption", v, vi, null);
				}
				else if (p.StartsWith("Litany of the Dead "))
				{
					if (p.EndsWith("Ability Bonus"))
					{
						vi = ParseNumber(trimmed);
						data.AddProperty("Strength", v, vi, null);
						data.AddProperty("Dexterity", v, vi, null);
						data.AddProperty("Constitution", v, vi, null);
						data.AddProperty("Intelligence", v, vi, null);
						data.AddProperty("Wisdom", v, vi, null);
						data.AddProperty("Charisma", v, vi, null);
					}
					else if (p.EndsWith("Combat Bonus"))
					{
						vi = ParseNumber(trimmed);
						data.AddProperty("Attack", v, vi, null);
						data.AddProperty("Damage", v, vi, null);
					}
				}
				else if (p == "Permanent Nihil")
				{
					vi = ParseNumber(trimmed);
					data.AddProperty("Negative Spell Power", v, vi, null);
					data.AddProperty("Poison Spell Power", v, vi, null);
				}
				else if (p.EndsWith(" Bane"))
				{
					if (p.StartsWith("Lesser ")) p = p.Substring(7);
					else if (p.StartsWith("Greater ")) p = p.Substring(8);
					data.AddProperty(p, origv == v ? null : v, vi, null);
				}
				else if (p == "Bloodrage Defense")
				{
					vi = ParseNumber(trimmed);
					data.AddProperty("Physical Resistance Rating", v, vi, null);
					data.AddProperty("Magical Resistance Rating", v, vi, null);
				}
				else if (p == "Creeping Dust Lore")
				{
					data.AddProperty("Acid Spell Critical Chance", v, vi, null);
					data.AddProperty("Cold Spell Critical Chance", v, vi, null);
				}
				else if (p == "Power of Creeping Dust")
				{
					data.AddProperty("Acid Spell Power", v, vi, null);
					data.AddProperty("Cold Spell Power", v, vi, null);
				}
				else data.AddProperty(p, origv == v ? null : v, vi, null);

				return true;
			}
		}

		List<ItemProperty> ParseOptions(DDOItemData data, XmlElement e)
		{
			var options = new List<ItemProperty>();

			var lis = e.GetElementsByTagName("li");
			foreach (XmlElement li in lis)
			{
				int count = data.Properties.Count;

				if (ParseEnhancement(data, li))
				{
					while (data.Properties.Count > count)
					{
						ItemProperty ip = data.Properties[count];
						data.Properties.Remove(ip);
						options.Add(ip);
					}
				}
			}

			return options;
		}

		bool ParseEnhancements(DDOItemData data, XmlElement row)
		{
			int orig = data.Properties.Count;
			try
			{
				var ul = row.GetElementsByTagName("ul");
				if (ul.Count == 0) return false;
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
							string p = ParseSetName(e);
							if (p != null) options.Add(new ItemProperty { Property = p, Type = "set" });
							else options = null;
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
							data.AddProperty("Nearly Finished", null, 1, options);
						else data.AddProperty("Almost There", null, 1, options);
					}
					else if (e.InnerText.StartsWith("Upgrades"))
					{
						options = new List<ItemProperty>();
						var aa = e.GetElementsByTagName("a");
						foreach (XmlElement a in aa)
						{
							if (a.GetAttribute("href").IndexOf("/page/Named_item_sets") > -1)
							{
								if (options.Find(m => m.Property == a.InnerText) == null)
									options.Add(new ItemProperty { Property = a.InnerText, Type = "set" });
							}
						}

						data.AddProperty("Upgradeable", "set", 1, options);
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

						data.AddProperty("Upgradeable", "secondary augment", 1, options);
					}
					else if (e.InnerText.StartsWith("One of the following", StringComparison.InvariantCultureIgnoreCase) || e.InnerText.StartsWith("Random effect", StringComparison.InvariantCultureIgnoreCase))
					{
						data.AddProperty("Random", null, 1, ParseOptions(data, e));
					}
					else if (e.InnerText.StartsWith("Contains a Random pair from the following", StringComparison.InvariantCultureIgnoreCase))
					{
						data.AddProperty("Random", null, 2, ParseOptions(data, e));
					}
					else if (e.InnerText.StartsWith("Upgradeable Item (Temple of Elemental Evil)")) ParseEnhancement(data, e);
					else if (e.InnerText.StartsWith("Upgradeable Item (") || e.InnerText.StartsWith("Suppressed Power"))
					{
						var lis = e.GetElementsByTagName("li");
						foreach (XmlElement li in lis)
						{
							if (li.InnerText.StartsWith("Adds ")) ParseEnhancement(data, li.ChildNodes[1]);
							else if (li.InnerText.StartsWith("Becomes ")) continue;
							// property is replaced
							else if (li.ChildNodes.Count >= 3)
							{
								int props = data.Properties.Count;
								// mimic adding the enhancement being replaced in order to get a copy of it
								ParseEnhancement(data, li.ChildNodes[0]);
								int count = data.Properties.Count;
								List<ItemProperty> op = new List<ItemProperty>();
								// remove the new copies and store in a separate list
								for (int i = 0; i < count - props; i++)
								{
									op.Add(data.Properties[data.Properties.Count - 1]);
									data.Properties.RemoveAt(data.Properties.Count - 1);
								}
								// remove the original properties that match the copies made
								foreach (ItemProperty ip in op)
								{
									ItemProperty cp = data.Properties.Find(p => p.Property == ip.Property && p.Type == ip.Type && p.Value == ip.Value);
									data.Properties.Remove(cp);
								}
								// add the new enhancement
								if (li.ChildNodes.Count <= 4) ParseEnhancement(data, li.ChildNodes[2]);
								// this should catch the special case of a spell being parsed
								else ParseEnhancement(data, li.ChildNodes[3]);
							}
						}
					}
					else if (e.InnerText.StartsWith("Attuned to Heroism"))
					{
						var lis = e.GetElementsByTagName("li");
						foreach (XmlElement li in lis)
						{
							if (li.InnerText.Trim().StartsWith("Attuned by")) continue;
							if (!li.ParentNode.ParentNode.InnerText.Trim().StartsWith("Attuned by")) continue;
							if (li.InnerText.Contains(" Augment Slot"))
							{
								data.AddProperty("Augment Slot", ParseAugmentSlot(li.InnerText), 0, null);
							}
							else if (li.InnerText.StartsWith("Adds a choice of one of the following"))
							{
								options = ParseOptions(data, li);
								data.AddProperty("Upgradeable", null, 0, options);
							}
							else
							{
								string set = ParseSetName(li);
								if (set != null)
								{
									// Planar Conflux isn't really a set on its own, it's the option of three sets
									data.AddProperty("Planar Conflux", null, 1, new List<ItemProperty>
									{
										new ItemProperty { Property = "Planar Focus: Erudition", Type = "set" },
										new ItemProperty { Property = "Planar Focus: Prowess", Type = "set" },
										new ItemProperty { Property = "Planar Focus: Subterfuge", Type = "set" }
									});
								}
								else if (li.InnerText.StartsWith("Adds ")) ParseEnhancement(data, li.ChildNodes[1]);
								else if (li.InnerText.StartsWith("Becomes ")) continue;
								// property is replaced
								else if (li.ChildNodes.Count >= 3)
								{
									int props = data.Properties.Count;
									// mimic adding the enhancement being replaced in order to get a copy of it
									ParseEnhancement(data, li.ChildNodes[0]);
									int count = data.Properties.Count;
									List<ItemProperty> op = new List<ItemProperty>();
									// remove the new copies and store in a separate list
									for (int i = 0; i < count - props; i++)
									{
										op.Add(data.Properties[data.Properties.Count - 1]);
										data.Properties.RemoveAt(data.Properties.Count - 1);
									}
									// remove the original properties that match the copies made
									foreach (ItemProperty ip in op)
									{
										ItemProperty cp = data.Properties.Find(p => p.Property == ip.Property && p.Type == ip.Type && p.Value == ip.Value);
										data.Properties.Remove(cp);
									}
									// add the new enhancement
									if (li.ChildNodes.Count <= 4) ParseEnhancement(data, li.ChildNodes[2]);
									// this should catch the special case of a spell being parsed
									else ParseEnhancement(data, li.ChildNodes[3]);
								}
							}
						}
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
					else if (e.InnerText.StartsWith("Against the Slave Lords Set Bonus"))
					{
						options = new List<ItemProperty>();
						string d = data.Name.StartsWith("Legendary ") ? "Legendary " : "";
						options.Add(new ItemProperty { Property = d + "Slave Lord's Might", Type = "set" });
						options.Add(new ItemProperty { Property = d + "Slave Lord's Sorcery", Type = "set" });
						options.Add(new ItemProperty { Property = d + "Slave's Endurance", Type = "set" });

						data.AddProperty("Against the Slave Lords Set Bonus", null, 1, options);
					}
					else ParseEnhancement(data, e);
				}
			}
			catch (Exception ex)
			{
				LogError("- parsing error with enhancements for item " + data.Name + Environment.NewLine + ex.Message);
			}

			return data.Properties.Count != orig;
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
					if (split[0] == "Cosmetic") return null;
					data.Category = (int)(ArmorCategory)Enum.Parse(typeof(ArmorCategory), split[0]);
					data.AddProperty("Armor Category", split[0], 0, null);
					tvpath = "Armor|" + split[0] + "|" + data.Name;
				}
				else if (r.InnerText.StartsWith("Minimum Level", StringComparison.InvariantCultureIgnoreCase))
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
					if (!ParseEnhancements(data, r)) return null;
				}
				else if (r.InnerText.IndexOf("drops on death", StringComparison.InvariantCultureIgnoreCase) > -1)
				{
					return null;
				}
				else if (r.InnerText.StartsWith("Location"))
				{
					string loc = ParseLocation(data, r);
					if (loc == "/page/Test_Dojo_Loot_Room" || loc == "/page/DDO_Store") return null;
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
					data.AddProperty("Offhand Category", split[0], 0, null);
					tvpath = "Offhand|" + split[0] + "|" + data.Name;
				}
				else if (r.InnerText.StartsWith("Minimum Level", StringComparison.InvariantCultureIgnoreCase))
				{
					ParseMinimumLevel(data, r);
				}
				else if (r.InnerText.StartsWith("Enhancements"))
				{
					if (!ParseEnhancements(data, r)) return null;
				}
				else if (r.InnerText.StartsWith("Shield Bonus"))
				{
					data.AddProperty("Armor Class", "shield", ParseNumber(r.InnerText), null);
				}
				else if (r.InnerText.IndexOf("drops on death", StringComparison.InvariantCultureIgnoreCase) > -1)
				{
					return null;
				}
				else if (r.InnerText.StartsWith("Location"))
				{
					string loc = ParseLocation(data, r);
					if (loc == "/page/Test_Dojo_Loot_Room" || loc == "/page/DDO_Store") return null;
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
						data.AddProperty("Weapon Category", split[0], 0, null);
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
						split[0] = split[0].Trim();
						if (split[0] == "Greatclub") split[0] = "Great Club";
						else if (split[0] == "Handwrap") split[0] = "Handwraps";
						else if (split[0] == "Handaxe") split[0] = "Hand Axe";
						else if (split[0] == "Longbow") split[0] = "Long Bow";
						ItemProperty ip = data.AddProperty("Weapon Type", split[0].Trim(), 0, null);
						data.AddProperty("Handedness", null, TwoHandedWeaponTypes.Contains(ip.Type) ? 2 : 1, null);
						if (split[1].IndexOf("throw", StringComparison.OrdinalIgnoreCase) > -1)
						{
							data.Category = (int)WeaponCategory.Throwing;
							data.AddProperty("Weapon Category", "Throwing", 0, null);
						}
						tvpath = "Weapon|" + (WeaponCategory)data.Category + "|" + data.Name;
					}
					catch
					{
						LogError("- parse error with weapon type for weapon named " + data.Name);
					}
				}
				else if (r.InnerText.StartsWith("Minimum Level", StringComparison.InvariantCultureIgnoreCase))
				{
					ParseMinimumLevel(data, r);
				}
				else if (r.InnerText.StartsWith("Enchantments"))
				{
					if (!ParseEnhancements(data, r)) return null;
				}
				else if (r.InnerText.IndexOf("drops on death", StringComparison.InvariantCultureIgnoreCase) > -1)
				{
					return null;
				}
				else if (r.InnerText.StartsWith("Location"))
				{
					string loc = ParseLocation(data, r);
					if (loc == "/page/Test_Dojo_Loot_Room" || loc == "/page/DDO_Store") return null;
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
						if (r.InnerText.Contains("Cosmetic")) return null;
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
				else if (r.InnerText.StartsWith("Minimum level", StringComparison.InvariantCultureIgnoreCase))
				{
					ParseMinimumLevel(data, r);
				}
				// because rune arms don't follow the standard weapon format
				else if (r.InnerText.StartsWith("Required Trait") && r.InnerText.Contains("Rune Arm"))
				{
					data.Slot = SlotType.Offhand;
					data.Category = (int)OffhandCategory.RuneArm;
					data.AddProperty("Weapon Type", "Rune Arm", 0, null);
					data.AddProperty("Offhand Category", OffhandCategory.RuneArm.ToString(), 0, null);
					tvpath = "Offhand|Rune Arm|" + data.Name;
				}
				else if (r.InnerText.StartsWith("Enchantments"))
				{
					if (!ParseEnhancements(data, r)) return null;
				}
				else if (r.InnerText.IndexOf("drops on death", StringComparison.InvariantCultureIgnoreCase) > -1)
				{
					return null;
				}
				else if (r.InnerText.StartsWith("Location"))
				{
					string loc = ParseLocation(data, r);
					if (loc == "/page/Test_Dojo_Loot_Room" || loc == "/page/DDO_Store") return null;
				}
			}

			return tvpath;
		}

		void InitialLoad_DoWork(object sender, DoWorkEventArgs e)
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

				if (itemName.Contains("(historic)")) continue;
				else if (itemName == "Enchanted Chocolates by Fabiano & Zelda") continue;
				else
				{
					var aas = doc.GetElementsByTagName("a");
					bool historic = false;
					foreach (XmlElement a in aas)
						if (a.GetAttribute("href") == "/page/Category:History")
						{
							historic = true;
							break;
						}
					if (historic) continue;
				}

				DDOItemData data = new DDOItemData(ItemDataSource.Dataset, MinorArtifacts.Contains(itemName)) { Name = itemName };

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

					if (tvpath != null)
					{
						ItemsCache.Add(data);
						Dispatcher.Invoke(new Action(() => { SetTreeViewItemAtPath(tvpath, data); }));
					}

					// special case tests for Slaver's items that can fit into multiple slots, so we create duplicates for the non-listed slot versions
					SlotType other = SlotType.None;
					if (data.Name == "Chains" || data.Name == "Legendary Chains") other = SlotType.Waist;
					else if (data.Name == "Five Rings" || data.Name == "Legendary Five Rings") other = SlotType.Trinket;
					else if (data.Name == "Shackles" || data.Name == "Legendary Shackles") other = SlotType.Feet;
					if (other != SlotType.None)
					{
						DDOItemData dup = data.Duplicate();
						dup.Slot = other;
						if (tvpath != null)
						{
							int c = tvpath.IndexOf('|');
							tvpath = other.ToString() + "|" + tvpath.Substring(c + 1);
							Dispatcher.Invoke(new Action(() => { SetTreeViewItemAtPath(tvpath, dup); }));

							foreach (var kvp in QuestLookup)
							{
								if (kvp.Value.Exists(it => it.Name == data.Name))
								{
									kvp.Value.Add(dup);
									break;
								}
							}
						}
					}
				}
			}
		}

		XmlDocument DownloadWebpage(string url)
		{
			try
			{
				WebClient wc = new WebClient();
				Stream quest = wc.OpenRead(url);
				// sgml reader to help format and process html into xml
				SgmlReader sgmlReader = new SgmlReader();
				sgmlReader.DocType = "HTML";
				sgmlReader.WhitespaceHandling = WhitespaceHandling.None;
				sgmlReader.CaseFolding = CaseFolding.ToLower;
				sgmlReader.InputStream = new StreamReader(quest);

				// create xml document
				XmlDocument doc = new XmlDocument();
				doc.PreserveWhitespace = false;
				doc.XmlResolver = null;
				doc.Load(sgmlReader);

				return doc;
			}
			catch
			{
				LogError("Error downloading webpage " + url);
				return null;
			}
		}

		DDOAdventurePackData LoadAdventurePack(string name)
		{
			// we first get the page because it's possible it redirects
			string url = "https://ddowiki.com/page/" + name.Replace(' ', '_');
			XmlDocument doc = DownloadWebpage(url);
			if (doc == null)
			{
				LogError("Attempted to load adventure pack " + name + " at url " + url);
				return null;
			}
			// find the edit url
			var aas = doc.GetElementsByTagName("a");
			foreach (XmlElement a in aas)
			{
				if (a.GetAttribute("accesskey") == "e")
				{
					XmlDocument rdoc = DownloadWebpage("https://ddowiki.com" + a.GetAttribute("href"));
					var taNodes = rdoc.GetElementsByTagName("textarea");
					if (taNodes.Count == 0) continue;
					DDOAdventurePackData apd = new DDOAdventurePackData();
					foreach (XmlElement ta in taNodes)
					{
						if (ta.GetAttribute("id") != "wpTextbox1") continue;
						apd.FreeToVIP = !ta.InnerText.Contains("[[Category:Expansion Packs]]");
						apd.Name = name;
						return apd;
					}

					break;
				}
			}

			return null;
		}

		DDOAdventurePackData GetAdventurePackData(string adpname)
		{
			if (!AdpackLookup.ContainsKey(adpname))
			{
				DDOAdventurePackData apd = LoadAdventurePack(adpname);
				AdpackLookup[adpname] = apd;
			}

			return AdpackLookup[adpname];
		}

		DDOAdventurePackData GetCreateAdventurePackData(string adpname, bool freetovip)
		{
			if (!AdpackLookup.ContainsKey(adpname))
			{
				DDOAdventurePackData apd = new DDOAdventurePackData() { Name = adpname, FreeToVIP = freetovip };
				AdpackLookup[adpname] = apd;
			}

			return AdpackLookup[adpname];
		}

		void AddQuestToAdventurePack(DDOQuestData qd, string apdname, bool freetovip)
		{
			DDOAdventurePackData apd = GetCreateAdventurePackData(apdname, freetovip);
			apd.Quests.Add(qd);
			qd.Adpack = apd;
		}

		string GetNameFromPartialPath(string pp)
		{
			return pp.Substring(pp.LastIndexOf('/') + 1).Replace('_', ' ').Replace("%27", "'");
		}

		void QuestParse_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker bw = sender as BackgroundWorker;
			int progress = 0;
			List<KeyValuePair<string, List<DDOItemData>>> epiccrafting = new List<KeyValuePair<string, List<DDOItemData>>>();
			var qllist = QuestLookup.ToList();
			foreach (var kvp in qllist)
			{
				if (kvp.Key.StartsWith("Epic version "))
				{
					epiccrafting.Add(kvp);
					continue;
				}

				bw.ReportProgress(++progress, kvp.Key);
				XmlDocument doc = DownloadWebpage("https://ddowiki.com" + kvp.Key.Replace("page", "edit"));

				var taNodes = doc.GetElementsByTagName("textarea");
				if (taNodes.Count == 0) continue;

				DDOQuestData questdata = new DDOQuestData();
				foreach (var item in kvp.Value)
				{
					item.QuestFoundIn = questdata;
					questdata.Items.Add(item);
				}

				if (kvp.Key == "/page/DDO_Store")
				{
					questdata.Name = "DDO Store";
					continue;
				}
				else if (kvp.Key == "/page/Advance_to_level_15")
				{
					questdata.Name = "Iconic Gear";
					AddQuestToAdventurePack(questdata, "Iconic Gear", false);
					continue;
				}
				else if (kvp.Key == "/page/Treasure_of_Crystal_Cove" || kvp.Key == "/page/Anniversary_Party" || kvp.Key == "/page/Anniversary_Card_Collection" ||
						kvp.Key == "/page/The_Night_Revels" || kvp.Key == "/page/Mimic_Hunt" || kvp.Key == "/page/Mabar_Endless_Night_Festival" || kvp.Key == "/page/Festivult" ||
						kvp.Key == "/page/Risia_Ice_Games")
				{
					questdata.Name = GetNameFromPartialPath(kvp.Key);
					questdata.IsFree = true;
					AddQuestToAdventurePack(questdata, "Special Events", true);
					continue;
				}
				else if (kvp.Key == "/page/Cauldron_of_Sora_Katra")
				{
					questdata.Name = GetNameFromPartialPath(kvp.Key);
					AddQuestToAdventurePack(questdata, "Attack on Stormreach", true);
					continue;
				}
				else if (kvp.Key == "/page/Tome_of_Untold_Legends")
				{
					questdata.Name = GetNameFromPartialPath(kvp.Key);
					AddQuestToAdventurePack(questdata, "The Necropolis, Part 4", true);
					continue;
				}
				else if (kvp.Key == "/page/Dreamforge")
				{
					questdata.Name = GetNameFromPartialPath(kvp.Key);
					AddQuestToAdventurePack(questdata, "The Dreaming Dark", true);
					continue;
				}
				else if (kvp.Key == "/page/Magma_Forge")
				{
					questdata.Name = GetNameFromPartialPath(kvp.Key);
					AddQuestToAdventurePack(questdata, "Shadow Under Thunderholme", true);
					continue;
				}
				else if (kvp.Key == "/page/Altar_of_Fecundity")
				{
					questdata.Name = GetNameFromPartialPath(kvp.Key);
					AddQuestToAdventurePack(questdata, "The Vale of Twilight", true);
					continue;
				}
				else if (kvp.Key == "/page/Syranian_Forged_Weaponry")
				{
					questdata.Name = GetNameFromPartialPath(kvp.Key);
					AddQuestToAdventurePack(questdata, "Masterminds of Sharn", true);
				}
				else if (kvp.Key == "/page/The_Underdark")
				{
					questdata.Name = GetNameFromPartialPath(kvp.Key);
					AddQuestToAdventurePack(questdata, "Menace of the Underdark", false);
				}

				foreach (XmlElement ta in taNodes)
				{
					if (ta.GetAttribute("id") != "wpTextbox1") continue;

					// this is a quest whose page redirects
					// there are a number of reasons why a location page redirects
					if (ta.InnerText.StartsWith("#REDIRECT"))
					{
						questdata.Name = kvp.Key.Substring(6).Replace('_', ' ');
						questdata.IsFree = questdata.IsRaid = false;
						int c = ta.InnerText.IndexOf("[[");
						string adpname = ta.InnerText.Substring(c + 2, ta.InnerText.IndexOf("]]", c) - c - 2).Trim();

						DDOAdventurePackData apd = null;
						// this is a redirect to the adpack page, because it's too hard for the DDO Wiki people to keep things well organized.
						if (kvp.Key == "/page/The_Abandoned_Excavation" || kvp.Key == "/page/The_Maleficent_Cabal" || kvp.Key == "/page/Catacombs" || kvp.Key == "/page/Ruins_of_Gianthold")
						{
							apd = GetAdventurePackData(adpname);
						}
						else if (kvp.Key == "/page/Silver_Flame_Nugget" || kvp.Key == "/page/Emerald_Claw_Nugget")
						{
							// adpname is an item page, so we have to hardcode the actual adpack name
							apd = GetAdventurePackData("The Necropolis, Part 4");
						}
						else if (kvp.Key == "/page/Special_event_items")
						{
							// this is a unique location, it covers multiple quests across multiple special events, but has no adpack, so we fake it
							apd = GetCreateAdventurePackData("Special Events", true);
							questdata.IsFree = true;
						}
						else if (kvp.Key == "/page/Blue_Water_Inn")
						{
							// this redirects to a wilderness zone page, so hardcode the proper adpack
							apd = GetAdventurePackData("Mists of Ravenloft");
						}
						else if (kvp.Key == "/page/Necropolis")
						{
							// apparently a name change/fix needed a page redirect, so hardcode the first of the adpacks
							apd = GetAdventurePackData("The Necropolis, Part 1");
						}
						else if (kvp.Key == "/page/Crafted" || kvp.Key == "/page/Stone_of_Experience")
						{
							// this isn't really a quest, and it's not appropriate to have an adpack created for it
							continue;
						}
						else if (kvp.Key == "/page/None")
						{
							// this is a one-off item that is given to players during Delera's Tomb quests
							apd = GetAdventurePackData("Delera's Tomb");
						}
						else if (kvp.Key == "/page/Return_to_the_Sanctuary")
						{
							apd = GetAdventurePackData("The Catacombs");
						}
						else if (kvp.Key == "/page/Altar_of_Vulkoor")
						{
							apd = GetAdventurePackData("The Red Fens");
						}
						else if (kvp.Key == "/page/Champion_Hunter" || kvp.Key == "/page/Favor_Rewards")
						{
							// this is for items purchasable with mysterious remnants or as rewards for patron favor, so the quest needs to be marked as free but with no adpack
							questdata.IsFree = true;
						}
						else if (kvp.Key == "/page/Orchard_of_the_Macabre")
						{
							// another name change/fix needed a page redirect (sure, right), so hardcode the proper adpack
							apd = GetAdventurePackData("The Necropolis, Part 4");
						}
						else if (kvp.Key == "/page/Black_Dragonscale_Armor" || kvp.Key == "/page/Blue_Dragonscale_Armor" || kvp.Key == "/page/White_Dragonscale_Armor" ||
								 kvp.Key == "/page/Dragoncraft_Armor" || kvp.Key == "/page/Elfcraft_Armor" || kvp.Key == "/page/Giantcraft_Armor")
						{
							// this is a reference to Gianthold crafting, so hardcode the proper adpack
							apd = GetAdventurePackData("Ruins of Gianthold");
						}
						else if (kvp.Key == "/page/Dragontouched_Armor")
						{
							// this is a reference to Reaver's Reach crafting, so hardcode the proper adpack
							apd = GetAdventurePackData("The Reaver's Reach");
						}
						else if (kvp.Key == "/page/A_Break_in_the_Ice")
						{
							apd = GetAdventurePackData("Shadowfell Conspiracy");
						}
						else
						{
							LogError("Redirecting quest from " + kvp.Key + " to " + adpname + ". " + kvp.Value.Count + " items, first is " + kvp.Value[0].WikiURL);
						}

						if (apd != null)
						{
							apd.Quests.Add(questdata);
							questdata.Adpack = apd;
						}
						continue;
					}

					string[] props = ta.InnerText.Split('|');
					foreach (string p in props)
					{
						string[] ps;
						if (p.Contains("}}")) ps = p.Substring(0, p.IndexOf("}}")).Split('=');
						else ps = p.Split('=');
						if (ps.Length != 2) continue;
						ps[0] = ps[0].Trim();
						ps[1] = ps[1].Trim();
						if (string.IsNullOrWhiteSpace(ps[1])) continue;
						if (string.Compare(ps[0], "name", true) == 0)
						{
							questdata.Name = ps[1];
						}
						else if (string.Compare(ps[0], "adpack", true) == 0)
						{
							DDOAdventurePackData apd = GetAdventurePackData(ps[1]);
							apd.Quests.Add(questdata);
							questdata.Adpack = apd;
						}
						else if (string.Compare(ps[0], "raid", true) == 0)
						{
							questdata.IsRaid = string.Compare(ps[1], "yes", true) == 0;
						}
						else if (string.Compare(ps[0], "free", true) == 0)
						{
							questdata.IsFree = string.Compare(ps[1], "yes", true) == 0;
						}
					}

					if (questdata.Adpack == null)
					{
						if (string.IsNullOrWhiteSpace(questdata.Name))
						{
							questdata.Name = GetNameFromPartialPath(kvp.Key);
							questdata.IsFree = true;
							LogError("Quest page without proper adpack nor templating : " + kvp.Key + ", " + kvp.Value.Count + " items, first is " + kvp.Value[0].WikiURL);
						}
					}
				}
			}

			foreach (var kvp in epiccrafting)
			{
				bw.ReportProgress(++progress, kvp.Key);
				string heroicname = kvp.Key.Substring(16).Replace("Item:", "").Trim();
				DDOItemData id = ItemsCache.Find(i => i.Name == heroicname);
				if (id != null)
				{
					foreach (var item in kvp.Value)
					{
						item.QuestFoundIn = id.QuestFoundIn;
						id.QuestFoundIn.Items.Add(item);
					}
				}
				else LogError("Couldn't find heroic item " + heroicname + ", original string is " + kvp.Key);
			}
		}

		void InitialLoad_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			tbStatusBarText.Text = Path.GetFileName(files[e.ProgressPercentage]);
			tbProgressText.Text = (e.ProgressPercentage + 1).ToString() + " of " + files.Length;
			pbProgressBar.Value = e.ProgressPercentage;
		}

		void QuestParse_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			tbStatusBarText.Text = e.UserState.ToString();
			tbProgressText.Text = e.ProgressPercentage + " of " + QuestLookup.Count;
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

		List<DDOItemData> AllItems;
		DDODataset dataset;
		string datasetFilepath;

		void TraverseItems(ItemCollection ic)
		{
			foreach (TreeViewItem tvi in ic)
			{
				if (tvi.Items.Count > 0) TraverseItems(tvi.Items);
				else
				{
					DDOItemData item = tvi.Tag as DDOItemData;
					int si = -1;
					for (int i = 0; i < AllItems.Count; i++)
					{
						if (string.Compare(item.Name, AllItems[i].Name, true) < 0)
						{
							si = i;
							break;
						}
					}
					if (si == -1) AllItems.Add(item);
					else AllItems.Insert(si, item);
				}
			}
		}

		private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.SelectedPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\..\\DDONamedGearPlanner\\");
			if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
			datasetFilepath = Path.Combine(fbd.SelectedPath, "ddodata.dat");

			dataset = new DDODataset();
			dataset.Initialize();
			dataset.AdventurePacks = AdpackLookup.Select(s => s.Value).OrderBy(s => s.Name).ToList();

			// first go through all set bonuses and populate the properties into the itemproperty list
			foreach (var set in dataset.Sets)
			{
				foreach (var sb in set.Value.SetBonuses)
				{
					foreach (var b in sb.Bonuses) dataset.AddItemProperty(b.Property, b.Type, null);
				}
			}

			// sort all items alphabetically into a single list
			AllItems = new List<DDOItemData>();
			TraverseItems(tvList.Items);

			BackgroundWorker bw = new BackgroundWorker();
			bw.WorkerReportsProgress = true;
			bw.DoWork += ProcessItems;
			bw.ProgressChanged += ProcessItemsProgress;
			bw.RunWorkerCompleted += ProcessItemsCompleted;
			bw.RunWorkerAsync();
		}

		private void ProcessItemsCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			tbProgressText.Text = null;
			tbStatusBarText.Text = "Saving dataset...";
			// write dataset out to file
			FileStream fs = new FileStream(datasetFilepath, FileMode.Create);
			BinaryFormatter bf = new BinaryFormatter();
			try
			{
				bf.Serialize(fs, dataset);
				tbStatusBarText.Text = "Dataset saved";
			}
			catch (Exception ex)
			{
				tbStatusBarText.Text = "Error saving dataset";
				System.Windows.MessageBox.Show("Error writing ddo dataset!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				fs.Close();
			}
		}

		private void ProcessItemsProgress(object sender, ProgressChangedEventArgs e)
		{
			if (e.UserState.ToString() == "items")
			{
				tbProgressText.Text = (e.ProgressPercentage + 1).ToString() + " of " + AllItems.Count;
				tbStatusBarText.Text = "Processing item : " + AllItems[e.ProgressPercentage].Name;
			}
		}

		private void ProcessItems(object sender, DoWorkEventArgs e)
		{
			for (int i = 0; i < AllItems.Count; i++)
			{
				(sender as BackgroundWorker).ReportProgress(i, "items");
				string result = dataset.AddItem(AllItems[i]);
				if (result != null) LogError(result);
			}

			foreach (var sp in dataset.SlotExclusiveItemProperties)
			{
				sp.Value.Sort((a, b) => string.Compare(a.Property, b.Property));
			}
		}

		private void GenerateItemPropertyReport_Click(object sender, RoutedEventArgs e)
		{
			if (dataset == null)
			{
				System.Windows.MessageBox.Show("Save dataset first", "Hey", MessageBoxButton.OK, MessageBoxImage.Stop);
				return;
			}

			string filename = "item property report.txt";
			File.Delete(filename);
			var file = File.AppendText(filename);

			foreach (var ip in dataset.ItemProperties)
			{
				bool propwritten = false;
				foreach (var item in ip.Value.Items)
				{
					bool itemwritten = false;
					foreach (var p in item.Properties)
					{
						if (p.Property != ip.Key) continue;
						if (p.Type != null && p.Type.Contains(" "))
						{
							if (!propwritten)
							{
								file.WriteLine("[" + ip.Key + "]");
								propwritten = true;
							}
							if (!itemwritten)
							{
								file.WriteLine("{" + item.Name + "}");
								itemwritten = true;
							}
							file.WriteLine(p.Type);
						}
					}
				}

				if (propwritten) file.WriteLine("");
			}

			file.Flush();
			file.Close();

			System.Diagnostics.Process.Start(filename);
		}

		private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
