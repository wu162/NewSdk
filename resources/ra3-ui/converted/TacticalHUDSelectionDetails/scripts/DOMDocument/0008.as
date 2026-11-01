onClipEvent(load){
   if(_parent.healthPct != undefined)
   {
      var vFrame = _parent.healthPct + 1;
      this.gotoAndStop(vFrame);
   }
}
