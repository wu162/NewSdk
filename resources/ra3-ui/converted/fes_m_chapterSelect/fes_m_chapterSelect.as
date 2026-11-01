// WARNING: This class was decompiled without detecting uninitialized class fields.
class fes_m_chapterSelect extends Cafe2_BaseUIScreen
{
   function fes_m_chapterSelect(screen)
   {
      super(screen);
      trace("fes_m_chapterSelect::fes_m_chapterSelect()");
      Cafe2_BaseUIScreen.m_thisClass = this;
      this.m_chapterSelectPanel = screen.chapterSelectPanel;
   }
   function frameDelayedInit()
   {
      this.m_localMessageHandlerFunc = _global.bind0(this,this.localMessageHandler);
      _global.gMH.addMessageHandler(this.m_localMessageHandlerFunc);
      this.m_eventHandlerIndex = _global.gEH.addLocalEventHandler(this.localEventHandler);
      _global.gEH.enableLocalEventHandler();
      this.initGUI();
      this.startIntro();
   }
   function initGUI()
   {
      _global.gSM.setOnExitScreen(_global.bind0(this,this.onScreenExit));
      var _loc4_ = new Array();
      var _loc5_ = Cafe2_BaseUIScreen.m_screen.button_Back;
      _loc5_.enable();
      _loc5_.setOnMouseUpFunction(_global.bind0(this,this.OnBackButtonClick));
      _loc4_.push(_loc5_);
      var _loc3_ = Cafe2_BaseUIScreen.m_screen.button_Play;
      _loc3_.enable();
      _loc3_.setOnMouseUpFunction(_global.bind0(this,this.OnPlayButtonClick));
      _loc4_.push(_loc3_);
      this.m_chapterSelectCallback = _global.bind1DynamicParams(this,this.onChapterSelectChanged);
      this.m_chapterSelectPanel.addObserver(this.m_chapterSelectCallback);
      this.registerIntroOuttroComponents(_loc4_);
   }
   function onChapterSelectChanged()
   {
      fscommand("CallGameFunction","%SetMap?Map=" + String(Cafe2_BaseUIScreen.m_screen.chapterSelectPanel.getSelectedMapId()));
      fscommand("CallGameFunction","%Campaign_SetMission?MissionIndex=" + String(this.m_chapterSelectPanel.getSelectedMission()));
   }
   function localMessageHandler(messageCode)
   {
      var _loc1_ = true;
      _loc1_ = false;
      return _loc1_;
   }
   function localEventHandler(keyCode, controller)
   {
      if(keyCode !== INPUTCODE.ESCAPE)
      {
         _global.gFM.handleDefaultInputs(keyCode);
      }
      else
      {
         this.OnBackButtonClick();
      }
   }
   function onScreenExit()
   {
      this.m_chapterSelectPanel.removeObserver(this.m_chapterSelectCallback);
      delete this.m_chapterSelectCallback;
      _global.gMH.removeMessageHandler(this.m_localMessageHandlerFunc);
      delete this.m_localMessageHandlerFunc;
      _global.gEH.removeLocalEventHandler(this.m_eventHandlerIndex);
      Cafe2_BaseUIScreen.m_screen = null;
      Cafe2_BaseUIScreen.m_thisClass = null;
      delete _global.fes_m_chapterSelect;
      delete _global.Cafe2_BaseUIScreen;
   }
   function OnBackButtonClick()
   {
      _global.gSM.goBackScreen();
   }
   function OnPlayButtonClick()
   {
      var _loc3_ = this.m_chapterSelectPanel.getSelectedMission();
      fscommand("CallGameFunction","%LoadAutoSaveSilent?FactionIndex=" + this.m_chapterSelectPanel.getSelectedCampaign());
      fscommand("CallGameFunction","%Campaign_SetMission?MissionIndex=" + _loc3_);
      _global.GAME.COOPMODE = _global.ENUM.GAME_COOPMODE_CHOOSER;
      _global.GAME.PLAY_MISSION_INTRO_MOVIE = true;
      _global.gSM.changeMainScreen(_global.SCREEN.FES_COOP_MISSION_BRIEFING);
   }
   function OnQuickLaunchButtonClick()
   {
      var _loc2_ = this.m_chapterSelectPanel.getSelectedMission();
      fscommand("CallGameFunction","%Campaign_SetMission?MissionIndex=" + _loc2_);
      fscommand("CallGameFunction","%Mission_SetAI?AIIndex=0");
      fscommand("CallGameFunction","%Campaign_SetDifficulty?Difficulty=" + this.m_chapterSelectPanel.getSelectedDifficulty());
      fscommand("CallGameFunction","%Campaign_StartMission");
   }
}
