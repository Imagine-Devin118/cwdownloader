using System.Collections.Generic;

namespace ImagineClass.Library
{
    public class Model<T> : IObservable<T> where T : new()
    {
        private T state;
        private List<IObserver<T>> observers = new List<IObserver<T>>();

        //此函数不可以在View层调用，否则会产生死循环
        public void SetState(T newState)
        {
            if (newState == null)
            {
                throw new System.ArgumentNullException(nameof(newState));
            }
            this.state = newState;
            NotifyAll();
        }

        public T GetState()
        {
            // return state == null ? new T() : ObjectTool.DeepCopy<T>(state);
            return state == null ? new T() : state;
        }
        public void Acctach(IObserver<T> ob)
        {
            if (ob != null) observers?.Add(ob);
        }

        private void NotifyAll()
        {
            foreach (var ob in observers)
            {
                ob.Render(state);
            }
        }

        public void UnAcctach(IObserver<T> ob)
        {
            if (observers.Contains(ob)) observers.Remove(ob);
        }
    }
}