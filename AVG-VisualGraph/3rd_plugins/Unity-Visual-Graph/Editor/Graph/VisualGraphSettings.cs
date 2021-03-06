using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VisualGraphRuntime;

namespace VisualGraphInEditor
{
	public static class VisualGraphSettings
	{
		public static bool autoSave = true;

		public static void Save()
		{
			if (autoSave)
			{
				AssetDatabase.SaveAssets();
			}
		}
	}
}
