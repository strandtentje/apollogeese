using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Graphical
{
    class Rotate : OrderedHyrarchy, I3DParameterized
    {
        public float Angle { get; private set; }

		public override void LoadDefaultParameters (object defaultParameter)
		{
			this.Settings ["angle"] = defaultParameter;
		}

        protected override void Initialize(Settings modSettings)
        {
            Angle = modSettings.GetFloat("angle");
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
