#initclip
class fes_m_skirmishGameSetup extends GameSetupBase
{
   var m_screenButtons;
   var registerIntroOuttroComponents;
   var startIntro;
   function fes_m_skirmishGameSetup(screen)
   {
      super(screen);
      trace("fes_m_skirmishGameSetup::fes_m_skirmishGameSetup() screen = " + screen);
      _global.GAME.MODE = _global.ENUM.GAME_MODE_SKIRMISH;
   }
   function frameDelayedInit()
   {
      fscommand("CallGameFunction","%SetGameSetupMode?Mode=Skirmish");
      super.frameDelayedInit();
      this.initSkirmishGUI();
      this.startIntro();
   }
   function initSkirmishGUI()
   {
      var _loc3_ = Cafe2_BaseUIScreen.m_screen.gameSettings.rulesPanel;
      trace("fes_m_skirmishGameSetup::initSkirmishGUI()");
      _loc3_.commentaryCheckbox.hide();
      _loc3_.commentaryCheckbox.disable();
      _loc3_.enableVoipCheckbox.hide();
      _loc3_.enableVoipCheckbox.disable();
      loadButton.setOnMouseUpFunction(_global.bind0(this,this.onLoadButtonClick));
      loadButton.enable();
      this.m_screenButtons.push(loadButton);
      this.registerIntroOuttroComponents(this.m_screenButtons);
   }
   function onLoadButtonClick()
   {
      _global.GAME.SAVELOADSCREEN_TYPE = _global.ENUM.SAVELOADSCREEN_LOAD;
      _global.gSM.changeMainScreen(_global.SCREEN.FEG_SAVELOAD);
   }
   function onScreenExit()
   {
      trace("fes_m_skirmishGameSetup::onScreenExit()");
      super.onScreenExit();
      delete _global.fes_m_skirmishGameSetup;
   }
}
#endinitclip
