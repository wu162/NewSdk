// WARNING: This class was decompiled without detecting uninitialized class fields.
class Cafe2_Imp_BaseControl extends MovieClip
{
   var m_isDynamic = false;
   static var FOCUS_DIR_LR = "Left/Right";
   static var FOCUS_DIR_UD = "Up/Down";
   static var FOCUS_DIR_NONE = "None";
   function Cafe2_Imp_BaseControl()
   {
      super();
      this._visible = false;
      this.m_bHighlighted = false;
      this.m_isLoaded = false;
      this.m_nAssets = 0;
      this.m_nTotalAssets = 0;
      this.m_handledInputs = new Object();
      this.m_cbParam = new Object();
      this.m_soundId = new Object();
      _global.gCT.register(this);
      this.m_objFM = eval(this.m_refFM);
   }
   function show()
   {
      this.m_bVisible = true;
      this._visible = true;
   }
   function hide()
   {
      this.m_bVisible = false;
      this._visible = false;
   }
   function isVisible()
   {
      return this.m_bVisible;
   }
   function enable()
   {
      this.m_bEnabled = true;
      this.enableGUI();
   }
   function disable()
   {
      this.m_bEnabled = false;
      this.disableGUI();
   }
   function isEnabled()
   {
      return this.m_bEnabled;
   }
   function setFocusManager(refFMObj)
   {
      if(refFMObj == undefined && (this.m_focusDirs == FOCUS_DIR_UD || this.m_focusDirs == FOCUS_DIR_LR))
      {
         trace("WARNING: Cafe2_Imp_BaseControl - Invalid focus manager this=" + this + ",m_refFM=" + refFMObj);
         return undefined;
      }
      switch(this.m_focusDirs)
      {
         case FOCUS_DIR_UD:
            this.setOnInput(_global.INPUTCODE.UP,this.bind0(refFMObj,refFMObj.selectPreviousItem));
            this.setOnInput(_global.INPUTCODE.DOWN,this.bind0(refFMObj,refFMObj.selectNextItem));
            return;
         case FOCUS_DIR_LR:
            this.setOnInput(_global.INPUTCODE.LEFT,this.bind0(refFMObj,refFMObj.selectPreviousItem));
            this.setOnInput(_global.INPUTCODE.RIGHT,this.bind0(refFMObj,refFMObj.selectNextItem));
            return;
         case FOCUS_DIR_NONE:
            return;
         default:
            if(this.m_focusDirs != undefined)
            {
               trace("WARNING: Cafe2_Imp_BaseControl - Invalid focus direction property this=" + this + ",m_focusDirs=" + this.m_focusDirs);
            }
            this.m_focusDirs = FOCUS_DIR_NONE;
            return;
      }
   }
   function autoAddToFocusManger()
   {
      if(this.m_bVisible == true)
      {
         this._visible = true;
      }
      if(this.m_tabIndex >= 0)
      {
         this.m_objFM.addItemAtIndex(this,this.m_tabIndex);
         if(this.m_initiallySelected == true)
         {
            this.m_objFM.selectItemByIdx(this.m_tabIndex);
         }
      }
   }
   function isLoaded()
   {
      return this.m_isLoaded;
   }
   function setSelected(bSelected)
   {
      if(bSelected == true && (this.m_bEnabled == false || this.m_bVisible == false))
      {
         trace("WARNING: Cafe2_Imp_BaseControl::setSelected() component disabled || hidden, this=" + this);
      }
      this.m_bHighlighted = bSelected;
      if(this.m_bEnabled == true)
      {
         if(this.m_bHighlighted == true)
         {
            this.highlight();
            this.m_onFocusFunc(this);
         }
         else
         {
            this.unhighlight();
            this.m_onBlurFunc(this);
         }
      }
   }
   function isSelected()
   {
      if(this.m_bHighlighted == true)
      {
         return true;
      }
      return false;
   }
   function setOnChange(fxn)
   {
      this.m_onChangeFunc = fxn;
   }
   function setOnFocus(fxn)
   {
      this.m_onFocusFunc = fxn;
   }
   function setOnBlur(fxn)
   {
      this.m_onBlurFunc = fxn;
   }
   function setOnInput(keyCode, func, param, soundId)
   {
      this.m_handledInputs["KEY_" + keyCode] = func;
      this.m_cbParam["KEY_" + keyCode] = param;
      if(soundId != null)
      {
         this.m_soundId["KEY_" + keyCode] = soundId;
      }
      else
      {
         this.m_soundId["KEY_" + keyCode] = null;
      }
   }
   function handleInput(keyCode)
   {
      if(this.m_bEnabled == false)
      {
         return false;
      }
      var _loc3_ = this.m_handledInputs[String("KEY_" + keyCode)];
      if(String(_loc3_) == "[function]" || _loc3_ != null)
      {
         if(this.m_soundId["KEY_" + keyCode] != null)
         {
            _root.playSound(this.m_soundId["KEY_" + keyCode]);
         }
         if(this.m_cbParam["KEY_" + keyCode] == null)
         {
            _loc3_(this);
         }
         else
         {
            _loc3_(this.m_cbParam["KEY_" + keyCode]);
         }
         return true;
      }
      return false;
   }
   function clearInputHandlers()
   {
      this.m_handledInputs = new Object();
      this.m_cbParam = new Object();
      this.m_soundId = new Object();
   }
   function assetLoaded(strName)
   {
      trace("Cafe2ControlBase::assetLoaded");
      this.m_nAssets = this.m_nAssets + 1;
      if(this.m_nAssets >= this.m_nTotalAssets)
      {
         this.onAllAssetsLoaded();
      }
   }
   function commonInit()
   {
      if(this.m_initiallySelected == false)
      {
         this.unhighlight();
      }
      if(this.m_bVisible == false)
      {
         this.hide();
      }
      if(this.m_bEnabled == false)
      {
         this.disable();
      }
      this.m_isLoaded = true;
      this.autoAddToFocusManger();
      this._parent.onControlLoaded(this);
      this.logic_init();
   }
   static function defaultInitObject()
   {
      var _loc1_ = new Object();
      _loc1_.m_bEnabled = true;
      _loc1_.m_bVisible = true;
      _loc1_.m_focusDirs = FOCUS_DIR_NONE;
      _loc1_.m_isDynamic = true;
      _loc1_.m_refFM = "_root.gFM";
      _loc1_.m_tabIndex = -1;
      _loc1_.m_initiallySelected = false;
      return _loc1_;
   }
   function getWidth()
   {
      if(this.m_width != undefined)
      {
         return this.m_width;
      }
      return this._width;
   }
   function setFocusDirs(focusDirs)
   {
      switch(focusDirs)
      {
         case FOCUS_DIR_UD:
         case FOCUS_DIR_LR:
         case FOCUS_DIR_NONE:
            this.m_focusDirs = focusDirs;
            return;
         default:
            trace("WARNING: Cafe2_Imp_BaseControl::setFocusDirs() invalid focus direction property this=" + this + ",focusDirs=" + focusDirs);
            this.m_focusDirs = FOCUS_DIR_NONE;
            return;
      }
   }
   function bind0(o, f)
   {
      return function()
      {
         f.call(o);
      };
   }
}
