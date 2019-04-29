using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Manager_Plikow
{
    class MyDirectory : DiscElement
    {
        public MyDirectory(string path) : base(path)
        {
            FileInfo fileInfo = new FileInfo(path);
        }


        public string Path { get; }

        public List<DiscElement> GetSubElements()
        {
            List<DiscElement> files = new List<DiscElement>();
            MyFile file = new MyFile(path);
            foreach (string myFiles in Directory.GetFiles(path))
            {
                files.Add(new MyFile(myFiles));
            }
            foreach (string myDirs in Directory.GetDirectories(path))
            {
                files.Add(new MyDirectory(myDirs));
            }
            return files;
        }

        public override long GetSize()
        {
            string[] allFiles = Directory.GetFiles(path, "*.*");
            long fullSize = 0;
            foreach (string name in allFiles)
            {
                FileInfo info = new FileInfo(name);
                fullSize += info.Length;
            }
            return fullSize;
        }


        public override bool IsFile()
        {
            return false;
        }


        public override string GetName()
        {
            return path.Split('\\').Last();
        }


        public override string GetExtension()
        {
            return "<dir>";
        }
    }
}
