on(press){
   if(_parent._parent.selectedTabNum != Number(_parent._name))
   {
      _parent.gotoAndPlay("_down");
   }
}
on(release){
   if(_parent._parent.selectedTabNum != Number(_parent._name))
   {
      _parent.gotoAndPlay("_over");
   }
   _parent.client.OnClicked(_parent);
}
on(rollOver, dragOver){
   if(_parent._parent.selectedTabNum != Number(_parent._name))
   {
      _parent.gotoAndPlay("_over");
   }
   _parent.client.OnRollOver(_parent);
}
on(rollOut, dragOut){
   if(_parent._parent.selectedTabNum != Number(_parent._name))
   {
      _parent.gotoAndPlay("_up");
   }
   _parent.client.OnRollOut(_parent);
}
