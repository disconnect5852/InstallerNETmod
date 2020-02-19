using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Zip;
using System.Net.Http;
using System.Windows.Threading;
using System.Windows;
using System.Threading;

namespace Installer
{
    internal class ChoiceBean
    {
        internal ChoiceBean(string name, string url) {
            this.name = name;
            this.url = url;
        }

        internal string name {get; set; }
        internal string url {get; set; }

        public override string ToString()
        {
            return name;
        }
    }

    internal class InstLogic // converted from Java version,all secret are replaced with random numbers, characters, obfuscated confusing structure, to make reverse engineering harder. Also ran through a real obfuscator
    {
        internal string email { get; set; }
        internal string pass { get; set; }
        internal string dest { get; set; }
        private static readonly byte[] replace1 = (byte[])(Array)new sbyte[] { 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98 }; //chars where second watermark should be placed
        internal System.Collections.IList choicess { get; set; }
        private static readonly HttpClient client=new HttpClient();
        public delegate void UpdateDownMessage(String message);
        private static readonly sbyte[] salt = new sbyte[] { 18, -12, -45, 88, 123, 1, 127, 12, 45, 5, 89, 61, -24, -65, 34, 31 };
        private const String DB2 = FOU + "index.php?reginstall=";
        private const bool DEBUG = false;
        private static readonly ParallelOptions para = new ParallelOptions { MaxDegreeOfParallelism = 12 };
        private const String FOU="http://users.atw.hu/hurqaleee/";
        private const String DB = FOU + "beta/l6r846.ztp";
        private const String OPTION = FOU + "beta/choice.cqd";
        private static string mainhashlink;
        private static string decpass;
        private static readonly byte[] replace= (byte[])(Array) new sbyte[]{97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97,97}; //chars where watermark should be placed
        private static readonly byte[] iv = new byte[]
        { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };
        //private static readonly byte[] ov = new byte[] { 0x2A, 0x6A, 0x0A, 0xE5, 0x3D, 0x73, 0xE1, 0xA8, 0x37, 0xD4, 0xBB, 0x6C, 0x6A, 0xAA, 0x12, 0x97 };
        private static readonly sbyte[] vrtML = new sbyte[] { 170, 3, 67, 53, 27, 2, 12, 22, 46, 32, 56, 37, 100, 4, 27, 16, 62, 101, 38, 29, 17, 31, 108, 60, 101, 32, 5, 38, 18, 16, 97, 37, 18, 22, 55, 101, 36, 38, 53, 23, 59, 2, 96, 53, 53, 54, 35, 18, 37, 62, 44, 17, 31, 36, 101, 56, 51, 102, 33, 21, 30, 74, 46, 109, 54, 21, 39, 61, 27, 7, 38, 28, 89, 18, 37, 24, 60, 12, 120, 56, 13, 53, 58, 6, 44, 18, 150, 37, 21, 45, 37, 25, 7, 21, 35, 52, 53, 56, 53, 38 };
        private static byte[] uid1;
        private static byte[] uid2;
        //public static string destination {get; set;}
        public InstLogic() {
            checkChoices();
            //ServicePointManager.DefaultConnectionLimit = 1000;
            //Task.Factory.StartNew(() => SecondFoo())
            ServicePointManager.DefaultConnectionLimit = 20;
        }

        private void checkChoices() {
            try {
                TextReader opts = new StreamReader(ConvertToStream(OPTION));
			    string rawline=opts.ReadLine();

                Application.Current.Dispatcher.Invoke(() => { MainWindow.thiswindow.choice.Items.RemoveAt(0); });
                while (rawline !=null) {
				    String[] line= rawline.Split(';');
                    if (DEBUG) Console.WriteLine(rawline);
                    Application.Current.Dispatcher.Invoke(() => { Installer.MainWindow.thiswindow.choice.Items.Add(new ChoiceBean(line[0], line[1])); });
                    rawline = opts.ReadLine();                
			    }
            }
            catch (Exception e)
            {
                if (DEBUG) Console.WriteLine(e.ToString());
                Application.Current.Dispatcher.Invoke(() => { Installer.MainWindow.thiswindow.choice.Items.Clear(); });
                Application.Current.Dispatcher.Invoke(() => { Installer.MainWindow.thiswindow.choice.Items.Add("Problem reaching the database, either server, or connection problem"); });
                Application.Current.Dispatcher.Invoke(() => { Installer.MainWindow.thiswindow.choice.Items.Add("Sorry. Check your internet connection, or try again later"); });
                }
	    }

