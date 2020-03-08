﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DownUnder.UI.Widgets.DataTypes;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets.BaseWidgets
{
    public class SpacedList : Layout
    {
        #region Private Fields

        private bool _disable_update_area = false;

        #endregion

        public float ListSpacing { get; set; } = 30f;

        public SpacedList(Widget parent = null)
            : base(parent)
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            OnListChange += UpdateArea;
            EmbedChildren = false;
        }

        public override RectangleF Area
        {
            get => base.Area;
            set
            {
                base.Area = value;
                _disable_update_area = true;
                _widgets.AlignHorizontalWrap(area_backing.Width, false, ListSpacing);
                _disable_update_area = false;
            }
        }

        private void UpdateArea(object sender, EventArgs args)
        {
            UpdateArea(true);
        }

        protected override void UpdateArea(bool update_parent)
        {
            if (_disable_update_area) return;
            _widgets.AlignHorizontalWrap(area_backing.Width, debug_output, ListSpacing);
            base.UpdateArea(update_parent);
        }
    }
}
