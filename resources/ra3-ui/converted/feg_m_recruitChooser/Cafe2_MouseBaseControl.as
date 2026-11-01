// WARNING: This class was decompiled without detecting uninitialized class fields.
class Cafe2_MouseBaseControl extends Cafe2_Imp_BaseControl
{
   function Cafe2_MouseBaseControl()
   {
      super();
      this.m_objFM.addItem(this);
      this.m_tabIndex = -1;
      this.m_introComplete = false;
      this.m_outtroComplete = true;
   }
   function setIntroCallback(callBackFunc)
   {
      this.m_animationCompleteCallBackFunc = callBackFunc;
      if(this.m_animationCompleteCallBackFunc != null && this.m_introComplete)
      {
         this.m_animationCompleteCallBackFunc(this);
      }
   }
   function noIntro()
   {
      this.m_introComplete = true;
      this.restoreVisualState();
   }
   function intro()
   {
      this.m_introComplete = false;
      this.m_contentClip.gotoAndPlay("_intro");
   }
   function outtro(callBackFunc)
   {
      this.m_animationCompleteCallBackFunc = callBackFunc;
      this.m_outtroComplete = false;
      if(this.isEnabled())
      {
         this.m_contentClip.gotoAndPlay("_outtro");
      }
      else
      {
         trace("doing the enterframe thing");
         this.onEnterFrame = this.outtroComplete;
      }
   }
   function introComplete()
   {
      this.m_introComplete = true;
      if(this.m_animationCompleteCallBackFunc != null)
      {
         this.m_animationCompleteCallBackFunc(this);
      }
      this.restoreVisualState();
   }
   function isIntroComplete()
   {
      return this.m_introComplete;
   }
   function outtroComplete()
   {
      this.m_outtroComplete = true;
      this.m_animationCompleteCallBackFunc(this);
      this.onEnterFrame = null;
      trace("another outtro done");
   }
   function isOuttroComplete()
   {
      return this.m_outtroComplete;
   }
}
