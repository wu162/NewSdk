on(press){
   if(client.selectedTabButton != this)
   {
      gotoAndStop("_down");
      play();
   }
   client.OnPressed(this);
}
on(release){
   if(client.selectedTabButton != this)
   {
      gotoAndStop("_over");
      play();
      client.OnClicked(this);
   }
}
on(rollOver, dragOver){
   if(client.selectedTabButton != this)
   {
      gotoAndStop("_over");
      play();
   }
   client.OnRollOver(this);
}
on(rollOut, dragOut){
   if(client.selectedTabButton != this)
   {
      gotoAndStop("_up");
      play();
   }
   client.OnRollOut(this);
}
