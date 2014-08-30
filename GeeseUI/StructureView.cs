using System;
using System.Threading;
using System.ComponentModel;
using OpenTK;
using GeeseUI.Graph;

namespace GeeseUI
{
	public class StructureView 
	{
		public Structure Structure { get; private set; }

		public static StructureView Start (Structure structure)
		{			
			StructureView reality = null;

			Semaphore ready = new Semaphore(0, 1);

			Thread windowThread = new Thread(delegate() {				
	            using (GameWindow window = new GameWindow())
	            {
					reality = new StructureView(window, structure);
					ready.Release();	                
	                window.Run(60f);   
	            }
			});

			windowThread.SetApartmentState(ApartmentState.STA);

			windowThread.Start();

			ready.WaitOne();

			return reality;
		}

		public StructureView (GameWindow window, Structure Structure) : base(2048)
		{
			this.Structure = Structure;
		}
	}
}

