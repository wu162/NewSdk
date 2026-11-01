// WARNING: This class was decompiled without detecting uninitialized class fields.
class feg_m_profileStats extends Cafe2_BaseUIScreen
{
   function feg_m_profileStats(screen)
   {
      super(screen);
      Cafe2_BaseUIScreen.m_thisClass = this;
      trace("------------------------------------------------------------------");
      trace("feg_m_profileStats::feg_m_profileStats()");
   }
   function frameDelayedInit()
   {
      trace("feg_m_profileStats::frameDelayedInit()");
      this.initGUI();
      this.startIntro();
   }
   function initGUI()
   {
      trace("feg_m_profileStats::initGUI()");
      Cafe2_BaseUIScreen.m_screen.btnMainMenu.show();
      Cafe2_BaseUIScreen.m_screen.btnMainMenu.enable();
      Cafe2_BaseUIScreen.m_screen.btnMainMenu.setOnMouseUpFunction(_global.bind0(this,this.onMainMenuClicked));
      this.registerIntroOuttroComponents([Cafe2_BaseUIScreen.m_screen.btnMainMenu]);
      _global.gSM.setOnExitScreen(_global.bind0(this,this.onScreenExit));
   }
   function onScreenExit()
   {
      trace("feg_m_profileStats::onScreenExit()");
      this.clearData();
      delete _global.feg_m_profileStats;
      delete _global.Cafe2_BaseUIScreen;
   }
   function onMainMenuClicked()
   {
      trace("feg_m_profileStats::onMainMenuClicked()");
      this.backButtonClicked();
   }
}
