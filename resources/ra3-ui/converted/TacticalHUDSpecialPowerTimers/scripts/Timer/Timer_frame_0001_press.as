on(press){
   if(client != undefined)
   {
      client.OnTimerPressed(this);
   }
}
on(release){
   if(client != undefined)
   {
      client.OnTimerClicked(this);
   }
}
on(rollOver){
   if(mouseOverObserver != undefined)
   {
      mouseOverObserver.OnTimerRollOver(this);
   }
   if(client != undefined)
   {
      client.OnTimerRollOver(this);
   }
}
on(rollOut){
   if(mouseOverObserver != undefined)
   {
      mouseOverObserver.OnTimerRollOut(this);
   }
   if(client != undefined)
   {
      client.OnTimerRollOut(this);
   }
}
