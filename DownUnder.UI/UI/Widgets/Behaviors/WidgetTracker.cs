﻿using System;
using System.Collections.Generic;
using DownUnder.UI.UI.Widgets.CustomEventArgs;
using DownUnder.UI.UI.Widgets.DataTypes;

namespace DownUnder.UI.UI.Widgets.Behaviors
{
    /// <summary> Used by <see cref="WidgetBehavior"/>s to keep track of <see cref="Widgets.Widget"/>s. Call <see cref="Forget"/> on discarding to avoid cluttering used <see cref="Widget"/>s with tag information. </summary>
    public class WidgetTracker {
        private readonly WidgetBehavior _behavior;
        private readonly string _key;
        private readonly string _value;
        private readonly Dictionary<string, EventHandler> _persistent_events = new Dictionary<string, EventHandler>();
        private readonly Dictionary<string, EventHandler<RectangleFSetArgs>> _persistent_resize_events = new Dictionary<string, EventHandler<RectangleFSetArgs>>();
        private readonly Dictionary<string, EventHandler<Point2SetArgs>> _persistent_point2set_events = new Dictionary<string, EventHandler<Point2SetArgs>>();
        private readonly bool _use_tag;
        private Widget _widget;

        public WidgetTracker(WidgetBehavior behavior) {
            _behavior = behavior;
            _use_tag = false;
        }

        public WidgetTracker(WidgetBehavior behavior, string key, string value) {
            _behavior = behavior;
            _key = key;
            _value = value;
            _use_tag = true;
        }

        public Widget Widget {
            get => _widget;
            set {
                if (_widget == value) return;
                if (_widget != null) {
                    RemoveAllPersistentEvents();
                    if (_use_tag) _widget.BehaviorTags[_behavior.GetType()][_key] = null;
                    _behavior.Parent.Remove(_widget);
                }
                _widget = value;
                if (value == null) return;
                AddAllPersistentEvents();
                if (_use_tag) _behavior.SetTag(value, _key, _value);
                _behavior.Parent.Add(value);
            }
        }

        public bool FindIn(WidgetList widgets) {
            if (!_use_tag) throw new Exception("Cannot look for matching tags because tags were not given on construction.");
            for (int i = 0; i < widgets.Count; i++) {
                if (widgets[i].BehaviorTags[_behavior.GetType()][_key] == _value) {
                    Widget = widgets[i];
                    return true;
                }
            }

            return false;
        }

        public void Forget() => Widget = null;

        public void AddPersistentEvent(string nameof_event, Action<object, EventArgs> action)
        {
            if (_persistent_events.TryGetValue(nameof_event, out EventHandler handler)) handler += new EventHandler(action);
            else {
                handler = new EventHandler(action);
                _persistent_events.Add(nameof_event, handler);
            }

            _widget?.GetType().GetEvent(nameof_event).AddEventHandler(_widget, handler);
        }

        public void AddPersistentEvent(string nameof_event, Action<object, RectangleFSetArgs> action)
        {
            if (_persistent_resize_events.TryGetValue(nameof_event, out EventHandler<RectangleFSetArgs> handler)) handler += new EventHandler<RectangleFSetArgs>(action);
            else {
                handler = new EventHandler<RectangleFSetArgs>(action);
                _persistent_resize_events.Add(nameof_event, handler);
            }

            _widget?.GetType().GetEvent(nameof_event).AddEventHandler(_widget, handler);
        }

        public void AddPersistentEvent(string nameof_event, Action<object, Point2SetArgs> action) {
            if (_persistent_point2set_events.TryGetValue(nameof_event, out EventHandler<Point2SetArgs> handler)) handler += new EventHandler<Point2SetArgs>(action);
            else {
                handler = new EventHandler<Point2SetArgs>(action);
                _persistent_point2set_events.Add(nameof_event, handler);
            }

            _widget?.GetType().GetEvent(nameof_event).AddEventHandler(_widget, handler);
        }

        private void AddAllPersistentEvents() {
            foreach (var handler in _persistent_events) _widget.GetType().GetEvent(handler.Key).AddEventHandler(_widget, handler.Value);
            foreach (var handler in _persistent_resize_events) _widget.GetType().GetEvent(handler.Key).AddEventHandler(_widget, handler.Value);
            foreach (var handler in _persistent_point2set_events) _widget.GetType().GetEvent(handler.Key).AddEventHandler(_widget, handler.Value);
        }

        private void RemoveAllPersistentEvents() {
            foreach (var handler in _persistent_events) _widget.GetType().GetEvent(handler.Key).RemoveEventHandler(_widget, handler.Value);
            foreach (var handler in _persistent_resize_events) _widget.GetType().GetEvent(handler.Key).RemoveEventHandler(_widget, handler.Value);
            foreach (var handler in _persistent_point2set_events) _widget.GetType().GetEvent(handler.Key).RemoveEventHandler(_widget, handler.Value);
        }
    }
}
