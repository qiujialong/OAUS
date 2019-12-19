using System;
using System.Collections.Generic;
using System.Text;

namespace OAUS.Core
{
    public class FileUnit :IComparable<FileUnit>
    {
        public FileUnit() { }
        public FileUnit(string file, float ver)
        {
            this.fileRelativePath = file;
            this.version = ver;
        }

        #region FileRelativePath
        private string fileRelativePath;
        public string FileRelativePath
        {
            get { return fileRelativePath; }
            set { fileRelativePath = value; }
        } 
        #endregion

        #region Version
        private float version;
        public float Version
        {
            get { return version; }
            set { version = value; }
        } 
        #endregion

        public override bool Equals(object obj)
        {
            FileUnit unit = obj as FileUnit;
            if (unit == null)
            {
                return false;
            }

            return this.fileRelativePath == unit.fileRelativePath;
        }

        public int CompareTo(FileUnit other)
        {
            return this.fileRelativePath.CompareTo(other.fileRelativePath);
        }

        public override int GetHashCode()
        {
            return this.fileRelativePath.GetHashCode();
        }
    }
}
