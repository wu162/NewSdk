onClipEvent(load){
   var component = _parent._parent;
   this.stringValue = "$" + component.stringName + _parent._name;
   this.TheString._width = component.stringWidth;
   var color = new Color(TheString);
   var externName = "GameLoading:PlayerColor:" + _parent._name;
   var colorVal = _root.GetExtern(externName);
   if(colorVal == undefined)
   {
      colorVal = 4280283648 * (Number(_parent._name) + 1);
   }
   color.setRGB(Number(colorVal));
}
