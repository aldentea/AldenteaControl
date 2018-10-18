using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls.Primitives;

using System.Diagnostics;

namespace Aldentea.Wpf.Controls
{
	/// <summary>
	/// FolderTreeView.xaml の相互作用ロジック
	/// </summary>
	public partial class FolderTreeView : UserControl
	{
		ObservableCollection<DirectoryData> directoryItems = new ObservableCollection<DirectoryData>();

		// 06/30/2011 by aldentea : 追加．
		/// <summary>
		/// [ネットワーク]をルートに表示するか否かの値を取得／設定します．
		/// </summary>
		//public bool DisplayNetworkFolder { get; set; }

		public SpecialFoldersFlag DisplaySpecialFolders { get; set; }

		#region *コンストラクタ(FolderTreeView)
		public FolderTreeView()
		{
			InitializeComponent();

			this.DataContext = directoryItems;
			foreach (System.IO.DriveInfo drive in System.IO.DriveInfo.GetDrives())
			{
				directoryItems.Add(new DirectoryData(drive));
			}


		}
		#endregion

		#region *SelectedPathプロパティ
		/// <summary>
		/// 選択しているディレクトリのフルパスを取得／設定します．
		/// </summary>
		public string SelectedPath
		{
			get
			{
				return (string)GetValue(SelectedPathProperty);
			}
			set
			{
				this.OnSelectedPathChanged(
						new RoutedPropertyChangedEventArgs<string>(
								this.SelectedPath,
								value,
								SelectedPathChangedEvent
						)
				);
				SetValue(SelectedPathProperty, value);
			}
		}

		public static readonly DependencyProperty SelectedPathProperty
			= DependencyProperty.Register("SelectedPath", typeof(string), typeof(FolderTreeView),
						new FrameworkPropertyMetadata(string.Empty));

		#endregion

		#region *SelectedPathChangedイベント
		public event RoutedPropertyChangedEventHandler<string> SelectedPathChanged
		{
			add
			{
				this.AddHandler(SelectedPathChangedEvent, value);
			}
			remove
			{
				this.RemoveHandler(SelectedPathChangedEvent, value);
			}
		}

		public static readonly RoutedEvent SelectedPathChangedEvent
			= EventManager.RegisterRoutedEvent("SelectedPathChanged", RoutingStrategy.Bubble,
					typeof(RoutedPropertyChangedEventHandler<string>), typeof(FolderTreeView));

		protected virtual void OnSelectedPathChanged(RoutedPropertyChangedEventArgs<string> e)
		{
			this.RaiseEvent(e);
		}
		#endregion

		#region *ディレクトリを新規作成(Mkdir)
		/// <summary>
		/// 選択中のディレクトリに，新規ディレクトリを作成します．
		/// </summary>
		public void Mkdir()
		{
			if (Directory.Exists(SelectedPath))
			{
				string directoryName = DateTime.Now.ToString("yyyyMMddHHmmss");
				string directoryFullPath = Path.Combine(SelectedPath, directoryName);
				while (Directory.Exists(directoryFullPath))
				{
					directoryName = directoryName + "_";
					directoryFullPath = Path.Combine(SelectedPath, directoryName);
				}
				Directory.CreateDirectory(directoryFullPath);
			}

		}
		#endregion


		#region *treeView1ロード時
		private void treeView1_Loaded(object sender, RoutedEventArgs e)
		{
			AddSpecialFoldersToTree(this.DisplaySpecialFolders);
			ExtendAndSelectPath();
		}

