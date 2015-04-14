using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Threading;
using System.Collections.Generic;
using OpenTK.Input;
using Graphical;

namespace BorrehSoft.ApolloGeese.Extensions.Graphical
{
	public class GraphicalView : Service
	{
		public override string Description {
			get {
				return "Creates OpenGL Window";
			}
		}

        private Display Window;
        private float TargetUpdates { get; set; }
        private float TargetFPS { get; set; }
        private Service Updater = Stub, Renderer = Stub;
        private int WindowWidth, WindowHeight;
        private string WindowTitle;

		protected override void Initialize (Settings modSettings)
		{
            TargetUpdates = (float)modSettings.GetInt("updaterate", 60);
            TargetFPS = (float)modSettings.GetInt("framerate", 60);
            WindowWidth = modSettings.GetInt("width", 800);
            WindowHeight = modSettings.GetInt("height", 600);
            WindowTitle = modSettings.GetString("title", "graphics");

            AllBrancesLoaded += GraphicalView_AllBrancesLoaded;
		}

        void GraphicalView_AllBrancesLoaded(object sender, EventArgs e)
        {
            Thread windowThread = new Thread(StartWindow);
            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.Start();
        }

        private void StartWindow()
        {
            using (Window = new Display(UpdateBranch, RenderBranch, WindowWidth, WindowHeight, WindowTitle))
            {
                Window.Run(TargetUpdates, TargetFPS);
            }
        }

        RenderInteraction renderInteraction = new RenderInteraction();

        private void RenderBranch(double time)
        {
            renderInteraction.SetTime(time);
            Renderer.FastProcess(renderInteraction);
        }

        UpdateInteraction updateInteraction = new UpdateInteraction();

        private void UpdateBranch(Queue<KeyboardKeyEventArgs> input, double time)
        {
            updateInteraction.SetTimedelta(time);
            updateInteraction.AppendInteractions(input);
            Updater.FastProcess(updateInteraction);
        }

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
            if (e.Name == "updater")
                this.Updater = e.NewValue;
            if (e.Name == "renderer")
                this.Renderer = e.NewValue;
		}

		protected override bool Process (IInteraction parameters)
		{
			return false;
		}
	}
}

