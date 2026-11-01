if(!initialized)
{
   var baseMessageHeight = messageMC._height;
   messageMC.field.text = "$" + qualifyName(_parent,"Message") + "&outline";
   messageMC.field.autoSize = "left";
   messageMC.field._height;
   var actualMessageHeight = messageMC._height;
   if(actualMessageHeight > baseMessageHeight)
   {
      var heightIncrease = actualMessageHeight - baseMessageHeight;
      middleMC._height += heightIncrease;
      bottomMC._y += heightIncrease;
      backgroundMC._height += heightIncrease;
   }
   initialized = true;
}
stop();
