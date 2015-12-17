using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileCopyForm
{
    public class FileCopyManager
    {
        private Queue<CopyInfo> copyInfoQueue;
        private List<Thread> threadList = new List<Thread>();
        private object copyQueueLock = new object();
        private object progressBarLock = new object();
        private object copyThreadEndCntLock = new object();
        private long fileTotalSize = 0;
        private long progressSize = 0;
        private short copyThreadEndCnt = 0;
        private string currentCopyFile = string.Empty;

        public long FileTotalSize { get { return fileTotalSize; } }
        public long ProgressSize { get { return progressSize; } }
        public string CurrentCopyFile { get { return currentCopyFile; } }
        public short CopyThreadEndCnt { get { return copyThreadEndCnt; } }

        public FileCopyManager(Queue<CopyInfo> copyInfoQueue, long fileTotalSize)
        {
            this.copyInfoQueue = copyInfoQueue;
            this.fileTotalSize = fileTotalSize;
        }

        private CopyInfo DequeueCopyInfo()
        {
            lock (copyQueueLock)
            {
                if (this.copyInfoQueue.Count > 0)
                    return this.copyInfoQueue.Dequeue();
                else
                    return null;
            }
        }

        public string AsyncCopy()
        {
            for (int i = 0; i < Properties.Settings.Default.CopyThreadCount; ++i)
            {
                Thread th = new Thread(new ThreadStart(AsyncCopyThread));
                this.threadList.Add(th);
                th.Start();
            }

            CopyProgressBarForm progressBarForm = new CopyProgressBarForm(this);
            progressBarForm.Show();

            while (this.copyThreadEndCnt != Properties.Settings.Default.CopyThreadCount)
                Thread.Sleep(1000);

            return "SUCCESS";
        }

        private void AsyncCopyThread()
        {
            try
            {
                while (true)
                {
                    CopyInfo copyInfo = this.DequeueCopyInfo();

                    if (copyInfo == null)
                        break;

                    if (File.Exists(copyInfo.srcPath))
                    {
                        FileInfo srcFileInfo = new FileInfo(copyInfo.srcPath);
                        srcFileInfo.IsReadOnly = false;
                        CheckTargetFileAndCreate(copyInfo.targetPath);
                        FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.AllAccess, copyInfo.targetPath);
                        permission.Demand();
                        this.currentCopyFile = srcFileInfo.Name;
                        File.Copy(copyInfo.srcPath, copyInfo.targetPath, true);
                        this.IncreaseProgressSize(copyInfo.fileSize);
                    }
                }

            }
            catch (System.Exception ex)
            {
                LogManager.WriteLog(eLOG_LEVEL.ERROR, ex.ToString());
            }

            CopyThreadEnd();
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

        private void IncreaseProgressSize(long fileSize)
        {
            lock (progressBarLock)
            {
                this.progressSize += fileSize;
            }
        }

        private void CopyThreadEnd()
        {
            lock (copyThreadEndCntLock)
            {
                this.copyThreadEndCnt++;
            }
        }

        public void StopThread()
        {
            foreach (Thread th in this.threadList)
            {
                if (th.IsAlive)
                    th.Abort();
            }
        }
    }
}
