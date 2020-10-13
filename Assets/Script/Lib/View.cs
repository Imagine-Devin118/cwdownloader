using System;
using System.Collections.Generic;
using FairyGUI;
using Imaginelearning.FairyGUI.Extensions;

namespace ImagineClass.Library
{
    public abstract class View<T> : IDisposable, IObserver<T> where T : new()
    {
        protected GComponent rootUI;
        protected readonly Model<T> model;
        protected IDictionary<string, GObject> components;
        protected IDictionary<string, Controller> controllers;
        protected IDictionary<string, Transition> transitions;
        protected virtual string[] ComponentNames { get; } = { };
        protected virtual string[] ControllerNames { get; } = { };
        protected virtual string[] TransitionNames { get; } = { };

        public View(Model<T> model, GComponent rootUI)
        {
            if (model is null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }
            this.model = model;
            this.rootUI = rootUI;

            components = rootUI.GetElements<GObject>(ComponentNames);
            controllers = rootUI.GetControllers(ControllerNames);
            transitions = rootUI.GetTransitions(TransitionNames);


            model.Acctach(this);
        }

        //不能在Render下使用SetState方法
        public abstract void Render(T data);

        public virtual void Dispose()
        {
            model.UnAcctach(this);
        }

        ~View()
        {
            model.UnAcctach(this);
        }
    }
}
