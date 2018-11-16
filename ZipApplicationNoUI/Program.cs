using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ZipApplicationNoUI.Properties;

namespace ZipApplicationNoUI
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //InitializeComponent();      //ファイアログボックスで前回選択したディレクトリを記憶するメソッド

            if (!SelectFileDialog()) return;

            //メイン処理に戻ったとき(ファイルが選択されたとき、選択せずに閉じられたとき)、filePathに値が入っていなければ、アプリを終了する。
            if (filePath.Count == 0)
            {
                MessageBox.Show("アプリケーションを終了します。");
                return;
            }

            SelectFolderDialog();         //生成したZipファイルの保存先選択ダイアログメソッド

            //メイン処理に戻ったとき(フォルダが選択されたとき、選択せずに閉じられたとき)、saveFilePathに値が入っていなければ、アプリを終了する。
            if (saveFilePath == null)
            {
                MessageBox.Show("アプリケーションを終了します。");
                return;
            }

            Zip();                       //Zip化するメソッド

            CreateText();

            //Console.ReadLine();         //コンソールを出している場合、処理が終了してもコンソール画面を残すメソッド
        }

        static string directoryPath;
        static List<string> filePath;
        static string saveFilePath;
        static string textFileName;
        static string password;

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
        private static bool SelectFileDialog()
        {
            var retry = true;
            while (retry)
            {
                retry = false;

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = directoryPath;
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.Title = "ZIP化したいファイルを選択し、「OK」を押してください(複数可)。";
                    openFileDialog.Multiselect = true;

                    //ファイルダイアログのOKボタンが押されなかったときの処理
                    if (openFileDialog.ShowDialog() != DialogResult.OK) return false;

                    Console.WriteLine("選択ファイルパス：");

                    filePath = new List<string>();
                    foreach (string i in openFileDialog.FileNames)
                    {
                        filePath.Add(i);
                        Console.WriteLine(i);
                    }

                    var zeroFiles = filePath.Select(t => new FileInfo(t)).Where(t => t.Length == 0).ToList();
                    if (zeroFiles.Count != 0)
                    {
                        MessageBox.Show(string.Join("\r\n", zeroFiles.Select(t => "\"" + t.FullName + "\"")) + "\r\n上記のファイルは０KBです。全ファイルを選択しなおしてください。");
                        retry = true;
                    }
                }
            }

            return true;
        }

        //ファイル名入力及び保存先フォルダ選択ダイアログ生成メソッド
        private static void SelectFolderDialog()
        {
            string sourceDirectory = directoryPath;                        //圧縮するファイルが入っているフォルダのパスをsouceDirectoryに格納
            var saveFileDialog = new SaveFileDialog            //圧縮したファイルを保存するフォルダを指定するダイアログボックスの生成
            {
                Title = "ZIPファイル名を入力して、保存先を指定し、「OK」を押してください。",
                InitialDirectory = Path.GetDirectoryName(sourceDirectory),
                Filter = "ZIPファイル(*.zip)|*.zip"
            };

            //if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                saveFilePath = saveFileDialog.FileName;
                Console.WriteLine("\r\nZIPファイル名：\r\n" + Path.GetFileName(saveFileDialog.FileName) + "\r\n");
                Console.WriteLine("保存先：\r\n" + saveFileDialog.FileName);
                textFileName = saveFileDialog.FileName;
            }
        }

        //Zip圧縮メソッド
        private static void Zip()
        {
            password = System.Web.Security.Membership.GeneratePassword(8, 0);                //パスワード自動生成

            var zip = ZipFile.Create(saveFilePath);
            zip.Password = password;                                //自動生成したパスワードをZipファイルに設定
            zip.BeginUpdate();

            foreach (var i in filePath)
            {
                Add(zip, i);
            }
            zip.CommitUpdate();

            Console.WriteLine("パスワード：\r\n" + password);
        }

        //Zip圧縮の際にファイルパスからファイル名を抽出するメソッド
        private static void Add(ZipFile zip, string path)
        {
            var name = Path.GetFileName(path);          //path(指定したパス文字列)のファイル名と拡張子を、nameに代入
            zip.Add(path, name);                        //zipメソッドで作成された圧縮後のファイル名に、パスとファイル名をAddする
        }

        //ファイル名とパスワードをメモ帳に入力するメソッド
        private static void CreateText()
        {
            var zipFileName = Path.GetFileName(textFileName);
            //第1引数：ファイルパス　第2引数：追記するテキスト
            File.WriteAllText(textFileName.Replace(".zip", "") + ".txt", "添付ファイル名：" + zipFileName + "\r\n" + "パスワード：" + password);
            Process.Start(textFileName.Replace(".zip", "") + ".txt");
        }
    }
}
