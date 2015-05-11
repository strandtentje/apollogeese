using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

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
