using Manager_Plikow.Util;


namespace Manager_Plikow
{
    class Item
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Size { get; set; }
        public string Date { get; set; }
        public string Access { get; set; }
        public string Path { get; set; }

        public Item()
        {
        }
        public Item(DiscElement discElement)
        {
            Name = discElement.GetName();
            Extension = discElement.GetExtension();
            Size = FileSizeUtils.ByteToHumanReadable(discElement.GetSize());
            Date = discElement.CreationDate.ToString();
            Path = discElement.path;
        }
        public bool IsDirectory()
        {
            if (Extension == "<dir>")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
