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
	/// <summary>
	/// FolderBrowserDialog.xaml の相互作用ロジック
	/// </summary>
	public partial class FolderBrowserDialog : Window
	{
		public FolderBrowserDialog()
		{
			InitializeComponent();
		}

		#region *Descriptionプロパティ
		/// <summary>
		/// ユーザに向けての説明を取得／設定します．
		/// </summary>
		public string Description
		{
			get
			{
				return (string)GetValue(DescriptionProperty);
			}
			set
			{
				SetValue(DescriptionProperty, value);
			}
		}

		public static readonly DependencyProperty DescriptionProperty
			= DependencyProperty.Register("Description", typeof(string), typeof(FolderBrowserDialog),
						new FrameworkPropertyMetadata("フォルダを選んでください")
						);
		#endregion

		#region *SelectedPathプロパティ
		public string SelectedPath
		{
			get
			{
				return (string)GetValue(SelectedPathProperty);
			}
			set
			{
				SetValue(SelectedPathProperty, value);
			}
		}

		public static readonly DependencyProperty SelectedPathProperty
			= DependencyProperty.Register("SelectedPath", typeof(string), typeof(FolderBrowserDialog),
						new FrameworkPropertyMetadata(OnSelectedPropertyChanged)
				);
		#endregion

		// 09/07/2011 by aldentea
		#region *AllowNewプロパティ
		/// <summary>
		/// フォルダの新規作成が可能かどうかの値を取得／設定します．
		/// </summary>
		public bool AllowNew
		{
			get
			{
				return buttonNew.IsEnabled;
			}
			set
			{
				buttonNew.IsEnabled = value;
			}
		}
		#endregion

		// 06/30/2011 by aldentea
		/// <summary>
		/// [ネットワーク]を表示するかどうかの値を取得／設定します．
		/// </summary>
		public SpecialFoldersFlag DisplaySpecialFolders
		{
			get
			{
				return folderTreeView1.DisplaySpecialFolders;
			}
			set
			{
				folderTreeView1.DisplaySpecialFolders = value;
			}
		}

		static void OnSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			//((FolderBrowserDialog)d).folderTreeView1.SelectedPath = (string)e.NewValue;
		}

		private void buttonOK_Click(object sender, RoutedEventArgs e)
		{
			//if (this.Owner != null)
			//{
				this.DialogResult = true;
			//}
		}

		private void buttonCancel_Click(object sender, RoutedEventArgs e)
		{
			//if (this.Owner != null)
			//{
				this.DialogResult = false;
			//}

		}

		private void buttonNew_Click(object sender, RoutedEventArgs e)
		{
			folderTreeView1.Mkdir();
		}

		// 12/09/2011 by aldentea
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.folderTreeView1.Focus();
		}

		// 12/09/2011 by aldentea
		private void folderTreeView1_KeyDown(object sender, KeyEventArgs e)
		{
			this.buttonOK.Focus();
		}

	}
}
