on(release){
   client.OnClicked(this);
}
on(rollOver){
   if(!isOver)
   {
      isOver = true;
      if(isEnabled)
      {
         box.gotoAndPlay("_over");
      }
      client.OnRollOver(this);
   }
}
on(rollOut){
   if(isOver)
   {
      isOver = false;
      if(isEnabled)
      {
         box.gotoAndPlay("_up");
      }
      client.OnRollOut(this);
   }
}
