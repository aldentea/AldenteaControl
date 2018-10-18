using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Aldentea.Wpf.Controls
{

	#region DirectoryDataクラス
	internal class DirectoryData : INotifyPropertyChanged
	{

		DirectoryInfo _dInfo;

		#region *FullNameプロパティ
		/// <summary>
		/// フルパスを取得します．
		/// </summary>
		public string FullName
		{
			get
			{
				return _dInfo.FullName;
			}
		}
		#endregion

		#region *Nameプロパティ
		/// <summary>
		/// 表示するディレクトリ名を取得します．
		/// </summary>
		public string Name
		{
			get
			{
				return _dInfo.Name;
			}
		}
		#endregion

		#region *Iconプロパティ
		public BitmapSource Icon { get; set; }
		#endregion

		#region *Childrenプロパティ
		/// <summary>
		/// 子ディレクトリのDirectoryDataのコレクションを取得します．
		/// </summary>
		public ObservableCollection<DirectoryData> Children
		{
			get
			{
				if (this.hasDummy)
				{
					_children.Clear();
					//System.Diagnostics.Trace.WriteLine(string.Format("Detect subdirectories of {0}.", this._dInfo.FullName));
					foreach (DirectoryInfo directoryInfo in _dInfo.GetDirectories())
					{
						//_childrenを取得．
						try
						{
							_children.Add(new DirectoryData(directoryInfo));
						}
						catch (UnauthorizedAccessException)
						{
							// アクセスできないディレクトリはスキップする．
						}
					}
					this.hasDummy = false;
				}
				return _children;
			}

		}
		#endregion

		ObservableCollection<DirectoryData> _children = new ObservableCollection<DirectoryData>();
		bool hasDummy = false;

		#region *コンストラクタ(DirectoryData)
		public DirectoryData(DriveInfo driveInfo)
		{
			this._dInfo = driveInfo.RootDirectory;
			this.Icon = GetFolderIcon(driveInfo.Name);

			if (
				driveInfo.IsReady && driveInfo.RootDirectory.GetDirectories().Length > 0)
			{
				_children.Add(new DirectoryData());
				this.hasDummy = true;
			}

		}

		public DirectoryData(DirectoryInfo directoryInfo)
		{
			this._dInfo = directoryInfo;
			this.Icon = _defaultIcon;

			if (_dInfo.GetDirectories().Length > 0)
			{
				_children.Add(new DirectoryData());
				this.hasDummy = true;
			}
		}

		// ダミーデータ用のインスタンスを生成．
		private DirectoryData()
		{
		}
		#endregion

		#region *子要素を追加(AddChild)
		public void AddChild(string path)
		{
			Children.Add(new DirectoryData(new DirectoryInfo(path)));
		}
		#endregion

		#region *子要素を削除(RemoveChild)
		public void RemoveChild(string path)
		{
			DirectoryData removingChild = FindChild(path);
			if (removingChild != null)
			{
				Children.Remove(removingChild);
			}
		}
		#endregion

		#region *子要素のパスを変更(Update)
		public void Update(string oldPath, string newPath)
		{
			DirectoryData updatingChild = FindChild(oldPath);
			if (updatingChild != null)
			{
				updatingChild._dInfo = new DirectoryInfo(newPath);
				OnPropertyChanged("FullName");
				OnPropertyChanged("Name");
			}
		}
		#endregion

		DirectoryData FindChild(string path)
		{
			foreach (var child in Children)
			{
				if (child.FullName == path)
				{
					return child;
				}
			}
			return null;
		}
		
		
		readonly static BitmapSource _defaultIcon
			= GetFolderIcon(Environment.GetFolderPath(Environment.SpecialFolder.System));

		#region *[static]フォルダ用アイコンを取得(GetFolderIcon)
		static BitmapSource GetFolderIcon(string path)
		{
			BitmapSource icon = null;

			SHFILEINFO psf = new SHFILEINFO();
			SHGetFileInfo(path, 0, ref psf, Marshal.SizeOf(psf), SHGFI_ICON | SHGFI_SMALLICON);
			if (psf.hIcon != IntPtr.Zero)
			{
				icon = Imaging.CreateBitmapSourceFromHIcon(psf.hIcon, Int32Rect.Empty, null);
			}
			return icon;
		}

		struct SHFILEINFO
		{
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		}

		const uint SHGFI_ICON = 0x100;
		const uint SHGFI_LARGEICON = 0x0;
		const uint SHGFI_SMALLICON = 0x1;

		[DllImport("shell32.dll")]
		static extern int SHGetFileInfo(
					string pszPath,
					uint dwFileAttributes,
					ref SHFILEINFO psfi,
					int cbSizeFileInfo,
					uint uFlags);

		#endregion



		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		protected void OnPropertyChanged(string propertyName)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

	}
	#endregion

}
