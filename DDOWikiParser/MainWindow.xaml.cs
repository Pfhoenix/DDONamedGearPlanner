using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace DDOWikiParser
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		string[] files;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
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

				bw.RunWorkerAsync();
			}
		}

		void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			for (int i = 0; i < files.Length; i++)
			{
				(sender as BackgroundWorker).ReportProgress(i);
			}
		}

		void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			tbStatusBarText.Text = Path.GetFileName(files[e.ProgressPercentage]);
			tbProgressText.Text = (e.ProgressPercentage + 1).ToString() + " of " + files.Length;
			pbProgressBar.Value = e.ProgressPercentage;
		}
	}
}
