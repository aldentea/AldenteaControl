using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Aldentea.Wpf.Controls
{
	//	Usage:
	//
	//	BackgroundWorkerDialog bwd = new BackgroundWorkerDialog { Current = 0, Total = fileNames.Count };
	//	BackgroundWorker bw = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
	//	bw.DoWork += new DoWorkEventHandler(bw_DoWork);
	//	bw.ProgressChanged += (sender, e) => { bwd.Current = e.ProgressPercentage; };
	//	bw.RunWorkerCompleted += (sender, e) => { bwd.Close(); };
	//	bwd.CancelClicked += (sender, e) => { bw.CancelAsync(); };
	//
	//	bw.RunWorkerAsync(fileNames);
	//	bwd.ShowDialog();
	//
	// 	void bw_DoWork(object sender, DoWorkEventArgs e)
	//	{
	//		BackgroundWorker bw = (BackgroundWorker)sender;
	//		for (int i = 0; i < ○○○○[e.Argumentから取得]; )	// iのインクリメントはforブロックの最終行で行っている．
	//		{
	//			if (bw.CancellationPending)
	//			{
	//			 return;
	//			}
	//
	//			// ここで個別の処理を行う．
	//		  bw.ReportProgress(++i);
	//		}
	//	}


	/// <summary>
	/// BackgroundWorkerDialog.xaml の相互作用ロジック
	/// </summary>
	public partial class BackgroundWorkerDialog : Window
	{

		public static readonly DependencyProperty TotalProperty = 
					DependencyProperty.Register("Total", typeof(int), typeof(BackgroundWorkerDialog), new FrameworkPropertyMetadata(100));
		public int Total
		{
			get
			{
				return (int)GetValue(TotalProperty);
			}
			set
			{
				SetValue(TotalProperty, value);
			}
		}

		public static readonly DependencyProperty CurrentProperty = 
					DependencyProperty.Register("Current", typeof(int), typeof(BackgroundWorkerDialog), new FrameworkPropertyMetadata(0));
		public int Current
		{
			get
			{
				return (int)GetValue(CurrentProperty);
			}
			set
			{
				SetValue(CurrentProperty, value);
			}
		}


		public BackgroundWorkerDialog()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{


		}

		public event EventHandler CancelClicked = delegate { };

		private void buttonCancel_Click(object sender, RoutedEventArgs e)
		{
			this.CancelClicked(this, EventArgs.Empty);
		}
	}



}
