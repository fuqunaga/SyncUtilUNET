using UnityEngine;
using System.Collections.Generic;

namespace LocalClustering
{
    [RequireComponent(typeof(Camera))]
    public class DebugDraw : MonoBehaviour
    {
        #region type define
        public abstract class Primitive<Data>
        {
            protected abstract string shaderName { get; }
            
            public void Add(Data data) { _datas.Add(data); }

            Material _material;
            protected Material material { get { return _material ?? (_material = new Material(Shader.Find(shaderName))); } }
            protected List<Data> _datas = new List<Data>();
        }



        public class Line : Primitive<Line.Data>
        {
            protected override string shaderName { get { return "Custom/LocalCulster/SimpleColor"; } }

            public class Data
            {
                public Color col;
                public float width;
                public Vector3 startPos;
                public Vector3 endPos;
            }

            public void Draw()
            {
                _datas.ForEach(data =>
                {
                    material.SetColor("_Color", data.col);
                    material.SetPass(0);

                    GL.Begin(GL.QUADS);

                    DrawLine(data.startPos, data.endPos, data.width);

                    GL.End();
                });

                _datas.Clear();
            }

            void DrawLine(Vector3 v0, Vector3 v1, float lineWidth)
            {
                Vector3 n = ((new Vector3(v1.y, v0.x, 0.0f)) - (new Vector3(v0.y, v1.x, 0.0f))).normalized * lineWidth;
                GL.Vertex3(v0.x - n.x, v0.y - n.y, v0.z);
                GL.Vertex3(v0.x + n.x, v0.y + n.y, v0.z);
                GL.Vertex3(v1.x + n.x, v1.y + n.y, v1.z);
                GL.Vertex3(v1.x - n.x, v1.y - n.y, v1.z);
            }
        }


        public class Circle : Primitive<Circle.Data>
        {
            protected override string shaderName { get { return "Custom/LocalCulster/SimpleCircle"; } }

            public class Data
            {
                public Vector3 pos;
                public float radius;
                public Color col;
            }


            protected void DrawBase(float aspect)
            {
                _datas.ForEach(data =>
                {
                    var pos = data.pos;
                    var heightHalf = data.radius;
                    var widthHalf = heightHalf / aspect;

                    material.SetColor("_Color", data.col);
                    material.SetPass(0);

                    GL.Begin(GL.QUADS);

                    GL.TexCoord(new Vector3(0, 0, 0));
                    GL.Vertex3(pos.x - widthHalf, pos.y - heightHalf, pos.z);

                    GL.TexCoord(new Vector3(1, 0, 0));
                    GL.Vertex3(pos.x + widthHalf, pos.y - heightHalf, pos.z);

                    GL.TexCoord(new Vector3(1, 1, 0));
                    GL.Vertex3(pos.x + widthHalf, pos.y + heightHalf, pos.z);

                    GL.TexCoord(new Vector3(0, 1, 0));
                    GL.Vertex3(pos.x - widthHalf, pos.y + heightHalf, pos.z);

                    GL.End();
                });
                _datas.Clear();
            }

            public void Draw()
            {
                DrawBase(1f);
            }

            public void Draw2D(float aspect)
            {
                DrawBase(aspect);
            }
        }
        #endregion


        Line _line = new Line();
        Line _line2D = new Line();
        Circle _circle = new Circle();
        Circle _circle2D = new Circle();


        #region Ortho

        public void LineStripOn2D(Vector2[] lineStrip, Color col, float width = 1f)
        {
            for (var i = 0; i < lineStrip.Length - 1; ++i)
            {
                LineOn2D(lineStrip[i], lineStrip[i + 1], col, width);
            }
        }


        public void LineOn2D(Vector2 start, Vector2 end, Color col, float width = 1f)
        {
            _line2D.Add(new Line.Data()
            {
                col = col,
                width = 1.0f / Screen.width * width * 0.5f, // pixel to viewSpace
                startPos = start,
                endPos = end
            });
        }

        public void Rect(Rect rect, Color col, float width = 1f)
        {
            LineStripOn2D(
                new Vector2[]
                {
                    new Vector2(rect.xMin, rect.yMin),
                    new Vector2(rect.xMin, rect.yMax),
                    new Vector2(rect.xMax, rect.yMax),
                    new Vector2(rect.xMax, rect.yMin),
                    new Vector2(rect.xMin, rect.yMin),
                },
                col,
                width
            );
        }

        public void CircleOn2D(Vector2 pos, float radius, Color col)
        {
            _circle2D.Add(new Circle.Data()
            {
                pos = pos,
                radius = radius,
                col = col
            });
        }

        #endregion


        #region View

        [System.Diagnostics.Conditional("DEBUG")]
        void CheckValidOnView(Vector3 p)
        {
            if (p.z < GetComponent<Camera>().nearClipPlane) Debug.LogWarning("Z < nearClipPlane. Will NOT draw. " + p);
        }

        public void LineOnLocal(Vector3 start, Vector3 end, Color col, float width = 1f)
        {
            CheckValidOnView(start);
            CheckValidOnView(end);

            _line.Add(new Line.Data()
            {
                col = col,
                width = width,
                startPos = start,
                endPos = end
            });
        }


        public void CircleOnLocal(Vector3 pos, float radius, Color col)
        {
            CheckValidOnView(pos);
            _circle.Add(new Circle.Data() { pos = pos, radius = radius, col = col });
        }

        #endregion


        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            float aspect = (float)source.width / source.height;

            Graphics.Blit(source, destination);
            Graphics.SetRenderTarget(destination);

            GL.PushMatrix();
            GL.LoadOrtho();
            _circle2D.Draw2D(aspect);
            _line2D.Draw();
            GL.PopMatrix();

            GL.PushMatrix();
            GL.LoadIdentity();
            GL.MultMatrix(Matrix4x4.Scale(new Vector3(1f, 1f, -1f))); // Z反転
            _circle.Draw();
            _line.Draw();
            GL.PopMatrix();

        }
    }
}
