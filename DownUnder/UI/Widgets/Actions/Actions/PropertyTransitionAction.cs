﻿using DownUnder.Utilities;
using DownUnder.Utility;
using System;
using System.Reflection;

// https://www.youtube.com/watch?v=J4nM-F1kxs8

namespace DownUnder.UI.Widgets.Actions {
    /// <summary> Transition a given property to a given value over time. </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyTransitionAction<T> : WidgetAction {
        private ChangingValue<T> _changing_value;
        private readonly T _target_value;
        private PropertyInfo _property_info;
        private InterpolationSettings _interpolation;

        public readonly string PropertyName;
        public bool IsTransitioning => _changing_value == null ? false : _changing_value.IsTransitioning;

        public PropertyTransitionAction(string nameof_property, T target_value, InterpolationSettings? interpolation = null) {
            PropertyName = nameof_property;
            _target_value = target_value;
            if (interpolation != null) _interpolation = interpolation.Value;
            else _interpolation = new InterpolationSettings(InterpolationType.fake_sin, 1f);

            DuplicateDefinition = DuplicateDefinitionType.matches_result;
            DuplicatePolicy = DuplicatePolicyType.cancel;
        }

        protected override bool InterferesWith(WidgetAction action) => (action is PropertyTransitionAction<T> p_action) ? PropertyName == p_action.PropertyName : false;

        protected override bool Matches(WidgetAction action)
        {
            return action is PropertyTransitionAction<T> action_t
                && action_t._target_value.Equals(_target_value);
        }

        public override object InitialClone() => new PropertyTransitionAction<T>(PropertyName, _target_value);
        
        protected override void ConnectToParent() {
            _property_info = typeof(Widget).GetProperty(PropertyName);
            _changing_value = new ChangingValue<T>((T)_property_info.GetValue(Parent));
            _changing_value.InterpolationSettings = _interpolation;

            _changing_value.SetTargetValue(_target_value);
            Parent.OnUpdate += Update;
        }

        protected override void DisconnectFromParent() {
            Parent.OnUpdate -= Update;
        }

        private void Update(object sender, EventArgs args) {
            if (!_changing_value.IsTransitioning) {
                EndAction();
                return;
            }

            _changing_value.Update(((Widget)sender).UpdateData.ElapsedSeconds);
            _property_info.SetValue(Parent, _changing_value.GetCurrent());
            if (!_changing_value.IsTransitioning) EndAction();
        }
    }
}