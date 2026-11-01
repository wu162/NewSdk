function onLoad()
{
   trace("feg_m_disconnect::onLoad()");
   this.onEnterFrame = _global.bind0(this,frameDelayedInit);
}
function frameDelayedInit()
{
   this.onEnterFrame = null;
   m_refGlobalEventHandler = _root.gEH;
   this.disconnectMC.quitButton.setOnMouseUpFunction(this.quitButtonClicked);
   gMH.addMessageHandler(localMessageHandler);
   this.disconnectMC.quitButton.enable();
   var _loc3_ = this.disconnectMC;
   m_playerClips = [_loc3_.entry0,_loc3_.entry1,_loc3_.entry2,_loc3_.entry3,_loc3_.entry4,_loc3_.entry5];
   hideDisconnect();
}
function quitButtonClicked()
{
   hideDisconnect();
   fscommand("CallGameFunction","%DisconnectQuitGame");
}
function onUnload()
{
   trace("feg_m_disconnect::onUnload()");
   gMH.removeMessageHandler(localMessageHandler);
   delete _global.feg_m_disconnect;
}
function localMessageHandler(messageCode)
{
   trace("feg_m_disconnect::localMessageHandler() messageCode = \'" + messageCode + "\'");
   var _loc2_ = false;
   switch(messageCode)
   {
      case _global.MSGCODE.FE_SHOW_MP_DISCONNECT:
         showDisconnect();
         _loc2_ = true;
         break;
      case _global.MSGCODE.FE_HIDE_MP_DISCONNECT:
         hideDisconnect();
         _loc2_ = true;
   }
   return _loc2_;
}
function showDisconnect()
{
   trace("feg_m_disconnect::showDisconnect()");
   if(m_refGlobalEventHandler.getCurrentLocalEventHandler() == undefined)
   {
      m_refGlobalEventHandler.setLocalEventHandler(null);
   }
   m_eventHandlerIndex = m_refGlobalEventHandler.addPriorityLocalEventHandler(localEventHandler,m_eventHandlerPriority);
   var _loc3_ = this.disconnectMC.quitButton;
   _loc3_.setText("$APT:DISCONNECTQUITGAME");
   _loc3_.show();
   _loc3_.enable();
   this.shadeMC._visible = true;
   this.disconnectMC._visible = true;
   m_playAudio = true;
   updateScreen();
   this.onEnterFrame = _global.bind0(this,updateScreen);
}
function hideDisconnect()
{
   trace("feg_m_disconnect::hideDisconnect()");
   this.onEnterFrame = null;
   m_playAudio = false;
   this.shadeMC._visible = false;
   this.disconnectMC._visible = false;
   if(m_eventHandlerIndex != -1)
   {
      m_refGlobalEventHandler.removeLocalEventHandler(m_eventHandlerIndex);
   }
   m_eventHandlerIndex = -1;
}
function localEventHandler(keyCode, controller)
{
   m_FM.handleDefaultInputs(keyCode);
}
function updateScreen()
{
   var _loc5_ = new Object();
   loadVariables("QueryGameEngine?DISCONNECT_INFO",_loc5_);
   var _loc3_ = _loc5_.DISCONNECT_INFO_NAMES.split(",");
   var _loc4_ = _loc5_.DISCONNECT_INFO_TIMES.split(",");
   var _loc2_ = 0;
   var _loc1_ = 0;
   while(_loc1_ < m_playerClips.length)
   {
      m_playerClips[_loc1_]._visible = false;
      if(_loc3_[_loc1_].length > 0)
      {
         m_playerClips[_loc2_]._visible = true;
         m_playerClips[_loc2_].playerTF.text = _loc3_[_loc1_];
         m_playerClips[_loc2_].timeTF.text = _loc4_[_loc1_];
         _loc2_ = _loc2_ + 1;
      }
      _loc1_ = _loc1_ + 1;
   }
}
trace("feg_m_disconnect loading now");
var PADDING = 5;
var m_messageCallback;
var m_playAudio = false;
var m_eventHandlerIndex = -1;
var m_eventHandlerPriority = 1;
var m_refGlobalEventHandler;
var m_playerClips;
var OVERLAP_OFFSET = 4;
onLoad();
trace("feg_m_disconnect loaded");
stop();
