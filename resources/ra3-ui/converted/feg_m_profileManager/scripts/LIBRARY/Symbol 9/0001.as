#initclip
class Cafe2_BaseUIScreen
{
   var m_introCount;
   static var m_screen;
   static var m_thisClass;
   var m_changeToScreen = null;
   var m_executeOuttroCallBack = null;
   var m_uiComponentArray = new Array();
   function Cafe2_BaseUIScreen(screen)
   {
      m_screen = screen;
      m_screen.onEnterFrame = _global.bind0(this,this.frameDelayedInitCallback);
   }
   function frameDelayedInit()
   {
      trace("\n\nCafe2_BaseUIScreen::frameDelayedInitCallback() :");
      var _loc1_ = new Array("N.B. You *must* override frameDelayedInit() in your","\nscreens and move the call to initGUI() to frameDelayedInit().\n");
      trace(_loc1_[0] + _loc1_[1]);
   }
   function frameDelayedInitCallback()
   {
      m_screen.onEnterFrame = null;
      this.frameDelayedInit();
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
   function startOuttroAndCallBack(doAfterOuttroComplete)
   {
      trace("Cafe2_BaseUIScreen::startOuttro");
      this.m_executeOuttroCallBack = doAfterOuttroComplete;
      var _loc3_ = m_thisClass.m_uiComponentArray.length;
      var _loc2_;
      if(_loc3_ > 0)
      {
         _loc2_ = 0;
         while(_loc2_ < _loc3_)
         {
            m_thisClass.m_uiComponentArray[_loc2_].outtro(this.outtroAndCallBackComplete);
            _loc2_ = _loc2_ + 1;
         }
      }
      else
      {
         this.outtroAndCallBackComplete(null);
      }
   }
   function outtroAndCallBackComplete(theComponent)
   {
      trace("Cafe2_BaseUIScreen::outtroComplete() " + theComponent);
      var _loc1_ = 0;
      while(_loc1_ < m_thisClass.m_uiComponentArray.length)
      {
         if(theComponent == m_thisClass.m_uiComponentArray[_loc1_])
         {
            break;
         }
         _loc1_ = _loc1_ + 1;
      }
      m_thisClass.m_uiComponentArray.splice(_loc1_,1);
      if(m_thisClass.m_uiComponentArray.length <= 0)
      {
         m_thisClass.m_executeOuttroCallBack();
      }
   }
   function backButtonClicked()
   {
      trace("Cafe2_BaseUIScreen::backButtonClicked()");
      var _loc3_ = m_thisClass.m_uiComponentArray.length;
      var _loc2_;
      if(_loc3_ > 0)
      {
         _loc2_ = 0;
         while(_loc2_ < _loc3_)
         {
            m_thisClass.m_uiComponentArray[_loc2_].outtro(m_thisClass.executeBackFunction);
            _loc2_ = _loc2_ + 1;
         }
      }
      else
      {
         _global.gSM.goBackScreen();
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
   function bind0(obj, func)
   {
      return function()
      {
         func.call(obj);
      };
   }
}
#endinitclip
