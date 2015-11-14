using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;

namespace Graphical
{
    class Poly : OrderedHyrarchy
    {
        VertexList vertices;
        VertexBuffer vbo;
        bool isReset = true;
        string name;
        static int shapecount = 0;
        
        protected override void Initialize(Settings modSettings)
        {
            object vertexObjectListObject = modSettings.Get("vertices");
            IEnumerable<object> vertexObjectList = (IEnumerable<object>)vertexObjectListObject;
            vertices = new VertexList();

            foreach (object vertexObject in vertexObjectList)            
                vertices.Add((Settings)vertexObject);

            this.name = modSettings.GetString("name", (shapecount++).ToString());
        }

        public override string Description
        {
            get { return this.name; }
        }

        public override bool CentralProcess(IFast parameters)
        {
            if (isReset)
            {
                this.vbo = vertices.GetVBO();
                isReset = false;
            }

            vbo.Render();
            
            return true;
        }
    }
}
