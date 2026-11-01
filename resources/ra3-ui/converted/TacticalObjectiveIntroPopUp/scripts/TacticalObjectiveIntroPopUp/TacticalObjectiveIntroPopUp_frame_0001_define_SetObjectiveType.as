function SetObjectiveType(cat, bonusArg)
{
   var _loc3_ = bonusArg != 0;
   var _loc4_ = 11993018;
   var _loc6_ = 16772674;
   var _loc2_ = !_loc3_ ? _loc4_ : _loc6_;
   popUpMC.titleTF.textColor = _loc2_;
   var _loc5_ = new Color(popUpMC.dividerMC);
   _loc5_.setRGB(_loc2_);
   var _loc1_ = !_loc3_ ? "_primary" : "_bonus";
   SetOuterFrameState(popUpMC.outerFrameMC,_loc1_);
   SetInnerFrameState(popUpMC.innerFrameMC,_loc1_);
   popUpMC.symbolMC.gotoAndPlay(cat + _loc1_);
}
function Close()
{
   if(!closing)
   {
      gotoAndStop("_close");
      play();
      closing = true;
   }
}
function AdjustOuterFrameHeight(frameMC, heightChange)
{
   frameMC.bl._y += heightChange;
   frameMC.bottom._y += heightChange;
   frameMC.br._y += heightChange;
   frameMC.left._height += heightChange;
   frameMC.right._height += heightChange;
   frameMC.bg._height += heightChange;
}
function SetOuterFrameState(frameMC, stateLabel)
{
   frameMC.tl.gotoAndPlay(stateLabel);
   frameMC.top.gotoAndPlay(stateLabel);
   frameMC.tr.gotoAndPlay(stateLabel);
   frameMC.right.gotoAndPlay(stateLabel);
   frameMC.br.gotoAndPlay(stateLabel);
   frameMC.bottom.gotoAndPlay(stateLabel);
   frameMC.bl.gotoAndPlay(stateLabel);
   frameMC.left.gotoAndPlay(stateLabel);
}
function AdjustInnerFrameHeight(frameMC, heightChange)
{
   frameMC.bottom._y += heightChange;
   frameMC.left._height += heightChange;
   frameMC.right._height += heightChange;
   frameMC.bg._height += heightChange;
}
function SetInnerFrameState(frameMC, stateLabel)
{
   frameMC.top.gotoAndPlay(stateLabel);
   frameMC.right.gotoAndPlay(stateLabel);
   frameMC.bottom.gotoAndPlay(stateLabel);
   frameMC.left.gotoAndPlay(stateLabel);
}
function onUnload()
{
   delete SetObjectiveType;
   delete Close;
   delete AdjustOuterFrameHeight;
   delete SetOuterFrameState;
   delete AdjustInnerFrameHeight;
   delete SetInnerFrameState;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   var origTextHeight = popUpMC.textTF._height;
   popUpMC.titleTF.text = "$" + _global.qualifyName(this,"Title") + "&outline";
   popUpMC.textTF.autoSize = "left";
   popUpMC.textTF.text = "$" + _global.qualifyName(this,"Text") + "&outline";
   var newTextHeight = popUpMC.textTF._height;
   if(newTextHeight > origTextHeight)
   {
      var heightIncrease = newTextHeight - origTextHeight;
      AdjustOuterFrameHeight(popUpMC.outerFrameMC,heightIncrease);
      AdjustInnerFrameHeight(popUpMC.innerFrameMC,heightIncrease);
   }
   closing = false;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
