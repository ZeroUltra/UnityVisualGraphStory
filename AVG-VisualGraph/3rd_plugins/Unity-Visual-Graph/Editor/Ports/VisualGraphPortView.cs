using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VisualGraphRuntime;

namespace VisualGraphInEditor
{
    public abstract class VisualGraphPortView : VisualElement
    {
        public abstract void CreateView(VisualGraphPort port);
      //  public abstract void CreateViewLable(VisualGraphPort port);
    }
}