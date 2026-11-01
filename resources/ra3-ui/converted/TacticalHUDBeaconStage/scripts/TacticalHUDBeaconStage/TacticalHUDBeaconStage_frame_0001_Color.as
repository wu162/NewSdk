if(!initialized)
{
   if(_parent != undefined && _parent._parent != undefined && _parent._parent.beaconColor != undefined)
   {
      var colorizer1 = new Color(cornerUL);
      colorizer1.setRGB(_parent._parent.beaconColor);
      var colorizer2 = new Color(cornerUR);
      colorizer2.setRGB(_parent._parent.beaconColor);
      var colorizer3 = new Color(cornerLL);
      colorizer3.setRGB(_parent._parent.beaconColor);
      var colorizer4 = new Color(cornerLR);
      colorizer4.setRGB(_parent._parent.beaconColor);
      var colorizer5 = new Color(edgeT);
      colorizer5.setRGB(_parent._parent.beaconColor);
      var colorizer6 = new Color(edgeB);
      colorizer6.setRGB(_parent._parent.beaconColor);
      var colorizer7 = new Color(edgeL);
      colorizer7.setRGB(_parent._parent.beaconColor);
      var colorizer8 = new Color(edgeR);
      colorizer8.setRGB(_parent._parent.beaconColor);
   }
   var inset = 2;
   var clipWidth = _parent.messageText._width + 25;
   var clipHeight = _parent.messageText._height + 20;
   this._x = (- clipWidth) * 0.5;
   this._y = _parent.messageText._y - 11;
   background._xscale = clipWidth;
   background._yscale = clipHeight;
   cornerUL._x = inset;
   cornerUL._y = inset;
   cornerUR._x = background._width - inset;
   cornerUR._y = inset;
   cornerLL._x = inset;
   cornerLL._y = background._height - inset;
   cornerLR._x = background._width - inset;
   cornerLR._y = background._height - inset;
   var cornerSize = cornerUL._width;
   edgeT._x = cornerSize + inset * 2;
   edgeT._y = inset;
   edgeT._xscale = clipWidth - (cornerSize + inset + inset) * 2;
   edgeT._yscale = 1;
   edgeB._x = cornerSize + inset * 2;
   edgeB._y = background._height - inset - edgeB._height;
   edgeB._xscale = clipWidth - (cornerSize + inset + inset) * 2;
   edgeB._yscale = 1;
   edgeL._x = inset;
   edgeL._y = cornerSize + inset * 2;
   edgeL._xscale = 1;
   edgeL._yscale = clipHeight - (cornerSize + inset + inset) * 2;
   edgeR._x = background._width - inset - edgeR._width;
   edgeR._y = cornerSize + inset * 2;
   edgeR._xscale = 1;
   edgeR._yscale = clipHeight - (cornerSize + inset + inset) * 2;
   initialized = true;
}
stop();
