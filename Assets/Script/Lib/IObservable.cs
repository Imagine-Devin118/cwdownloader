using System;

namespace ImagineClass.Library
{
    public interface IObservable<T>
    {
        void SetState(T newState);

        T GetState();

        void Acctach(IObserver<T> ob);

        void UnAcctach(IObserver<T> ob);
    }

}