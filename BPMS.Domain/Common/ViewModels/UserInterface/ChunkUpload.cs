using System.Runtime.Serialization;

namespace BPMS.Domain.Common.ViewModels.UserInterface;

[DataContract]
public class ChunkMetaData
{
    [DataMember(Name = "uploadUid")]
    public string UploadUid { get; set; }
    [DataMember(Name = "fileName")]
    public string FileName { get; set; }
    [DataMember(Name = "relativePath")]
    public string RelativePath { get; set; }
    [DataMember(Name = "contentType")]
    public string ContentType { get; set; }
    [DataMember(Name = "chunkIndex")]
    public long ChunkIndex { get; set; }
    [DataMember(Name = "totalChunks")]
    public long TotalChunks { get; set; }
    [DataMember(Name = "totalFileSize")]
    public long TotalFileSize { get; set; }
}

public class FileResult
{
    public bool uploaded { get; set; }
    public string fileUid { get; set; }
}