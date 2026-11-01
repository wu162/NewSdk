#initclip
class fem_m_lanGameSetup extends GameSetupBase
{
   var initPlayButton;
   var initReadyControls;
   var m_screenButtons;
   var onShowMapPanel;
   var onShowRulesPanel;
   var refreshReadyControls;
   var registerIntroOuttroComponents;
   var startIntro;
   function fem_m_lanGameSetup(screen)
   {
      super(screen);
      trace("fem_m_lanGameSetup::fem_m_lanGameSetup() screen = " + screen);
   }
   function frameDelayedInit()
   {
      fscommand("CallGameFunction","%SetGameSetupMode?Mode=Network");
      super.frameDelayedInit();
      this.initReadyControls();
      this.initPlayButton();
      this.refreshComponents();
      this.registerIntroOuttroComponents(this.m_screenButtons);
      this.startIntro();
   }
   function initNavigationButtons(navButtons)
   {
      trace("fem_m_lanGameSetup::initNavigationButtons() ");
      super.initNavigationButtons(navButtons);
      Cafe2_BaseUIScreen.m_screen.gameSettings.mapTabButton.enable();
      Cafe2_BaseUIScreen.m_screen.gameSettings.mapTabButton.show();
      Cafe2_BaseUIScreen.m_screen.gameSettings.mapTabButton.setOnMouseUpFunction(_global.bind0(this,this.onShowMapPanel));
      navButtons.push(Cafe2_BaseUIScreen.m_screen.gameSettings.mapTabButton);
      Cafe2_BaseUIScreen.m_screen.gameSettings.rulesTabButton.enable();
      Cafe2_BaseUIScreen.m_screen.gameSettings.rulesTabButton.show();
      Cafe2_BaseUIScreen.m_screen.gameSettings.rulesTabButton.setOnMouseUpFunction(_global.bind0(this,this.onShowRulesPanel));
      navButtons.push(Cafe2_BaseUIScreen.m_screen.gameSettings.rulesTabButton);
      Cafe2_BaseUIScreen.m_screen.gameSettings.mapTabButton.setSelected(true);
   }
   function onScreenExit()
   {
      trace("fem_m_lanGameSetup::onScreenExit()");
      super.onScreenExit();
      delete _global.fem_m_lanGameSetup;
   }
   function refreshComponents()
   {
      super.refreshComponents();
      this.refreshReadyControls();
   }
}
#endinitclip
