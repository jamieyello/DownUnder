﻿using System;
using System.Collections;
using System.Collections.Generic;
using DownUnder.UI.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.UI.Widgets.Behaviors.Visual;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using static DownUnder.UI.UI.Widgets.DataTypes.GeneralVisualSettings;

namespace DownUnder.UI.UI.Widgets.Behaviors
{
    public class GroupBehaviorCollection : IList<GroupBehaviorPolicy>
    {
        private List<GroupBehaviorPolicy> _policies = new List<GroupBehaviorPolicy>();

        public GroupBehaviorCollection() { }
        public GroupBehaviorCollection(IEnumerable<GroupBehaviorPolicy> policies)
        {
            foreach (var policy in policies) _policies.Add((GroupBehaviorPolicy)policy.Clone());
        }

        public static GroupBehaviorCollection BasicDesktopFunctions
        {
            get
            {
                var result = new GroupBehaviorCollection
                {
                    new GroupBehaviorPolicy { Behavior = new ApplyInputScrolling() },
                    new GroupBehaviorPolicy { Behavior = new SpawnRightClickDropDown() },
                };
                return result;
            }
        }

        public static GroupBehaviorCollection BasicVisuals
        {
            get
            {
                var result = new GroupBehaviorCollection
                {
                    new GroupBehaviorPolicy() { Behavior = new DrawBackground() },
                    new GroupBehaviorPolicy() { Behavior = new DrawOutline() },
                };
                return result;
            }
        }

        private static GroupBehaviorCollection PlasmaOverrides
        {
            get
            {
                MouseGlow glow = MouseGlow.SubtleGray;
                glow.ActivationPolicy = MouseGlow.MouseGlowActivationPolicy.hovered_over;

                ShadingBehavior blue = ShadingBehavior.SubtleBlue;
                blue.BorderVisibility = 0.3f;
                blue.GradientVisibility = new Point2(0.2f, 0.2f);

                var result = new GroupBehaviorCollection {
                new GroupBehaviorPolicy() { Behavior = new MouseGlow(), NecessaryVisualRole = VisualRoleType.button },
                new GroupBehaviorPolicy() { Behavior = new ShadingBehavior { ShadeColor = Color.White, BorderWidth = 10, BorderVisibility = 0.05f, GradientVisibility = new Point2(0.05f, 0.03f) }, NecessaryVisualRole = VisualRoleType.text_edit_widget },
                new GroupBehaviorPolicy() { Behavior = new BlurBackground(), NecessaryVisualRole = VisualRoleType.pop_up },
                new GroupBehaviorPolicy() { Behavior = glow, NecessaryVisualRole = VisualRoleType.pop_up },
                new GroupBehaviorPolicy() { Behavior = blue, NecessaryVisualRole = VisualRoleType.pop_up },
                new GroupBehaviorPolicy() { Behavior = new Neurons(), NecessaryVisualRole = VisualRoleType.flashy_background },
            };

                return result;
            }
        }

        public static GroupBehaviorCollection PlasmaDesktop => BasicDesktopFunctions.WithOverrides(BasicVisuals.WithOverrides(PlasmaOverrides));

        public GroupBehaviorCollection WithOverrides(IEnumerable<GroupBehaviorPolicy> policies)
        {
            var result = new GroupBehaviorCollection(this);
            foreach (var new_policy in policies)
            {
                while (result.HasConflictWith(new_policy) != -1) result._policies.RemoveAt(HasConflictWith(new_policy));
                result._policies.Add(new_policy);
            }
            return result;
        }

        public int HasConflictWith(GroupBehaviorPolicy policy)
        {
            for (int i = 0; i < _policies.Count; i++)
            {
                if (_policies[i].ConflictsWith(policy)) return i;
            }

            return -1;
        }

        public List<Type> GetBehaviorTypes()
        {
            List<Type> result = new List<Type>();
            for (int i = 0; i < _policies.Count; i++)
            {
                result.Add(_policies[i].Behavior.GetType());
            }
            return result;
        }

        #region IList

        public GroupBehaviorPolicy this[int index] { get => ((IList<GroupBehaviorPolicy>)_policies)[index]; set => ((IList<GroupBehaviorPolicy>)_policies)[index] = value; }

        public int Count => ((ICollection<GroupBehaviorPolicy>)_policies).Count;

        public bool IsReadOnly => ((ICollection<GroupBehaviorPolicy>)_policies).IsReadOnly;

        public void Add(GroupBehaviorPolicy item)
        {
            ((ICollection<GroupBehaviorPolicy>)_policies).Add(item);
        }

        public void Clear()
        {
            ((ICollection<GroupBehaviorPolicy>)_policies).Clear();
        }

        public bool Contains(GroupBehaviorPolicy item)
        {
            return ((ICollection<GroupBehaviorPolicy>)_policies).Contains(item);
        }

        public void CopyTo(GroupBehaviorPolicy[] array, int arrayIndex)
        {
            ((ICollection<GroupBehaviorPolicy>)_policies).CopyTo(array, arrayIndex);
        }

        public IEnumerator<GroupBehaviorPolicy> GetEnumerator()
        {
            return ((IEnumerable<GroupBehaviorPolicy>)_policies).GetEnumerator();
        }

        public int IndexOf(GroupBehaviorPolicy item)
        {
            return ((IList<GroupBehaviorPolicy>)_policies).IndexOf(item);
        }

        public void Insert(int index, GroupBehaviorPolicy item)
        {
            ((IList<GroupBehaviorPolicy>)_policies).Insert(index, item);
        }

        public bool Remove(GroupBehaviorPolicy item)
        {
            return ((ICollection<GroupBehaviorPolicy>)_policies).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<GroupBehaviorPolicy>)_policies).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_policies).GetEnumerator();
        }

        #endregion
    }
}
