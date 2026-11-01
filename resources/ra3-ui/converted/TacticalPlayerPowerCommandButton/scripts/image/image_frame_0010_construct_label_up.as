onClipEvent(construct){
   _type = "RenderImage";
   _imageMap = "";
   _mode = "";
}
onClipEvent(load){
   _imageMap = qualifyName(_parent._parent,"Image");
   colorizer = new Color(this);
   if(_parent._parent.imageTint != undefined)
   {
      colorizer.setRGB(_parent._parent.imageTint);
   }
}
