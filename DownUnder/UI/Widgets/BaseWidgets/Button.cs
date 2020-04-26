﻿using System;
using DownUnder.UI.Widgets.DataTypes;
using DownUnder.UI.Widgets.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DownUnder.UI.Widgets.BaseWidgets {
    public class Button : Widget {
        private bool _disable_update_area = false;
        private WidgetList _children_backing = new WidgetList();
        private Texture2D _image_backing;
        
        /// <summary> If set to true the area of this <see cref="Button"/> will encompass all content within. </summary>
        public bool FitAreaToContent { get; set; } = true;
        public Label Label { get; private set; }
        public float ImageEdgeSpace { get; set; } = 20f;

        public string Text {
            get => Label.Text;
            set => Label.Text = value;
        }
        
        /// <summary> The image to be displayed in this <see cref="Button"/>. (if any) </summary>
        public Texture2D Image {
            get => _image_backing;
            set {
                _image_backing = value;
                AlignContent();
            }
        }
        
        public Button(IParent parent = null) : base(parent) => SetDefaults();
        private void SetDefaults() {
            Label = new Label(this, "New Button") {
                PassthroughMouse = true,
                ConstrainAreaToText = true,
                DrawBackground = false,
                DrawOutline = false
            };
            _children_backing.Add(Label);
            
            OnDraw += DrawImage;
        }

        public void AlignContent() {
            Label.Area = Label.Area.WithCenter(Area.SizeOnly());
            //Console.WriteLine($"Set label area to {Label.Area}");
        }

        private void DrawImage(object sender, EventArgs args) {
            if (Image == null) return;
            SpriteBatch.Draw(
                Image, 
                Image.Bounds.ToRectangleF().FittedIn(DrawingArea, ImageEdgeSpace).ToRectangle(true), 
                Color.White);
        }
        
        public override WidgetList Children => _children_backing;
        protected override void HandleChildRemoval(Widget widget) => throw new NotImplementedException();

        protected override object DerivedClone() {
            Button c = new Button();
            c.FitAreaToContent = FitAreaToContent;
            c.ImageEdgeSpace = ImageEdgeSpace;
            c.Image = _image_backing;
            return c;
        }

        internal override void SignalChildAreaChanged() {
            if (_disable_update_area) return;
            bool _previous_disable_update_area = _disable_update_area;
            _disable_update_area = true;

            AlignContent();

            _disable_update_area = _previous_disable_update_area;
            base.SignalChildAreaChanged();
        }
        
        public override Point2 MinimumSize {
            get => base.MinimumSize;
            set => base.MinimumSize = value;
        }
    }
}
