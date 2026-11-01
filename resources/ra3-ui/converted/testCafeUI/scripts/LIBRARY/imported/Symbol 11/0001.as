#initclip
class Cafe2_BaseUIScreen
{
   var m_introCount;
   static var m_screen;
   static var m_thisClass;
   var m_changeToScreen = null;
   var m_uiComponentArray = new Array();
   function Cafe2_BaseUIScreen()
   {
   }
   function outtroComplete(theComponent)
   {
      trace("Cafe2_BaseUIScreen::outtroComplete() " + theComponent);
      var _loc2_ = 0;
      while(_loc2_ < m_thisClass.m_uiComponentArray.length)
      {
         if(theComponent == m_thisClass.m_uiComponentArray[_loc2_])
         {
            break;
         }
         _loc2_ = _loc2_ + 1;
      }
      m_thisClass.m_uiComponentArray.splice(_loc2_,1);
      if(m_thisClass.m_uiComponentArray.length <= 0)
      {
         if(m_thisClass.m_changeToScreen != null)
         {
            _global.gSM.changeMainScreen(m_thisClass.m_changeToScreen);
         }
      }
   }
   function introComplete(theComponent)
   {
      trace("Cafe2_BaseUIScreen::introComplete()");
      theComponent.restoreVisualState();
      m_thisClass.m_introCount--;
      if(m_thisClass.m_introCount <= 0)
      {
         m_thisClass.onIntrosComplete();
      }
   }
   function onIntrosComplete()
   {
      trace("Cafe2_BaseUIScreen::onIntrosComplete()");
      _global.gEH.setLocalEventHandler(this.localEventHandler);
      _global.gEH.enableCurrentLocalEventHandler();
   }
   function registerIntroOuttroComponents(components)
   {
      trace("Cafe2_BaseUIScreen::registerIntroOuttroComponents()");
      this.m_uiComponentArray = components;
      this.m_introCount = this.m_uiComponentArray.length;
      var _loc2_ = 0;
      while(_loc2_ < this.m_introCount)
      {
         this.m_uiComponentArray[_loc2_].setIntroCallback(this.introComplete);
         _loc2_ = _loc2_ + 1;
      }
   }
   function executeBackFunction(theComponent)
   {
      trace("Cafe2_BaseUIScreen::executeBackFunction" + theComponent);
      var _loc2_ = 0;
      while(_loc2_ < m_thisClass.m_uiComponentArray.length)
      {
         if(theComponent == m_thisClass.m_uiComponentArray[_loc2_])
         {
            break;
         }
         _loc2_ = _loc2_ + 1;
      }
      m_thisClass.m_uiComponentArray.splice(_loc2_,1);
      if(m_thisClass.m_uiComponentArray.length <= 0)
      {
         _global.gSM.goBackScreen();
      }
   }
   function startIntro()
   {
      trace("Cafe2_BaseUIScreen::startIntro()");
      var _loc3_ = this.m_uiComponentArray.length;
      var _loc2_ = 0;
      while(_loc2_ < _loc3_)
      {
         this.m_uiComponentArray[_loc2_].intro();
         _loc2_ = _loc2_ + 1;
      }
   }
   function startOuttro(newScreen)
   {
      trace("Cafe2_BaseUIScreen::startOuttro");
      this.m_changeToScreen = newScreen;
      var _loc3_ = m_thisClass.m_uiComponentArray.length;
      var _loc2_;
      if(_loc3_ > 0)
      {
         _loc2_ = 0;
         while(_loc2_ < _loc3_)
         {
            m_thisClass.m_uiComponentArray[_loc2_].outtro(this.outtroComplete);
            _loc2_ = _loc2_ + 1;
         }
      }
      else
      {
         this.outtroComplete(null);
      }
   }
   function backButtonClicked()
   {
      trace("Cafe2_BaseUIScreen::backButtonClicked()");
      var _loc2_ = m_thisClass.m_uiComponentArray.length;
      var _loc1_ = 0;
      while(_loc1_ < _loc2_)
      {
         m_thisClass.m_uiComponentArray[_loc1_].outtro(m_thisClass.executeBackFunction);
         _loc1_ = _loc1_ + 1;
      }
   }
   function localEventHandler(keyCode, controller)
   {
      trace("Cafe2_BaseUIScreen::localEventHandler() keyCode == " + keyCode);
      _global.gFM.handleDefaultInputs(keyCode);
   }
   function clearData()
   {
      m_screen = null;
      m_thisClass = null;
      this.m_changeToScreen = null;
      this.m_uiComponentArray = null;
   }
}
#endinitclip
