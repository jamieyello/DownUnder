﻿using System;
using DownUnder.UI.UI.Widgets.CustomEventArgs;
using DownUnder.UI.UI.Widgets.DataTypes.AnimatedGraphics;

namespace DownUnder.UI.UI.Widgets.Behaviors.Visual
{
    public class DrawSwitchGraphic : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.VISUAL_FUNCTION };
        bool IsToggled = false;

        public SwitchingGraphic Graphic;

        public DrawSwitchGraphic() { }
        public DrawSwitchGraphic(SwitchingGraphic graphic)
        {
            Graphic = graphic;
        }

        protected override void Initialize()
        {
            if (Parent.IsGraphicsInitialized) Initialize(this, EventArgs.Empty);
        }

        protected override void ConnectEvents()
        {
            Parent.OnGraphicsInitialized += Initialize;
            Parent.OnUpdate += Update;
            Parent.OnDraw += Draw;
            Parent.OnClick += ToggleAnimation;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnGraphicsInitialized -= Initialize;
            Parent.OnUpdate -= Update;
            Parent.OnDraw -= Draw;
            Parent.OnClick -= ToggleAnimation;
        }

        public override object Clone()
        {
            var result = new DrawSwitchGraphic();
            result.Graphic = (SwitchingGraphic)Graphic.Clone();
            return result;
        }

        void Initialize(object sender, EventArgs args)
        {
            Graphic.Initialize(Parent.GraphicsDevice);
        }

        void Update(object sender, EventArgs args)
        {
            Graphic.Update(Parent.UpdateData.ElapsedSeconds);
        }

        void Draw(object sender, WidgetDrawArgs args)
        {
            Graphic.Draw(args);
        }

        void ToggleAnimation(object sender, EventArgs args)
        {
            if (!IsToggled)
            {
                Graphic.Progress.SetTargetValue(1f);
                IsToggled = true;
            }
            else
            {
                Graphic.Progress.SetTargetValue(0f);
                IsToggled = false;
            }
        }
    }
}
