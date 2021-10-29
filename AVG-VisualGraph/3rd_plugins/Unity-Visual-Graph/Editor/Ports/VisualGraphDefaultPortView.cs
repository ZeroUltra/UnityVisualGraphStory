using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using VisualGraphRuntime;
using UnityEditor;
using System;

namespace VisualGraphInEditor
{
	[CustomPortView(typeof(VisualGraphPort))]
	public sealed class VisualGraphDefaultPortView : VisualGraphPortView
    {
        public override void CreateView(VisualGraphPort port)
        {
			TextField leftField = new TextField();
			leftField.value = port.Name;
			leftField.style.width = 80;
			leftField.RegisterCallback<ChangeEvent<string>>(
				(evt) =>
				{
					if (string.IsNullOrEmpty(evt.newValue) == false)
					{
						port.Name = evt.newValue;
					}
				}
			);
			Add(leftField);
			
		}
		public  void CreateViewLable(VisualGraphPort port)
		{
			Label leftField = new Label(port.Name);
			leftField.style.width = 25;
			Add(leftField);
			
		}

    }
}
