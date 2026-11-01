onClipEvent(load){
   gotoAndPlay(!_parent.isEnabled ? "_disabled" : (!_parent.isOver ? "_up" : "_over"));
}
