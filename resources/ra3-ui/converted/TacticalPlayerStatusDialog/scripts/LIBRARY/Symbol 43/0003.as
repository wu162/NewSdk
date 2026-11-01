onClipEvent(load){
   if(_root.displayLeft != undefined)
   {
      var left = _root.displayLeft;
      var top = _root.displayTop;
      var ancestor = _parent;
      while(ancestor != _root)
      {
         left -= ancestor._x;
         top -= ancestor._y;
         ancestor = ancestor._parent;
      }
      _X = left;
      _Y = top;
      _width = _root.displayWidth;
      _height = _root.displayHeight;
   }
}
