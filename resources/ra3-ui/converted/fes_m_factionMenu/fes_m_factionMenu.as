// WARNING: This class was decompiled without detecting uninitialized class fields.
class fes_m_factionMenu extends Cafe2_BaseUIScreen
{
   function fes_m_factionMenu(screen)
   {
      super(screen);
      Cafe2_BaseUIScreen.m_thisClass = this;
      _global.gSM.setOnExitScreen(_global.bind0(this,this.onScreenExit));
   }
   function frameDelayedInit()
   {
      trace("fes_m_factionMenu::frameDelayedInit()");
      this.initGUI();
   }
   function initGUI()
   {
      this.m_localMessageHandlerFunc = _global.bind1DynamicParams(this,this.localMessageHandler);
      _global.gMH.addMessageHandler(this.m_localMessageHandlerFunc);
      Cafe2_BaseUIScreen.m_screen.sovietFaction.show();
      Cafe2_BaseUIScreen.m_screen.sovietFaction.enable();
      Cafe2_BaseUIScreen.m_screen.sovietFaction.noIntro();
      Cafe2_BaseUIScreen.m_screen.sovietFaction.setOnMouseDownFunction(_global.bind0(this,this.sovietFactionButtonClicked));
      Cafe2_BaseUIScreen.m_screen.alliesFaction.show();
      Cafe2_BaseUIScreen.m_screen.alliesFaction.enable();
      Cafe2_BaseUIScreen.m_screen.alliesFaction.setOnMouseDownFunction(_global.bind0(this,this.alliesFactionButtonClicked));
      Cafe2_BaseUIScreen.m_screen.alliesFaction.noIntro();
      Cafe2_BaseUIScreen.m_screen.japaneseFaction.show();
      Cafe2_BaseUIScreen.m_screen.japaneseFaction.enable();
      Cafe2_BaseUIScreen.m_screen.japaneseFaction.setOnMouseDownFunction(_global.bind0(this,this.japaneseFactionButtonClicked));
      Cafe2_BaseUIScreen.m_screen.japaneseFaction.noIntro();
      Cafe2_BaseUIScreen.m_screen.backButton.show();
      Cafe2_BaseUIScreen.m_screen.backButton.enable();
      Cafe2_BaseUIScreen.m_screen.backButton.setOnMouseUpFunction(_global.bind0(this,this.onBackButtonClick));
      Cafe2_BaseUIScreen.m_screen.backButton.noIntro();
      Cafe2_BaseUIScreen.m_screen.playButton.show();
      Cafe2_BaseUIScreen.m_screen.playButton.enable();
      Cafe2_BaseUIScreen.m_screen.playButton.setOnMouseUpFunction(_global.bind0(this,this.requestNewCampaign));
      Cafe2_BaseUIScreen.m_screen.playButton.noIntro();
      Cafe2_BaseUIScreen.m_screen.factionButtonController.SetSelectedRadioButton(Cafe2_BaseUIScreen.m_screen.sovietFaction);
      this.sovietFactionButtonClicked();
   }
   function onScreenExit()
   {
      trace("fes_m_factionMenu::onScreenExit()");
      _global.gMH.removeMessageHandler(this.m_localMessageHandlerFunc);
      delete this.m_localMessageHandlerFunc;
      this.clearData();
      delete _global.fes_m_factionMenu;
      delete _global.Cafe2_BaseUIScreen;
   }
   function sovietFactionButtonClicked()
   {
      trace("fes_m_factionMenu::sovietFactionButtonClicked()");
      _global.GAME.MODE = _global.ENUM.GAME_MODE_CAMPAIGN;
      _global.GAME.CAMPAIGN_TYPE = _global.ENUM.CAMPAIGN_TYPE_SOVIET;
      factionDescriptionTF.text = "$SOVIET_FACTION_DESCRIPTION&Outline";
      factionDescriptionShadowTF.text = "$SOVIET_FACTION_DESCRIPTION";
   }
   function alliesFactionButtonClicked()
   {
      trace("fes_m_factionMenu::alliesFactionButtonClicked()");
      _global.GAME.MODE = _global.ENUM.GAME_MODE_CAMPAIGN;
      _global.GAME.CAMPAIGN_TYPE = _global.ENUM.CAMPAIGN_TYPE_ALLIES;
      factionDescriptionTF.text = "$ALLIES_FACTION_DESCRIPTION&Outline";
      factionDescriptionShadowTF.text = "$ALLIES_FACTION_DESCRIPTION";
   }
   function japaneseFactionButtonClicked()
   {
      trace("fes_m_factionMenu::japaneseFactionButtonClicked()");
      _global.GAME.MODE = _global.ENUM.GAME_MODE_CAMPAIGN;
      _global.GAME.CAMPAIGN_TYPE = _global.ENUM.CAMPAIGN_TYPE_RISINGSUN;
      factionDescriptionTF.text = "$JAPANESE_FACTION_DESCRIPTION&Outline";
      factionDescriptionShadowTF.text = "$JAPANESE_FACTION_DESCRIPTION";
   }
   function requestNewCampaign()
   {
      trace("fes_m_factionMenu::requestNewCampaign()" + _global.GAME.CAMPAIGN_TYPE);
      fscommand("CallGameFunction","%Campaign_SetFaction?FactionIndex=" + _global.GAME.CAMPAIGN_TYPE);
      fscommand("CallGameFunction","%Campaign_NewGame");
   }
   function goToTutorialMenu()
   {
      gotoShellScreen(_global.SCREEN.FEG_TUTORIALMENU,[_global.SCREEN.FEG_MAIN_MENU]);
   }
   function localMessageHandler(messageCode)
   {
      var _loc3_ = false;
      if(messageCode === _global.MSGCODE.FE_PROCEED_TO_TUTORIAL)
      {
         this.goToTutorialMenu();
         _loc3_ = true;
      }
      return _loc3_;
   }
   function onBackButtonClick()
   {
      fscommand("CallGameFunction","%Campaign_ExitGame");
      this.backButtonClicked();
   }
}
