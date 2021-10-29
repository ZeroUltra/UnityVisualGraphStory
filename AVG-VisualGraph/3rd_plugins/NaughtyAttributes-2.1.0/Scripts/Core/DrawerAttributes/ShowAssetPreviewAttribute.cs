using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ShowAssetPreviewAttribute : DrawerAttribute
	{
		public const int DefaultWidth = 64;
		public const int DefaultHeight = 64;

		public int Width { get; private set; }
		public int Height { get; private set; }
		public bool IsShowLable { get; private set; }
		public bool IsShowProperty { get; private set; }
		public ShowAssetPreviewAttribute(int width = DefaultWidth, int height = DefaultHeight, bool isShowLable = false,bool isShowProperty=false)
		{
			Width = width;
			Height = height;
			IsShowLable = isShowLable;
			IsShowProperty = isShowProperty;
		}
	}
}
