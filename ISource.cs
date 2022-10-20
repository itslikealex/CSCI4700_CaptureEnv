namespace CSCI4700_CaptureEnv
{
    public interface ISource
    {
        string FriendlyName { get; set; }
        object GetCompressedMediaType();
        object GetSourceNode(object aOutputNode);
        void Access(bool aState);
    }
}
