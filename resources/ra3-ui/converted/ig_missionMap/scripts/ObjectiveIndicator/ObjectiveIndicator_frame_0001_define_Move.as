function Move(newX, newY)
{
   _parent.MoveIndicator(this,newX,newY);
}
function SetSelected(val)
{
   var _loc1_ = val != 0;
   var _loc2_;
   if(_loc1_ != isSelected)
   {
      isSelected = _loc1_;
      _alpha = !_loc1_ ? 50 : 100;
      _loc2_ = getDepth();
      if(_loc1_)
      {
         _loc2_ += 1000;
      }
      else
      {
         _loc2_ -= 1000;
      }
      swapDepths(_loc2_);
   }
}
function SetVisibility(val)
{
   _visible = val != 0;
}
function OnBound()
{
   if(initialized)
   {
      fscommand(qualifyName(this,"OnInitialized"),"");
   }
}
function OnDestroying()
{
   if(initialized)
   {
      fscommand(qualifyName(this,"OnClipDestroyed"),"");
   }
}
function OnClicked()
{
   trace("ObjectiveIndicator OnClicked(), this = " + String(this));
   fscommand(qualifyName(this,"OnClicked"),"");
}
function onUnload()
{
   delete Move;
   delete SetSelected;
   delete SetVisibility;
   delete OnBound;
   delete OnDestroying;
   delete OnClicked;
   delete onUnload;
}
if(!initialized)
{
   textMC.text = "$" + qualifyName(this,"Ordinal");
   primary.gotoAndPlay(category);
   bonus.gotoAndPlay(category);
   if(isBonus)
   {
      primary._visible = false;
   }
   else
   {
      bonus._visible = false;
   }
   isSelected = false;
   _alpha = 50;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
