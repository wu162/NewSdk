function AddStage(idArg)
{
   var _loc3_ = Number(idArg);
   var _loc2_ = this[String(_loc3_)];
   if(_loc2_ != undefined)
   {
      _loc2_.removeMovieClip();
   }
   _loc2_ = this.attachMovie("LoadMovieStage",String(_loc3_),_loc3_);
   return Pointer.encode(_loc2_);
}
function RemoveStage(idArg)
{
   var _loc3_ = Number(idArg);
   var _loc2_ = this[String(_loc3_)];
   _loc2_.removeMovieClip();
}
function onUnload()
{
   delete AddStage;
   delete RemoveStage;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
