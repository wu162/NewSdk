function OnRelease(buttonClip)
{
   if(buttonClip == Yes)
   {
      fscommand(qualifyName(_parent._parent,"OnYesClicked"),"");
   }
   else if(buttonClip == No)
   {
      fscommand(qualifyName(_parent._parent,"OnNoClicked"),"");
   }
   _parent.gotoAndPlay("_close");
}
function onUnload()
{
   delete OnRelease;
   delete onUnload;
}
