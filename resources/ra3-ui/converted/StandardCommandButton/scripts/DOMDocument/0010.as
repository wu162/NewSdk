onClipEvent(construct){
   _type = "RenderImageDisabled";
   _imageMap = "";
}
onClipEvent(load){
   _imageMap = qualifyName(_parent,"Image");
}
