using System;
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
		string HtmlCachePath;
		CancellationTokenSource cts = new CancellationTokenSource();
		bool onFirstPage = false;
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
					MinCrawlDelayPerDomainMilliSeconds = 100,
					MaxCrawlDepth = 1
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
				btnStart.Content = "Get Items";
			}
			else
			{
				tbStatusBar.Text = "Category crawl started at " + DateTime.Now.ToShortTimeString() + " ...";
				Stopwatch sw = new Stopwatch();
				sw.Start();

				foreach (TreeViewItem tvi in tvCachedPages.Items)
				{
					onFirstPage = true;
					currentTVI = tvi;

					var config = new CrawlConfiguration
					{
						MinCrawlDelayPerDomainMilliSeconds = 100,
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

				sw.Stop();
				tbStatusBar.Text = "Item crawl took " + (sw.ElapsedMilliseconds / 1000.0) + " seconds";

				btnStop.IsEnabled = false;
				btnStart.Content = "Done";
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
			if (httpStatus == System.Net.HttpStatusCode.OK)
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
			tvCachedPages.Items.Add(tvi);
		}

		void ItemCrawlCompleted(object sender, PageCrawlCompletedArgs e)
		{
			var httpStatus = e.CrawledPage.HttpResponseMessage.StatusCode;
			if (httpStatus == System.Net.HttpStatusCode.OK)
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
				tvCachedPages.Items.Remove(tvCachedPages.SelectedItem);
			}
		}

		private void TvCachedPages_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeViewItem tvi = tvCachedPages.SelectedItem as TreeViewItem;
			string uri = tvi.Tag.ToString();
			UpdateCurrentUri(uri);
		}

		private void TvCachedPages_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			TreeViewItem tvi = tvCachedPages.SelectedItem as TreeViewItem;
			if (tvi != null)
			{
				wbWebpageView.Navigate(tvi.Tag.ToString());
				try
				{
					tbHtmlView.Text = File.ReadAllText(tvi.Tag.ToString());
				}
				catch { }
			}
		}
	}
}
