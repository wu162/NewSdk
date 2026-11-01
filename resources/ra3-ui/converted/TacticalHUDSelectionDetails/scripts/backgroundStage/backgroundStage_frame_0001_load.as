onClipEvent(load){
   if(_parent.backgroundClip != undefined && _parent.backgroundClip.length > 0)
   {
      var clip = this.attachMovie(_parent.backgroundClip + (!_parent.isSelected ? "Normal" : "Highlighted"),"clip",0);
      clip.gotoAndPlay(_parent.currentStateLabel);
   }
}
