on(rollOver, dragOver){
   fscommand(qualifyName(_parent._parent,"OnEnter"),GetMousePosString());
   isMouseOver = true;
}
on(rollOut, dragOut){
   isMouseOver = false;
   fscommand(qualifyName(_parent._parent,"OnLeave"),"");
}
