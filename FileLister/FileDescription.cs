
namespace FileLister
{
    public class FileDescription
    {
        public string Date { get; set; }
        public string FileName { get; set; }
        public string Directory { get; set; }
        public string Description { get; set; }
        public string BugNumber { get; set; }
        public string Developer { get; set; }

        public override string ToString()
        {
            return string.Format(@"{0};{1};{2};{3};{4};{5}",
                Date,
                FileName,
                Directory,
                Description,
                BugNumber,
                Developer);
        }
    }
}
