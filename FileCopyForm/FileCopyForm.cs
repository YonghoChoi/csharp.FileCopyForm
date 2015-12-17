using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileCopyForm
{
    public partial class FileCopyForm : Form
    {
        FileQueue fileQueue = new FileQueue();

        public FileCopyForm()
        {
            InitializeComponent();
        }

        private void btnFileSelect_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(tbOutPath.Text))
            {
                MessageBox.Show("출력 경로를 선택하지 않았습니다.");
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "파일|*.*";
            openFileDialog.Title = "파일 선택";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
                Process(new Thread(new ParameterizedThreadStart(RunSelectCopy)), openFileDialog.FileNames);
        }

        private void btnFolderSelect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbOutPath.Text))
            {
                MessageBox.Show("출력 경로를 선택하지 않았습니다.");
                return;
            }

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                fileQueue.ListupDirectory(fbd.SelectedPath.Length, fbd.SelectedPath, tbOutPath.Text);
                Process(new Thread(new ThreadStart(RunFolderCopy)), null);
            }
            else
                MessageBox.Show("잘못 선택하셨습니다.");
        }

        private void Process(Thread th, object arg)
        {
            if (arg != null)
                th.Start(arg);
            else
                th.Start();
        }

        private void RunSelectCopy(object obj)
        {
            string[] files = (string[])obj;
            foreach (string srcFile in files)
            {
                string[] split = srcFile.Split('\\');
                if (split.Length <= 0)
                    continue;


                string targetFile = string.Format(@"{0}\{1}", tbOutPath.Text, split[split.Length - 1]);
                fileQueue.EnqueueCopyInfo(srcFile, targetFile);
            }

            FileCopyManager fileCopyManager = new FileCopyManager(fileQueue.CopyInfoQueue, fileQueue.FileTotalSize);
            fileCopyManager.AsyncCopy();
        }

        private void RunFolderCopy()
        {
            FileCopyManager fileCopyManager = new FileCopyManager(fileQueue.CopyInfoQueue, fileQueue.FileTotalSize);
            fileCopyManager.AsyncCopy();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelectOutPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
                tbOutPath.Text = fbd.SelectedPath;
            else
                MessageBox.Show("잘못 선택하셨습니다.");
        }

    }
}
