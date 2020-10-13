using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using UnityEngine;

namespace ImagineClass
{
    /// <summary>
    /// Zip压缩帮助类
    /// </summary>
    public class ZipHelper
    {
        /// <summary>
        /// 压缩文件夹 包括子文件夹
        /// </summary>
        /// <param name="filesPath"></param>
        /// <param name="zipFilePath"></param>
        /// <param name="compressionLevel"></param>
        public void CreateZipFile(string filesPath, string zipFilePath, int compressionLevel = 9)
        {
            if (!Directory.Exists(filesPath))
            {
                return;
            }
            if (Path.GetExtension(zipFilePath) != ".zip")
            {
                zipFilePath = zipFilePath + ".zip";
            }
            string[] filenames = Directory.GetFiles(filesPath, "*.*", SearchOption.AllDirectories);

            ZipOutputStream stream = new ZipOutputStream(File.Create(zipFilePath));
            stream.SetLevel(compressionLevel); // 压缩级别 0-9
            byte[] buffer = new byte[4096]; //缓冲区大小

            foreach (string file in filenames)
            {
                if (file != zipFilePath)
                {
                    ZipEntry entry = new ZipEntry(file.Replace(filesPath + "\\", ""));
                    entry.DateTime = DateTime.Now;
                    stream.PutNextEntry(entry);
                    using (FileStream fs = File.OpenRead(file))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = fs.Read(buffer, 0, buffer.Length);
                            stream.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
            }
            stream.Finish();
            stream.Close();
        }

        ///<summary>
        /// 清空指定的文件夹，但不删除文件夹
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteFolder(string dir)
        {
            System.Threading.Tasks.Parallel.ForEach(Directory.GetFileSystemEntries(dir), (d) =>
            {
                try
                {
                    if (File.Exists(d))
                    {
                        FileInfo fi = new FileInfo(d);
                        if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                            fi.Attributes = FileAttributes.Normal;
                        File.Delete(d);//直接删除其中的文件
                    }
                    else
                    {
                        DirectoryInfo d1 = new DirectoryInfo(d);
                        if (d1.GetFiles().Length != 0)
                        {
                            DeleteFolder(d1.FullName);////递归删除子文件夹
                        }
                        Directory.Delete(d, true);
                    }
                }
                catch
                {

                }
            });
            if (Directory.GetFileSystemEntries(dir).Length > 0)
            {
                DeleteFolder(dir);
            }
        }
        public static void CopyDirectory(string srcPath, string destPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)     //判断是否文件夹
                    {
                        if (!Directory.Exists(destPath + "\\" + i.Name))
                        {
                            Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                        }
                        CopyDirectory(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                    }
                    else
                    {
                        File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 功能：解压zip格式的文件。
        /// </summary>
        /// <param name="zipFilePath">压缩文件路径</param>
        /// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>
        public static async void UnZip(string zipFilePath, string unZipDir = "", Action<string> OnZipping = null, Action OnComplete = null)
        {
            if (zipFilePath == string.Empty)
            {
                throw new Exception("压缩文件不能为空！");
            }
            if (!File.Exists(zipFilePath))
            {
                throw new FileNotFoundException("压缩文件不存在！");
            }
            //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹
            if (unZipDir == string.Empty)
                unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
            if (!unZipDir.EndsWith("/"))
                unZipDir += "/";
            if (!Directory.Exists(unZipDir))
                Directory.CreateDirectory(unZipDir);

            using (var s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        Directory.CreateDirectory(unZipDir + directoryName);
                    }
                    if (directoryName != null && !directoryName.EndsWith("/"))
                    {
                    }
                    if (fileName != String.Empty)
                    {
                        var zippingName = theEntry.Name;
                        await new WaitForEndOfFrame();
                        OnZipping?.Invoke(zippingName);
                        using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                        {

                            int size;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            streamWriter.Close();
                        }
                    }
                }
                s.Close();
            }
            OnComplete?.Invoke();
        }

        /// <summary>
        /// 根据压缩包路径读取此压缩包内文件个数
        /// </summary>
        /// <param name="zipFilePath"></param>
        /// <returns></returns>
        public static int FileInZipCount(string zipFilePath)
        {
            int iNew;
            ZipEntry zipEntry_ = null;
            FileStream fsFile_ = null;
            ZipFile zipFile_ = null;
            try
            {
                fsFile_ = new FileStream(zipFilePath, FileMode.OpenOrCreate);
                zipFile_ = new ICSharpCode.SharpZipLib.Zip.ZipFile(fsFile_);
                long l_New = zipFile_.Count;
                iNew = System.Convert.ToInt32(l_New);
                return iNew;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message + "/" + ex.StackTrace);
                return 0;
            }
            finally
            {
                if (zipFile_ != null)
                {
                    if (zipFile_.IsUpdating)
                        zipFile_.CommitUpdate();
                    zipFile_.Close();
                }
                if (fsFile_ != null)
                    fsFile_.Close();
                if (zipEntry_ != null)
                    zipEntry_ = null;
            }
        }

    }


}
