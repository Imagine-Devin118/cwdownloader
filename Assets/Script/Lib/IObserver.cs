namespace ImagineClass.Library
{
    public interface IObserver<T>
    {
        void Render(T data);
    }
}