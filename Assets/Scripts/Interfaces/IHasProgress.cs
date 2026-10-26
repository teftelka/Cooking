using System;

namespace Interfaces
{
    // Anything with a timed process shown by a progress bar: cutting, washing, merging, cooking
    public interface IHasProgress
    {
        // 0..1 while the process runs. The bar is hidden at 0 (idle) and at 1 or more (done)
        event EventHandler<ProgressChangedEventArgs> OnProgressChanged;
    }

    public class ProgressChangedEventArgs : EventArgs
    {
        public float progress;
    }
}
