using BannerBuff.Behaviours;
using BannerBuff.TypeDefinitions;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

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

        public void onCastClick()
        {
            var elements = new List<InquiryElement>();
            foreach(var el in BuffManager.Instance.GetRegisteredBuffList())
            {
                elements.Add(new InquiryElement(el.stringID, el.Name, new ImageIdentifier(CharacterCode.CreateFrom(Hero.MainHero.CharacterObject))));
            }            
            var inquiry = new MultiSelectionInquiryData("Cast a spell",
                "Select a spell to cast from the list",
                elements,
                true,
                1,
                "OK",
                "Cancel",
                onPositiveResult,
                onNegativeResult);
            InformationManager.ShowMultiSelectionInquiry(inquiry, true);
        }

        private void onNegativeResult(List<InquiryElement> obj)
        {
            InformationManager.HideInquiry();
        }

        private void onPositiveResult(List<InquiryElement> obj)
        {
            if(obj.Count != 1)
            {
                InformationManager.HideInquiry();
                //Show error message?
            }
            else
            {
                var selectedbuff = obj[0].Identifier as string;
                BuffManager.Instance.AddBuffForHeroAndDuration(Hero.MainHero, selectedbuff, 4f);
                InformationManager.HideInquiry();
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