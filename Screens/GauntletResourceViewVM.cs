using BannerBuff.Behaviours;
using BannerBuff.TypeDefinitions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace BannerBuff.Screens
{
    public class GauntletResourceViewVM : ViewModel
    {
        private string _resourceText = "Default Text";

        [DataSourceProperty]
        public string ResourceText
        {
            get
            {
                return this._resourceText;
            }
            set
            {
                if (value != this._resourceText)
                {
                    this._resourceText = value;
                    base.OnPropertyChanged("ResourceText");
                }
            }
        }

        public void Refresh()
        {
            if(Hero.MainHero != null)
            {
                var state = ResourceManager.Instance.GetResourceForMainHero();
                if(state != null)
                {
                    this.ResourceText = ResourceState.ResourceName + ": " + state.CurrentAmount.ToString();
                }
            }
        }
    }
}