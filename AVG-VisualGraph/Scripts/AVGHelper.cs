using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LJ.VisualAVG
{
    public class AVGHelper
    {
        public const string None = "None";
        /// <summary>
        /// 位置关系
        /// </summary>
        public enum PosType
        {
            [InspectorName("左")]
            Left = 1,
            [InspectorName("中")]
            Middle = 2,
            [InspectorName("右")]
            Right = 3
        }
        /// <summary>
        /// 远近关系
        /// </summary>
        public enum DistanceType
        {
            [InspectorName("正常")]
            Normal = 1,
            [InspectorName("前")]
            Front = 2,
            [InspectorName("超前")]
            SFront = 3
        }
        /// <summary>
        /// 动画类型
        /// </summary>
        public enum AnimaType
        {
            [InspectorName("介绍动画")]
            Intro,
            [InspectorName("插图动画")]
            Img,
            [InspectorName("CG动画")]
            Cg
        }
        /// <summary>
        /// 声音类型
        /// </summary>
        public enum SoundType
        {
            [InspectorName("背景")]
            Bgm = 1,
            [InspectorName("环境音")]
            EnvirSfx,
            [InspectorName("音效")]
            Sfx
        }

        /// <summary>
        /// 交互类型
        /// </summary>
        public enum InteractType
        {
            [InspectorName("点击")]
            Button = 1,
            [InspectorName("拖动")]
            Drag
        }

        /// <summary>
        /// 效果
        /// </summary>
        public enum FXType
        {
            None = 0,
            [InspectorName("淡入")]
            FadeIn,
            [InspectorName("淡出")]
            FadeOut
        }

        /// <summary>
        /// 效果2 只有淡入淡出
        /// </summary>
        public enum FXType2
        {
            None = 0,
            [InspectorName("淡入淡出")]
            FadeInOut,
        }

        //public enum RoleNameType
        //{
        //    None,
        //    [InspectorName("袁悠悠")]
        //    yyy,
        //    [InspectorName("澜正非")]
        //    lzf,
        //    [InspectorName("纪风")]
        //    jf,
        //    [InspectorName("杰西卡")]
        //    jxk,
        //    [InspectorName("尹柊")]
        //    yz,
        //    [InspectorName("秦川")]
        //    qc
        //}
    }
}