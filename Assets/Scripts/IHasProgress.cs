using System;

public interface IHasProgress
{
    //�����и�����Ӿ���ʵ���߼�
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs {
        public float progressNormalized;
    }
}
