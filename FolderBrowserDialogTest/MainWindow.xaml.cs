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

using Aldentea.Wpf.Controls;

namespace Aldentea.Wpf.FolderBrowserDialogTest
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new Aldentea.Wpf.Controls.FolderBrowserDialog { FontSize = 12 };

			dialog.Description = "函館から新青森まではスーパー白鳥に乗ったんだけど，これまた混みすぎ．もっと本数を増やせばいいんだけど，青函トンネルのキャパシティがもう限界なんでしょうか．";
			dialog.SelectedPath = "C:\\";
			dialog.DisplaySpecialFolders = SpecialFoldersFlag.Personal | SpecialFoldersFlag.CommonDocuments | SpecialFoldersFlag.MyMusic; 
			if (dialog.ShowDialog() == true)
			{
				MessageBox.Show(dialog.SelectedPath);
			}
		}
	}
}
