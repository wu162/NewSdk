function onUnload()
{
   delete AddSegment;
   delete Frameupdate;
   delete OnDone;
   delete onUnload;
}
function AddSegment(from, seg)
{
   var _loc2_ = from._x + seg * vSpacingX;
   var _loc3_ = from.duplicateMovieClip(from._name + "_" + seg,vDepth++,{_x:_loc2_});
}
function FrameUpdate()
{
   var _loc4_ = false;
   var _loc3_;
   var _loc5_;
   var _loc2_;
   while(!_loc4_)
   {
      _loc4_ = true;
      _loc3_ = _root.LoadScreenImage.GetPercentLoaded();
      if(vCurPercent < _loc3_ && vCurPercent < 100)
      {
         _loc5_ = Math.round(vNumSegments * vCurPercent / 100);
         vCurPercent++;
         _loc2_ = Math.round(vNumSegments * vCurPercent / 100);
         if(_loc5_ != _loc2_)
         {
            AddSegment(ProgressFull,_loc2_);
         }
         if(vCurPercent == 100)
         {
            clearInterval(vIntervalId);
            vIntervalId = setInterval(OnDone,300);
         }
         else
         {
            _loc4_ = vCurPercent >= _loc3_;
         }
      }
   }
}
function OnDone()
{
   clearInterval(vIntervalId);
   _root.LoadScreenImage.OnDone();
}
var vCurPercent = 0;
var vNumSegments = 70;
var vSpacingX = 11;
var vDepth = 100;
var seg = 1;
while(seg <= vNumSegments)
{
   AddSegment(ProgressEmpty,seg);
   seg++;
}
var vIntervalId = setInterval(FrameUpdate,100);
