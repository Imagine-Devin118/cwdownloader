using System.Linq;
using System.Collections.Generic;
using FairyGUI;

namespace Imaginelearning.FairyGUI.Extensions
{
    public static class FairyGUIExtensions
    {
        public static IList<T> GetElementsList<T>(this GComponent component, string[] names) where T : GObject
        {
            return names.Select(name => component.GetChild(name) as T).Where(x => x != null).ToList();
        }

        public static IDictionary<string, T> GetElements<T>(this GComponent component, string[] srcs) where T : GObject
        {
            var dictionary = new Dictionary<string, T>();
            for (int i = 0; i < srcs.Length; i++)
            {
                var result = GetCompnentInChildren(component, srcs[i]);
                dictionary.Add(srcs[i], result as T);
            }
            return dictionary;
        }

        public static IDictionary<string, Transition> GetTransitions(this GComponent component, string[] srcs)
        {
            var dictionary = new Dictionary<string, Transition>();
            for (int i = 0; i < srcs.Length; i++)
            {
                if (srcs[i].Contains("/"))
                {
                    var prefixPath = System.IO.Path.GetDirectoryName(srcs[i]).Replace("\\", "/");
                    var name = System.IO.Path.GetFileName(srcs[i]);
                    var com = component.GetCompnentInChildren(prefixPath);
                    dictionary.Add(srcs[i], com.asCom.GetTransition(name));
                }
                else
                {
                    dictionary.Add(srcs[i], component.GetTransition(srcs[i]));
                }
            }
            return dictionary;
        }

        public static IDictionary<string, Controller> GetControllers(this GComponent component, string[] srcs)
        {
            var dictionary = new Dictionary<string, Controller>();
            for (int i = 0; i < srcs.Length; i++)
            {
                if (srcs[i].Contains("/"))
                {
                    var prefixPath = System.IO.Path.GetDirectoryName(srcs[i]).Replace("\\", "/");;
                    var name = System.IO.Path.GetFileName(srcs[i]);
                    var com = component.GetCompnentInChildren(prefixPath);
                    dictionary.Add(srcs[i], com.asCom.GetController(name));
                }
                else
                {
                    dictionary.Add(srcs[i], component.GetController(srcs[i]));
                }
            }
            return dictionary;
        }

        public static GObject GetCompnentInChildren(this GComponent component, string relativePath)
        {
            var paths = relativePath.Split('/');
            GObject result = component;
            foreach (var path in paths)
            {
                if (result == null) break;
                result = result.asCom.GetChild(path);
            }
            return result;
        }
    }
}