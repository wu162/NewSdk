on(press){
   gotoAndStop("_down");
   play();
}
on(release){
   gotoAndStop("_release");
   play();
   _parent.OnSpinnerButtonClicked(this);
}
on(rollOver, dragOver){
   gotoAndStop("_over");
   play();
}
on(rollOut, dragOut){
   gotoAndStop("_up");
   play();
}
