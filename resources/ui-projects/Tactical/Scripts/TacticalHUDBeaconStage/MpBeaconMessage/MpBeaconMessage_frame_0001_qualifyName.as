if(!initialized)
{
   messageText.text = "$" + qualifyName(_parent,"Message") + "&outline";
   messageText.field.autoSize = "left";
   messageText.field._width;
   messageText._x = - messageText._width * 0.5;
   initialized = true;
}
stop();
