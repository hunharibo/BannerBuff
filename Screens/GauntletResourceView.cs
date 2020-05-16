using SandBox.View.Map;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;

namespace BannerBuff.Screens
{
    public class GauntletResourceView : MapView
    {

        public GauntletResourceViewVM DataSource { get; private set; }
        public GauntletLayer Layer { get; private set; }
        private GauntletMovie _movie;

        protected override void CreateLayout()
        {
            base.CreateLayout();
            this.DataSource = new GauntletResourceViewVM();
            GauntletLayer gauntletLayer = new GauntletLayer(7999);
            this.Layer = gauntletLayer;
            this._movie = gauntletLayer.LoadMovie("ResourceMapWidget", this.DataSource);
            this.Layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            base.MapScreen.AddLayer(this.Layer);
            this.DataSource.Refresh();
        }

        protected override void OnFinalize()
        {
            base.OnFinalize();
            this.DataSource.OnFinalize();
            base.MapScreen.RemoveLayer(this.Layer);
            this.Layer = null;
        }

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);
            this.DataSource.Refresh();
        }
    }
}
