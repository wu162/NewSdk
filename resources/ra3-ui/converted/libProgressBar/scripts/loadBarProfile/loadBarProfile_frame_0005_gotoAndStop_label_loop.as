if(barSize <= targetSize + 0.20000000298023224 && barSize >= targetSize - 0.20000000298023224)
{
   stop();
}
else
{
   gotoAndStop("loop");
   play();
}
