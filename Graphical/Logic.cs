using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorrehSoft.ApolloGeese.Extensions.Graphical
{
    class Display : GameWindow
    {
        delegate void UpdateCallback(Queue<KeyboardKeyEventArgs> input, double time);
        delegate void RenderCallback(double time);

        UpdateCallback Update;
        RenderCallback Render;

        public Display(UpdateCallback Update, RenderCallback Render) : base(800, 600)
        {
            KeyDown += Display_Key;
            KeyUp += Display_Key;

            this.Update = Update;
            this.Render = Render;
        }

        Queue<KeyboardKeyEventArgs> KeyboardEventQueue = new Queue<KeyboardKeyEventArgs>();

        void Display_Key(object sender, KeyboardKeyEventArgs e)
        {
            KeyboardEventQueue.Enqueue(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color4.MidnightBlue);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0 * ((double)Height / (double)Width), 1.0 * ((double)Height / (double)Width), 0.0, 4.0);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            this.Update(KeyboardEventQueue, e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            this.Render(KeyboardEventQueue, e.Time);

            this.SwapBuffers();
        }
    }
}
