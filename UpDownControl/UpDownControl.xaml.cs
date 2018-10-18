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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aldentea.Wpf.Controls
{
	/// <summary>
	/// UserControl1.xaml の相互作用ロジック
	/// </summary>
	public partial class UpDownControl : UserControl
	{
		public UpDownControl()
		{
			InitializeComponent();
		}


		// 07/23/2012 by aldentea : FillRuleプロパティはまだ設けていないが...
		#region *[dependency]Fillプロパティ
		/// <summary>
		/// ボタン内部の三角形を塗りつぶすBrushを取得／設定します．
		/// </summary>
		public Brush Fill
		{
			get
			{
				return (Brush)GetValue(FillProperty);
			}
			set
			{
				SetValue(FillProperty, value);
			}
		}

		public static readonly DependencyProperty FillProperty
			 = DependencyProperty.Register("Fill", typeof(Brush), typeof(UpDownControl), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 0, 0))));
		#endregion



		#region イベント

		/// <summary>
		/// 増加ボタンをクリックしたときに発生します．
		/// </summary>
		public event RoutedEventHandler UpClick = delegate { };

		/// <summary>
		/// 減少ボタンをクリックしたときに発生します．
		/// </summary>
		public event RoutedEventHandler DownClick = delegate { };

		#endregion

		#region イベントハンドラ


		#region ボタンクリック関連

		// ※Routedイベントの再発生の方法がよくわからん．

		private void repeatButtonUp_Click(object sender, RoutedEventArgs e)
		{
			e.Source = this;
			UpClick(this, e);
		}

		private void repeatButtonDown_Click(object sender, RoutedEventArgs e)
		{
			e.Source = this;
			DownClick(this, e);
		}

		#endregion

		#region サイズ変更関連

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			repeatButtonUp.Width = repeatButtonDown.Width = this.Width;
			repeatButtonUp.Height = repeatButtonDown.Height = this.Height / 2;
		}

		// 12/04/2013 by aldentea : WidthとHeightをそれぞれActualWidthとActualheightに変更．Y座標を微調整．
		private void repeatButtonUp_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			triangleUp.Points.Clear();

			double w = repeatButtonUp.ActualWidth - 4;
			double h = repeatButtonUp.ActualHeight - 4;

			if (w > 0 && h > 0)
			{
				if (w > h)
				{
					triangleUp.Points.Add(new Point(0.49 * h, 0.04 * h));
					triangleUp.Points.Add(new Point(0, 0.89 * h));
					triangleUp.Points.Add(new Point(0.98 * h, 0.89 * h));
				}
				else
				{
					triangleUp.Points.Add(new Point(0.365 * w, 0));
					triangleUp.Points.Add(new Point(0, 0.64 * w));
					triangleUp.Points.Add(new Point(0.73 * w, 0.64 * w));
				}
			}
		}

		// 12/04/2013 by aldentea : WidthとHeightをそれぞれActualWidthとActualheightに変更．Y座標を微調整．
		private void repeatButtonDown_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			triangleDown.Points.Clear();
			double w = repeatButtonDown.ActualWidth - 4;
			double h = repeatButtonDown.ActualHeight - 4;

			if (w > 0 && h > 0)
			{

				if (w > h)
				{
					triangleDown.Points.Add(new Point(0, 0.04 * h));
					triangleDown.Points.Add(new Point(0.49 * h, 0.89 * h));
					triangleDown.Points.Add(new Point(0.98 * h, 0.04 * h));
				}
				else
				{
					triangleDown.Points.Add(new Point(0, 0));
					triangleDown.Points.Add(new Point(0.365 * w, 0.64 * w));
					triangleDown.Points.Add(new Point(0.73 * w, 0));
				}
			}
		}

		#endregion

		#endregion

	}
}
