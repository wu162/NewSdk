// WARNING: This class was decompiled without detecting uninitialized class fields.
class feg_m_profileManager extends Cafe2_BaseUIScreen implements IListboxEventListener
{
   var m_profiles = new Array();
   var m_profileTokens = new Array();
   function feg_m_profileManager(screen)
   {
      super(screen);
      Cafe2_BaseUIScreen.m_screen = screen;
      Cafe2_BaseUIScreen.m_thisClass = this;
      Cafe2_BaseUIScreen.m_screen.newProfileMC._visible = false;
   }
   function frameDelayedInit()
   {
      trace("feg_m_profileManager::frameDelayedInit()");
      this.m_localMessageHandler = _global.bind1DynamicParams(this,this.localMessageHandler);
      _global.gMH.addMessageHandler(this.m_localMessageHandler);
      _global.gSM.setOnExitScreen(_global.bind0(this,this.onScreenExit));
      this.initGUI();
      this.startIntro();
   }
   function OnListboxIndexMouseClick(index, previouslySelectedIndex)
   {
   }
   function OnListboxIndexElementMouseClick(index, element, previouslySelectedIndex)
   {
      this.OnProfileClicked(index);
   }
   function initGUI()
   {
      var _loc3_ = new Array();
      _loc3_.push(this.configureButton(Cafe2_BaseUIScreen.m_screen.profileMC.btnNewProfile,_global.bind0(this,this.onNewProfile)));
      _loc3_.push(this.configureButton(Cafe2_BaseUIScreen.m_screen.profileMC.btnDeleteProfile,_global.bind0(this,this.onDeleteProfile)));
      _loc3_.push(this.configureButton(Cafe2_BaseUIScreen.m_screen.profileMC.btnAcceptProfile,_global.bind0(this,this.onSetProfile)));
      _loc3_.push(this.configureButton(Cafe2_BaseUIScreen.m_screen.newProfileMC.btnNewCancel,_global.bind0(this,this.onNewProfileCancel)));
      _loc3_.push(this.configureButton(Cafe2_BaseUIScreen.m_screen.newProfileMC.btnNewConfirm,_global.bind0(this,this.onNewProfileConfirm)));
      _loc3_.push(this.configureButton(Cafe2_BaseUIScreen.m_screen.btnMainMenu,_global.bind0(this,this.onMainMenuClicked)));
      Cafe2_BaseUIScreen.m_screen.newProfileMC.newProfileName.setOnEnterFunction(_global.bind0(this,this.onNewProfileConfirm));
      Cafe2_BaseUIScreen.m_screen.profileMC.txtProfileList.setNumEntries(6);
      Cafe2_BaseUIScreen.m_screen.profileMC.txtProfileList.setVerticalPadding(0);
      Cafe2_BaseUIScreen.m_screen.profileMC.txtProfileList.addListener(this);
      var _loc4_ = new Array();
      _loc4_.push({symbol:"std_mouseListboxEntryTextElementSymbol",width:523,leftMargin:2,rightMargin:0,useHitRegion:false});
      Cafe2_BaseUIScreen.m_screen.profileMC.txtProfileList.init(_loc4_,27);
      Cafe2_BaseUIScreen.m_screen.profileMC.txtProfileList.attachScrollbarController(Cafe2_BaseUIScreen.m_screen.profileMC.sldrProfiles);
      Cafe2_BaseUIScreen.m_screen.profileMC.btnAcceptProfile.disable();
      Cafe2_BaseUIScreen.m_screen.profileMC.btnDeleteProfile.disable();
      this.updateProfileList();
      this.updateCurrentProfile();
      this.highlightCurrentProfile();
      Cafe2_BaseUIScreen.m_screen.profileMC.txtProfileList.moveViewToFirst();
      Cafe2_BaseUIScreen.m_screen.profileMC.txtProfileList.refreshDisplay();
      Cafe2_BaseUIScreen.m_screen.profileMC.txtProfileList.refreshScrollbar();
      Cafe2_BaseUIScreen.m_screen.profileMC._visible = true;
      Cafe2_BaseUIScreen.m_screen.newProfileMC._visible = false;
      this.registerIntroOuttroComponents(_loc3_);
   }
   function configureButton(button, callback)
   {
      button.show();
      button.enable();
      button.setOnMouseUpFunction(callback);
      return button;
   }
   function localMessageHandler(messageCode)
   {
      var _loc3_ = false;
      switch(messageCode)
      {
         case _global.MSGCODE.FE_PROFILE_CREATE_SUCCESS:
            Cafe2_BaseUIScreen.m_screen.newProfileMC._visible = false;
            Cafe2_BaseUIScreen.m_screen.profileMC._visible = true;
            this.updateProfileList();
            this.updateCurrentProfile();
            this.highlightCurrentProfile();
            _loc3_ = true;
            break;
         case _global.MSGCODE.FE_PROFILE_CREATE_FAIL:
            _loc3_ = true;
            break;
         case _global.MSGCODE.FE_PROFILE_REFRESH_LIST:
            this.updateProfileList();
            this.highlightCurrentProfile();
            _loc3_ = true;
      }
      return _loc3_;
   }
   function updateProfileList()
   {
      var _loc3_ = new Object();
      loadVariables("QueryGameEngine?FE_PROFILE_LIST",_loc3_);
      this.m_profiles = _loc3_.FE_PROFILE_LIST.split(",");
      this.m_profileTokens.splice(0,this.m_profileTokens.length);
      var _loc2_ = 0;
      while(_loc2_ < this.m_profiles.length)
      {
         this.m_profileTokens.push(this.m_profiles[_loc2_] + "&Outline");
         _loc2_ = _loc2_ + 1;
      }
      Cafe2_BaseUIScreen.m_screen.profileMC.txtProfileList.setColumnData(0,this.m_profileTokens);
      Cafe2_BaseUIScreen.m_screen.profileMC.txtProfileList.refreshDisplay();
      Cafe2_BaseUIScreen.m_screen.profileMC.txtProfileList.refreshScrollbar();
   }
   function updateCurrentProfile()
   {
      var _loc1_ = new Object();
      loadVariables("QueryGameEngine?FE_CURRENT_PROFILE",_loc1_);
      Cafe2_BaseUIScreen.m_screen.profileMC.txtProfileName.text = _loc1_.FE_CURRENT_PROFILE;
   }
   function highlightCurrentProfile()
   {
      var _loc1_ = new Object();
      loadVariables("QueryGameEngine?FE_CURRENT_PROFILE_INDEX",_loc1_);
      var _loc2_ = _loc1_.FE_CURRENT_PROFILE_INDEX;
      Cafe2_BaseUIScreen.m_screen.profileMC.txtProfileList.setSelectedIndex(_loc2_);
      Cafe2_BaseUIScreen.m_screen.profileMC.btnAcceptProfile.disable();
      Cafe2_BaseUIScreen.m_screen.profileMC.btnDeleteProfile.disable();
   }
   function onMainMenuClicked()
   {
      trace("feg_m_profileManager::onMainMenuClicked()");
      this.backButtonClicked();
   }
   function onScreenExit()
   {
      _global.gMH.removeMessageHandler(this.m_localMessageHandler);
      delete this.m_localMessageHandler;
      Cafe2_BaseUIScreen.m_screen.txtProfileList.removeListener(this);
      Cafe2_BaseUIScreen.m_screen.txtProfileList.shutdown();
      this.clearData();
      delete _global.feg_m_profileManager;
      delete _global.Cafe2_BaseUIScreen;
   }
   function onNewProfile()
   {
      Cafe2_BaseUIScreen.m_screen.profileMC._visible = false;
      Cafe2_BaseUIScreen.m_screen.newProfileMC._visible = true;
      Cafe2_BaseUIScreen.m_screen.newProfileMC.newProfileName.takeFocus();
   }
   function onDeleteProfile()
   {
      var _loc2_ = Cafe2_BaseUIScreen.m_screen.profileMC.txtProfileList.getSelectedEntryIndex();
      var _loc3_ = this.m_profiles[_loc2_].slice(1);
      fscommand("CallGameFunction","%DeleteProfile?ProfileName=" + _loc3_);
   }
   function onSetProfile()
   {
      var _loc3_ = Cafe2_BaseUIScreen.m_screen.profileMC.txtProfileList.getSelectedEntryIndex();
      var _loc4_ = this.m_profiles[_loc3_].slice(1);
      fscommand("CallGameFunction","%SetProfileByToken?ProfileToken=" + _loc4_);
      this.updateProfileList();
      this.updateCurrentProfile();
      Cafe2_BaseUIScreen.m_screen.profileMC._visible = false;
      _global.gSM.goBackScreen();
   }
   function onNewProfileCancel()
   {
      this.m_newProfileName = "";
      Cafe2_BaseUIScreen.m_screen.newProfileMC._visible = false;
      Cafe2_BaseUIScreen.m_screen.newProfileMC.newProfileName.setContents("");
      Cafe2_BaseUIScreen.m_screen.profileMC._visible = true;
   }
   function onNewProfileConfirm()
   {
      this.m_newProfileName = Cafe2_BaseUIScreen.m_screen.newProfileMC.newProfileName.getContents();
      fscommand("CallGameFunction","%AddProfile?ProfileName=" + this.m_newProfileName);
      Cafe2_BaseUIScreen.m_screen.newProfileMC.newProfileName.setContents("");
   }
   function OnProfileClicked(index)
   {
      var _loc1_ = new Object();
      loadVariables("QueryGameEngine?FE_CURRENT_PROFILE_INDEX&FE_PROFILE_COUNT",_loc1_);
      if(index >= _loc1_.FE_PROFILE_COUNT)
      {
         return undefined;
      }
      var _loc2_ = _loc1_.FE_CURRENT_PROFILE_INDEX;
      if(index == _loc2_)
      {
         Cafe2_BaseUIScreen.m_screen.profileMC.btnAcceptProfile.disable();
         Cafe2_BaseUIScreen.m_screen.profileMC.btnDeleteProfile.disable();
      }
      else
      {
         Cafe2_BaseUIScreen.m_screen.profileMC.btnAcceptProfile.enable();
         Cafe2_BaseUIScreen.m_screen.profileMC.btnDeleteProfile.enable();
      }
   }
}
