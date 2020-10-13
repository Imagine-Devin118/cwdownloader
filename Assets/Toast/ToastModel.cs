using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImagineClass.Toast
{
    public class ToastModel
    {
        public ToastEmoji emoji;
        public string content;
    }
}

public enum ToastEmoji
{
    None,
    Tick,
    Sweat,
    Happy,
    Error,
    Info,
    Bell
}