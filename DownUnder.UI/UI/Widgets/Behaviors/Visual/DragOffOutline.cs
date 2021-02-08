﻿using System;
using DownUnder.UI.Utilities;
using DownUnder.UI.Utilities.CommonNamespace;
using DownUnder.UI.Utilities.Extensions;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual
{
    public class DragOffOutline : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.COSMETIC_MID_PERFORMANCE };

        private ChangingValue<RectangleF> rect = new ChangingValue<RectangleF>();
        private ChangingValue<float> round_amount = new ChangingValue<float>(0f);
        private ChangingValue<Color> rect_color = new ChangingValue<Color>();
        private bool drag_rect_to_mouse = false;
        private bool snap_rect_to_mouse = false;
        private float circumference = 20f;
        private float _expand_rect_on_release = 1f;

        public DragOffOutline() {
            round_amount.Interpolation = InterpolationType.fake_sin;
            round_amount.TransitionSpeed = 1f;
            rect.Interpolation = InterpolationType.linear;
            rect.TransitionSpeed = 20f;
            rect_color.TransitionSpeed = 4f;
        }

        protected override void Initialize() { }

        protected override void ConnectEvents() {
            Parent.OnDrawNoClip += DrawRect;
            Parent.OnUpdate += Update;
            Parent.OnDrag += StartAnimation;
            Parent.OnDrop += EndAnimation;
        }

        protected override void DisconnectEvents() {
            Parent.OnDrawNoClip -= DrawRect;
            Parent.OnUpdate -= Update;
            Parent.OnDrag -= StartAnimation;
            Parent.OnDrop -= EndAnimation;
        }

        private void StartAnimation(object sender, EventArgs args) {
            drag_rect_to_mouse = true;
            rect_color.SetTargetValue(new Color(0, 0, 0, 0), true);
            rect_color.TransitionSpeed = 4f;
            rect_color.SetTargetValue(Parent.VisualSettings.OutlineColor);
            //rect.SetTargetValue(Parent.DrawingArea, true);
            round_amount.SetTargetValue(1f);
        }

        private void EndAnimation(object sender, EventArgs args) {
            drag_rect_to_mouse = false;
            snap_rect_to_mouse = false;
            round_amount.SetTargetValue(0f);
            rect_color.TransitionSpeed = 1f;
            rect_color.SetTargetValue(new Color(0, 0, 0, 0), false);
            rect.SetTargetValue(rect.Current.WithScaledSize(_expand_rect_on_release).WithCenter(rect.Current), false);
        }

        private void Update(object sender, EventArgs args) {
            if (drag_rect_to_mouse) {
                bool? is_drop_acceptable = Parent.ParentDWindow.HoveredWidgets.Primary?.IsDropAcceptable(Parent.ParentDWindow.DraggingObject);
                Widget victim = Parent.ParentDWindow.HoveredWidgets.Primary;
                if (Parent.ParentDWindow.DraggingObject is Widget dragging_widget && is_drop_acceptable != null && is_drop_acceptable.Value) {
                    // Set to new Widget area
                    rect.SetTargetValue(
                        victim.DesignerObjects.GetAddWidgetArea(dragging_widget).WithOffset(victim.PositionInWindow),
                        snap_rect_to_mouse);
                    _expand_rect_on_release = 1f;
                    round_amount.SetTargetValue(.5f);
                }
                else {
                    // Set to default circle
                    rect.SetTargetValue(new RectangleF(0, 0, circumference, circumference).WithCenter(Parent.UpdateData.UIInputState.CursorPosition), snap_rect_to_mouse);
                    _expand_rect_on_release = 2f;
                    round_amount.SetTargetValue(1f);
                }
            }
            rect.Update(Parent.UpdateData.ElapsedSeconds);
            rect_color.Update(Parent.UpdateData.ElapsedSeconds);
            round_amount.Update(Parent.UpdateData.ElapsedSeconds);
        }

        private void DrawRect(object sender, EventArgs args) {
            RectangleF r = rect.Current;
            Parent.SpriteBatch.DrawRoundedRect(r, Math.Min(r.Width, r.Height) * 0.5f * round_amount.Current, rect_color.Current, 4f);
        }

        public override object Clone() {
            return new DragOffOutline();
        }
    }
}
