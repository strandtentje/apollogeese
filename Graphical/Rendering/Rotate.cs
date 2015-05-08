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
        public float Angle { get; private set; }

        protected override void Initialize(Settings modSettings)
        {
            if (modSettings.Has("default"))
                Angle = modSettings.GetFloat("default");
        }

        public override string Description
        {
            get { return "rotation"; }
        }

        public void SetParameters(float P, float Q, float R)
        {
            this.Angle = R;
        }

        public override bool CentralProcess(IFast parameters)
        {
            GL.PushMatrix();
            
            GL.Rotate(Angle, Vector3d.UnitZ);

            return true;
        }

        public override bool Finalizer(IFast parameters)
        {
            GL.PopMatrix();

            return true;
        }
    }
}
