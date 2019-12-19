using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OAUS.Core;
using System.IO;

namespace OAUS.Server
{
    public partial class FileVersionForm : Form
    {
        private bool changed = false;
        private UpdateConfiguration fileConfig;
        public FileVersionForm(UpdateConfiguration _fileConfig)
        {
            InitializeComponent();
            this.fileConfig = _fileConfig;
            
            this.BindData();
        }

        private void BindData()
        {
            if (this.fileConfig.FileList.Count == 0)
            {
                List<string> files = ESBasic.Helpers.FileHelper.GetOffspringFiles(AppDomain.CurrentDomain.BaseDirectory + "FileFolder\\");
                foreach (string fileRelativePath in files)
                {
                    this.fileConfig.FileList.Add(new FileUnit(fileRelativePath, 1));
                }                
                this.fileConfig.Save();
                this.changed = true;
            }

            ((List<FileUnit>)(this.fileConfig.FileList)).Sort();
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = this.fileConfig.FileList;
            this.label1.Text = "最后更新时间：" + DateTime.Now.ToString();
            this.label_lastVersion.Text = "最后综合版本：" + this.fileConfig.ClientVersion;
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            AddFileVersionForm addForm = new AddFileVersionForm(this.fileConfig, null);
            addForm.addSuccessEvent += new ESBasic.CbGeneric(addForm_addSuccessEvent);
            addForm.Show();
        }

        void addForm_addSuccessEvent()
        {
            this.BindData();
            this.changed = true;
        }

        private void button_update_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择要修改的文件");
                return;
            }
            FileUnit fileObject = this.dataGridView1.SelectedRows[0].DataBoundItem as FileUnit;
            AddFileVersionForm addForm = new AddFileVersionForm(this.fileConfig, fileObject);
            addForm.addSuccessEvent += new ESBasic.CbGeneric(addForm_addSuccessEvent);
            addForm.Show();
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择要删除的文件");
                return;
            }
            FileUnit fileObject = this.dataGridView1.SelectedRows[0].DataBoundItem as FileUnit;
            DialogResult result = MessageBox.Show(string.Format("确定要删除文件{0}？", fileObject.FileRelativePath), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.fileConfig.FileList.Remove(fileObject); 
                this.fileConfig.Save();
                this.changed = true;
                this.BindData();
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
            {
                return;
            }

            DataGridView.HitTestInfo info = this.dataGridView1.HitTest(e.X, e.Y);
            if (info.RowIndex >= 0)
            {
                FileUnit fileObject = this.dataGridView1.SelectedRows[0].DataBoundItem as FileUnit;
                AddFileVersionForm addForm = new AddFileVersionForm(this.fileConfig, fileObject);
                addForm.addSuccessEvent += new ESBasic.CbGeneric(addForm_addSuccessEvent);
                addForm.Show();
            }
        }

        private void FileVersionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.changed)
            {
                ++this.fileConfig.ClientVersion;
                this.fileConfig.Save();
                MessageBox.Show(string.Format("综合版本更新为：{0}" ,this.fileConfig.ClientVersion));
            }
        }
    }
}
