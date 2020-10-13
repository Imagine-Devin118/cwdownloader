using FairyGUI;
using ImagineClass.Library;

namespace ImagineClass.Toast
{
    public class ToastView : View<ToastModel>
    {
        public ToastView(Model<ToastModel> model, GComponent rootUI) : base(model, rootUI)
        {
        }

        protected override string[] ComponentNames { get; } ={
            "toast/tips", "toast/loader"
        };

        protected override string[] TransitionNames { get; } ={
            "toast"
        };

        public override void Render(ToastModel data)
        {
            components["toast/tips"].asTextField.text = data.content;
            components["toast/loader"].asLoader.url = $"ui://Toast/{data.emoji.ToString()}";
            transitions["toast"].Play();
        }
    }
}