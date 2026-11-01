on(press){
   fscommand(qualifyName(this,"OnPressed"),"");
}
on(release){
   fscommand(qualifyName(this,"OnClicked"),"");
}
on(rollOver){
   _alpha = 100;
   fscommand(qualifyName(this,"OnRollOver"),"");
}
on(rollOut){
   _alpha = 50;
   fscommand(qualifyName(this,"OnRollOut"),"");
}
