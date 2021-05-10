using Terraria.Audio;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Terraria.GameContent.UI.States
{
	public partial class UIWorkshopHub : UIState, IHaveBackButtonCommand
	{
		private UIElement _buttonMods;
		private UIElement _buttonModSources;
		private UIElement _buttonModBrowser;
		private UIElement _buttonTBD;
		private bool _tML;

		private UIElement MakeButton_OpenModsMenu() {
			UIElement uIElement = MakeFancyButton("Images/UI/Workshop/HubResourcepacks", "tModLoader.MenuMods");
			uIElement.OnClick += Click_OpenModsMenu;
			_buttonMods = uIElement;
			return uIElement;
		}

		private void Click_OpenModsMenu(UIMouseEvent evt, UIElement listeningElement) {
			SoundEngine.PlaySound(10);
			Interface.modsMenu.PreviousUIState = this;
			Main.MenuUI.SetState(Interface.modsMenu);
		}

		private UIElement MakeButton_OpenModSourcesMenu() {
			UIElement uIElement = MakeFancyButton("Images/UI/Workshop/HubPublishResourcepacks", "tModLoader.MenuModSources");
			uIElement.OnClick += Click_OpenModSourcesMenu;
			_buttonModSources = uIElement;
			return uIElement;
		}

		private void Click_OpenModSourcesMenu(UIMouseEvent evt, UIElement listeningElement) {
			SoundEngine.PlaySound(10);
			Interface.modSources.PreviousUIState = this;
			Main.MenuUI.SetState(Interface.modSources);
		}

		private UIElement MakeButton_OpenModBrowserMenu() {
			UIElement uIElement = MakeFancyButton("Images/UI/Workshop/HubWorlds", "tModLoader.MenuModBrowser");
			uIElement.OnClick += Click_OpenModBrowserMenu;
			_buttonModBrowser = uIElement;
			return uIElement;
		}

		private void Click_OpenModBrowserMenu(UIMouseEvent evt, UIElement listeningElement) {
			//SoundEngine.PlaySound(10);
			//Interface.modBrowser.PreviousUIState = this;
			//Main.MenuUI.SetState(Interface.modBrowser);
		}

		private UIElement MakeButton_TBD() {
			UIElement uIElement = MakeFancyButton("Images/UI/Workshop/HubPublishWorlds", "ToBeDetermined");
			uIElement.OnClick += Click_OpenTBDMenu;
			_buttonTBD = uIElement;
			return uIElement;
		}

		private void Click_OpenTBDMenu(UIMouseEvent evt, UIElement listeningElement) {
			SoundEngine.PlaySound(10);
		}
	}
}
