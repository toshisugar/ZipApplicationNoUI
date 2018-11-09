using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Forms;
using ZipApplicationNoUI.Properties;

namespace ZipApplicationNoUI
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            InitializeComponent();
            SelectFileDialog();
            
        }

        //
        static string directoryPath;
        static string files;

        //前回見ていたパスを記憶するメソッド
        private static void InitializeComponent()
        {
            //カッコ内(デフォルトパス)が空白の場合trueを返す OR デフォルトパスが存在すればtrue
            if (string.IsNullOrWhiteSpace(Settings.Default.SourcePath) || !Directory.Exists(Settings.Default.SourcePath))
            {
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                Settings.Default.SourcePath = Path.Combine(desktopPath/*, "ZipFileApplication"*/);
                Settings.Default.Save();
            }

            directoryPath = Settings.Default.SourcePath;
        }

        //ファイル選択ダイアログ生成メソッド
        private static void SelectFileDialog()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            {
                openFileDialog.InitialDirectory = directoryPath;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Zip化したいファイルを選択してください(複数可)。";
                openFileDialog.Multiselect = true;


                //ファイルダイアログのOKボタンが押されたときの処理
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string files in openFileDialog.FileNames)
                    {
                    Console.WriteLine(files);
                    }
                    Console.ReadLine();
                //    //Get the path of specified file
                //    filePath = directoryPath = openFileDialog.InitialDirectory;
                //    Settings.Default.SourcePath = directoryPath = openFileDialog.FileName;
                //    Settings.Default.Save();

                //    //Read the contents of the file into a stream
                //    var fileStream = openFileDialog.OpenFile();

                //    using (StreamReader reader = new StreamReader(fileStream))
                //    {
                //        fileContent = reader.ReadToEnd();
                //    }
                //}
                //else
                //{
                //    //★★★キャンセルや×ボタンが押されたときの処理★★★
                }
            }
        }

        //Zip圧縮メソッド
        private static void zip()
        {
            string password = System.Web.Security.Membership.GeneratePassword(8, 0);                //パスワード自動生成

            var zip = ZipFile.Create(files);
            zip.BeginUpdate();
            zip.Password = password;                                                                //自動生成したパスワードをZipファイルに設定
            
            foreach (var filePath in "★★★ファイルパスが格納されているリスト名を入力してね！★★★")
            {
                Add(zip, @"filePath");
            }
            zip.CommitUpdate();
        }
        
        //Zip圧縮の際にファイルパスからファイル名を抽出するメソッド
        private static void Add(ZipFile zip, string path)
        {
            var name = Path.GetFileName(path);          //path(指定したパス文字列)のファイル名と拡張子を、nameに代入
            zip.Add(path, name);                        //zipメソッドで作成された圧縮後のファイル名に、パスとファイル名をAddする
        }






    }
}
