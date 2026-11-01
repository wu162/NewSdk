on(rollOver){
   gotoAndStop("_over");
   play();
   fscommand(qualifyName(_parent._parent,_parent,"OnButtonRollOver"),"");
}
on(release){
   fscommand(qualifyName(_parent._parent._parent,"OnButtonClicked"),"");
   _parent.OnRelease(this);
   gotoAndStop("_down");
   play();
   for(child in _parent)
   {
      if(_name != child)
      {
         _parent[child].gotoAndPlay("_exit");
      }
   }
}
on(rollOut, dragOut){
   gotoAndStop("_rollout");
   play();
   fscommand(qualifyName(_parent._parent,_parent,"OnButtonRollOut"),"");
}
