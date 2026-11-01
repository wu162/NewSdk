function SetPercent(to)
{
   if(to <= 0)
   {
      _visible = false;
      targetSize = 0;
      targetPercent = 0;
      textPercent = "";
   }
   else
   {
      _visible = true;
      targetLevel = int(to);
      if(targetLevel >= 100)
      {
         targetLevel = 100;
      }
      barSize = barFullSize._width;
      barSizeFactor = barSize / 100;
      targetPercent = targetLevel / barSize;
      targetSize = barSize * targetPercent * barSizeFactor;
      bar._width = targetSize;
      textPercent = targetLevel + "%" + "&dropShadow";
   }
}
SetPercent(-1);
stop();
