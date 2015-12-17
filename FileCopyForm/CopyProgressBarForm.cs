using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileCopyForm
{
    public partial class CopyProgressBarForm : Form
    {
        int dotCount = 1;
        int dotPeriod = 0;
        FileCopyManager fileCopyManager = null;

        public CopyProgressBarForm(FileCopyManager fileCopyManager)
        {
            InitializeComponent();
            this.fileCopyManager = fileCopyManager;
            progressBar1.Value = 0;
            progressBar1.Maximum = (int)this.fileCopyManager.FileTotalSize;
            InitProgress();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitProgress()
        {
            this.dotCount = 1;
            progressBar1.Value = 0;
            timer1.Start();
        }

        private bool IsFinished()
        {
            if (this.fileCopyManager.FileTotalSize == this.fileCopyManager.ProgressSize)
                return true;

            if (this.fileCopyManager.CopyThreadEndCnt == Properties.Settings.Default.CopyThreadCount)
                return true;

            return false;
        }

        private void PrintFileCopyMsg(string msg)
        {
            if (this.dotPeriod / 10 > 0)
            {
                string dot = string.Empty;
                for (int i = 0; i < this.dotCount; ++i)
                    dot += ".";

                if (++this.dotCount > 3)
                    this.dotCount = 1;

                lbMsg.Text = string.Format("{0} {1}", msg, dot);
                this.dotPeriod = 0;
            }
            else
                dotPeriod++;
        }
    }
}
