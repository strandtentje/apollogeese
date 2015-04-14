using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Graphical
{
    class Rotate : OrderedHyrarchy  
    {
        protected override void Initialize(Settings modSettings)
        {
            
        }

        public override string Description
        {
            get { return "rotation"; }
        }

        public override bool CentralProcess(IFast parameters)
        {
            GL.PushMatrix();
            
            GL.Rotate(45f, Vector3d.UnitZ);

            return true;
        }

        public override bool Finalizer(IFast parameters)
        {
            GL.PopMatrix();

            return true;
        }
    }
}
