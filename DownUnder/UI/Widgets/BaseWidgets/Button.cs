﻿using System;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets.BaseWidgets
{
    public class Button : Widget
    {
        #region Private Fields
        
        private bool _disable_update_area = false;
        private WidgetList _children_backing = new WidgetList();
        private Texture2D _image_backing;

        #endregion

        #region Public Properties
        
        /// <summary> If set to true the area of this <see cref="Button"/> will encompass all content within. </summary>
        public bool FitAreaToContent { get; set; } = true;
        
        public string Text
        {
            get => Label.Text;
            set => Label.Text = value;
        }

        public Label Label { get; private set; }

        public float ImageEdgeSpace { get; set; } = 20f;

        /// <summary> The image to be displayed in this <see cref="Button"/>. (if any) </summary>
        public Texture2D Image
        {
            get => _image_backing;
            set
            {
                _image_backing = value;
                AlignContent();
            }
        }

        #endregion

        #region Constructors

        public Button(IParent parent)
            : base(parent)
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            Label = new Label(this, "New Button")
            {
                PassthroughMouse = true,
                ConstrainAreaToText = true,
                DrawBackground = false,
                DrawOutline = false
            };
            _children_backing.Add(Label);
            
            if (IsGraphicsInitialized) InitializeGraphics(this, EventArgs.Empty);
            else OnGraphicsInitialized += InitializeGraphics;

            OnDraw += DrawImage;
        }

        #endregion

        #region Private Methods
        
        #endregion

        #region Public Methods

        public void AlignContent()
        {
            Label.Area = Label.Area.WithCenter(Area);
        }

        #endregion

        #region Events

        private void InitializeGraphics(object sender, EventArgs args)
        {
            
        }

        private void DrawImage(object sender, EventArgs args)
        {
            if (Image != null)
            {
                SpriteBatch.Draw(
                    Image, 
                    Image.Bounds.ToRectangleF().FittedIn(DrawingArea, ImageEdgeSpace).ToRectangle(), 
                    Color.White);
            }
        }

        #endregion

        #region Overrides

        public override WidgetList Children => _children_backing;
        
        protected override object DerivedClone(Widget parent)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateArea(bool update_parent)
        {
            if (_disable_update_area) return;
            bool _previous_disable_update_area = _disable_update_area;
            _disable_update_area = true;

            AlignContent();

            _disable_update_area = _previous_disable_update_area;
            base.UpdateArea(update_parent);
        }

        public override Point2 MinimumSize
        {
            get => base.MinimumSize;
            set => base.MinimumSize = value;
        }

        #endregion
    }
}