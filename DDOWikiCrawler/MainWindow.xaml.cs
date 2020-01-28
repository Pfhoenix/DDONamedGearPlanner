using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Abot2.Crawler;
using Abot2.Poco;
using System.IO;
using System.Diagnostics;

namespace DDOWikiCrawler
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		string[] IgnoredCategories =
		{
			"Collars",
			"Weapons_by_handedness",
			"Weapons_by_type",
			"Weapons_by_damage_type",
			"Item_materials",
			"Unique_item_enchantments",
			"Potions",
			"Wands",
			"Hair_dyes",
			"Cosmetic_items",
			"Quest_items",
			"DDO_Store_items",
			"Items_by_update",
			"Items",
			"Items_by_effect",
			"Root",
			"DDO_library"
		};

		string HtmlCachePath;
		CancellationTokenSource cts = new CancellationTokenSource();
		bool onFirstPage = false;
		bool recrawling = false;
		TreeViewItem currentTVI;

		public MainWindow()
		{
			InitializeComponent();

			HtmlCachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HtmlCache");
			Directory.CreateDirectory(HtmlCachePath);
		}

		private async void BtnStart_Click(object sender, RoutedEventArgs e)
		{
			btnStart.IsEnabled = false;
			btnStop.IsEnabled = true;

			if (tvCachedPages.Items.Count == 0)
			{
				onFirstPage = true;

				var config = new CrawlConfiguration
				{
					MinCrawlDelayPerDomainMilliSeconds = 50,
					MaxCrawlDepth = 1,
					LoginUser = "Pfhoenix",
					LoginPassword = "danneskjold",
					IsAlwaysLogin = true
				};
				PoliteWebCrawler crawler = new PoliteWebCrawler(config);
				crawler.PageCrawlStarting += PageCrawlStarting;
				crawler.ShouldCrawlPageDecisionMaker = (ptc, cc) =>
				{
					if (string.Compare(ptc.Uri.Authority, "ddowiki.com", true) != 0) return new CrawlDecision { Allow = false, Reason = "Not on ddowiki domain" };
					if (!ptc.Uri.AbsolutePath.StartsWith("/page/")) return new CrawlDecision { Allow = false, Reason = "Only crawl pages" };
					if (!onFirstPage && !ptc.Uri.AbsolutePath.Contains("Category:")) return new CrawlDecision { Allow = false, Reason = "Not a category page" };

					onFirstPage = false;

					return new CrawlDecision { Allow = true };
				};
				crawler.PageCrawlCompleted += CategoryCrawlCompleted;

				tbStatusBar.Text = "Category crawl started at " + DateTime.Now.ToShortTimeString() + " ...";
				Stopwatch sw = new Stopwatch();
				sw.Start();

				await crawler.CrawlAsync(new Uri("https://ddowiki.com/page/Items"), cts);

				sw.Stop();
				tbStatusBar.Text = "Category crawl took " + (sw.ElapsedMilliseconds / 1000.0) + " seconds";
				btnStop.IsEnabled = false;
				btnStart.IsEnabled = true;
				btnStart.Content = "Get All Items";
			}
			else
			{
				if (tvCachedPages.SelectedItem == null || !((TreeViewItem)tvCachedPages.SelectedItem).HasItems)
				{
					recrawling = false;
					tbStatusBar.Text = "Item crawl started at " + DateTime.Now.ToShortTimeString() + "...";
				}
				else
				{
					recrawling = true;
					tbStatusBar.Text = "Item recrawl started at " + DateTime.Now.ToShortTimeString() + "...";
				}

				Stopwatch sw = new Stopwatch();
				sw.Start();

				if (recrawling)
				{
					onFirstPage = true;

					var config = new CrawlConfiguration
					{
						MinCrawlDelayPerDomainMilliSeconds = 50,
						MaxCrawlDepth = 1
					};
					PoliteWebCrawler crawler = new PoliteWebCrawler(config);
					crawler.PageCrawlStarting += PageCrawlStarting;
					crawler.ShouldCrawlPageDecisionMaker = (ptc, cc) =>
					{
						if (string.Compare(ptc.Uri.Authority, "ddowiki.com", true) != 0) return new CrawlDecision { Allow = false, Reason = "Not on ddowiki domain" };
						if (!ptc.Uri.AbsolutePath.StartsWith("/page/")) return new CrawlDecision { Allow = false, Reason = "Only crawl pages" };
						if (!onFirstPage && !ptc.Uri.AbsolutePath.Contains("Item:")) return new CrawlDecision { Allow = false, Reason = "Not an item page" };

						onFirstPage = false;

						return new CrawlDecision { Allow = true };
					};
					crawler.PageCrawlCompleted += ItemCrawlCompleted;

					await crawler.CrawlAsync(new Uri((tvCachedPages.SelectedItem as TreeViewItem).Tag.ToString()), cts);
				}
				else
				{
					foreach (TreeViewItem tvi in tvCachedPages.Items)
					{
						onFirstPage = true;
						currentTVI = tvi;

						var config = new CrawlConfiguration
						{
							MinCrawlDelayPerDomainMilliSeconds = 50,
							MaxCrawlDepth = 1
						};
						PoliteWebCrawler crawler = new PoliteWebCrawler(config);
						crawler.PageCrawlStarting += PageCrawlStarting;
						crawler.ShouldCrawlPageDecisionMaker = (ptc, cc) =>
						{
							if (string.Compare(ptc.Uri.Authority, "ddowiki.com", true) != 0) return new CrawlDecision { Allow = false, Reason = "Not on ddowiki domain" };
							if (!ptc.Uri.AbsolutePath.StartsWith("/page/")) return new CrawlDecision { Allow = false, Reason = "Only crawl pages" };
							if (!onFirstPage && !ptc.Uri.AbsolutePath.Contains("Item:")) return new CrawlDecision { Allow = false, Reason = "Not an item page" };

							onFirstPage = false;

							return new CrawlDecision { Allow = true };
						};
						crawler.PageCrawlCompleted += ItemCrawlCompleted;

						await crawler.CrawlAsync(new Uri(tvi.Tag.ToString()), cts);
					}
				}

				sw.Stop();
				if (recrawling) tbStatusBar.Text = "Item recrawl took ";
				else tbStatusBar.Text = "Item crawl took ";
				tbStatusBar.Text += (sw.ElapsedMilliseconds / 1000.0) + " seconds";

				btnStop.IsEnabled = false;
				btnStart.Content = "Recrawl Items";
				btnStart.IsEnabled = true;
			}
		}

		private void BtnStop_Click(object sender, RoutedEventArgs e)
		{
			btnStart.IsEnabled = true;
			btnStop.IsEnabled = false;

			cts.Cancel();
		}

		void PageCrawlStarting(object sender, PageCrawlStartingArgs e)
		{
			Dispatcher.Invoke(new Action(() => { UpdateCurrentUri(e.PageToCrawl.Uri.ToString()); }));
		}

		void UpdateCurrentUri(string uri)
		{
			txtCurrentPage.Text = uri;
		}

		void CategoryCrawlCompleted(object sender, PageCrawlCompletedArgs e)
		{
			var httpStatus = e.CrawledPage.HttpResponseMessage.StatusCode;
			if (httpStatus == HttpStatusCode.OK)
			{
				if (e.CrawledPage.Uri.AbsolutePath.Contains("/page/Category:"))
					Dispatcher.Invoke(new Action(() => { AddToCategoryCachedPagesList(e.CrawledPage.Uri.ToString()); }));
			}

			Dispatcher.Invoke(new Action(() => { UpdateCurrentUri(null); }));
		}

		void AddToCategoryCachedPagesList(string uri)
		{
			int i = uri.IndexOf("Category:");
			TreeViewItem tvi = new TreeViewItem();
			tvi.Header = uri.Substring(i + 9);
			tvi.Tag = uri;
			if (IgnoredCategories.Contains(tvi.Header.ToString())) return;
			tvCachedPages.Items.Add(tvi);
		}

		void ItemCrawlCompleted(object sender, PageCrawlCompletedArgs e)
		{
			var httpStatus = e.CrawledPage.HttpResponseMessage?.StatusCode;
			if (httpStatus == HttpStatusCode.OK)
			{
				if (e.CrawledPage.Uri.AbsolutePath.StartsWith("/page/Item:"))
				{
					// generate filename for html cache file
					string filename = WebUtility.UrlDecode(e.CrawledPage.Uri.AbsolutePath).Substring(11);
					if (!filename.StartsWith("+") && !filename.StartsWith("-"))
					{
						foreach (char c in Path.GetInvalidFileNameChars())
							filename = filename.Replace(c, '_');
						filename += ".html";
						filename = Path.Combine(HtmlCachePath, filename);
						// recrawling means overwriting existing files
						if (File.Exists(filename) && recrawling) File.Delete(filename);
						// check if html cache file exists
						if (!File.Exists(filename))
						{
							File.WriteAllText(Path.Combine(HtmlCachePath, filename), e.CrawledPage.Content.Text);
						}
						Dispatcher.Invoke(new Action(() => { AddToItemCachedPagesList(WebUtility.UrlDecode(e.CrawledPage.Uri.ToString()), filename); }));
					}
				}
			}
		}

		void AddToItemCachedPagesList(string uri, string filename)
		{
			int i = uri.IndexOf("Item:");
			TreeViewItem tvi = new TreeViewItem();
			tvi.Header = uri.Substring(i + 5);
			tvi.Tag = filename;
			currentTVI.Items.Add(tvi);
		}

		private void TvCachedPages_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete || e.Key == Key.Back)
			{
				TreeViewItem tvi = tvCachedPages.SelectedItem as TreeViewItem;
				if (!tvi.HasItems)
				{
					if (File.Exists(tvi.Tag.ToString())) File.Delete(tvi.Tag.ToString());
					tvCachedPages.Items.Remove(tvi);
				}
			}
		}

		private void TvCachedPages_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeViewItem tvi = tvCachedPages.SelectedItem as TreeViewItem;
			string uri = tvi.Tag.ToString();
			UpdateCurrentUri(uri);
			if (tvi.Tag.ToString().Contains("Category"))
			{
				if (tvi.HasItems) btnStart.Content = "Recrawl Items";
				else btnStart.Content = "Get All Items";

				if (!btnStop.IsEnabled) btnStart.IsEnabled = true;
			}
			else
			{
				btnStart.Content = null;
				btnStart.IsEnabled = false;
			}
		}

		private void TvCachedPages_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			TreeViewItem tvi = tvCachedPages.SelectedItem as TreeViewItem;
			if (tvi != null)
			{
				wbWebpageView.Source = new Uri(tvi.Tag.ToString());
				//wbWebpageView.Navigate(tvi.Tag.ToString());
			}
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if ((sender as TabControl).SelectedIndex == 1)
			{
				dynamic doc = wbWebpageView.Document;
				tbHtmlView.Text = doc?.documentElement.InnerHtml;
			}
		}
	}
}
