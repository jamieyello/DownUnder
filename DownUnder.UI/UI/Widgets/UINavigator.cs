﻿using System.Collections.Generic;
using DownUnder.UI.UI.Widgets.DataTypes;
using DownUnder.UI.Utilities.CommonNamespace;

namespace DownUnder.UI.UI.Widgets
{
    public class UINavigator
    {
        DWindow parent;

        Stack<Widget> underlying_widgets = new Stack<Widget>();

        public WidgetTransitionAnimation DefaultTransition = WidgetTransitionAnimation.Slide(Direction2D.left);
        public WidgetTransitionAnimation DefaultBackTransition = WidgetTransitionAnimation.Slide(Direction2D.right);

        public UINavigator(DWindow parent)
        {
            this.parent = parent;
        }

        public void NavigateTo(Widget widget)
        {
            underlying_widgets.Push(parent.DisplayWidget);
            parent.DisplayWidget.AnimatedReplace(widget, DefaultTransition, false);
        }

        public void NavigateBack()
        {
            if (underlying_widgets.Count == 0) return;
            parent.DisplayWidget.AnimatedReplace(underlying_widgets.Pop(), DefaultBackTransition);
        }
    }
}
