﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DownUnder.UI.Widgets.DataTypes.AnimatedGraphics {
    public sealed class SwitchingGraphic : ICloneable {
        public ChangingValue<float> Progress { get; private set; } = new ChangingValue<float>(0f);
        public bool IsInitialized { get; private set; }
        BasicEffect basicEffect;

        readonly VertexPositionColor[] StartingVertex;
        readonly VertexPositionColor[] EndingVertex;

        ChangingValue<Color> color = new ChangingValue<Color>(Color.White);

        public SwitchingGraphic(
            VertexPositionColor[] starting_vertex,
            VertexPositionColor[] ending_vertex
        ) {
            StartingVertex = starting_vertex;
            EndingVertex = ending_vertex;
        }

        public void Initialize(GraphicsDevice gd) {
            basicEffect = new BasicEffect(gd);
            IsInitialized = true;
        }

        internal void Update(float step) {
            Progress.Update(step);
            color.Update(step);
        }

        internal void Draw(WidgetDrawArgs args) {
            var progress = Progress.Current;

            var vert = new VertexPositionColor[StartingVertex.Length];
            for (var i = 0; i < vert.Length; i++)
                vert[i] = StartingVertex[i].Lerp(EndingVertex[i], progress);

            basicEffect.Projection = args.GetStretchedProjection();

            args.RestartImmediate();

            foreach (var effectPass in basicEffect.CurrentTechnique.Passes) {
                effectPass.Apply();
                args.GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList, vert, 0, vert.Length / 3);
            }

            args.RestartDefault();
        }

        public void SetStateStart() =>
            Progress.SetTargetValue(0f);

        public void SetStateEnd() =>
            Progress.SetTargetValue(1f);

        static VertexPositionColor[] PauseVertex(float pause_gap, Color? color = null) {
            var vert = new VertexPositionColor[12];
            var pg = pause_gap / 2f;

            // TL
            vert[0].Position = new Vector3(-1f, -1f, 0f);
            vert[1].Position = new Vector3(-1f, 1f, 0f);
            vert[2].Position = new Vector3(-pg, 1f, 0f);

            // BL
            vert[3].Position = new Vector3(-1f, -1f, 0f);
            vert[4].Position = new Vector3(-pg, 1f, 0f);
            vert[5].Position = new Vector3(-pg, -1f, 0f);

            // TR
            vert[8].Position = new Vector3(1f, -1f, 0f);
            vert[7].Position = new Vector3(1f, 1f, 0f);
            vert[6].Position = new Vector3(pg, 1f, 0f);

            // BR
            vert[11].Position = new Vector3(1f, -1f, 0f);
            vert[10].Position = new Vector3(pg, 1f, 0f);
            vert[9].Position = new Vector3(pg, -1f, 0f);

            for (var i = 0; i < vert.Length; i++)
                vert[i].Color = color ?? Color.White;

            return vert;
        }

        static VertexPositionColor[] PlayVertex(Color? color = null) {
            var vert = new VertexPositionColor[12];

            // TL
            vert[0].Position = new Vector3(-1f, -1f, 0f);
            vert[1].Position = new Vector3(-1f, 1f, 0f);
            vert[2].Position = new Vector3(1f, 0f, 0f);

            // BL
            vert[3].Position = new Vector3(-1f, -1f, 0f);
            vert[4].Position = new Vector3(1f, 0f, 0f);
            vert[5].Position = new Vector3(-1f, 1f, 0f);

            // TR
            vert[8].Position = new Vector3(1f, -1f, 0f);
            vert[7].Position = new Vector3(1f, 1f, 0f);
            vert[6].Position = new Vector3(1f, 1f, 0f);

            // BR
            vert[11].Position = new Vector3(1f, -1f, 0f);
            vert[10].Position = new Vector3(1f, 1f, 0f);
            vert[9].Position = new Vector3(1f, -1f, 0f);

            for (var i = 0; i < vert.Length; i++)
                vert[i].Color = color ?? Color.White;

            return vert;
        }

        public static SwitchingGraphic PausePlayGraphic(float pause_gap = 0.8f) {
            var result = new SwitchingGraphic(PauseVertex(pause_gap), PlayVertex());
            result.Progress.InterpolationSettings = InterpolationSettings.Faster;
            return result;
        }

        object ICloneable.Clone() =>
            Clone();

        public SwitchingGraphic Clone() {
            var starting_vertex_c = new VertexPositionColor[StartingVertex.Length];
            for (var i = 0; i < StartingVertex.Length; i++)
                starting_vertex_c[i] = StartingVertex[i];

            var ending_vertex_c = new VertexPositionColor[EndingVertex.Length];
            for (var i = 0; i < EndingVertex.Length; i++)
                ending_vertex_c[i] = EndingVertex[i];

            return new SwitchingGraphic(starting_vertex_c, ending_vertex_c) {
                color = color.Clone(true),
                Progress = Progress.Clone(true)
            };
        }
    }
}
