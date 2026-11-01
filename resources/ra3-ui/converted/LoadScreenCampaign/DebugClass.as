// WARNING: This class was decompiled without detecting uninitialized class fields.
class DebugClass
{
   var extern = {DoTrace:1,InGame:0};
   var m_codePrefix = "";
   function DebugClass()
   {
   }
   function InitDebugClass(codePrefix)
   {
      this.m_codePrefix = codePrefix + ":";
      this.Enable(false);
   }
   function Enable(enable)
   {
      if(!Boolean(Number(this.extern.InGame)))
      {
         this.extern.DoTrace = enable;
      }
      this.m_enabled = Boolean(Number(this.extern.DoTrace));
   }
   function Trace(show)
   {
      if(this.m_enabled)
      {
         trace(this.m_codePrefix + show);
      }
   }
   function Dump(obj, msg)
   {
      if(this.m_enabled)
      {
         trace("Dump:" + msg + ":" + typeof obj);
         for(var _loc3_ in obj)
         {
            trace("\t" + _loc3_ + ": " + obj[_loc3_] + " : " + typeof obj[_loc3_]);
         }
      }
   }
}
