if(!initialized)
{
   upButton._visible = true;
   downButton._visible = true;
   thumb._visible = true;
   bar._visible = true;
   pageSize = 0;
   lineSize = 0;
   upButtonOffset = upButton._y;
   downButtonOffset = _height - downButton._y;
   barTopMargin = bar._y;
   barBottomMargin = _height - (bar._y + bar._height);
   _parent.OnScrollBarLoaded(this);
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
