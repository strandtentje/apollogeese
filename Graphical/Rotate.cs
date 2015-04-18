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
    class Rotate : OrderedHyrarchy, I3DParameterized
    {
        protected override void Initialize(Settings modSettings)
        {
            if (modSettings.Has("default"))
                R = modSettings.GetFloat("default");
        }

        public override string Description
        {
            get { return "rotation"; }
        }

        public float P { get; set; }

        public float Q { get; set; }

        public float R { get; set; }

        public override bool CentralProcess(IFast parameters)
        {
            GL.PushMatrix();
            
            GL.Rotate(R, Vector3d.UnitZ);

            return true;
        }

        public override bool Finalizer(IFast parameters)
        {
            GL.PopMatrix();

            return true;
        }
    }
}
