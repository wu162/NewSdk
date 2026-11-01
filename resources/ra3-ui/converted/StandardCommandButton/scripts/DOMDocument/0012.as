on(press){
   if(isEnabled)
   {
      gotoAndStop("_down");
      play();
   }
}
on(release){
   if(isEnabled)
   {
      gotoAndStop("_release");
      play();
   }
   fscommand(qualifyName(this,"OnClicked"),"");
}
on(rollOver){
   if(isEnabled)
   {
      gotoAndStop("_over");
      play();
      RollOver.gotoAndPlay("_show");
      RollOverTop.gotoAndPlay("_show");
   }
   isMouseOver = true;
   fscommand(qualifyName(this,"OnRollOver"),"");
}
on(rollOut, dragOut){
   if(isEnabled)
   {
      gotoAndStop("_up");
      play();
      RollOver.gotoAndPlay("_hide");
      RollOverTop.gotoAndPlay("_hide");
   }
   isMouseOver = false;
   fscommand(qualifyName(this,"OnRollOut"),"");
}
