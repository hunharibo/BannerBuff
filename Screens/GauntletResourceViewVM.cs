using BannerBuff.Services;
using BannerBuff.TypeDefinitions;
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
            this.ResourceText = ResourceType.Magicka.ToString() + ": " + ResourceManager.Instance.GetResourceForMainHero(ResourceType.Magicka).CurrentAmount.ToString();
        }
    }
}
