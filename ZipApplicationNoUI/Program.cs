﻿using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Forms;
using ZipApplicationNoUI.Properties;
using System.Diagnostics;

namespace ZipApplicationNoUI
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //InitializeComponent();      //ファイアログボックスで前回選択したディレクトリを記憶するメソッド
            SelectFileDialog();         //Zip圧縮するファイル選択ダイアログメソッド
            SelectFolderDialog();       //生成したZipファイルの保存先選択ダイアログメソッド
            Zip();                      //Zip化するメソッド
            CreateText();
            //Console.ReadLine();         //処理が終了してもコンソール画面を残す
        }

        //
        static string directoryPath;
        static List<string> filePath = new List<string>();
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
        private static void SelectFileDialog()
        {
            //var fileContent = string.Empty;
            //var filePath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            {
                openFileDialog.InitialDirectory = directoryPath;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "ZIP化したいファイルを選択し、「OK」を押してください(複数可)。";
                openFileDialog.Multiselect = true;

                //ファイルダイアログのOKボタンが押されたときの処理
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine("選択ファイルパス：");
                    foreach (string i in openFileDialog.FileNames)
                    {
                        filePath.Add(i);
                        Console.WriteLine(i);
                    }

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

            zip.BeginUpdate();
                                                                //自動生成したパスワードをZipファイルに設定

            foreach (var i in filePath)
            {
                Add(zip, i);
            }
            zip.Password = password;
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
            //第1引数：ファイルパス
            //第2引数：追記するテキスト 
            var zipFileName = Path.GetFileName(textFileName);
            File.WriteAllText(textFileName + ".txt", "添付ファイル名：" + zipFileName + "\r\n" + "パスワード：" + password);
            Process.Start(textFileName + ".txt");
        }

        //ファイル名とパスワードが入力されたメモ帳を開くメソッド
    }
}
