on(press){
   fscommand(qualifyName(_parent,"OnPress"),GetMousePosString());
}
on(release){
   fscommand(qualifyName(_parent,"OnRelease"),GetMousePosString());
}
on(rollOver){
   mouseOver = true;
}
on(rollOut){
   mouseOver = false;
}
