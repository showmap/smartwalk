namespace SmartWalk.Shared.DataContracts
{
    public enum PictureSize
    {
        /// <summary>
        /// Original image without resizing.
        /// </summary>
        Full,
        /// <summary>
        /// The resized thumbnail with 1000x1000 map pixels.
        /// </summary>
        Medium,
        /// <summary>
        /// The resized thumbnail with 200x200 map pixels.
        /// </summary>
        Small
    }
}