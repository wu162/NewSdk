function SetDisplayResolution(widthArg, heightArg)
{
   var _loc1_ = Number(widthArg);
   var _loc3_ = Number(heightArg);
   var _loc2_ = 1024;
   var _loc4_ = 768;
   if(_loc1_ * _loc4_ > _loc3_ * _loc2_)
   {
      _loc1_ *= _loc4_ / _loc3_;
      panel._x -= (_loc1_ - _loc2_) * 0.5;
      trace("panel._x = " + String(panel._x));
   }
}
function GameCode(func, params)
{
   trace("Call " + _root.CodePrefix + "::" + func + "(" + params + ")");
   fscommand(_root.CodePrefix + "::" + func,params);
}
function GameCodeNoPrefix(func, params)
{
   trace("Call(NP) " + func + "(" + params + ")");
   fscommand(func,params);
}
function PlaySound(name)
{
   if(!_root.Initialized)
   {
      return undefined;
   }
   fscommand("PlaySound",name);
}
function InitialSetup()
{
   _root.Initialized = false;
   if(extern.InGame)
   {
      _global.InGame = true;
      _global.InBetaDemo = undefined;
      _global.InDreamMachineDemo = undefined;
   }
   if(_global.InBetaDemo == undefined && extern.InBetaDemo)
   {
      _global.InBetaDemo = true;
   }
   if(_global.InDreamMachineDemo == undefined && extern.InDreamMachineDemo)
   {
      _global.InDreamMachineDemo = true;
   }
   _root.InitFunc();
   _root.Initialized = true;
}
function EnableComponents(path)
{
   trace("EnableComponents:" + path);
   fscommand("EnableComponents",path);
}
function DisableComponents(path)
{
   trace("DisableComponents:" + path);
   fscommand("DisableComponents",path);
}
function GetExtern(ext, testDefault)
{
   var _loc3_;
   if(_global.InGame)
   {
      _loc3_ = extern[ext];
   }
   else
   {
      if(_root[ext] == undefined)
      {
         _loc3_ = testDefault;
      }
      else
      {
         _loc3_ = _root[ext];
      }
      trace("GetExtern(" + ext + ")=>" + _loc3_);
   }
   return _loc3_;
}
function SetExtern(ext, to)
{
   if(_global.InGame)
   {
      extern[ext] = to;
   }
   else
   {
      _root[ext] = to;
      trace("SetExtern(" + ext + ")=>" + to);
   }
}
function ToggleExtern(ext)
{
   var _loc3_;
   if(_global.InGame)
   {
      _loc3_ = !Number(extern[ext]);
      extern[ext] = _loc3_;
   }
   else
   {
      _loc3_ = !_root[ext];
      _root[ext] = _loc3_;
   }
   return _loc3_;
}
function StopBackground()
{
   fscommand("SetBackground","off");
}
function FadeInBackground()
{
   fscommand("SetBackground","fadein");
}
function FadeOutBackground()
{
   fscommand("SetBackground","fadeout");
}
function FindAbsolute(of, from)
{
   if(from._parent == undefined)
   {
      return from[of];
   }
   return FindAbsolute(of,from._parent) + from[of];
}
function OpenChat()
{
   panel.gotoAndPlay("_open");
}
function CloseChat()
{
   panel.gotoAndPlay("_close");
}
function EnableSendButton()
{
   panel.mcInGameChat.Send.gotoAndPlay("_reveal");
}
function DisableSendButton()
{
   panel.mcInGameChat.Send.gotoAndPlay("_disabled");
}
_root.CodePrefix = "AptInGameChat";
colorInnerFrameA = "3399FF";
colorInnerFrameB = "3399FF";
colorLine = "3399FF";
colorGadgetBox = "3399FF";
colorGadgetGameSpeed = "3399FF";
colorTextDark = "0x57512B";
colorTextBright = "0x31210F";
colorSubTextBright = "0xC2F47D";
colorSubTextDark = "0x57842B";
colorTitleA = "0xCDE47E";
tabTextColorDark = "0x123B15";
tabTextColorBritght = "0x8ac14d";
if(!_root.Initialized)
{
   if(_root.InitFunc == undefined)
   {
      _root.InitFunc = function()
      {
         GameCode("OnInitialized");
      };
   }
   InitialSetup();
}
OpenChat();
