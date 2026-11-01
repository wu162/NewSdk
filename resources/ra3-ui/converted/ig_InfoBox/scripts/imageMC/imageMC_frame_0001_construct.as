onClipEvent(construct){
   _type = "RenderImage";
   _imageMap = "";
   _mode = "";
}
onClipEvent(load){
   _imageMap = qualifyName(_parent._parent._parent,"Image");
}
