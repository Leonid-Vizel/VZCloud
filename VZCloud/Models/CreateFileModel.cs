namespace VZCloud.Models
{
    public class CreateFileModel
    {
        public bool Rewrite { get; set; }
        public string Path { get; set; }
        public byte[] Bytes { get; set; }
    }
}
