function TimerClipStack(stage, id, x)
{
   this.bottom = stage.attachMovie("Timer","Stack" + String(id),id);
   this.bottom._x = x;
   this.top = this.bottom;
   this.size = 1;
}
function Timer(stage, id, color, clip)
{
   this.stage = stage;
   this.id = id;
   this.color = color;
   this.Attach(clip);
}
function GetStackForNewTimer(timerColor)
{
   var _loc4_ = timerStacks.length;
   var _loc3_ = 0;
   var _loc2_;
   while(_loc3_ < _loc4_)
   {
      _loc2_ = timerStacks[_loc3_];
      if(_loc2_.color == timerColor)
      {
         _loc2_.clipStack.Push();
         return _loc2_;
      }
      _loc3_ = _loc3_ + 1;
   }
   var _loc6_ = new Object();
   _loc6_.color = timerColor;
   _loc6_.clipStack = new TimerClipStack(this,nextTimerStackId++,_loc4_ * timerStackWidth);
   _loc6_.timers = new Array();
   timerStacks.push(_loc6_);
   return _loc6_;
}
function KillTimerStack(timerStack)
{
   var _loc3_ = timerStacks.length;
   var _loc1_ = 0;
   while(_loc1_ < _loc3_)
   {
      if(timerStacks[_loc1_] == timerStack)
      {
         break;
      }
      _loc1_ = _loc1_ + 1;
   }
   var _loc2_;
   while(_loc1_ + 1 < _loc3_)
   {
      _loc2_ = timerStacks[_loc1_ + 1];
      timerStacks[_loc1_] = _loc2_;
      _loc2_.clipStack.bottom._x = _loc1_ * timerStackWidth;
      _loc1_ = _loc1_ + 1;
   }
   timerStacks.pop();
   if(timerStacks.length == 0)
   {
      nextTimerStackId = 0;
   }
   timerStack.clipStack.Clear();
}
function AddTimer(timerId, timerColor)
{
   var _loc3_ = GetStackForNewTimer(timerColor);
   var _loc2_ = new Timer(this,String(timerId),timerColor,_loc3_.clipStack.top);
   var _loc4_ = _loc3_.timers.length;
   _loc3_.timers.push(_loc2_);
   _loc2_.stack = _loc3_;
   _loc2_.index = _loc4_;
   this[String(timerId)] = _loc2_;
   return Pointer.encode(_loc2_);
}
function EraseTimer(timerId)
{
   var _loc5_ = this[String(timerId)];
   var _loc4_ = _loc5_.stack;
   var _loc2_ = _loc5_.index;
   var _loc6_ = _loc4_.timers.length;
   var _loc3_;
   while(_loc2_ + 1 < _loc6_)
   {
      _loc3_ = _loc4_.timers[_loc2_ + 1];
      _loc3_.Attach(_loc5_.clip);
      _loc4_.timers[_loc2_] = _loc3_;
      _loc3_.index = _loc2_;
      _loc2_ = _loc2_ + 1;
   }
   _loc4_.timers.pop();
   if(_loc4_.clipStack.size == 1)
   {
      KillTimerStack(_loc4_);
   }
   else
   {
      _loc4_.clipStack.Pop();
   }
   delete this[String(timerId)];
}
function onUnload()
{
   delete TimerClipStack;
   delete Timer;
   delete GetStackForNewTimer;
   delete KillTimerStack;
   delete AddTimer;
   delete EraseTimer;
   delete onUnload;
}
TimerClipStack.prototype.Push = function()
{
   var _loc2_ = this.top.childStage.attachMovie("Timer","child",0);
   _loc2_.mouseOverObserver = this.top;
   this.top = _loc2_;
   this.size = this.size + 1;
   return _loc2_;
};
TimerClipStack.prototype.Pop = function()
{
   var _loc2_ = this.top._parent._parent;
   if(_loc2_.overChild)
   {
      _loc2_.OnTimerRollOut(this.top);
   }
   this.top.removeMovieClip();
   this.top = _loc2_;
   this.size = this.size - 1;
};
TimerClipStack.prototype.Clear = function()
{
   this.bottom.removeMovieClip();
   delete this.bottom;
   delete this.top;
   delete this.size;
};
Timer.prototype.Attach = function(clip)
{
   this.clip = clip;
   if(clip.initialized)
   {
      clip.SetID(String(this.stage) + "." + this.id);
      clip.innerFrameColorizer.setRGB(this.color);
   }
   else
   {
      this.OnTimerInitialized = function(timerClip)
      {
         if(timerClip == this.clip)
         {
            timerClip.SetID(String(this.stage) + "." + this.id);
            timerClip.innerFrameColorizer.setRGB(this.color);
         }
      };
   }
   clip.client = this;
};
Timer.prototype.GetHelpSite = function()
{
   return String(this.clip.helpSite);
};
Timer.prototype.OnTimerClicked = function(timerClip)
{
   if(timerClip == this.clip)
   {
      fscommand(qualifyName(String(this.stage) + "." + this.id,"OnClicked"),"");
   }
};
Timer.prototype.OnTimerPressed = function(timerClip)
{
   if(timerClip == this.clip)
   {
      fscommand(qualifyName(String(this.stage) + "." + this.id,"OnPressed"),"");
   }
};
Timer.prototype.OnTimerRollOver = function(timerClip)
{
   if(timerClip == this.clip)
   {
      fscommand(qualifyName(String(this.stage) + "." + this.id,"OnRollOver"),"");
   }
};
Timer.prototype.OnTimerRollOut = function(timerClip)
{
   if(timerClip == this.clip)
   {
      fscommand(qualifyName(String(this.stage) + "." + this.id,"OnRollOut"),"");
   }
};
if(!initialized)
{
   timerStackWidth = 42;
   nextTimerStackId = 0;
   timerStacks = new Array();
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
