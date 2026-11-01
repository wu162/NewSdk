#initclip
class fem_m_coopStaging extends Cafe2_BaseUIScreen
{
   var clearData;
   var m_chapterSelectCallback;
   var m_chapterSelectPanel;
   var m_eventHandlerIndex;
   var m_localMessageHandlerFunc;
   var registerIntroOuttroComponents;
   var startIntro;
   var m_initSlot = true;
   static var MAX_MISSIONS = 9;
   function fem_m_coopStaging(screen)
   {
      super(screen);
      trace("fem_m_coopStaging::fem_m_coopStaging()");
      Cafe2_BaseUIScreen.m_thisClass = this;
      this.m_chapterSelectPanel = screen.missionChooserPanelMC.hostPanel.chapterSelectPanel;
   }
   function frameDelayedInit()
   {
      this.m_chapterSelectCallback = _global.bind1DynamicParams(this,this.onChapterSelectChanged);
      this.m_chapterSelectPanel.addObserver(this.m_chapterSelectCallback);
      this.m_localMessageHandlerFunc = _global.bind1DynamicParams(this,this.localMessageHandler);
      _global.gMH.addMessageHandler(this.m_localMessageHandlerFunc);
      this.m_eventHandlerIndex = _global.gEH.addLocalEventHandler(this.localEventHandler);
      _global.GAME.MODE = _global.ENUM.GAME_MODE_CAMPAIGN;
      this.initGUI();
   }
   function initGUI()
   {
      _global.gSM.setOnExitScreen(_global.bind0(this,this.onScreenExit));
      Cafe2_BaseUIScreen.m_screen.button_Back.setOnMouseUpFunction(_global.bind0(this,this.OnAbortButtonClick));
      Cafe2_BaseUIScreen.m_screen.button_Play.setOnMouseUpFunction(_global.bind0(this,this.OnPlayButtonClick));
      Cafe2_BaseUIScreen.m_screen.button_Load.setOnMouseUpFunction(_global.bind0(this,this.OnLoadButtonClick));
      Cafe2_BaseUIScreen.m_screen.button_Back.enable();
      if(_global.GAME.COOPMODE == _global.ENUM.GAME_COOPMODE_HOST)
      {
         Cafe2_BaseUIScreen.m_screen.button_Play.show();
         Cafe2_BaseUIScreen.m_screen.button_Play.enable();
         Cafe2_BaseUIScreen.m_screen.button_Load.show();
         Cafe2_BaseUIScreen.m_screen.button_Load.enable();
         Cafe2_BaseUIScreen.m_screen.missionChooserPanelMC.hostPanel._visible = true;
         Cafe2_BaseUIScreen.m_screen.missionChooserPanelMC.clientPanel._visible = false;
      }
      else
      {
         Cafe2_BaseUIScreen.m_screen.button_Play.hide();
         Cafe2_BaseUIScreen.m_screen.button_Load.hide();
         Cafe2_BaseUIScreen.m_screen.missionChooserPanelMC.hostPanel._visible = false;
         Cafe2_BaseUIScreen.m_screen.missionChooserPanelMC.clientPanel._visible = true;
      }
      this.onChapterSelectChanged();
      this.initCoopCampaignPlayerSlots();
      this.registerIntroOuttroComponents([Cafe2_BaseUIScreen.m_screen.button_Back,Cafe2_BaseUIScreen.m_screen.button_Play,Cafe2_BaseUIScreen.m_screen.button_Load]);
      this.startIntro();
   }
   function localMessageHandler(messageCode)
   {
      var _loc3_ = true;
      switch(messageCode)
      {
         case _global.MSGCODE.FE_COOP_CAMPAIGN_STAGING_STEP_MISSION_BRIEFING:
            _global.gSM.changeMainScreen(_global.SCREEN.FES_COOP_MISSION_BRIEFING);
            break;
         case _global.MSGCODE.FE_MP_UPDATE_GAME_SETTINGS:
            if(this.m_initSlot)
            {
               this.initCoopCampaignPlayerSlots();
               this.m_initSlot = false;
            }
            break;
         default:
            _loc3_ = false;
      }
      return _loc3_;
   }
   function localEventHandler(keyCode, controller)
   {
      if(keyCode !== INPUTCODE.ESCAPE)
      {
         _global.gFM.handleDefaultInputs(keyCode);
      }
      else
      {
         this.OnAbortButtonClick();
      }
   }
   function onScreenExit()
   {
      _global.gMH.removeMessageHandler(this.m_localMessageHandlerFunc);
      delete this.m_localMessageHandlerFunc;
      _global.gEH.removeLocalEventHandler(this.m_eventHandlerIndex);
      this.m_chapterSelectPanel.removeObserver(this.m_chapterSelectCallback);
      delete this.m_chapterSelectCallback;
      this.clearData();
      delete _global.fem_m_coopStaging;
      delete _global.Cafe2_BaseUIScreen;
   }
   function OnAbortButtonClick()
   {
      fscommand("CallGameFunction","Campaign_ExitGame");
      _global.GAME.MODE = _global.ENUM.GAME_MODE_NONE;
      fscommand("CallGameFunction","%SetGameSetupMode?Mode=None");
      _global.gotoShellScreen(_global.SCREEN.FEG_MAIN_MENU,[]);
   }
   function OnPlayButtonClick()
   {
      fscommand("CallGameFunction","%CoopCampaignFlow_MissionBriefing");
      _global.gotoShellScreen(_global.SCREEN.FES_COOP_MISSION_BRIEFING,[]);
   }
   function OnLoadButtonClick()
   {
      var _loc2_ = new Object();
      loadVariables("QueryGameEngine?IS_NAT_DONE",_loc2_);
      if(_loc2_.IS_NAT_DONE == "1")
      {
         _global.GAME.SAVELOADSCREEN_TYPE = _global.ENUM.SAVELOADSCREEN_LOAD;
         _global.gSM.changeMainScreen(_global.SCREEN.FEG_SAVELOAD);
      }
   }
   function onChapterSelectChanged()
   {
      if(_global.GAME.COOPMODE == _global.ENUM.GAME_COOPMODE_HOST)
      {
         fscommand("CallGameFunction","%SetMapType?MapType=1");
         fscommand("CallGameFunction","%SetMap?Map=" + String(this.m_chapterSelectPanel.getSelectedMapId()));
         fscommand("CallGameFunction","%Campaign_SetFaction?FactionIndex=" + String(this.m_chapterSelectPanel.getSelectedCampaign()));
         fscommand("CallGameFunction","%Campaign_SetMission?MissionIndex=" + String(this.m_chapterSelectPanel.getSelectedMission()));
      }
   }
   function initCoopCampaignPlayerSlots()
   {
      var _loc1_ = new Object();
      loadVariables("QueryGameEngine?PLAYER_STATUS?Slot=0&PLAYER_STATUS?Slot=1",_loc1_);
      Cafe2_BaseUIScreen.m_screen.commanderPanelMC.commanderNameTF.text = "$PLAYER_STATUS_0_0";
      Cafe2_BaseUIScreen.m_screen.commanderPanelMC.co_commanderNameTF.text = "$PLAYER_STATUS_1_0";
   }
}
#endinitclip
stop();
