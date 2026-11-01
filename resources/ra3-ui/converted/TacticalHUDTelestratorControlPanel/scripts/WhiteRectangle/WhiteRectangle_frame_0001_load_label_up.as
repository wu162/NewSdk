onClipEvent(load){
   var boxColor = _parent.red << 16 | _parent.green << 8 | _parent.blue;
   colorizer = new Color(this);
   colorizer.setRGB(boxColor);
}
