using System;

public interface IHasProgress
{
    //处理切割进度视觉的实现逻辑
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs {
        public float progressNormalized;
    }
}
