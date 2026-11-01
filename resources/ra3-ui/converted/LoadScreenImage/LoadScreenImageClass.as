// WARNING: This class was decompiled without detecting uninitialized class fields.
class LoadScreenImageClass extends MovieClip
{
   static var m_classRoot = "LoadScreenImage";
   var m_loadingString = undefined;
   static var debug = new DebugClass(m_classRoot);
   function LoadScreenImageClass()
   {
      super();
      this.m_apt = new AptApiClass(m_classRoot,this);
      this.m_percentLoaded = 0;
   }
   function onUnload()
   {
      delete this.m_apt;
      delete debug;
      AptZombieClass.DeleteFunctions(this);
      AptZombieClass.DeleteClass(m_classRoot + "Class");
   }
   function OnDone()
   {
      trace("All done");
      this.m_apt.Call("OnLoadScreenDone");
   }
   function RegisterLoadingString(loadingString)
   {
      this.m_loadingString = loadingString;
   }
   function GetPercentLoaded(Void)
   {
      return this.m_percentLoaded;
   }
   function _SetPercentLoaded(percent)
   {
      this.m_percentLoaded = Number(percent);
   }
}
