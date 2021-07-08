﻿//using DownUnder.UI.Widgets.DataTypes.AnimatedGraphics;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Text;

//namespace DownUnder.UI.Widgets.Behaviors.Visual
//{
//    class DrawSwitchGraphic : WidgetBehavior
//    {
//        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.VISUAL_FUNCTION };
//        bool IsToggled = false;

//        public SwitchingGraphic Graphic;

//        public DrawSwitchGraphic() { }
//        public DrawSwitchGraphic(SwitchingGraphic graphic)
//        {
//            Graphic = graphic;
//        }

//        protected override void Initialize()
//        {
//            if (Parent.IsGraphicsInitialized) Initialize(this, EventArgs.Empty);
//        }

//        protected override void ConnectEvents()
//        {
//            Parent.OnGraphicsInitialized += Initialize;
//            Parent.OnUpdate += Update;
//            Parent.OnDraw += Draw;
//            Parent.OnClick += ToggleAnimation;
//        }

//        protected override void DisconnectEvents()
//        {
//            Parent.OnGraphicsInitialized -= Initialize;
//            Parent.OnUpdate -= Update;
//            Parent.OnDraw -= Draw;
//            Parent.OnClick -= ToggleAnimation;
//        }

//        public override object Clone()
//        {
//            var result = new DrawSwitchGraphic();
//            result.Graphic = (AnimatedGraphic)Graphic.Clone();
//            return result;
//        }

//        void Initialize(object sender, EventArgs args)
//        {
//            Graphic.InitializeExternal(Parent.GraphicsDevice);
//        }

//        void Update(object sender, EventArgs args)
//        {
//            Graphic.UpdateExternal(Parent.UpdateData.ElapsedSeconds);
//        }

//        void Draw(object sender, WidgetDrawArgs args)
//        {
//            Graphic.DrawExternal(args);
//        }

//        void ToggleAnimation(object sender, EventArgs args)
//        {
//            if (!IsToggled)
//            {
//                Graphic.Progress.SetTargetValue(1f);
//                IsToggled = true;
//            }
//            else
//            {
//                Graphic.Progress.SetTargetValue(0f);
//                IsToggled = false;
//            }
//        }
//    }
//}