        public bool isLegit(string email, string inpass, string destination)
        {

            var rawstream = ConvertToStream(DB);
            var reader = new BinaryReader(new InflaterInputStream(DecryptStream(rawstream))/*, System.Text.Encoding.BigEndianUnicode*/);
            reader.ReadByte();
            mainhashlink = reader.ReadString();
            reader.ReadByte();
            decpass = reader.ReadString();
            if (DEBUG) Console.WriteLine("hesslink: " + mainhashlink + " decpass: " + decpass);
            //soronként a fájl végéig˘˘
            int sorz = 0;
            try {
                while (true)
                {
                    sorz++;
                    reader.ReadByte();
                    string user = reader.ReadString();
                    reader.ReadByte();
                    string pass = reader.ReadString();
                    reader.ReadByte();
                    string id1 = reader.ReadString();
                    reader.ReadByte();
                    string id2 = reader.ReadString();
                    bool notify = reader.ReadBoolean();
                    //var str = new Read(rawstream);
                    //ArrayToString(reader.ReadBytes(32));
                    if (user.Equals(email, StringComparison.InvariantCultureIgnoreCase) && pass.Equals(inpass, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (DEBUG) Console.WriteLine("user: " + user + " pass: " + pass + " id1: " + id1 + " id2: " + id2 + " notify: " + notify);
                        if (id1.Equals("wrz", StringComparison.InvariantCultureIgnoreCase)) {
                            Directory.Delete(Path.Combine(destination,"Assets","Dev","Addon"), true);
                            Environment.Exit(0);
                            return false;
                        }
                        uid1 = Encoding.ASCII.GetBytes(id1);
                        uid2 = Encoding.ASCII.GetBytes(id2);
                        if (DEBUG) Console.WriteLine("uid1: " + ArrayToString(uid1) + " uid2: " + ArrayToString(uid2));
                        try
                        {
                            WebRequest.Create(DB2 + id2).GetResponse();
                           if (DEBUG) Console.WriteLine(DB2 + id2);
                        }
                        catch (Exception e)
                        {
                            if (DEBUG)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        }
                        return true;
                    }
                }
            }
            catch (EndOfStreamException e)
            {
                if (DEBUG) {
                    Console.WriteLine("sorzok: " + sorz);
                    Console.WriteLine(e.ToString());
                }
                reader.Close();
                Application.Current.Dispatcher.Invoke(() => { Installer.MainWindow.thiswindow.gemail.Background= System.Windows.Media.Brushes.Red; Installer.MainWindow.thiswindow.gepassword.Background = System.Windows.Media.Brushes.Red; System.Windows.Forms.MessageBox.Show("Email or password is wrong", "Problem?", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop); });
                return false;
            }
        }

        private static string downloadZip(string down, HttpClient client)
        {
            string tempfile = Path.GetTempFileName();
            /*var fs = new FileStream(tempfile, FileMode.Open, FileAccess.Write);
            ConvertToStream(down).CopyTo(fs);
            fs.Close();*/

                if (DEBUG) Console.WriteLine("starting download: " + down);
                var fileStream = File.Create(tempfile);
                //byte[] ret= client.GetByteArrayAsync(down).Result;
                Stream x= client.GetStreamAsync(down).Result;
                //x.Seek(0, SeekOrigin.Begin);
                x.CopyTo(fileStream);
                fileStream.Close();
                /*wc.DownloadFile(down, tempfile);
                wc.Dispose();*/
            if (DEBUG) Console.WriteLine("finished download: " + down);
            return tempfile;
        }

        internal void LetsDoThis()
        {
            LetsDoThis(email,pass,dest);
        }
        internal void LetsDoThis(string email, string psw, string destination)
        {
            if (Directory.Exists(destination) && isLegit(email, psw, destination))
            {
                List<string[]> filelist = checkFiles(mainhashlink, destination);
                //Console.WriteLine("Maincheck ");
                //ChoiceBean[] cb;
                //Application.Current.Dispatcher.Invoke(() => { MainWindow.thiswindow.choice.SelectedItems.CopyTo(cb, 0); });
                foreach ( ChoiceBean emt in choicess)
                {
                    filelist.AddRange(checkFiles(emt.url, destination));
                    if (DEBUG) Console.WriteLine("check: "+ emt.url);
                }
                if (DEBUG) Console.WriteLine("fileset count: " + filelist.Count);
                Console.WriteLine("downloading ");
                int counter=0;
                Application.Current.Dispatcher.Invoke(() => { Installer.MainWindow.thiswindow.downbutton.Content = "Downloading " + filelist.Count + " files..."; });
                var client = new HttpClient();
                Parallel.ForEach(filelist, para, file =>
                {
                    //Installer.MainWindow.thiswindow.downbutton.Content = "Downloaded " + counter +" of " + fileset.Count + " files";
                    //if (DEBUG) System.Console.WriteLine("file: " + file[0]);

                    string downfileeee = downloadZip(file[0],client);
                    //UnzipFromArray(downloadZip(file[0], client), destination);
                    UnzipFromFile(downfileeee, destination, file[1]);
                    counter++;
                    Application.Current.Dispatcher.Invoke(() =>Installer.MainWindow.thiswindow.downbutton.Content = "Downloaded " + counter + " of " + filelist.Count + " files");
                });
                Console.WriteLine("enddown ");
                Application.Current.Dispatcher.Invoke(() => { Installer.MainWindow.thiswindow.downbutton.Content = "Download completed"; });
            }
        }

        private List<string[]> checkFiles(string hashfileurl, string destination)
        {
            if (DEBUG) Console.WriteLine("HashfileURL: "+hashfileurl);
            List<string[]> returnlist= new List<string[]>();
            string rootofhash=hashfileurl.Substring(0, hashfileurl.LastIndexOf('/')+1);
            try
            {
                StreamReader hashread = new StreamReader(ConvertToStream(hashfileurl), true);
                string hashline = hashread.ReadLine();
                MD5 md5 = MD5.Create();
                int counter=1;
                while (hashline != null)
                {
                    //if (DEBUG) System.Console.WriteLine("Hashline: " + hashline);
                    string[] line = hashline.Split(new char[] { ' ' }, 2);
                    hashline = hashread.ReadLine();
                    string relUrl = line[1].Substring(1);
                    string checkfile =Path.Combine(destination, relUrl);
                    //System.Console.WriteLine("checkfile: " + checkfile);
                    if (File.Exists(checkfile))
                    {
                        byte[] filedata = File.ReadAllBytes(checkfile);
                        if (checkfile.EndsWith("geopcdx", true, null))
                        {
                            filedata = ReplaceBytes(filedata, uid1, replace, uid2, replace1);
                        }
                        counter++;
                        Application.Current.Dispatcher.Invoke(() => { Installer.MainWindow.thiswindow.downbutton.Content = "Checked " + counter + " installed files "; });
                        if (line[0].Equals(byteArrayToHex(md5.ComputeHash(filedata)), StringComparison.InvariantCultureIgnoreCase))
                            continue;
                    }
                    if (true) System.Console.WriteLine("Adding to downloadlist: " + relUrl);
                    returnlist.Add(new string[2] { rootofhash + relUrl.Replace('\\', '/').Replace(" ", "%20") + ".zip", relUrl });
                }
            } catch (Exception ex)
                {
                    Console.WriteLine("exception: "+ex.StackTrace);
                }
            return returnlist;
        }

        private void UnzipFromFile(string zipPath, string destination, string relPath) {

            try {
            ZipFile zipfile = new ZipFile(zipPath);
            zipfile.Password = decpass;
            foreach (ZipEntry zipEntry in zipfile)
                {
                    String entryFileName = zipEntry.Name;
                    string fullZipToPath = Path.Combine(destination, relPath);
                    //if (DEBUG) Console.WriteLine(fullZipToPath);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    string fileName = Path.GetFileName(fullZipToPath);
                    if (fileName.Length == 0)
                    {
                        continue;
                    }

                    byte[] buffer = new byte[zipEntry.Size];
                    StreamUtils.ReadFully(zipfile.GetInputStream(zipEntry), buffer);
                    if (fileName.EndsWith("geopcdx", true, null))
                    {
                        /*byte[] markedone1 = ReplaceBytes(buffer, replace, uid1);
                        buffer = ReplaceBytes(markedone1, replace1, uid2);
                        if (buffer==null) buffer = markedone1;*/
                        buffer = ReplaceBytes(buffer, replace, uid1, replace1, uid2);
                    }
                    File.WriteAllBytes(fullZipToPath, buffer);
                    zipfile.Close();
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }


        private static Stream ConvertToStream(string fileUrl)
        {
            try
            {
                if (DEBUG) Console.WriteLine("ConvertToStream: "+fileUrl);
                return client.GetStreamAsync(fileUrl).Result;
            }
            catch (Exception ex) {
                if (DEBUG) Console.WriteLine("down error: " + fileUrl);
                throw ex;
            }
        }

        private static Stream DecryptStream(Stream cipheredStream/*, sbyte[] Key, sbyte[] IV*/)
        {
            sbyte cvwm = (sbyte)(84);
            sbyte[] stml = new sbyte[vrtML.Length];
            for (int i = 0; i < vrtML.Length; i++)
            {
                stml[i] = (sbyte)(vrtML[i] ^ cvwm);
            }
            //System.Console.WriteLine("hsghsgsdgsddsghdsgsdgsdgsd" + stml.Length);
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes((byte[])(Array)stml, (byte[])(Array)salt, 7);
            byte[] key = rfc2898.GetBytes(16);
            Aes rijAlg = AesManaged.Create();
            rijAlg.Mode = CipherMode.CBC;
            rijAlg.Padding = PaddingMode.PKCS7;
            rijAlg.KeySize = key.Length*8;
            rijAlg.BlockSize = 128;
            rijAlg.Key = key;
            rijAlg.IV = (byte[])(Array) iv;

                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                return new CryptoStream(cipheredStream, decryptor, CryptoStreamMode.Read);
        }

        private static byte[] ReplaceBytes(byte[] input, byte[] search, byte[] replacebyte, byte[] search2, byte[] replacebyte2)
        {
            int index = FindBytes(input, search);
            int index1 = FindBytes(input, search2);
            if (index > 0)
            {
                for (int j = 0; j < replacebyte.Length; j++)
                {
                    input[index + j] = replacebyte[j];
                }
                if (index1 > 0)
                {
                    for (int j = 0; j < replacebyte2.Length; j++)
                    {
                        input[index1 + j] = replacebyte2[j];
                    }
                }
                return input;
            }
            //Console.WriteLine("problem at vízjel");
            return null;
        }

        private static int FindBytes(byte[] src, byte[] find)
        {
            int index = -1;
            int matchIndex = 0;
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] == find[matchIndex])
                {
                    if (matchIndex == (find.Length - 1))
                    {
                        index = i - matchIndex;
                        break;
                    }
                    matchIndex++;
                }
                else if (src[i] == find[0])
                {
                    matchIndex = 1;
                }
                else
                {
                    matchIndex = 0;
                }

            }
            return index;
        }


        private static String byteArrayToHex(byte[] a) {
		    StringBuilder sb = new StringBuilder();
		    foreach(byte b in a)
                sb.AppendFormat("{0:x2}", b/* & 0xff*/);

		    return sb.ToString();
	    }

        static string ArrayToString(byte[] lol) {
            StringBuilder sb = new StringBuilder();
            sb.Append("0 ikszbájtok: ");
            foreach (var b in lol)
            {
                sb.Append(String.Format("{0:x2} ", b));
            }
            return sb.ToString();
        }
        static string ArrayToString(char[] lol)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("0 ikszchárok: ");
            foreach (var b in lol)
            {
                sb.Append(b);
            }
            return sb.ToString();
        }
    }
}
