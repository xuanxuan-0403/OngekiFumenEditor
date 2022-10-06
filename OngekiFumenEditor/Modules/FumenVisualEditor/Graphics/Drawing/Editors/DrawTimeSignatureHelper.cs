﻿using Caliburn.Micro;
using FontStashSharp;
using OngekiFumenEditor.Base;

using OngekiFumenEditor.Modules.FumenVisualEditor;
using OngekiFumenEditor.Utils.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static OngekiFumenEditor.Modules.FumenVisualEditor.Graphics.Drawing.ILineDrawing;

namespace OngekiFumenEditor.Modules.FumenVisualEditor.Graphics.Drawing.Editors
{
    public class DrawTimeSignatureHelper
    {
        public struct CacheDrawTimeLineResult
        {
            public double Y { get; set; }
            public string Display { get; set; }
        }

        private List<CacheDrawTimeLineResult> drawLines = new();

        private IStringDrawing stringDrawing;
        private ILineDrawing lineDrawing;

        public DrawTimeSignatureHelper()
        {
            stringDrawing = IoC.Get<IStringDrawing>();
            lineDrawing = IoC.Get<ISimpleLineDrawing>();
        }

        public void DrawLines(IFumenEditorDrawingContext target)
        {
            drawLines.Clear();

            var fumen = target.Editor.Fumen;

            if (target.Editor.Setting.BeatSplit == 0)
                return;

            var timelines = TGridCalculator.GetVisbleTimelines(
                fumen.BpmList,
                fumen.MeterChanges,
                Math.Max(0, target.Rect.MinY),
                target.Rect.MaxY,
                target.Editor.Setting.JudgeLineOffsetY,
                target.Editor.Setting.BeatSplit,
                target.Editor.Setting.VerticalDisplayScale
            );

            var transDisp = target.Rect.Width * 0.4f;
            var maxDispAlpha = 0.3f;
            var minDispAlpha = 0f;

            if (target.Editor.EditorObjectVisibility != System.Windows.Visibility.Visible)
                timelines = timelines.Where(x => x.beatIndex == 0);
            else
                minDispAlpha = maxDispAlpha;
            var eDisp = target.Rect.Width - transDisp;

            using var d = ObjectPool<List<LineVertex>>.GetWithUsingDisposable(out var list, out _);
            list.Clear();

            var displayAudioTime = target.Editor.Setting.DisplayTimeFormat == Models.EditorSetting.TimeFormat.AudioTime;

            foreach ((var t, var y, _) in timelines)
            {
                var str = string.Empty;
                if (displayAudioTime)
                {
                    var audioTime = TGridCalculator.ConvertTGridToAudioTime(t, target.Editor);
                    str = $"{audioTime.Minutes,-2}:{audioTime.Seconds,-2}:{audioTime.Milliseconds,-3}";
                }
                else
                    str = t.ToString();

                drawLines.Add(new()
                {
                    Display = str,
                    Y = y
                });

                list.Add(new(new(0, (float)y), new(1, 1, 1, 0), VertexDash.Solider));
                list.Add(new(new(0, (float)y), new(1, 1, 1, maxDispAlpha), VertexDash.Solider));
                list.Add(new(new(transDisp, (float)y), new(1, 1, 1, minDispAlpha), VertexDash.Solider));
                list.Add(new(new(eDisp, (float)y), new(1, 1, 1, minDispAlpha), VertexDash.Solider));
                list.Add(new(new(target.ViewWidth, (float)y), new(1, 1, 1, maxDispAlpha), VertexDash.Solider));
                list.Add(new(new(target.ViewWidth, (float)y), new(1, 1, 1, 0), VertexDash.Solider));
            }

            lineDrawing.Draw(target, list, 1);
        }

        public void DrawTimeSigntureText(IFumenEditorDrawingContext target)
        {
            foreach (var pair in drawLines)
                stringDrawing.Draw(
                    pair.Display,
                    new(target.ViewWidth,
                    (float)pair.Y + 10),
                    Vector2.One,
                    12,
                    0,
                    Vector4.One,
                    new(1, 0.5f),
                    IStringDrawing.StringStyle.Normal,
                    target,
                    default,
                    out _
            );
        }
    }
}
