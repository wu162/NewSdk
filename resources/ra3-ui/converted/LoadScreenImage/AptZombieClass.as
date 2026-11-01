// WARNING: This class was decompiled without detecting uninitialized class fields.
class AptZombieClass extends MovieClip
{
   static var s_classes = new Array();
   static var s_objects = new Array();
   static var debug = new DebugClass("AptZombieClass::");
   function AptZombieClass()
   {
      super();
      this._visible = false;
   }
   static function AddClass(linkageIdentifier, className)
   {
      s_classes.push(linkageIdentifier);
      Object.registerClass(linkageIdentifier,className);
   }
   static function AddObject(obj)
   {
      debug.Trace("AddObject:" + obj);
      s_objects.push(obj);
   }
   static function DeleteClass(className)
   {
      debug.Trace("DeleteClass:" + className);
      Object.registerClass(className,null);
      delete _global[className];
   }
   static function DeleteFunctions(obj)
   {
      debug.Trace("DeleteFunctions:" + obj._target);
      var _loc1_;
      for(var _loc3_ in obj)
      {
         _loc1_ = typeof obj[_loc3_];
         debug.Trace("   " + _loc3_ + ":" + _loc1_);
         if(_loc1_ == "movieclip")
         {
            DeleteFunctions(obj[_loc3_],true);
         }
         else if(_loc1_ == "function")
         {
            delete obj[_loc3_];
         }
      }
   }
   function onUnload()
   {
      var _loc1_ = 0;
      while(_loc1_ < s_objects.length)
      {
         _loc1_ = _loc1_ + 1;
      }
      _loc1_ = 0;
      while(_loc1_ < s_classes.length)
      {
         _loc1_ = _loc1_ + 1;
      }
   }
}
