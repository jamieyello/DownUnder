﻿using System;

namespace DownUnder.UI.UI.Widgets.Behaviors
{
    /// <summary> A <see cref="WidgetBehavior"/> that has <see cref="ISubWidgetBehavior"/>s that are modify this <see cref="WidgetBehavior"/>. </summary>
    public interface IEditorDisplaySubBehaviors
    {
        Type[] BaseBehaviorPreviews { get; }
    }
}
