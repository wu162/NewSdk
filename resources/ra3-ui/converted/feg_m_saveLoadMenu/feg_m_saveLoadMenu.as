// WARNING: This class was decompiled without detecting uninitialized class fields.
class feg_m_saveLoadMenu extends Cafe2_BaseUIScreen implements IListboxEventListener
{
   static var SAVEMODE_REPLAY = 0;
   static var SAVEMODE_SKIRMISH = 1;
   static var SAVEMODE_CAMPAIGN = 2;
   static var SAVEMODE_CAMPAIGNMP = 3;
   static var SAVEFILTER_ANY = 0;
   static var SAVEFILTER_USER = 1;
   static var SAVEFILTER_AUTO = 2;
   var m_fileNames = new Array();
   var m_fileMaps = new Array();
   var m_fileTimes = new Array();
   var m_fileDates = new Array();
   var m_fileIds = new Array();
   var m_selectedSaveIndex = -1;
   function feg_m_saveLoadMenu(screen)
   {
      super();
      trace("feg_m_saveLoadMenu::feg_m_saveLoadMenu() screen = " + screen);
      Cafe2_BaseUIScreen.m_screen = screen;
      Cafe2_BaseUIScreen.m_thisClass = this;
      switch(_global.GAME.MODE)
      {
         case _global.ENUM.GAME_MODE_CAMPAIGN:
         case _global.ENUM.GAME_MODE_TUTORIAL:
            if(_global.GAME.COOPMODE == _global.ENUM.GAME_COOPMODE_HOST || _global.GAME.COOPMODE == _global.ENUM.GAME_COOPMODE_CLIENT)
            {
               feg_m_saveLoadMenu.m_saveMode = SAVEMODE_CAMPAIGNMP;
            }
            else
            {
               feg_m_saveLoadMenu.m_saveMode = SAVEMODE_CAMPAIGN;
            }
            break;
         case _global.ENUM.GAME_MODE_SKIRMISH:
            feg_m_saveLoadMenu.m_saveMode = SAVEMODE_SKIRMISH;
            break;
         case _global.ENUM.GAME_MODE_MULTIPLAYER:
            feg_m_saveLoadMenu.m_saveMode = SAVEMODE_CAMPAIGNMP;
            break;
         case _global.ENUM.GAME_MODE_SHELL:
         case _global.ENUM.GAME_MODE_NONE:
            feg_m_saveLoadMenu.m_saveMode = SAVEMODE_REPLAY;
            break;
         default:
            trace("feg_m_saveLoadMenu::feg_m_saveLoadMenu() Bad _global.GAME.MODE encountered.");
      }
      if(_global.gSM.getLastScreenId() == _global.SCREEN.FEM_MP_POSTGAME_ROUNDUP)
      {
         feg_m_saveLoadMenu.m_saveMode = SAVEMODE_REPLAY;
      }
      Cafe2_BaseUIScreen.m_thisClass.initGUI();
   }
   function OnListboxIndexMouseClick(index, previouslySelectedIndex)
   {
      var _loc3_;
      if(this.m_fileIds[index] != undefined)
      {
         trace("OnListboxIndexMouseClick::m_selectedSaveIndex::" + this.m_selectedSaveIndex + "::m_fileIds[index]::" + this.m_fileIds[index]);
         _loc3_ = new Object();
         loadVariables("QueryGameEngine?LASTMOUSEDOUBLECLICK",_loc3_);
         if(_loc3_.LASTMOUSEDOUBLECLICK != 0)
         {
            if(_global.GAME.SAVELOADSCREEN_TYPE == _global.ENUM.SAVELOADSCREEN_SAVE)
            {
               this.OnSaveButtonClick();
            }
            else
            {
               this.OnLoadButtonClick();
            }
         }
         this.m_selectedSaveIndex = this.m_fileIds[index];
         if(this.m_selectedSaveIndex != -1)
         {
            Cafe2_BaseUIScreen.m_screen.button_Delete.enable();
            if(_global.GAME.SAVELOADSCREEN_TYPE == _global.ENUM.SAVELOADSCREEN_LOAD)
            {
               Cafe2_BaseUIScreen.m_screen.button_SaveLoad.enable();
            }
         }
         else
         {
            Cafe2_BaseUIScreen.m_screen.button_Delete.disable();
            if(_global.GAME.SAVELOADSCREEN_TYPE == _global.ENUM.SAVELOADSCREEN_LOAD)
            {
               Cafe2_BaseUIScreen.m_screen.button_SaveLoad.disable();
            }
         }
         if(_global.GAME.SAVELOADSCREEN_TYPE == _global.ENUM.SAVELOADSCREEN_SAVE)
         {
            this.SetSaveNameTextfieldContents(this.m_selectedSaveIndex);
         }
      }
   }
   function OnListboxIndexElementMouseClick(index, element, previouslySelectedIndex)
   {
   }
   function initGUI()
   {
      trace("feg_m_saveLoadMenu::initGUI()");
      _global.gSM.setOnExitScreen(this.onScreenExit);
      _global.gMH.addMessageHandler(this.localMessageHandler);
      this.m_eventHandlerIndex = _global.gEH.addLocalEventHandler(Cafe2_BaseUIScreen.m_thisClass.localEventHandler);
      _global.gEH.enableLocalEventHandler();
      if(_global.GAME.SAVELOADSCREEN_TYPE == _global.ENUM.SAVELOADSCREEN_SAVE)
      {
         Cafe2_BaseUIScreen.m_screen.button_SaveLoad.setText("$APT:SAVE");
         if(feg_m_saveLoadMenu.m_saveMode == SAVEMODE_REPLAY)
         {
            Cafe2_BaseUIScreen.m_screen.titleBarMC.setTitle("$APT:HEADERSAVEREPLAY",true,true);
         }
         else
         {
            Cafe2_BaseUIScreen.m_screen.titleBarMC.setTitle("$APT:HEADERSAVEGAME",true,true);
         }
         Cafe2_BaseUIScreen.m_screen.button_SaveLoad.SetOnMouseUpFunction(_global.bind0(this,this.OnSaveButtonClick));
         feg_m_saveLoadMenu.m_saveFilter = SAVEFILTER_USER;
         Cafe2_BaseUIScreen.m_screen.txt_saveFileName.show();
         Cafe2_BaseUIScreen.m_screen.SaveBottomImg._visible = true;
         Cafe2_BaseUIScreen.m_screen.addCommentaryPanel._visible = false;
         Cafe2_BaseUIScreen.m_screen.saveFileList.setNumEntries(17);
         this.SetSaveNameTextfieldContents(this.m_selectedSaveIndex);
      }
      else
      {
         Cafe2_BaseUIScreen.m_screen.button_SaveLoad.setText("$APT:LOAD",true,true);
         if(feg_m_saveLoadMenu.m_saveMode == SAVEMODE_REPLAY)
         {
            Cafe2_BaseUIScreen.m_screen.titleBarMC.setTitle("$APT:HEADERLOADREPLAY",true,true);
            Cafe2_BaseUIScreen.m_screen.addCommentaryPanel._visible = true;
            Cafe2_BaseUIScreen.m_screen.saveFileList.setNumEntries(19);
         }
         else
         {
            Cafe2_BaseUIScreen.m_screen.titleBarMC.setTitle("$APT:HEADERLOADGAME",true,true);
            Cafe2_BaseUIScreen.m_screen.addCommentaryPanel._visible = false;
            Cafe2_BaseUIScreen.m_screen.saveFileList.setNumEntries(22);
         }
         Cafe2_BaseUIScreen.m_screen.button_SaveLoad.SetOnMouseUpFunction(_global.bind0(this,this.OnLoadButtonClick));
         feg_m_saveLoadMenu.m_saveFilter = SAVEFILTER_ANY;
         Cafe2_BaseUIScreen.m_screen.txt_saveFileName.hide();
         Cafe2_BaseUIScreen.m_screen.SaveBottomImg._visible = false;
      }
      Cafe2_BaseUIScreen.m_screen.button_Cancel.setOnMouseUpFunction(_global.bind0(this,this.OnCancelButtonClick));
      Cafe2_BaseUIScreen.m_screen.button_Delete.setOnMouseUpFunction(_global.bind0(this,this.OnDeleteButtonClick));
      Cafe2_BaseUIScreen.m_screen.saveFileListScrollbar.enableGUI();
      Cafe2_BaseUIScreen.m_screen.saveFileListScrollbar.show();
      Cafe2_BaseUIScreen.m_screen.saveFileList.setVerticalPadding(0);
      Cafe2_BaseUIScreen.m_screen.saveFileList.addListener(this);
      var _loc3_ = new Array();
      _loc3_.push({symbol:"std_mouseListboxEntryTextElementSymbol",width:300,leftMargin:0,rightMargin:0,useHitRegion:false,fontSize:14});
      _loc3_.push({symbol:"std_mouseListboxEntryTextElementSymbol",width:314,leftMargin:0,rightMargin:0,useHitRegion:false,fontSize:14});
      _loc3_.push({symbol:"std_mouseListboxEntryTextElementSymbol",width:110,leftMargin:0,rightMargin:0,useHitRegion:false,fontSize:14});
      _loc3_.push({symbol:"std_mouseListboxEntryTextElementSymbol",width:76,leftMargin:0,rightMargin:26,useHitRegion:false,fontSize:14});
      Cafe2_BaseUIScreen.m_screen.saveFileList.init(_loc3_,20);
      Cafe2_BaseUIScreen.m_screen.saveFileList.attachScrollbarController(Cafe2_BaseUIScreen.m_screen.saveFileListScrollbar);
      this.updateListboxData();
      this.registerIntroOuttroComponents([Cafe2_BaseUIScreen.m_screen.button_Cancel,Cafe2_BaseUIScreen.m_screen.button_Delete,Cafe2_BaseUIScreen.m_screen.button_SaveLoad]);
      this.startIntro();
      trace("feg_m_saveLoadMenu::initGui() done.");
   }
   function localMessageHandler(messageCode)
   {
      var _loc2_ = true;
      switch(messageCode)
      {
         case _global.MSGCODE.FE_SAVEGAMES_UPDATE:
            Cafe2_BaseUIScreen.m_thisClass.updateListboxData();
            break;
         case _global.MSGCODE.FE_CLOSE_SAVELOAD:
            Cafe2_BaseUIScreen.m_thisClass.backButtonClicked();
            break;
         default:
            _loc2_ = false;
      }
      return _loc2_;
   }
   function localEventHandler(keyCode, controller)
   {
      trace("fes_saveLoadMenu::localEventHandler() keyCode == " + keyCode);
      if(keyCode !== INPUTCODE.ESCAPE)
      {
         _global.gFM.handleDefaultInputs(keyCode);
      }
      else
      {
         Cafe2_BaseUIScreen.m_thisClass.OnCancelButtonClick();
      }
   }
   function onScreenExit()
   {
      trace("feg_m_saveLoadMenu::onScreenExit()");
      Cafe2_BaseUIScreen.m_screen.saveFileList.shutdown();
      this.clearData();
      Cafe2_BaseUIScreen.m_screen.saveFileList.removeListener(this);
      _global.gMH.removeMessageHandler(Cafe2_BaseUIScreen.m_thisClass.localMessageHandler);
      _global.gEH.removeLocalEventHandler(Cafe2_BaseUIScreen.m_thisClass.m_eventHandlerIndex);
      Cafe2_BaseUIScreen.m_screen = null;
      Cafe2_BaseUIScreen.m_thisClass = null;
      delete _global.feg_m_saveLoadMenu;
      delete _global.Cafe2_BaseUIScreen;
      trace("feg_m_saveLoadMenu::onScreenExit() done.");
   }
   function updateListboxData()
   {
      trace("fes_m_saveLoadMenu::getData()");
      var _loc3_ = new Object();
      loadVariables("QueryGameEngine?SAVE_FILES?GameMode=" + String(feg_m_saveLoadMenu.m_saveMode) + "|Filter=" + String(feg_m_saveLoadMenu.m_saveFilter),_loc3_);
      if(_loc3_.SAVE_FILES_IDS != undefined && _loc3_.SAVE_FILES_IDS.length > 0)
      {
         trace("fes_m_saveLoadMenu::getData() Loadvar SAVE_FILES successful.");
         this.m_fileIds = _loc3_.SAVE_FILES_IDS.split(",");
         this.m_fileNames = _loc3_.SAVE_FILES_NAMES.split(",");
         this.m_fileMaps = _loc3_.SAVE_FILES_MAPS.split(",");
         this.m_fileTimes = _loc3_.SAVE_FILES_TIMES.split(",");
         this.m_fileDates = _loc3_.SAVE_FILES_DATES.split(",");
      }
      else
      {
         delete this.m_fileIds;
         delete this.m_fileNames;
         delete this.m_fileMaps;
         delete this.m_fileTimes;
         delete this.m_fileDates;
         this.m_fileIds = new Array();
         this.m_fileNames = new Array();
         this.m_fileMaps = new Array();
         this.m_fileTimes = new Array();
         this.m_fileDates = new Array();
      }
      if(_global.GAME.SAVELOADSCREEN_TYPE == _global.ENUM.SAVELOADSCREEN_SAVE)
      {
         this.m_fileIds.unshift("-1");
         this.m_fileNames.unshift("$GUI:NEWSAVEGAME");
         this.m_fileMaps.unshift("");
         this.m_fileTimes.unshift("");
         this.m_fileDates.unshift("");
      }
      Cafe2_BaseUIScreen.m_screen.saveFileList.setColumnData(0,this.m_fileNames);
      Cafe2_BaseUIScreen.m_screen.saveFileList.setColumnData(1,this.m_fileMaps);
      Cafe2_BaseUIScreen.m_screen.saveFileList.setColumnData(2,this.m_fileTimes);
      Cafe2_BaseUIScreen.m_screen.saveFileList.setColumnData(3,this.m_fileDates);
      if(_global.GAME.SAVELOADSCREEN_TYPE == _global.ENUM.SAVELOADSCREEN_SAVE)
      {
         this.m_selectedSaveIndex = -1;
         this.SetSaveNameTextfieldContents(this.m_selectedSaveIndex);
         Cafe2_BaseUIScreen.m_screen.button_Delete.disable();
      }
      else if(_loc3_.SAVE_FILES_IDS != undefined && _loc3_.SAVE_FILES_IDS.length > 0)
      {
         this.m_selectedSaveIndex = this.m_fileIds[0];
         Cafe2_BaseUIScreen.m_screen.button_Delete.enable();
      }
      else
      {
         Cafe2_BaseUIScreen.m_screen.button_SaveLoad.disable();
         Cafe2_BaseUIScreen.m_screen.button_Delete.disable();
         this.m_selectedSaveIndex = -1;
      }
      Cafe2_BaseUIScreen.m_screen.saveFileList.setSelectedIndex(0);
      Cafe2_BaseUIScreen.m_screen.saveFileList.moveViewToFirst();
      Cafe2_BaseUIScreen.m_screen.saveFileList.refreshDisplay();
      Cafe2_BaseUIScreen.m_screen.saveFileList.refreshScrollbar();
      trace("fes_m_saveLoadMenu::getData() SAVE_FILES = " + String(this.m_fileIds));
   }
   function OnCancelButtonClick()
   {
      trace("feg_m_saveLoadMenu::OnCancelButtonClick()");
      fscommand("CallGameFunction","%SaveLoadClosing");
   }
   function OnSaveButtonClick()
   {
      trace("feg_m_saveLoadMenu::OnSaveButtonClick()");
      fscommand("CallGameFunction","%SaveFile?FileId=" + String(this.m_selectedSaveIndex) + "|GameMode=" + String(feg_m_saveLoadMenu.m_saveMode) + "|Description=" + String(Cafe2_BaseUIScreen.m_screen.txt_saveFileName.getContents()));
   }
   function OnLoadButtonClick()
   {
      trace("feg_m_saveLoadMenu::OnLoadButtonClick()");
      var _loc2_;
      if(feg_m_saveLoadMenu.m_saveMode == SAVEMODE_CAMPAIGNMP)
      {
         _loc2_ = new Object();
         loadVariables("QueryGameEngine?IS_NAT_DONE",_loc2_);
         if(Number(_loc2_.IS_NAT_DONE) != 1)
         {
            return undefined;
         }
      }
      Cafe2_BaseUIScreen.m_screen.button_SaveLoad.disable();
      fscommand("CallGameFunction","%LoadFile?FileId=" + String(this.m_selectedSaveIndex) + "|GameMode=" + String(feg_m_saveLoadMenu.m_saveMode) + "|AddCommentary=" + (!Cafe2_BaseUIScreen.m_screen.addCommentaryPanel.addCommentaryCheckbox.isChecked() ? String(0) : String(1)));
   }
   function OnDeleteButtonClick()
   {
      trace("feg_m_saveLoadMenu::OnDeleteButtonClick()");
      fscommand("CallGameFunction","%DeleteFile?FileId=" + String(this.m_selectedSaveIndex) + "|GameMode=" + String(feg_m_saveLoadMenu.m_saveMode));
   }
   function SetSaveNameTextfieldContents(fileId)
   {
      var _loc1_ = new Object();
      loadVariables("QueryGameEngine?SAVE_FILE_BY_INDEX?GameMode=" + String(feg_m_saveLoadMenu.m_saveMode) + "|Filter=" + String(feg_m_saveLoadMenu.m_saveFilter) + "|FileId=" + fileId,_loc1_);
      Cafe2_BaseUIScreen.m_screen.txt_saveFileName.setContents(_loc1_.SAVE_FILE_BY_INDEX);
   }
}
