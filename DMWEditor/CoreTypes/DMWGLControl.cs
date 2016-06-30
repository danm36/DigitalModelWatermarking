using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMWEditor.CoreTypes
{
    class DMWGLControl : GLControl
    {
        public DMWGLControl()
            : base(new GraphicsMode(new ColorFormat(32), 24, 0, 8))
        {
            Instance = this;
        }

        public bool Loaded { get; private set; }
        public static DMWGLControl Instance { get; private set; }

        const float zNear = 0.01f;
        const float zFar = 2048.0f;
        const float FOV = 1.3333f;

        Matrix4 PVMMatrix, projectionMatrix, viewMatrix;
        Vector3 cameraLocation = new Vector3(0.0f, 1.0f, -6.0f), cameraFocus = new Vector3(0, 0, 0);

        protected override void OnLoad(EventArgs e)
        {
            if (DesignMode)
                return;

            GL.ClearColor(Color.SkyBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Cw);

            Application.Idle += (s, ea) =>
            {
                Invalidate();
            };

            Loaded = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!Loaded)
            {
                base.OnPaint(e);
                e.Graphics.FillRectangle(new LinearGradientBrush(new Point(0, 0), new Point(0, Height), Color.AliceBlue, Color.SkyBlue), new Rectangle(0, 0, Width, Height));
                return;
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if(Mainform.currentlyLoadedModel != null)
                Mainform.currentlyLoadedModel.Draw();

            SwapBuffers();
        }

        public void UpdateMatrices()
        {
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FOV, (float)Width / (float)Height, zNear, zFar);

            cameraFocus.Y += 0.00001f; //Prevents weird NaN error

            viewMatrix = Matrix4.LookAt(cameraLocation, cameraFocus, new Vector3(0.0f, 1.0f, 0.0f));

            PVMMatrix = viewMatrix * projectionMatrix; //* model;

            for (int i = 0; i < Shader.loadedShaders.Count; i++)
            {
                GL.UseProgram(Shader.loadedShaders[i].shaderProgramHandle);
                GL.UniformMatrix4(Shader.loadedShaders[i].uPVMMatrixLocation, false, ref PVMMatrix);
            }
        }

        bool mouseDown = false;
        Point mouseDownLocation, mouseLastLocation;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            mouseDown = true;
            mouseDownLocation = mouseLastLocation = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            mouseDown = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!mouseDown)
                return;

            Vector3 delta = new Vector3(e.X - mouseLastLocation.X, -(e.Y - mouseLastLocation.Y), 0);
            Vector3 deltaTransformed = Vector3.Transform(delta, viewMatrix.ExtractRotation().Inverted());
            float deltaMod = (float)Math.Max((cameraLocation - cameraFocus).LengthSquared / 4096.0, 0.05f);

            if ((ModifierKeys & Keys.Control) != 0)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    delta.Z = delta.Y;
                    delta.Y = 0;

                    deltaTransformed = Vector3.Transform(delta, viewMatrix.ExtractRotation().Inverted());
                }

                cameraLocation += deltaTransformed * deltaMod;
                cameraFocus += deltaTransformed * deltaMod;
            }
            else
            {
                deltaTransformed = (deltaTransformed * -1) / 32;

                float locFocDist = (cameraLocation - cameraFocus).Length;
                Vector3 direction = (cameraLocation - cameraFocus).Normalized();
                direction += deltaTransformed / 4;

                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    cameraFocus = cameraLocation + (direction * -1) * locFocDist;
                else
                    cameraLocation = cameraFocus + direction * locFocDist;
            }

            UpdateMatrices();
            mouseLastLocation = e.Location;
        }


        protected override void OnMouseEnter(EventArgs e)
        {
            Focus();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if(Focused)
                Parent.Focus();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            float locFocDist = (cameraLocation - cameraFocus).Length;

            locFocDist = Math.Max(locFocDist - (float)e.Delta / 120 / (1 / (locFocDist / 50)), 0.01f);

            Vector3 direction = (cameraLocation - cameraFocus).Normalized();
            cameraLocation = cameraFocus + direction * locFocDist;

            UpdateMatrices();
        }
    }
}
