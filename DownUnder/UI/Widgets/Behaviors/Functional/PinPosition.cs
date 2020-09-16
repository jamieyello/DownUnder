﻿using DownUnder.UI.Widgets.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Behaviors.Functional
{
    public class PinPosition : WidgetBehavior
    {
        public override string[] BehaviorIDs { get; protected set; } = new string[] { DownUnderBehaviorIDs.FUNCTION };

        public InnerWidgetLocation Pin { get; set; }

        protected override void Initialize()
        {
        }

        protected override void ConnectEvents()
        {
            Parent.OnParentWidgetSet += ApplyPin;
            Parent.OnParentResize += ApplyPin;
        }

        protected override void DisconnectEvents()
        {
            Parent.OnParentWidgetSet -= ApplyPin;
            Parent.OnParentResize -= ApplyPin;
        }

        public override object Clone()
        {
            PinPosition c = new PinPosition();
            c.Pin = (InnerWidgetLocation)Pin.Clone();
            return c;
        }

        private void ApplyPin(object sender, EventArgs args)
        {
            Pin.ApplyLocation(Parent.ParentWidget, Parent);
        }
    }
}
