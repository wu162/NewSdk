function AddSegment(from, seg)
{
   var _loc2_ = from._x + seg * vSpacingX;
   var _loc3_ = from.duplicateMovieClip(from._name + "_" + seg,vDepth++,{_x:_loc2_});
}
function SetPercent(newPercent)
{
   var _loc2_ = false;
   var _loc3_;
   var _loc1_;
   while(!_loc2_)
   {
      _loc2_ = true;
      trace("vCurPercent:" + vCurPercent + " ," + newPercent);
      if(vCurPercent < newPercent && vCurPercent < 100)
      {
         _loc3_ = Math.round(vNumSegments * vCurPercent / 100);
         vCurPercent++;
         trace("vCurPercent:" + vCurPercent);
         _loc1_ = Math.round(vNumSegments * vCurPercent / 100);
         if(_loc3_ != _loc1_)
         {
            AddSegment(ProgressFull,_loc1_);
         }
         if(vCurPercent == 100)
         {
            clearInterval(vIntervalId);
         }
         else
         {
            _loc2_ = vCurPercent >= newPercent;
         }
      }
   }
}
var vCurPercent = 0;
var vNumSegments = 25;
var vSpacingX = 11;
var vDepth = 100;
var seg = 1;
while(seg <= vNumSegments)
{
   AddSegment(ProgressEmpty,seg);
   seg++;
}
var vIntervalId = setInterval(FrameUpdate,100);
