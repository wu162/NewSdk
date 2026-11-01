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
function SetBarTo(index, percent)
{
   _root.UIClip.ProgressBars.SetBarTo(index,percent);
}
function AnimsOpen()
{
   frames.gotoAndPlay(1);
   screen.gotoAndPlay(1);
   textAnim.gotoAndPlay(1);
   pageTitle.gotoAndPlay(1);
}
_root.CodePrefix = "AptGameLoading";
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
