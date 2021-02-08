﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DownUnder.UI.UI.Widgets.CustomEventArgs;

namespace DownUnder.UI.UI.Widgets.Behaviors
{
    [DataContract] public class BehaviorManager : IEnumerable<WidgetBehavior>
    {
        [DataMember] public Widget Parent { get; set; }
        [DataMember] List<WidgetBehavior> _behaviors = new List<WidgetBehavior>();
        [DataMember] public GroupBehaviorManager GroupBehaviors { get; private set; }

        public BehaviorFinder Common;

        public BehaviorManager()
        {
            Common = new BehaviorFinder(this);
        }

        public BehaviorManager(Widget parent)
        {
            Common = new BehaviorFinder(this);
            Parent = parent;
            GroupBehaviors = new GroupBehaviorManager(parent);
        }

        [OnDeserialized]
        void Deserialize(StreamingContext context)
        {
            Common = new BehaviorFinder(this);
        }

        public WidgetBehavior this[int index] { get => _behaviors[index]; set => throw new NotImplementedException(); }

        public bool HasBehaviorOfType(Type type) {
            foreach (WidgetBehavior behavior in this) {
                if (behavior.GetType() == type) return true;
            }

            return false;
        }

        public T Get<T>() where T : WidgetBehavior {
            foreach (WidgetBehavior behavior in this) {
                if (behavior.GetType() == typeof(T)) return (T)behavior;
            }

            return default;
        }

        public List<Type> GetTypes()
        {
            List<Type> result = new List<Type>();
            foreach (WidgetBehavior b in _behaviors) result.Add(b.GetType());
            return result;
        }

        public int Count => _behaviors.Count;

        public bool IsReadOnly => ((IList<WidgetBehavior>)_behaviors).IsReadOnly;

        public void Add(WidgetBehavior behavior) {
            if (!TryAdd(behavior)) throw new Exception($"Cannot add {nameof(WidgetBehavior)}s to this {nameof(WidgetBehavior)}.");
        }

        public void Add<T>(T behavior, out T added_behavior) where T : WidgetBehavior {
            if (!TryAdd(behavior, out added_behavior)) throw new Exception($"Cannot add {nameof(WidgetBehavior)}s to this {nameof(WidgetBehavior)}.");
        }

        public void Insert(int index, WidgetBehavior behavior)
        {
            if (!TryInsert(index, behavior)) throw new Exception($"Cannot add {nameof(WidgetBehavior)}s to this {nameof(WidgetBehavior)}.");
        }

        public void Insert<T>(int index, T behavior, out T added_behavior) where T : WidgetBehavior
        {
            if (!TryInsert(index, behavior, out added_behavior)) throw new Exception($"Cannot add {nameof(WidgetBehavior)}s to this {nameof(WidgetBehavior)}.");
        }

        public bool TryAdd(WidgetBehavior behavior)
        {
            return TryInsert(_behaviors.Count, behavior, out var _);
        }

        public bool TryAdd<T>(T behavior, out T added_behavior) where T : WidgetBehavior
        {
            return TryInsert(_behaviors.Count, behavior, out added_behavior);
        }

        public bool TryInsert(int index, WidgetBehavior behavior)
        {
            return TryInsert(index, behavior, out var _);
        }

        public bool TryInsert<T>(int index, T behavior, out T added_behavior) where T : WidgetBehavior
        {
            if (Contains(behavior.GetType()))
            {
                added_behavior = null;
                return false;
            }
            if (behavior.IsSubBehavior
                && !Contains(behavior.BaseBehaviorType)) {
                TryAdd((WidgetBehavior)Activator.CreateInstance(behavior.BaseBehaviorType));
            }
            _behaviors.Insert(index, behavior);
            behavior.Parent = Parent;
            added_behavior = behavior;
            Parent?.InvokeOnAddBehavior(new WidgetBehaviorArgs(added_behavior));
            return true;
        }

        public void AddRange(IEnumerable<WidgetBehavior> behaviors) {
            foreach (WidgetBehavior behavior in behaviors) Add(behavior);
        }

        public void Clear() {
            foreach (WidgetBehavior behavior in _behaviors) behavior.Disconnect();
            _behaviors.Clear();
        }

        public bool Contains(Type type) {
            for (int i = 0; i < _behaviors.Count; i++) {
                if (_behaviors[i].GetType() == type) return true;
            }

            return false;
        }
        public bool Contains(WidgetBehavior behavior) => _behaviors.Contains(behavior);
        public void CopyTo(WidgetBehavior[] array, int arrayIndex) => _behaviors.CopyTo(array, arrayIndex);
        public IEnumerator<WidgetBehavior> GetEnumerator() => _behaviors.GetEnumerator();
        public int IndexOf(WidgetBehavior behavior) => _behaviors.IndexOf(behavior);

        public void RemoveAt(int index)
        {
            WidgetBehavior removed = _behaviors[index];
            removed.Disconnect();
            _behaviors.RemoveAt(index);
            Parent?.InvokeOnRemoveBehavior(new WidgetBehaviorArgs(removed));
        }

        public bool Remove(WidgetBehavior behavior) {
            if (_behaviors.Contains(behavior)) {
                behavior.Disconnect();
                _behaviors.Remove(behavior);
                Parent?.InvokeOnRemoveBehavior(new WidgetBehaviorArgs(behavior));
                return true;
            }
            return false;
        }

        public bool RemoveType(Type type) {
            foreach (WidgetBehavior behavior in this) {
                if (behavior.GetType() == type) {
                    return Remove(behavior);
                }
            }

            return false;
        }

        /// <summary> Remove all <see cref="WidgetBehavior"/>s with the given <see cref="WidgetBehavior.BehaviorIDs"/>. </summary>
        /// <param name="behavior_id"> The ID <see cref="string"/> to look for. </param>
        /// <returns> A <see cref="List{WidgetBehavior}"/> of removed <see cref="WidgetBehavior"/>s. (if any) </returns>
        public List<WidgetBehavior> RemoveIDed(string behavior_id) {
            var removed = new List<WidgetBehavior>();
            for (int i = 0; i < _behaviors.Count; i++) {
                if (_behaviors[i].BehaviorIDs.Contains(behavior_id)) {
                    removed.Add(_behaviors[i]);
                    _behaviors.RemoveAt(i--);
                }
            }
            return removed;
        }

        /// <summary> Remove all <see cref="WidgetBehavior"/>s with the given <see cref="WidgetBehavior.BehaviorIDs"/>. </summary>
        /// <param name="behavior_ids"> The <see cref="string"/> <see cref="IEnumerable"/> IDs to look for. </param>
        /// <returns> A <see cref="List{WidgetBehavior}"/> of removed <see cref="WidgetBehavior"/>s. (if any) </returns>
        public List<WidgetBehavior> RemoveIDed(IEnumerable<string> behavior_ids) {
            var removed = new List<WidgetBehavior>();
            foreach (string behavior_id in behavior_ids) removed.AddRange(RemoveIDed(behavior_id));
            return removed;
        }

        IEnumerator IEnumerable.GetEnumerator() => _behaviors.GetEnumerator();
    }
}
