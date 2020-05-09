using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Reflection;
using System.Web;

namespace TMI.Web
{
    public class Rar_FileHelper
    {
        /// <summary>
        /// 添加文件打包为Rar压缩文件
        /// </summary>
        /// <param name="ArrFiles"></param>
        /// <returns>返回压缩文件路径</returns>
        public string AddFileToRar(List<String> ArrFiles,string Extension = ".rar", string RarNameTop = "Dec_Rec_rar_")
        {
            string Default_Folder = "";//保存文件目录信息
            string PCNameStr = "";// HttpContext.Current.Server.MachineName;
            if (HttpContext.Current != null && HttpContext.Current.Server != null)
            {
                PCNameStr = HttpContext.Current.Server.MachineName;
                //创建现有的文件
                Default_Folder = HttpContext.Current.Server.MapPath("/");
            }
            else
            {
                Default_Folder = System.AppDomain.CurrentDomain.BaseDirectory;
            }
            Default_Folder += "\\DownLoadRar\\";
            if (!string.IsNullOrEmpty(PCNameStr))
                Default_Folder += "\\" + PCNameStr;
            else
            {
                //创建现有的文件
                Default_Folder += DateTime.Now.ToString("yyyy-MM-dd");
            }
            //加一个随机数文件夹
            Default_Folder += "\\" + new Random().Next(1, 9999).ToString("0000");

            string RarName = RetFolderName(RarNameTop + (new Random()).Next(1, 999).ToString("000"));

            if (Directory.Exists(Default_Folder))
                //删除相关的临时文件夹
                DeleteDateFolder(Default_Folder);
            else
                Directory.CreateDirectory(Default_Folder);

            //案件相关的临时文件夹名称
            //CreateFolder(Default_FolderChildren);

            //把文件放入小文件件
            for (int i = 0; i < ArrFiles.Count(); i++)
            {
                FileInfo OFileInfo = new FileInfo(ArrFiles[i]);
                if (OFileInfo.Exists)
                {
                    OFileInfo.CopyTo(Default_Folder + "\\" + OFileInfo.Name);
                }
            }

            //文件夹压缩
            string RarPath = Default_Folder + "\\" + RarName + Extension;
            bool RarIsRead = AddRar(Default_Folder, RarPath);

            if (RarIsRead)
            {
                return RarPath;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 检测 是否是Rar压缩文件
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public bool chkIsRar(string ext)
        {
            if (ext.Contains(".7-zip"))
            {
                return true;
            }
            else if (ext.Contains(".zip"))
            {
                return true;
            }
            else if (ext.Contains(".rar"))
            {
                return true;
            }
            return false;

        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <returns>返回压缩后的路径</returns>
        public bool AddRar(string yasuoPath, string yasuoPathSave)
        {
            bool bo = false;
            try
            {
                TimeSpan times = new TimeSpan();
                //压缩文件
                System.Diagnostics.Process pro = new System.Diagnostics.Process();
                pro.StartInfo.FileName = @"C:\Program Files (x86)\WinRAR\WinRAR.exe";//WinRAR所在路径
                //pro.StartInfo.FileName = "WinRAR.exe";//WinRAR所在路径
                pro.StartInfo.WorkingDirectory = yasuoPath;
                pro.StartInfo.UseShellExecute = true;
                pro.StartInfo.CreateNoWindow = true;
                #region WinRar 用到的命令注释
                //[a] 添加到压缩文件
                //afzip 执行zip压缩方式，方便用户在不同环境下使用。（取消该参数则执行rar压缩）
                //-m0 存储 添加到压缩文件时不压缩文件。共6个级别【0-5】，值越大效果越好，也越慢
                //ep1 依名称排除主目录（生成的压缩文件不会出现不必要的层级）
                //r   修复压缩档案
                //t   测试压缩档案内的文件 
                //as  同步压缩档案内容  
                //-p  给压缩文件加密码方式为：-p123456 
                #endregion
                if (yasuoPathSave.ToLower().IndexOf(".rar")>0)
                    pro.StartInfo.Arguments = "a -as -r " + yasuoPathSave + " * ";
                else
                    pro.StartInfo.Arguments = "a -afzip -as -r " + yasuoPathSave + " * ";
                pro.Start();
                times = pro.TotalProcessorTime;
                bo = pro.WaitForExit(300000);//设定5分钟
                if (!bo)
                    pro.Kill();
                pro.Close();
                pro.Dispose();
            }
            catch
            {
                bo = false;
            }
            return bo;
        }

        /// <summary>
        /// 创建 文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        public void CreateFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        public void DeleteFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
        }

        /// <summary>
        /// 删除相关的临时文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        public void DeleteDateFolder(string folderPath)
        {
            string FolderDate = folderPath.Substring(folderPath.LastIndexOf("\\") + 1);
            string FolderFather = folderPath.Substring(0, folderPath.LastIndexOf("\\"));
            DirectoryInfo DirInfo = new DirectoryInfo(FolderFather);
            DirectoryInfo[] DirInfoS = null;
            if (DirInfo.Exists)
            {
                DirInfoS = DirInfo.GetDirectories();
            }
            string FolderToday = (FolderFather + "\\" + DateTime.Now.ToString("yyyy-MM-dd"));
            if (DirInfoS != null)
            {
                for (int i = 0; i < DirInfoS.Length; i++)
                {
                    if (DirInfoS[i].FullName != FolderToday)
                    {
                        if (Directory.Exists(DirInfoS[i].FullName))
                        {
                            Directory.Delete(DirInfoS[i].FullName, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 删除 文件夹中的所有文件
        /// </summary>
        /// <param name="folderPath"></param>
        public void DeleteFiles(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string[] deleteFiles = Directory.GetFiles(folderPath);
                if (deleteFiles.Length > 0)
                {
                    for (int i = 0; i < deleteFiles.Length; i++)
                    {
                        File.Delete(deleteFiles[i]);
                    }
                }
                string[] deleteDirectorys = Directory.GetDirectories(folderPath);
                if (deleteDirectorys.Length > 0)
                {
                    for (int i = 0; i < deleteDirectorys.Length; i++)
                    {
                        DeleteFiles(deleteDirectorys[i]);
                    }
                }
                else
                {
                    Directory.Delete(folderPath);
                }
            }
        }

        public void CreatePic(byte[] imgBytes, string filePath)
        {
            File.WriteAllBytes(filePath, imgBytes);
        }

        public string RetFolderName(string FolderName)
        {
            char[] FFchars = System.IO.Path.GetInvalidFileNameChars();
            string NewFolderName = "";
            bool IsIn = false;
            string Word = "";
            try
            {
                if (FolderName != "")
                {
                    for (int i = 0; i < FolderName.Length; i++)
                    {
                        Word = FolderName.Substring((i < (FolderName.Length - 1) ? i : (FolderName.Length - 1)), 1);
                        for (int x = 0; x < FFchars.Length; x++)
                        {
                            if (Word == FFchars[x].ToString())
                                IsIn = true;
                        }
                        if (IsIn == false)
                        {
                            NewFolderName += Word;
                        }
                        IsIn = false;
                    }
                }
                else
                {
                    NewFolderName = FolderName;
                }
            }
            catch
            {
                NewFolderName = FolderName;
            }
            return NewFolderName;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public byte[] ReadFile(string fileName)
        {
            FileStream pFileStream = null;
            byte[] pReadByte = new byte[0];
            try
            {
                pFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(pFileStream);
                r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
                pReadByte = r.ReadBytes((int)r.BaseStream.Length);
                return pReadByte;
            }
            catch
            {
                return pReadByte;
            }
            finally
            {
                if (pFileStream != null)
                    pFileStream.Close();
            }
        }

    }
}