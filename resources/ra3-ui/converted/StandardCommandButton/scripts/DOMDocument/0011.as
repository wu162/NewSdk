onClipEvent(construct){
   _type = "RenderImage";
   _imageMap = "";
   _mode = "";
}
onClipEvent(load){
   _imageMap = qualifyName(_parent,"Image");
   colorizer = new Color(this);
   if(_parent.imageTint != undefined)
   {
      colorizer.setRGB(_parent.imageTint);
   }
}
