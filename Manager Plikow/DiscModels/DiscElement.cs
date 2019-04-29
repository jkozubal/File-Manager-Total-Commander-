using System;
using System.IO;

namespace Manager_Plikow
{
    public abstract class DiscElement
    {
        public DiscElement(string path)
        {
            this.path = path;
        }

        public string path { get; }

        DateTime creationDate;
        public DateTime CreationDate
        {
            get
            {
                if (creationDate.Ticks == 0)
                {
                    creationDate = File.GetCreationTime(path);
                }
                return creationDate;
            }

        }
        public abstract bool IsFile();
        public abstract string GetName();
        public abstract string GetExtension();
        public abstract long GetSize();
    }
}
