﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static DownUnder.UI.Widgets.Behaviors.Visual.MouseGlow.MouseGlowActivationPolicy;
using System.Runtime.Serialization;

namespace DownUnder.UI.Widgets.Behaviors.Visual {
    [DataContract]
    public sealed class MouseGlow : WidgetBehavior {
        public override string[] BehaviorIDs { get; protected set; } = { DownUnderBehaviorIDs.COSMETIC_MID_PERFORMANCE };
        Texture2D _circle;
        Point _position = new Point(0, 0);
        bool _follow;
        readonly ChangingValue<Color> _draw_color = new ChangingValue<Color>(Color.Transparent, InterpolationSettings.Default);

        public enum MouseGlowActivationPolicy {
            /// <summary> Will always glow. </summary>
            always_active,
            /// <summary> Will never glow. </summary>
            disabled,
            /// <summary> Will glow if the cursor is anywhere above the <see cref="Widget"/>'s area. </summary>
            hovered_over,
            /// <summary> (Default) Will only glow when this is the primary hovered widget. </summary>
            primary_hovered
        }

        [DataMember] public Color Color { get; set; } = new Color(70, 70, 70, 255);
        [DataMember] public int Diameter { get; set; } = 1024;
        [DataMember] public InterpolationSettings ShineSpeed { get; set; } = InterpolationSettings.Faster;
        [DataMember] public InterpolationSettings ShineOffSpeed { get; set; } = InterpolationSettings.Default;
        [DataMember] public MouseGlowActivationPolicy ActivationPolicy { get; set; } = primary_hovered;
        [DataMember] public bool ScaleWithSize { get; set; } = true;
        /// <summary> If true this will deactivate if <see cref="Widget.IsActive"/> is false. true by default. </summary>
        [DataMember] public bool RespectActivation { get; set; } = true;

        protected override void Initialize() {
            if (Parent.ParentDWindow == null)
                return;

            LoadImage(Parent, EventArgs.Empty);
        }

        protected override void ConnectEvents() {
            Parent.OnParentWindowSet += LoadImage;
            Parent.OnDrawBackground += DrawImage;
            Parent.OnUpdate += Update;
        }

        protected override void DisconnectEvents() {
            Parent.OnParentWindowSet -= LoadImage;
            Parent.OnDrawBackground -= DrawImage;
            Parent.OnUpdate -= Update;
        }

        public override object Clone() =>
            new MouseGlow {
                Color = Color,
                Diameter = Diameter,
                ShineSpeed = ShineSpeed,
                ShineOffSpeed = ShineOffSpeed,
                ActivationPolicy = ActivationPolicy,
                ScaleWithSize = ScaleWithSize
            };

        void LoadImage(object sender, EventArgs args) =>
            _circle ??= Parent.Content.Load<Texture2D>("DownUnder Native Content/Images/Blurred Circle 512");

        void Update(object sender, EventArgs args) {
            _follow =
                ActivationPolicy == primary_hovered && Parent.IsPrimaryHovered
                || ActivationPolicy == hovered_over && Parent.IsHoveredOver
                || ActivationPolicy == always_active;
            _follow &= Parent.IsActive || !RespectActivation;

            _draw_color.InterpolationSettings = _follow ? ShineSpeed : ShineOffSpeed;

            var color = _follow ? Color : Color.Transparent;
            _draw_color.SetTargetValue(color);

            _draw_color.Update(Parent.UpdateData.ElapsedSeconds);
        }

        void DrawImage(object sender, WidgetDrawArgs args) {
            var diameter = ScaleWithSize
                ? (int)(Diameter * Parent.Size.MaxFloat() / 140)
                : Diameter;

            if (_follow)
                _position = new Point(
                    (int)args.CursorPosition.X - diameter / 2,
                    (int)args.CursorPosition.Y - diameter / 2
                );

            if (_draw_color.Current == Color.Transparent)
                return;

            args.SpriteBatch.Draw(
                _circle,
                new Rectangle(_position.X, _position.Y, diameter, diameter),
                _draw_color.Current
            );
        }

        public static MouseGlow SubtleGray() =>
            new MouseGlow {
                Diameter = 6000,
                Color = new Color(30, 30, 30, 30)
            };
    }
}