		// 07/09/2014 by aldentea : treeView1_Loadedから分離．
		#region *特殊フォルダをツリーに追加(AddSpecialFoldersToTree)
		void AddSpecialFoldersToTree(SpecialFoldersFlag flags)
		{
			if (flags.HasFlag(SpecialFoldersFlag.Personal))
			{
				directoryItems.Add(new DirectoryData(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Personal))));
			}
			if (flags.HasFlag(SpecialFoldersFlag.CommonDocuments))
			{
				directoryItems.Add(new DirectoryData(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments))));
			}
			if (flags.HasFlag(SpecialFoldersFlag.MyDocuments))
			{
				// [マイドキュメント]をルートに追加．
				directoryItems.Add(new DirectoryData(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))));
			}
			if (flags.HasFlag(SpecialFoldersFlag.MyPictures))
			{
				// [マイピクチャ]をルートに追加．
				directoryItems.Add(new DirectoryData(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures))));
			}
			if (flags.HasFlag(SpecialFoldersFlag.MyMusic))
			{
				// [マイミュージック]をルートに追加．
				directoryItems.Add(new DirectoryData(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic))));
			}
			if (flags.HasFlag(SpecialFoldersFlag.MyVideos))
			{
				// [マイビデオ]をルートに追加．
				directoryItems.Add(new DirectoryData(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos))));
			}
			if (flags.HasFlag(SpecialFoldersFlag.Network))
			{
				// [ネットワーク]をルートに追加．
				directoryItems.Add(new DirectoryData(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.NetworkShortcuts))));
			}
		}
		#endregion

		#endregion

		#region *SelectedPathの子を展開(ExtendAndSelectPath)
		/// <summary>
		/// 選択されているディレクトリに子ディレクトリを作成します．
		/// </summary>
		private void ExtendAndSelectPath()
		{
			if (!string.IsNullOrEmpty(this.SelectedPath))
			{
				ExtendAndSelect(directoryItems, treeView1, this.SelectedPath);
			}
		}
		#endregion

		// 04/18/2011 by aldentea : gdgdだけどとりあえずできた．
		#region *指定したパスの項目を選択する(ExpandAndSelect)
		void ExtendAndSelect(IList<DirectoryData> childDirectoriesData, ItemsControl parentItem, string path)
		{
			int n = GetChildItem(childDirectoriesData, parentItem, path);
			if (n < 0)
			{
				// とりあえずおとなしく終わらせることにする．
				return;
			}
			if (childDirectoriesData[n].FullName == path)
			{
				ItemContainerGenerator generator = parentItem.ItemContainerGenerator;
				Action SelectItem = delegate
				{
					parentItem = (TreeViewItem)parentItem.ItemContainerGenerator.ContainerFromIndex(n);
					((TreeViewItem)parentItem).IsSelected = true;
				};
				if (generator.Status == GeneratorStatus.ContainersGenerated)
				{
					SelectItem();
				}
				else
				{
					generator.StatusChanged += (sender, e) => { IfGeneratorReady(generator, SelectItem); };
				}
			}
			else
			{
				ItemContainerGenerator generator = parentItem.ItemContainerGenerator;
				Action ExpandItem = delegate
				{
					// 次の階層を探す．
					parentItem = (TreeViewItem)parentItem.ItemContainerGenerator.ContainerFromIndex(n);
					((TreeViewItem)parentItem).IsExpanded = true;
					ExtendAndSelect(childDirectoriesData[n].Children, parentItem, path);	// recursing.
				};

				if (generator.Status == GeneratorStatus.ContainersGenerated)
				{
					ExpandItem();
				}
				else
				{
					generator.StatusChanged += (sender, e) => { IfGeneratorReady(generator, ExpandItem); };
				}

			}
		}

		Action<ItemContainerGenerator, Action> IfGeneratorReady = new Action<ItemContainerGenerator, Action>(
			(generator, action) =>
			{
				if (generator.Status == GeneratorStatus.ContainersGenerated)
				{
					action.Invoke();
				}
			});

		int GetChildItem(IList<DirectoryData> childDirectoryData, ItemsControl parent, string path)
		{
			int maxLength = 0;
			int n = -1;
			for (int i = 0; i < childDirectoryData.Count; i++)
			{
				string directory_name = childDirectoryData[i].FullName;
				if (directory_name.Length > maxLength && path.StartsWith(directory_name))
				{
					n = i;
					maxLength = directory_name.Length;
				}
			}
			return n;
		}
		#endregion

		#region *SelectedItemが変更したとき(treeView1_SelectedItemChanged)
		private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			// ※どうやって選択されたパスを展開していくか？
			if (treeView1.SelectedItem is DirectoryData)
			{
				this.SelectedPath = ((DirectoryData)treeView1.SelectedItem).FullName;
			}
			else
			{
				this.SelectedPath = string.Empty;
			}
		}
		#endregion

		#region *ツリービューの項目が選択された時(treeViewItem_Selected)
		void treeViewItem_Selected(object sender, RoutedEventArgs e)
		{
			var item = e.Source as TreeViewItem;
			if (item != null)
			{
				var data = item.Header as DirectoryData;
				if (data != null)
				{
					this.SelectedPath = data.FullName;
					//e.Handled = true;

				}
			}
		}
		#endregion


		#region ディレクトリ監視関連

		Dictionary<DirectoryData, System.IO.FileSystemWatcher> watchers = new Dictionary<DirectoryData, FileSystemWatcher>();

		static Stopwatch stopwatch = new Stopwatch();

		// 07/09/2014 by aldentea : watcher生成時の例外処理を追加．
		#region *ツリービューの項目が開かれた時(treeViewItem_Expanded)
		void treeViewItem_Expanded(object sender, RoutedEventArgs e)
		{
			stopwatch.Start();
			Debug.WriteLine("treeViewItem_Expanded start.");

			var item = e.Source as TreeViewItem;
			if (item != null)
			{
				item.BringIntoView();

				// 監視を開始する
				DirectoryData dData = (DirectoryData)item.Header;
				string path = dData.FullName;
				Debug.WriteLine(path);
				FileSystemWatcher watcher;
				try
				{
					watcher = new System.IO.FileSystemWatcher(path);
				}
				catch (Exception ex)
				{
					Debug.WriteLine("treeViewItem_Expanded abort.");
					// CDドライブにディスクが入っていないときなど．
					return;
				}
				watcher.NotifyFilter = NotifyFilters.DirectoryName;
				// とりあえずCreatedイベントとRemoveイベントだけ処理する．
				watcher.Created += new FileSystemEventHandler(watcher_Created);
				watcher.Deleted += new FileSystemEventHandler(watcher_Deleted);
				watcher.Renamed += new RenamedEventHandler(watcher_Renamed);
				//watcher.SynchronizingObject = this;
				watcher.EnableRaisingEvents = true;
				watchers.Add(dData, watcher);


				//e.Handled = true;
			}
			stopwatch.Stop();
			Debug.WriteLine(string.Format("treeViewItem_Expanded end. {0}ms.", stopwatch.ElapsedMilliseconds));
			stopwatch.Reset();
		}
		#endregion

		#region *ツリービューの項目がたたまれた時(treeViewItem_Collapsed)
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void treeViewItem_Collapsed(object sender, RoutedEventArgs e)
		{
			var item = e.Source as TreeViewItem;
			if (item != null)
			{
				// 監視を終了する
				var watcher = watchers[(DirectoryData)item.Header];
				watcher.EnableRaisingEvents = false;
				watcher.Dispose();
				watchers.Remove((DirectoryData)item.Header);
				watcher = null;

				e.Handled = true;
			}

		}
		#endregion

		#region *ディレクトリが新規作成された時(watcher_Created)
		void watcher_Created(object sender, FileSystemEventArgs e)
		{
			string path = ((FileSystemWatcher)sender).Path;
			foreach (var dData in watchers.Keys)
			{
				if (dData.FullName == path)
				{
					Dispatcher.Invoke(new System.Action<DirectoryData, string>(AddChildTo), new object[] { dData, e.FullPath });
				}
			}

		}
		#endregion

		#region *ディレクトリが削除された時(watcher_Deleted)
		void watcher_Deleted(object sender, FileSystemEventArgs e)
		{
			string path = ((FileSystemWatcher)sender).Path;
			foreach (var dData in watchers.Keys)
			{
				if (dData.FullName == path)
				{
					Dispatcher.Invoke(new System.Action<DirectoryData, string>(RemoveChildFrom), new object[] { dData, e.FullPath });
				}
			}
		}
		#endregion

		#region *ディレクトリの名前が変更された時(watcher_Renamed)
		void watcher_Renamed(object sender, RenamedEventArgs e)
		{
			string path = ((FileSystemWatcher)sender).Path;
			foreach (var dData in watchers.Keys)
			{
				if (dData.FullName == path)
				{
					Dispatcher.Invoke(new System.Action<DirectoryData, string, string>(UpdateChild), new object[] { dData, e.OldFullPath, e.FullPath });
				}
			}
		}
		#endregion

		void AddChildTo(DirectoryData parent, string path)
		{
			parent.AddChild(path);
		}

		void RemoveChildFrom(DirectoryData parent, string path)
		{
			parent.RemoveChild(path);
		}

		void UpdateChild(DirectoryData parent, string oldPath, string newPath)
		{
			parent.Update(oldPath, newPath);
		}

		#endregion


	}

	#region SpecialFoldersFlag列挙体
	[Flags]
	public enum SpecialFoldersFlag
	{
		None = 0,
		/// <summary>
		/// 何だよこれ，[マイドキュメント]と一緒？
		/// </summary>
		Personal = 0x01,
		CommonDocuments = 0x02,
		MyDocuments = 0x04,
		MyPictures = 0x08,
		MyMusic = 0x10,
		MyVideos = 0x20,
		/// <summary>
		/// [ネットワーク]を指すんだけど，あんまり上手く機能していない...
		/// </summary>
		Network = 0x1000
	}
	#endregion


}
