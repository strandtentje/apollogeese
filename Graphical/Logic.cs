﻿using OpenTK;
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
    internal class Display : GameWindow
    {
        internal delegate void UpdateCallback(Queue<KeyboardKeyEventArgs> input, double time);
        internal delegate void RenderCallback(double time);

        UpdateCallback Update;
        RenderCallback Render;

        internal Display(UpdateCallback Update, RenderCallback Render, int width, int height, string title) : base(
            width, height, 
            new GraphicsMode(
                new ColorFormat(32), 32, 8, 4, 
                new ColorFormat(32), 2), title
        ) {
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
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.ClearColor(0f,0f,0f,0.5f);
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

            this.Render(e.Time);

            this.SwapBuffers();
        }
    }
}
