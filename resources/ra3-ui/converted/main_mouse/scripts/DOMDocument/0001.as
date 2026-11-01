function gblSMChangeScreen()
{
   trace("main_mouse::gblSMChangeScreen()");
   if(gSM.m_intervalId != -1)
   {
      clearInterval(gSM.m_intervalId);
      gSM.m_intervalId = -1;
   }
   _global.gShellScreen.screen.changeScreen();
}
function initGlobalVariables()
{
   trace("main_mouse_functions::initGlobalVariables()");
   if(_root.gFWS == undefined)
   {
      trace("***** WARNING: gFWS not defined! ");
      trace("***** Cafe 2.0 Framework requires var gFWS to be set to an object implementing ICafe2_FWSettings in configs.as");
   }
   _root.PLATFORM = gFWS.getPlatform();
   _root.SCREEN_WIDTH = gFWS.getScreenWidth();
   _root.SCREEN_HEIGHT = gFWS.getScreenHeight();
   _root.LANGUAGE = gFWS.getLanguage();
   _root.ltxt = gFWS.ltxt;
   _root.cafeTrace = gFWS.cafeTrace;
   _root.playSound = gFWS.playSound;
   _root.stopSound = gFWS.stopSound;
   trace("PLATFORM = " + PLATFORM);
   trace("SCREEN_WIDTH = " + SCREEN_WIDTH);
   trace("SCREEN_HEIGHT = " + SCREEN_HEIGHT);
   if(PLATFORM == "FLASH")
   {
      trace("Resetting the PLATFORM to PC while in Flash");
      PLATFORM = "PC";
   }
   _global.PLATFORM = _root.PLATFORM;
   _global.SCREEN_WIDTH = _root.SCREEN_WIDTH;
   _global.SCREEN_HEIGHT = _root.SCREEN_HEIGHT;
   _global.LANGUAGE = _root.LANGUAGE;
   _global.ltxt = _root.gFWS.ltxt;
   _global.cafeTrace = _root.gFWS.cafeTrace;
   _global.playSound = _root.gFWS.playSound;
   _global.stopSound = _root.gFWS.stopSound;
   var _loc3_ = System.capabilities.os;
   if(_loc3_.length > 0)
   {
      _global.IS_FLASH = true;
   }
   else
   {
      _global.IS_FLASH = false;
   }
}
function gotoShellScreen(sScreenId, screenStack)
{
   _global.gSM.changeMainScreen(_global.SCREEN.NONE);
   _global.gSM.clearStack();
   _global.gSM.popScreen();
   var _loc2_ = 0;
   while(_loc2_ < screenStack.length)
   {
      _global.gSM.changeMainScreen(screenStack[_loc2_]);
      _loc2_ = _loc2_ + 1;
   }
   _global.gSM.changeMainScreen(sScreenId);
   _global.showFEBackgroundMovie();
   _global.gShellScreen._visible = true;
}
function globalMessageHandler(messageCode)
{
   var _loc2_ = false;
   var _loc3_;
   var _loc4_;
   switch(messageCode)
   {
      case _global.MSGCODE.FE_HIDE:
         trace("main_mouse::globalMessageHandler() handling message _global.MSGCODE.FE_HIDE = \'" + _global.MSGCODE.FE_HIDE + "\'");
         _global.gSM.clearStack();
         _global.gSM.changeMainScreen(_global.SCREEN.NONE);
         _global.gShellScreen.screen.unloadContent();
         _global.gShellScreen._visible = false;
         _global.hideFEBackgroundMovie();
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_SHOW_MAIN_MENU:
         trace("main_mouse::globalMessageHandler() handling message _global.MSGCODE.FE_SHOW_MAIN_MENU = \'" + _global.MSGCODE.FE_SHOW_MAIN_MENU + "\'");
         gotoShellScreen(_global.SCREEN.FEG_MAIN_MENU,[]);
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_SHOW_SAVE_MENU:
         trace("main_mouse::globalMessageHandler() handling message _global.MSGCODE.FE_SHOW_SAVE_MENU = \'" + _global.MSGCODE.FE_SHOW_SAVE_MENU + "\'");
         gotoShellScreen(_global.SCREEN.FEG_SAVELOAD,[]);
         _global.GAME.SAVELOADSCREEN_TYPE = _global.ENUM.SAVELOADSCREEN_SAVE;
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_SHOW_LOAD_MENU:
         trace("main_mouse::globalMessageHandler() handling message _global.MSGCODE.FE_SHOW_LOAD_MENU = \'" + _global.MSGCODE.FE_SHOW_LOAD_MENU + "\'");
         gotoShellScreen(_global.SCREEN.FEG_SAVELOAD,[]);
         _global.GAME.SAVELOADSCREEN_TYPE = _global.ENUM.SAVELOADSCREEN_LOAD;
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_SHOW_LOADSCREEN:
         trace("main_mouse::globalMessageHandler() handling message _global.MSGCODE.FE_SHOW_LOADSCREEN = \'" + _global.MSGCODE.FE_SHOW_LOADSCREEN + "\'");
         gotoShellScreen(_global.SCREEN.FEG_LOADSCREEN,[]);
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_SHOW_MP_LOADSCREEN:
         trace("main_mouse::globalMessageHandler() handling message _global.MSGCODE.FE_SHOW_MP_LOADSCREEN = \'" + _global.MSGCODE.FE_SHOW_MP_LOADSCREEN + "\'");
         gotoShellScreen(_global.SCREEN.FEG_MP_LOADSCREEN,[]);
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_SHOW_MISSION_BRIEFING:
         trace("main_mouse::globalMessageHandler() handling message _global.MSGCODE.FE_SHOW_MISSION_BRIEFING = \'" + _global.MSGCODE.FE_SHOW_MISSION_BRIEFING + "\'");
         fscommand("CallGameFunction","%Campaign_Initialize");
         _global.GAME.COOPMODE = _global.ENUM.GAME_COOPMODE_CHOOSER;
         _global.GAME.PLAY_MISSION_INTRO_MOVIE = true;
         gotoShellScreen(_global.SCREEN.FES_COOP_MISSION_BRIEFING,[]);
         _loc2_ = true;
         break;
      case _global.MSGCODE.IGM_OPEN_SHELL_ROOT:
         trace("main_mouse::globalMessageHandler() handling message _global.MSGCODE.IGM_OPEN_SHELL_ROOT = \'" + _global.MSGCODE.IGM_OPEN_SHELL_ROOT + "\'");
         gotoShellScreen(_global.SCREEN.IGM_SHELL_ROOT,[]);
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_SHOW_MP_GAME_SETUP:
         trace("main_mouse::globalMessageHandler() handling message _global.MSGCODE.FE_SHOW_MP_GAME_SETUP = \'" + _global.MSGCODE.FE_SHOW_MP_GAME_SETUP + "\'");
         gotoShellScreen(_global.SCREEN.FEM_MP_SETUP,[_global.SCREEN.FEG_MAIN_MENU,_global.SCREEN.FEM_MP_ONLINE_MULTIPLAYER]);
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_SHOW_SKIRMISH_RESULTS:
         trace("main_mouse::globalMessageHandler() handling message _global.MSGCODE.FE_SHOW_SKIRMISH_RESULTS = \'" + _global.MSGCODE.FE_SHOW_SKIRMISH_RESULTS + "\'");
         gotoShellScreen(_global.SCREEN.FEM_MP_POSTGAME_ROUNDUP,[]);
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_SHOW_CAMPAIGN_RESULTS:
         trace("main_mouse::globalMessageHandler() handling message _global.MSGCODE.FE_SHOW_CAMPAIGN_RESULTS= \'" + _global.MSGCODE.FE_SHOW_CAMPAIGN_RESULTS + "\'");
         gotoShellScreen(_global.SCREEN.FES_COOP_POSTGAME_ROUNDUP,[]);
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_SHOW_LAN_RESULTS:
         trace("main_mouse::globalMessageHandler() handling message _global.MSGCODE.FE_SHOW_LAN_RESULTS= \'" + _global.MSGCODE.FE_SHOW_LAN_RESULTS + "\'");
         gotoShellScreen(_global.SCREEN.FEM_MP_POSTGAME_ROUNDUP,[]);
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_SHOW_ONLINE_RESULTS:
         trace("main_mouse::globalMessageHandler() handling message _global.MSGCODE.FE_SHOW_ONLINE_RESULTS= \'" + _global.MSGCODE.FE_SHOW_ONLINE_RESULTS + "\'");
         gotoShellScreen(_global.SCREEN.FEM_MP_POSTGAME_ROUNDUP,[]);
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_SHOW_AUTO_MATCH_RESULTS:
         trace("main_mouse::globalMessageHandler() handling message _global.MSGCODE.FE_SHOW_AUTO_MATCH_RESULTS= \'" + _global.MSGCODE.FE_SHOW_AUTO_MATCH_RESULTS + "\'");
         gotoShellScreen(_global.SCREEN.FEM_MP_POSTGAME_ROUNDUP,[]);
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_SHOW_TUTORIAL_RESULTS:
         trace("main_mouse::globalMessageHandler() handling message _global.MSGCODE.FE_SHOW_TUTORIAL= \'" + _global.MSGCODE.FE_SHOW_TUTORIAL + "\'");
         gotoShellScreen(_global.SCREEN.FEG_TUTORIALMENU,[_global.SCREEN.FEG_MAIN_MENU]);
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_MP_ONLINE_GAME_JOINED_SUCCESS:
         _global.comLink.screen.close();
         if(_global.GAME.MODE == _global.ENUM.GAME_MODE_SKIRMISH)
         {
            fscommand("CallGameFunction","%SetGameSetupMode?Mode=None");
         }
         else if(_global.GAME.MODE == _global.ENUM.GAME_MODE_CAMPAIGN)
         {
            fscommand("CallGameFunction","%Campaign_ExitGame");
            fscommand("CallGameFunction","%SetGameSetupMode?Mode=None|DontExitGame=1");
         }
         else if(_global.GAME.MODE == _global.ENUM.GAME_MODE_MULTIPLAYER)
         {
            fscommand("CallGameFunction","%SetGameSetupMode?Mode=None|DontExitGame=1");
         }
         _global.GAME.MODE = _global.ENUM.GAME_MODE_MULTIPLAYER;
         gotoShellScreen(_global.SCREEN.FEM_MP_SETUP,[_global.SCREEN.FEG_MAIN_MENU,_global.SCREEN.FEM_MP_ONLINE_MULTIPLAYER]);
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_MP_ONLINE_GAME_CONNECT_SUCCESS:
         _global.comLink.screen.close();
         _global.GAME.MODE = _global.ENUM.GAME_MODE_MULTIPLAYER;
         gotoShellScreen(_global.SCREEN.FEM_MP_SETUP,[_global.SCREEN.FEG_MAIN_MENU,_global.SCREEN.FEM_MP_ONLINE_MULTIPLAYER]);
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_COOP_ONLINE_GAME_JOINED_SUCCESS:
         _global.comLink.screen.close();
         if(_global.GAME.MODE == _global.ENUM.GAME_MODE_SKIRMISH)
         {
            fscommand("CallGameFunction","%SetGameSetupMode?Mode=None");
         }
         else if(_global.GAME.MODE == _global.ENUM.GAME_MODE_MULTIPLAYER || _global.GAME.MODE == _global.ENUM.GAME_MODE_CAMPAIGN)
         {
            fscommand("CallGameFunction","%SetGameSetupMode?Mode=None|DontExitGame=1");
         }
         _global.GAME.COOPMODE = _global.ENUM.GAME_COOPMODE_CLIENT;
         _global.GAME.MODE = _global.ENUM.GAME_MODE_CAMPAIGN;
         gotoShellScreen(_global.SCREEN.FEM_COOPSTAGING,[_global.SCREEN.FEG_MAIN_MENU]);
         fscommand("CallGameFunction","%SetGameSetupMode?Mode=Online");
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_TEAM2v2_ONLINE_GAME_CONNECT_SUCCESS:
         trace("main_mouse_functions::globalMessageHandler() handling message _global.MSGCODE.FE_TEAM2v2_ONLINE_GAME_CONNECT_SUCCESS = \'" + _global.MSGCODE.FE_TEAM2v2_ONLINE_GAME_CONNECT_SUCCESS + "\'");
         if(_global.GAME.MODE == _global.ENUM.GAME_MODE_SKIRMISH || _global.GAME.MODE == _global.ENUM.GAME_MODE_LAN_MP)
         {
            fscommand("CallGameFunction","%SetGameSetupMode?Mode=None");
         }
         _loc3_ = new Object();
         loadVariables("QueryGameEngine?IsPcGameHost",_loc3_);
         if(_loc3_.IsPcGameHost == "0")
         {
            if(_global.GAME.MODE == _global.ENUM.GAME_MODE_MULTIPLAYER || _global.GAME.MODE == _global.ENUM.GAME_MODE_CAMPAIGN)
            {
               fscommand("CallGameFunction","%SetGameSetupMode?Mode=None|DontExitGame=1");
            }
            _global.comLink.screen.close();
            _global.SHELL.INVITED_TO_TEAM_AUTOMATCH = true;
            gotoShellScreen(_global.SCREEN.FEM_MP_ONLINE_MULTIPLAYER,[_global.SCREEN.FEG_MAIN_MENU]);
            _loc2_ = true;
         }
         else
         {
            _global.comLink.screen.openForAutoMatchInvite();
            _loc2_ = false;
         }
         fscommand("CallGameFunction","%SetGameSetupMode?Mode=Online");
         _global.GAME.MODE = _global.ENUM.GAME_MODE_MULTIPLAYER;
         break;
      case _global.MSGCODE.FE_MP_ONLINE_SIGN_IN_FAILURE:
         if(_global.GAME.MODE == _global.ENUM.GAME_MODE_MULTIPLAYER || _global.GAME.MODE == _global.ENUM.GAME_MODE_CAMPAIGN)
         {
            _global.GAME.MODE = _global.ENUM.GAME_MODE_NONE;
            fscommand("CallGameFunction","%SetGameSetupMode?Mode=None");
            gotoShellScreen(_global.SCREEN.FEG_MAIN_MENU,[]);
         }
         _loc2_ = false;
         break;
      case _global.MSGCODE.FE_MP_ONLINE_SIGN_OUT_SUCCESS:
      case _global.MSGCODE.FE_MP_ONLINE_DISCONNECTED:
         _loc4_ = _global.GAME.MODE == _global.ENUM.GAME_MODE_CAMPAIGN && (_global.GAME.COOPMODE == _global.ENUM.GAME_COOPMODE_HOST || _global.GAME.COOPMODE == _global.ENUM.GAME_COOPMODE_CLIENT);
         if(_global.GAME.MODE == _global.ENUM.GAME_MODE_MULTIPLAYER || _loc4_)
         {
            _global.GAME.MODE = _global.ENUM.GAME_MODE_NONE;
            fscommand("CallGameFunction","%SetGameSetupMode?Mode=None");
            gotoShellScreen(_global.SCREEN.FEG_MAIN_MENU,[]);
            _global.comLink.screen.close();
         }
         _loc2_ = false;
         break;
      case _global.MSGCODE.FE_MP_ONLINE_GAME_CONNECT_FAILURE:
         _global.GAME.MODE = _global.ENUM.GAME_MODE_NONE;
         fscommand("CallGameFunction","%SetGameSetupMode?Mode=None");
         gotoShellScreen(_global.SCREEN.FEG_MAIN_MENU,[]);
         break;
      case _global.MSGCODE.FE_MP_HOST_LEFT_GAME:
      case _global.MSGCODE.FE_MP_KICKED_FROM_MULTIPLAYER_GAME:
      case _global.MSGCODE.FE_MP_KICKED_FROM_COOP_GAME:
         if(_global.GAME.MODE == _global.ENUM.GAME_MODE_MULTIPLAYER)
         {
            _global.GAME.MODE = _global.ENUM.GAME_MODE_NONE;
            gotoShellScreen(_global.SCREEN.FEM_MP_ONLINE_MULTIPLAYER,[_global.SCREEN.FEG_MAIN_MENU]);
         }
         else
         {
            _global.GAME.MODE = _global.ENUM.GAME_MODE_NONE;
            fscommand("CallGameFunction","%SetGameSetupMode?Mode=None");
            gotoShellScreen(_global.SCREEN.FEG_MAIN_MENU,[]);
         }
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_LAN_HOST_LEFT_GAME:
         gotoShellScreen(_global.SCREEN.FEM_LAN_LOBBY,[_global.SCREEN.FEG_MAIN_MENU]);
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_NEXT_COMPONENT:
         _global.gFM.selectNextItem();
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_PREVIOUS_COMPONENT:
         _global.gFM.selectPreviousItem();
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_UNHANDLED_MOUSE_CLICK:
         _global.gFM.deselectFocusedComponent();
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_KEYBOARD_UP:
      case _global.MSGCODE.FE_KEYBOARD_DOWN:
      case _global.MSGCODE.FE_KEYBOARD_LEFT:
      case _global.MSGCODE.FE_KEYBOARD_RIGHT:
      case _global.MSGCODE.FE_KEYBOARD_ENTER:
         break;
      case _global.MSGCODE.FE_LOST_COCOMMANDER:
         gotoShellScreen(_global.SCREEN.FEG_RECRUITMENT_UI,[_global.SCREEN.FEG_MAIN_MENU]);
         _loc2_ = true;
   }
   return _loc2_;
}
function onMousewheelScroll(spins)
{
   if(_global.highlightedComponent != undefined)
   {
      _global.highlightedComponent.onMousewheelScroll(spins);
   }
}
getTimer();
_global.Util = new Object();
_global.Util.Truncate = function(nLength, sString)
{
   if(sString.length >= nLength)
   {
      return sString.substring(0,nLength);
   }
   return sString;
};
_global.Util.TruncateEllipse = function(nLength, sString)
{
   if(sString.length >= nLength)
   {
      return sString.substring(0,nLength) + "...";
   }
   return sString;
};
_global.Util.TruncateFieldEllipse = function(oTextField, nMaxWidth)
{
   var _loc3_ = oTextField.textWidth;
   var _loc5_;
   var _loc1_;
   if(_loc3_ >= nMaxWidth)
   {
      _loc5_ = oTextField.text;
      _loc1_ = oTextField.text.length;
      nMaxWidth -= 16;
      if(nMaxWidth <= 0)
      {
         nMaxWidth = 1;
      }
      while(_loc1_ > 0 && _loc3_ > nMaxWidth)
      {
         _loc1_ = _loc1_ - 1;
         oTextField.text = _loc5_.substr(0,_loc1_);
         _loc3_ = oTextField.textWidth;
      }
      oTextField.text += "...";
   }
};
_global.Util.TruncateField = function(oTextField, nMaxWidth)
{
   var _loc3_ = oTextField.textWidth;
   var _loc5_;
   var _loc1_;
   if(_loc3_ >= nMaxWidth)
   {
      _loc5_ = oTextField.text;
      _loc1_ = oTextField.text.length;
      if(nMaxWidth <= 0)
      {
         nMaxWidth = 1;
      }
      while(_loc1_ > 0 && _loc3_ > nMaxWidth)
      {
         _loc1_ = _loc1_ - 1;
         oTextField.text = _loc5_.substr(0,_loc1_);
         _loc3_ = oTextField.textWidth;
      }
   }
};
_global.Util.loadCafeIcon = function(iconToLoad, mcToLoadInto)
{
   mcToLoadInto.loadMovie("../cafe/artAssets/" + _global.PLATFORM + "/icons/" + iconToLoad + ".swf");
};
var INPUTCODE = new Object();
_global.INPUTCODE = INPUTCODE;
INPUTCODE.ALL = -100;
INPUTCODE.BASE_UP = 1000;
INPUTCODE.ENTER = 13;
INPUTCODE.ESCAPE = 27;
INPUTCODE.UP = 38;
INPUTCODE.DOWN = 40;
INPUTCODE.LEFT = 37;
INPUTCODE.RIGHT = 39;
INPUTCODE.SELECT = 300;
INPUTCODE.START = 301;
INPUTCODE.CROSS = 302;
INPUTCODE.TRIANGLE = 304;
INPUTCODE.CIRCLE = 303;
INPUTCODE.SQUARE = 305;
INPUTCODE.L1 = 306;
INPUTCODE.R1 = 307;
INPUTCODE.L2 = 308;
INPUTCODE.R2 = 309;
INPUTCODE.L3 = 310;
INPUTCODE.R3 = 311;
INPUTCODE.LAS = 501;
INPUTCODE.RAS = 502;
INPUTCODE.CONNECTED = 400;
INPUTCODE.DISCONNECTED = 401;
INPUTCODE.LAS_UP = 538;
INPUTCODE.LAS_DOWN = 540;
INPUTCODE.LAS_LEFT = 537;
INPUTCODE.LAS_RIGHT = 539;
INPUTCODE.RAS_UP = 638;
INPUTCODE.RAS_DOWN = 640;
INPUTCODE.RAS_LEFT = 637;
INPUTCODE.RAS_RIGHT = 639;
var sys = System.capabilities.os;
var externStr = "extern";
var externObj = eval(externStr);
if(sys.length > 0 || externObj.gPlatform == "PC")
{
   trace("running in pc flash");
   INPUTCODE.UP = 38;
   INPUTCODE.DOWN = 40;
   INPUTCODE.LEFT = 37;
   INPUTCODE.RIGHT = 39;
   INPUTCODE.CROSS = 88;
   INPUTCODE.TRIANGLE = 84;
   INPUTCODE.CIRCLE = 79;
   INPUTCODE.SQUARE = 83;
   INPUTCODE.L1 = 75;
   INPUTCODE.R1 = 76;
   INPUTCODE.L2 = 78;
   INPUTCODE.R2 = 77;
   INPUTCODE.START = 71;
}
INPUTCODE.ENTER_U = INPUTCODE.ENTER + INPUTCODE.BASE_UP;
INPUTCODE.UP_U = INPUTCODE.UP + INPUTCODE.BASE_UP;
INPUTCODE.DOWN_U = INPUTCODE.DOWN + INPUTCODE.BASE_UP;
INPUTCODE.LEFT_U = INPUTCODE.LEFT + INPUTCODE.BASE_UP;
INPUTCODE.RIGHT_U = INPUTCODE.RIGHT + INPUTCODE.BASE_UP;
INPUTCODE.SELECT_U = INPUTCODE.SELECT + INPUTCODE.BASE_UP;
INPUTCODE.START_U = INPUTCODE.START + INPUTCODE.BASE_UP;
INPUTCODE.CROSS_U = INPUTCODE.CROSS + INPUTCODE.BASE_UP;
INPUTCODE.TRIANGLE_U = INPUTCODE.TRIANGLE + INPUTCODE.BASE_UP;
INPUTCODE.CIRCLE_U = INPUTCODE.CIRCLE + INPUTCODE.BASE_UP;
INPUTCODE.SQUARE_U = INPUTCODE.SQUARE + INPUTCODE.BASE_UP;
INPUTCODE.L1_U = INPUTCODE.L1 + INPUTCODE.BASE_UP;
INPUTCODE.R1_U = INPUTCODE.R1 + INPUTCODE.BASE_UP;
INPUTCODE.L2_U = INPUTCODE.L2 + INPUTCODE.BASE_UP;
INPUTCODE.R2_U = INPUTCODE.R2 + INPUTCODE.BASE_UP;
INPUTCODE.L3_U = INPUTCODE.L3 + INPUTCODE.BASE_UP;
INPUTCODE.R3_U = INPUTCODE.R3 + INPUTCODE.BASE_UP;
INPUTCODE.LAS_UP_U = INPUTCODE.LAS_UP + INPUTCODE.BASE_UP;
INPUTCODE.LAS_DOWN_U = INPUTCODE.LAS_DOWN + INPUTCODE.BASE_UP;
INPUTCODE.LAS_LEFT_U = INPUTCODE.LAS_LEFT + INPUTCODE.BASE_UP;
INPUTCODE.LAS_RIGHT_U = INPUTCODE.LAS_RIGHT + INPUTCODE.BASE_UP;
INPUTCODE.RAS_UP_U = INPUTCODE.RAS_UP + INPUTCODE.BASE_UP;
INPUTCODE.RAS_DOWN_U = INPUTCODE.RAS_DOWN + INPUTCODE.BASE_UP;
INPUTCODE.RAS_LEFT_U = INPUTCODE.RAS_LEFT + INPUTCODE.BASE_UP;
INPUTCODE.RAS_RIGHT_U = INPUTCODE.RAS_RIGHT + INPUTCODE.BASE_UP;
var gFWS = new main_mouse_settings();
var gSM = new CafeFW_ScreenManager();
var gOM = new CafeFW_OverlayManager();
var gFM = new CafeFW_FocusManager("gFM");
var gCM = new CafeFW_ControllerManager();
var gCT = new CafeFW_ComponentTracker();
var gMH = new CafeFW_MessageHandler();
var gEH = new CafeFW_EventHandler(this);
Key.addListener(gEH);
gSM.setRefToGlobalEventHandler(gEH);
gSM.setRefToGlobalFocusManager(gFM);
_global.gFWS = _root.gFWS;
_global.gFM = _root.gFM;
_global.gEH = _root.gEH;
_global.gSM = _root.gSM;
_global.gOM = _root.gOM;
_global.gCM = _root.gCM;
_global.gCT = _root.gCT;
_global.gMH = _root.gMH;
_global.bind0 = function(o, f)
{
   return function()
   {
      return f.call(o);
   };
};
_global.bind1 = function(o, f, p1)
{
   return function()
   {
      return f.call(o,p1);
   };
};
_global.bind2 = function(o, f, p1, p2)
{
   return function()
   {
      return f.call(o,p1,p2);
   };
};
_global.bind3 = function(o, f, p1, p2, p3)
{
   return function()
   {
      return f.call(o,p1,p2,p3);
   };
};
_global.bind1DynamicParams = function(o, f)
{
   return function(p1)
   {
      return f.call(o,p1);
   };
};
_global.bind2DynamicParams = function(o, f)
{
   return function(p1, p2)
   {
      return f.call(o,p1,p2);
   };
};
_global.bind3DynamicParams = function(o, f)
{
   return function(p1, p2, p3)
   {
      return f.call(o,p1,p2,p3);
   };
};
_global.PlayShellMusic = function(audioAlias)
{
   trace("main_functions::_global.PlayShellMusic() audioAlias = " + audioAlias);
   fscommand("CallGameFunction","%Shell_PlayMusic?Value=" + audioAlias);
};
UISystemMessage = function(sysMessage)
{
   trace("UISystemMessage() sysMessage = " + sysMessage);
   gMH.handleStringMessage(sysMessage);
};
_global.gotoShellScreen = gotoShellScreen;
_global.showFEBackgroundMovie = function()
{
   _global.gShellScreen.screen.showBackgroundMovie();
};
_global.hideFEBackgroundMovie = function()
{
   _global.gShellScreen.screen.hideBackgroundMovie();
};
_global.getAbsoluteMouseLocation = function()
{
   var _loc2_ = new Array();
   _loc2_.push(_global.gShellScreen._xmouse);
   _loc2_.push(_global.gShellScreen._ymouse);
   return _loc2_;
};
gMH.addPriorityMessageHandler(globalMessageHandler,CafeFW_MessageHandler.PRIORITY_GLOBAL);
Cafe2_FocusableBody.includeCode();
Cafe2_FocusablePart.includeCode();
Cafe2_SldBarBody.includeCode();
Cafe2_OnOffAnimPart.includeCode();
Cafe2_ScrollbarBody.includeCode();
CafeFW_IndexMgr.includeCode();
var errCode = false;
errCode = Object.registerClass("std_mouseSliderSymbol",std_mouseSlider_visual);
trace("main_mouse_components: registerClass on std_mouseSliderSymbol returned " + errCode);
errCode = Object.registerClass("std_MousebuttonSymbol",std_Mousebutton_visual);
trace("main_mouse_components: registerClass on std_Mousebutton_visual returned " + errCode);
errCode = Object.registerClass("std_mouseScrollbarSymbol",std_mouseScrollbar_visual);
trace("main_mouse_components: registerClass on std_mouseScrollbar_visual returned " + errCode);
errCode = Object.registerClass("std_MouseCheckBoxSymbol",std_mouseCheckBox_visual);
trace("main_mouse_components: registerClass on std_mouseCheckBox_visual returned " + errCode);
errCode = Object.registerClass("std_mouseScrollingListboxSymbol",std_mouseScrollingListbox);
trace("main_mouse_components: registerClass on std_mouseScrollingListbox returned " + errCode);
errCode = Object.registerClass("std_mouseScrollingListboxEntrySymbol",std_mouseScrollingListboxEntry);
trace("main_mouse_components: registerClass on std_mouseScrollingListboxEntry returned " + errCode);
errCode = Object.registerClass("std_mouseListboxEntryTextElementSymbol",std_mouseListboxEntryTextElement);
trace("main_mouse_components: registerClass on std_mouseListboxEntryTextElement returned " + errCode);
errCode = Object.registerClass("std_mouseListboxEntryColorSwatchElementSymbol",std_mouseListboxEntryColorSwatchElement);
trace("main_mouse_components: registerClass on std_mouseListboxEntryColorSwatchElement returned " + errCode);
errCode = Object.registerClass("std_mouseListboxEntryRenderImageElementSymbol",std_mouseListboxEntryRenderImageElement);
trace("main_mouse_components: registerClass on std_mouseListboxEntryRenderImageElement returned " + errCode);
errCode = Object.registerClass("std_mouseListboxEntryGameStatusElementSymbol",std_mouseListboxEntryGameStatusElement);
trace("main_mouse_components: registerClass on std_mouseListboxEntryGameStatusElement returned " + errCode);
errCode = Object.registerClass("std_mouseDropDownMenuSymbol",std_mouseDropDownMenu);
trace("main_mouse_components: registerClass on std_mouseDropDownMenu returned " + errCode);
errCode = Object.registerClass("std_mouseTextInputFieldSymbol",std_mouseTextInputField);
trace("main_mouse_components: registerClass on std_mouseTextInputField returned " + errCode);
errCode = Object.registerClass("std_MouseAccordionButtonSymbol",std_MouseAccordionButton);
trace("main_mouse_components: registerClass on std_MouseAccordionButton returned " + errCode);
errCode = Object.registerClass("std_AccordionMenuSymbol",std_MouseAccordionMenu);
trace("main_mouse_components: registerClass on std_MouseAccordionMenu returned " + errCode);
errCode = Object.registerClass("2p5D_UISymbol",movieClip2p5DUIHelper);
trace("main_mouse_components: registerClass on movieClip2p5DUIHelper returned " + errCode);
errCode = Object.registerClass("blimpFactorySymbol",blimpAnimationFactory);
trace("main_mouse_components: registerClass on blimpAnimationFactory returned " + errCode);
errCode = Object.registerClass("bombFactorySymbol",bombAnimationFactory);
trace("main_mouse_components: registerClass on bombAnimationFactory returned " + errCode);
errCode = Object.registerClass("MovieComponentSymbol",MovieComponent);
trace("main_mouse_components: register MovieComponentSymbol " + errCode);
errCode = Object.registerClass("chatComponentSymbol",ChatComponent);
trace("register charComponentSymbol" + errCode);
errCode = Object.registerClass("careerStatsComponentSymbol",careerStatsComponent);
trace("register careerStatsComponentSymbol " + errCode);
errCode = Object.registerClass("std_mouseRadioButtonSymbol",std_mouseRadioButton);
trace("register std_mouseRadioButtonSymbol" + errCode);
errCode = Object.registerClass("std_mouseRadioButtonControllerSymbol",std_mouseRadioButtonController);
trace("register std_mouseRadioButtonControllerSymbol" + errCode);
errCode = Object.registerClass("LobbyListComponentSymbol",lobbyListComponent);
trace("main_mouse_components: registerClass on lobbyListComponent returned " + errCode);
errCode = Object.registerClass("GameListComponentSymbol",gameListComponent);
trace("main_mouse_components: registerClass on gobbyListComponent returned " + errCode);
errCode = Object.registerClass("playerListComponentSymbol",playerListComponent);
trace("main_mouse_components: registerClass on playerListComponent returned " + errCode);
errCode = Object.registerClass("inviteListComponentSymbol",inviteListComponent);
trace("main_mouse_components: registerClass on inviteListComponent returned " + errCode);
errCode = Object.registerClass("BuddyListComponentSymbol",buddyListComponent);
trace("main_mouse_components: registerClass on buddyListComponent returned " + errCode);
errCode = Object.registerClass("chapterSelectComponentSymbol",mouse_chapterSelectComponent);
trace("main_mouse_components: registerClass on chapterSelectComponent returned " + errCode);
errCode = Object.registerClass("contextMenuEntrySymbol",contextMenuEntry);
trace("main_mouse_components: registerClass on contextMenuEntry returned " + errCode);
errCode = Object.registerClass("ScreenTitleBarSymbol",fe_m_TitleBarComponent);
trace("main_mouse_components: registerClass on fe_m_TitleBarComponent returned " + errCode);
if(_global.SCREEN == undefined)
{
   _global.SCREEN = new Object();
}
_global.SCREEN.NONE = "";
_global.SCREEN.FEG_MAIN = "screens/feg_m_main.swf";
_global.SCREEN.FEG_BACKGROUND = "screens/feg_m_background";
_global.SCREEN.FEG_COMLINK = "screens/feg_m_comLink";
_global.SCREEN.FEG_CAFE_UI_WIDGET_TEST_SCREEN = "screens/testCafeUI";
_global.SCREEN.FEG_MAIN_MENU = "screens/feg_m_mainMenu3D";
_global.SCREEN.FEG_OPTIONS = "screens/feg_m_optionsSettings";
_global.SCREEN.FEG_LOADSCREEN = "screens/feg_m_load";
_global.SCREEN.FEG_MP_LOADSCREEN = "screens/fem_m_load";
_global.SCREEN.FEG_MESSAGE_BOX = "screens/feg_m_messageBox.swf";
_global.SCREEN.FEG_DISCONNECT = "screens/feg_m_disconnect.swf";
_global.SCREEN.FEG_CONTEXTMENU = "screens/feg_m_contextMenu";
_global.SCREEN.FEG_PROFILE_MANAGER = "screens/feg_m_profileManager";
_global.SCREEN.FEG_PROFILE_STATS = "screens/feg_m_profileStats";
_global.SCREEN.FEG_RECRUITMENT_UI = "screens/feg_m_recruitChooser";
_global.SCREEN.FEG_SAVELOAD = "screens/feg_m_saveLoadMenu";
_global.SCREEN.FEG_CREDITS = "screens/feg_m_credits";
_global.SCREEN.FEG_TUTORIALMENU = "screens/feg_m_tutorialMenu";
_global.SCREEN.FEG_FILMARCHIVE = "screens/feg_m_filmArchiveTheatre";
_global.SCREEN.FEM_MP_SETUP = "screens/fem_m_gameSetup";
_global.SCREEN.FEM_MP_ONLINE_MULTIPLAYER = "screens/fem_m_onlineMultiplayer";
_global.SCREEN.FEM_COOPSTAGING = "screens/fem_m_coopStaging";
_global.SCREEN.FEM_MP_POSTGAME_ROUNDUP = "screens/fem_m_postgameRoundup";
_global.SCREEN.FEM_LAN_LOBBY = "screens/fem_m_lanLobby";
_global.SCREEN.FEM_LAN_SETUP = "screens/fem_m_lanGameSetup";
_global.SCREEN.FES_SINGLE_PLAYER_MENU = "screens/fes_m_singleplayerMenu";
_global.SCREEN.FES_COOP_MISSION_BRIEFING = "screens/fes_m_missionBriefing";
_global.SCREEN.FES_COOP_OBJECTIVES_BRIEFING = "screens/fes_m_objectivesBriefing";
_global.SCREEN.FES_COOP_POSTGAME_ROUNDUP = "screens/fes_m_postgameRoundup";
_global.SCREEN.FES_SKIRMISH_SETUP = "screens/fes_m_skirmishGameSetup";
_global.SCREEN.FES_CHAPTER_SELECT = "screens/fes_m_chapterSelect";
_global.SCREEN.FES_FACTION_SELECT_MENU = "screens/fes_m_factionMenu";
_global.SCREEN.IGM_SHELL_ROOT = "InGameUI/igm_shellRoot";
_global.SCREEN.IGM_PAUSE_MENU = "InGameUI/igm_pauseMenu";
if(_global.MSGCODE == undefined)
{
   _global.MSGCODE = new Object();
}
_global.MSGCODE.FE_HIDE = 0;
_global.MSGCODE.FE_SHOW_LOADSCREEN = 100;
_global.MSGCODE.FE_UPDATE_LOADSCREEN = 101;
_global.MSGCODE.FE_LOADING_COMPLETE = 102;
_global.MSGCODE.FE_MESSAGE_SHOW = 103;
_global.MSGCODE.FE_MESSAGE_HIDE = 104;
_global.MSGCODE.FE_MOVIE_CANCELLED = 105;
_global.MSGCODE.FE_MOVIE_COMPLETED = 106;
_global.MSGCODE.FE_SHOW_MISSION_BRIEFING = 107;
_global.MSGCODE.FE_SHOW_MP_LOADSCREEN = 108;
_global.MSGCODE.FE_SHOW_MP_DISCONNECT = 109;
_global.MSGCODE.FE_HIDE_MP_DISCONNECT = 110;
_global.MSGCODE.FE_CANCEL_OPTIONS_SETTINGS_CONFIRMED = 201;
_global.MSGCODE.FE_PROCEED_TO_TUTORIAL = 305;
_global.MSGCODE.IG_PAUSE_MENU_REFRESH_OBJECTIVES = 1000;
_global.MSGCODE.IG_PAUSE_MENU_SUBMIT_MISSION_MAP = 1001;
_global.MSGCODE.IG_PAUSE_MENU_SELECT_OBJECTIVE = 1002;
_global.MSGCODE.IG_PAUSE_MENU_REFRESH_PLAYER_MUTED = 1003;
_global.MSGCODE.IG_PAUSE_MENU_REFRESH_PLAYER_STATUS = 1004;
if(_global.MSGCODE == undefined)
{
   _global.MSGCODE = new Object();
}
_global.MSGCODE.FE_LOAD_SHELL = 20000;
_global.MSGCODE.FE_SHOW_MAIN_MENU = 20001;
_global.MSGCODE.FE_SHOW_MP_GAME_SETUP = 20010;
_global.MSGCODE.FE_SHOW_SAVE_MENU = 20020;
_global.MSGCODE.FE_SHOW_LOAD_MENU = 20021;
_global.MSGCODE.FE_SAVEGAMES_UPDATE = 20022;
_global.MSGCODE.FE_CLOSE_SAVELOAD = 20023;
_global.MSGCODE.IGM_OPEN_SHELL_ROOT = 20100;
_global.MSGCODE.IGM_CLOSE_PAUSE_MENU = 20101;
_global.MSGCODE.FE_SHOW_SKIRMISH_RESULTS = 20200;
_global.MSGCODE.FE_SHOW_CAMPAIGN_RESULTS = 20201;
_global.MSGCODE.FE_SHOW_LAN_RESULTS = 20202;
_global.MSGCODE.FE_SHOW_ONLINE_RESULTS = 20203;
_global.MSGCODE.FE_SHOW_AUTO_MATCH_RESULTS = 20204;
_global.MSGCODE.FE_SHOW_TUTORIAL_RESULTS = 20205;
_global.MSGCODE.FE_MP_UPDATE_GAME_SETTINGS = 21200;
_global.MSGCODE.FE_MP_AUTOMATCH_SEARCH_FAILED = 21250;
_global.MSGCODE.FE_MP_AUTOMATCH_SEARCH_CANCELED = 21275;
_global.MSGCODE.FE_MP_ONLINE_SIGN_IN_SUCCESS = 21300;
_global.MSGCODE.FE_MP_ONLINE_SIGN_IN_FAILURE = 21301;
_global.MSGCODE.FE_MP_ONLINE_GAME_ENUMERATION_COMPLETE = 21302;
_global.MSGCODE.FE_MP_ONLINE_GAME_CONNECT_SUCCESS = 21303;
_global.MSGCODE.FE_MP_ONLINE_GAME_CONNECT_FAILURE = 21304;
_global.MSGCODE.FE_MP_ONLINE_REFRESH_GAMELIST = 21305;
_global.MSGCODE.FE_MP_ONLINE_REFRESH_LOBBYLIST = 21306;
_global.MSGCODE.FE_MP_ONLINE_REFRESH_PLAYERLIST = 21307;
_global.MSGCODE.FE_MP_ONLINE_REFRESH_INVITELIST = 21308;
_global.MSGCODE.FE_MP_ONLINE_INVITE_RECEIVED = 21309;
_global.MSGCODE.FE_MP_ONLINE_REFRESH_GAMEPLAYERS = 21310;
_global.MSGCODE.FE_COOP_ONLINE_GAME_CONNECT_SUCCESS = 21311;
_global.MSGCODE.FE_COOP_ONLINE_GAME_CONNECT_FAILURE = 21312;
_global.MSGCODE.FE_MP_ONLINE_LOBBY_JOINED = 21313;
_global.MSGCODE.FE_MP_REFRESH_CHAT_TEXTLIST = 21314;
_global.MSGCODE.FE_LOST_COCOMMANDER = 21315;
_global.MSGCODE.FE_MP_ONLINE_FRIEND_REQUEST_RECEIVED = 21316;
_global.MSGCODE.FE_MP_ONLINE_FRIEND_STATUS_RECEIVED = 21317;
_global.MSGCODE.FE_COOP_ONLINE_GAME_JOINED_SUCCESS = 21318;
_global.MSGCODE.FE_MP_ONLINE_CAN_HOST_JOIN_CHANGE = 21319;
_global.MSGCODE.FE_MP_ONLINE_SIGN_OUT_SUCCESS = 21320;
_global.MSGCODE.FE_MP_ONLINE_REFRESH_BUDDYLIST = 21321;
_global.MSGCODE.FE_MP_ONLINE_GAME_JOINED_SUCCESS = 21322;
_global.MSGCODE.FE_MP_ONLINE_GAME_COUNTDOWN_START = 21323;
_global.MSGCODE.FE_MP_ONLINE_GAME_COUNTDOWN_STOP = 21324;
_global.MSGCODE.FE_MP_ONLINE_GAME_JOINED_FAILURE = 21325;
_global.MSGCODE.FE_MP_ONLINE_GAME_CREATE_FAILURE = 21326;
_global.MSGCODE.FE_MP_ONLINE_GAME_INCORRECT_PASSWORD = 21327;
_global.MSGCODE.FE_MP_ONLINE_DISCONNECTED = 21330;
_global.MSGCODE.FE_MP_KICKED_FROM_MULTIPLAYER_GAME = 21331;
_global.MSGCODE.FE_MP_KICKED_FROM_COOP_GAME = 21332;
_global.MSGCODE.FE_MP_ONLINE_TOS_SHOW = 21333;
_global.MSGCODE.FE_MP_HOST_LEFT_GAME = 21334;
_global.MSGCODE.FE_COOP_CAMPAIGN_STAGING_STEP_MISSION_INTRO = 21340;
_global.MSGCODE.FE_COOP_CAMPAIGN_STAGING_STEP_RECRUITMENT = 21341;
_global.MSGCODE.FE_COOP_CAMPAIGN_STAGING_STEP_STAGING = 21342;
_global.MSGCODE.FE_COOP_CAMPAIGN_STAGING_STEP_MISSION_BRIEFING = 21343;
_global.MSGCODE.FE_COOP_CAMPAIGN_STAGING_STEP_OBJECTIVE_BRIEFING = 21344;
_global.MSGCODE.FE_COOP_CAMPAIGN_STAGING_START_COUNT_DOWN = 21345;
_global.MSGCODE.FE_COOP_CAMPAIGN_COCOMMANDER_READY = 21346;
_global.MSGCODE.FE_COOP_CAMPAIGN_COMMANDER_READY = 21347;
_global.MSGCODE.FE_TEAM2V2_ONLINE_GAME_CONNECT_SUCCESS = 21360;
_global.MSGCODE.FE_TEAM2v2_ONLINE_GAME_CONNECT_FAILURE = 21361;
_global.MSGCODE.FE_MP_AUTOMATCH_PLAYER_JOINED = 21362;
_global.MSGCODE.FE_MP_AUTOMATCH_HOST_STARTED_SEARCH = 21363;
_global.MSGCODE.FE_MP_AUTOMATCH_PLAYER_LEFT = 21364;
_global.MSGCODE.FE_MP_COUNTDOWN_PAST_POINT_OF_NO_RETURN = 21365;
_global.MSGCODE.FE_NEXT_COMPONENT = 21400;
_global.MSGCODE.FE_PREVIOUS_COMPONENT = 21401;
_global.MSGCODE.FE_UNHANDLED_MOUSE_CLICK = 21600;
_global.MSGCODE.FE_LAN_REFRESH_GAMELIST = 22000;
_global.MSGCODE.FE_LAN_REFRESH_PLAYERLIST = 22001;
_global.MSGCODE.FE_LAN_HOST_GAME_CREATED = 22002;
_global.MSGCODE.FE_LAN_CLIENT_GAME_JOINED = 22003;
_global.MSGCODE.FE_LAN_HOST_LEFT_GAME = 22004;
_global.MSGCODE.FE_MP_LAN_LOBBY_STATE_CHANGE = 22005;
_global.MSGCODE.FE_OPTIONS_REFRESH_AUDIO_CONTROLS = 25000;
_global.MSGCODE.FE_OPTIONS_REFRESH_GENERAL_CONTROLS = 25001;
_global.MSGCODE.FE_OPTIONS_REFRESH_GRAPHICS_CONTROLS = 25002;
_global.MSGCODE.FE_OPTIONS_REFRESH_HOTKEYS_CONTROLS = 25003;
_global.MSGCODE.FE_OPTIONS_REFRESH_NETWORK_CONTROLS = 25004;
_global.MSGCODE.FE_PROFILE_CREATE_SUCCESS = 25100;
_global.MSGCODE.FE_PROFILE_CREATE_FAIL = 25101;
_global.MSGCODE.FE_PROFILE_REFRESH_LIST = 25102;
if(_global.SOUND == undefined)
{
   _global.SOUND = new Object();
}
_global.SOUND.SHELL_NONE = "";
_global.SOUND.SHELL_MAIN = "MenuTrack";
_global.SOUND.BRIEFING = "BriefingTrack";
_global.SOUND.CREDITS = "CreditsTrack";
_global.SOUND.STATS_LOSE = "StatsLoseTrack";
_global.SOUND.STATS_WIN = "StatsWinTrack";
_global.SOUND.LAST_VO_PLAYED = "";
if(_global.ENUM == undefined)
{
   _global.ENUM = new Object();
}
_global.ENUM.GAME_MODE_NONE = 0;
_global.ENUM.GAME_MODE_CAMPAIGN = 1;
_global.ENUM.GAME_MODE_SKIRMISH = 2;
_global.ENUM.GAME_MODE_TUTORIAL = 3;
_global.ENUM.GAME_MODE_MULTIPLAYER = 4;
_global.ENUM.GAME_MODE_SHELL = 5;
_global.ENUM.GAME_MODE_LAN_MP = 6;
_global.ENUM.GAME_COOPMODE_CHOOSER = -1;
_global.ENUM.GAME_COOPMODE_HOST = 0;
_global.ENUM.GAME_COOPMODE_CLIENT = 1;
_global.ENUM.GAME_COOPMODE_AI = 2;
_global.ENUM.SAVELOADSCREEN_SAVE = 0;
_global.ENUM.SAVELOADSCREEN_LOAD = 1;
_global.ENUM.CAMPAIGN_TYPE_ALLIES = 0;
_global.ENUM.CAMPAIGN_TYPE_SOVIET = 1;
_global.ENUM.CAMPAIGN_TYPE_RISINGSUN = 2;
_global.ENUM.MOUSEBUTTONPRESS_LEFT = 0;
_global.ENUM.MOUSEBUTTONPRESS_MIDDLE = 1;
_global.ENUM.MOUSEBUTTONPRESS_RIGHT = 2;
_global.ENUM.CONTEXTMENU_INVALID_SELECTION = -1;
_global.ENUM.CONTEXTMENU_ADD_AS_FRIEND = 0;
_global.ENUM.CONTEXTMENU_INVITE_TO_GAME = 1;
_global.ENUM.CONTEXTMENU_WHISPER = 2;
_global.ENUM.CONTEXTMENU_PRIVATE_CHAT = 3;
_global.ENUM.CONTEXTMENU_IGNORE = 4;
_global.ENUM.CONTEXTMENU_REMOVE_FROM_FRIENDS = 5;
_global.ENUM.CONTEXTMENU_REMOVE_FROM_IGNORE = 6;
if(_global.GAME == undefined)
{
   _global.GAME = new Object();
}
_global.GAME.MODE = _global.ENUM.GAME_MODE_NONE;
_global.GAME.COOPMODE = _global.ENUM.GAME_COOPMODE_AI;
_global.GAME.SAVELOADSCREEN_TYPE = _global.ENUM.SAVELOADSCREEN_LOAD;
_global.GAME.CAMPAIGN_TYPE = _global.ENUM.CAMPAIGN_TYPE_SOVIET;
_global.GAME.PLAY_MISSION_INTRO_MOVIE = false;
if(_global.SHELL == undefined)
{
   _global.SHELL = new Object();
}
_global.SHELL.COMLINK_PROMPT_SEEN = false;
_global.SHELL.INVITED_TO_TEAM_AUTOMATCH = false;
this.createEmptyMovieClip("gShellScreen",20);
_global.gShellScreen = _root.gShellScreen;
_global.gShellScreen.loadMovie(_global.SCREEN.FEG_MAIN);
_global.gShellScreen._visible = true;
var hudMC = this.createEmptyMovieClip("hud",10);
hudMC.loadMovie("../common/hud/ig_HUD.swf");
this.createEmptyMovieClip("gMessageBox",900);
_global.gMessageBox = _root.gMessageBox;
_global.gMessageBox.loadMovie(_global.SCREEN.FEG_MESSAGE_BOX);
_global.gMessageBox._visible = true;
this.createEmptyMovieClip("gDisconnect",910);
_global.gDisconnect = _root.gDisconnect;
_global.gDisconnect.loadMovie(_global.SCREEN.FEG_DISCONNECT);
_global.gDisconnect._visible = true;
initGlobalVariables();
_root.main = _global.gFWS.main;
delayMainCall(5);
if(!initialized)
{
   initialized = true;
   fscommand("OnMainInitialized");
}
