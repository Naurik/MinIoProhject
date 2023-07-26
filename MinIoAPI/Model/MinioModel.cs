namespace MinIoClient.Model
{
    public class MinioModel
    {
        public string? BucketName { get; set; }
        public string? ObjectName { get; set; }
        public string? FilePath { get; set; }
        public string? ContentType { get; set; }
        public string? Format { get; set; }
    }
}
