onClipEvent(load){
   if(_parent.imageClip != undefined && _parent.imageClip.length > 0)
   {
      var clip = this.attachMovie(_parent.imageClip + (!_parent.isSelected ? "Normal" : "Highlighted"),"clip",0);
      clip.gotoAndPlay(_parent.currentStateLabel);
   }
}
