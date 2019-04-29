using System.IO;
using System.Linq;

namespace Manager_Plikow
{
    class MyFile : DiscElement
    {
        public MyFile(string path) : base(path)
        {
            FileInfo fileInfo = new FileInfo(path);
        }

        public override string GetExtension()
        {
            return path.Split('.').Last();
        }

        public override string GetName()
        {
            return path.Split('\\').Last().Split('.').First();
        }

        public override long GetSize()
        {
            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.Length;
        }

        public override bool IsFile()
        {
            return true;
        }
    }
}
