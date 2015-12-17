using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCopyForm
{
    public class CopyInfo
    {
        public string srcPath;
        public string targetPath;
        public long fileSize;
    }

    public class FileQueue
    {
#region MEMBER_VALUE
        private Queue<CopyInfo> copyInfoQueue = new Queue<CopyInfo>();
        long fileTotalSize = 0;

        internal Queue<CopyInfo> CopyInfoQueue  { get { return copyInfoQueue; } }
        internal long FileTotalSize { get { return fileTotalSize; } }

        public void Init()
        {
            this.copyInfoQueue.Clear();
        }
#endregion

#region DIRECTORY_MANAGING
        public void DeleteDirectory(string targetDir)
        {
            File.SetAttributes(targetDir, FileAttributes.Normal);

            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }

        public bool CheckTargetDirectoryAndCreate(string targetDirectory)
        {
            if (!Directory.Exists(targetDirectory))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(targetDirectory);
                ParentDirectoryCreate(dirInfo);
                Directory.CreateDirectory(targetDirectory);
            }
            return true;
        }

        public bool CheckTargetFileAndCreate(string targetFile)
        {
            FileInfo targetFileInfo = new FileInfo(targetFile);
            if (!targetFileInfo.Directory.Exists)
            {
                ParentDirectoryCreate(targetFileInfo.Directory);
                Directory.CreateDirectory(targetFileInfo.DirectoryName);
            }
            return true;
        }

        public void ParentDirectoryCreate(DirectoryInfo dirInfo)
        {
            DirectoryInfo parent = dirInfo.Parent;
            if (!parent.Exists)
                ParentDirectoryCreate(parent);

            Directory.CreateDirectory(parent.FullName);
        }

        public List<FileInfo> LookupFiles(string targetPath)
        {
            string[] files = Directory.GetFiles(targetPath);
            if (files.Length > 0)
            {
                List<FileInfo> fileInfoList = new List<FileInfo>();
                foreach (string file in files)
                    fileInfoList.Add(new FileInfo(file));

                return fileInfoList;
            }
            else
                return new List<FileInfo>();
        }
#endregion

#region FILE_COPY
        public void EnqueueCopyInfo(string srcFile, string targetFile)
        {
            if (File.Exists(srcFile) == true)
            {
                FileInfo srcFileInfo = new FileInfo(srcFile);
                CopyInfo copyInfo = new CopyInfo();
                copyInfo.srcPath = srcFile;
                copyInfo.targetPath = targetFile;
                copyInfo.fileSize = srcFileInfo.Length;
                this.fileTotalSize += srcFileInfo.Length;
                this.copyInfoQueue.Enqueue(copyInfo);
            }
        }
#endregion
        
#region COPY_FILE_LIST_UP
        public void ListupDirectory(int srcBasePathLength, string srcPath, string targetPath)
        {
            string[] files = Directory.GetFiles(srcPath);
            string[] dirs = Directory.GetDirectories(srcPath);

            foreach (string srcFile in files)
            {
                string targetFile = string.Format("{0}{1}", targetPath, srcFile.Substring(srcBasePathLength));
                EnqueueCopyInfo(srcFile, targetFile);
            }

            foreach (string dir in dirs)
                ListupDirectory(srcBasePathLength, dir, targetPath);
        }
#endregion
    }
}
