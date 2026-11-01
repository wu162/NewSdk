// WARNING: This class was decompiled without detecting uninitialized class fields.
class AptApiClass
{
   static var extern = {DoTrace:0,InGame:0,Cnc3PcDemo:0};
   static var debug = new DebugClass("AptApiClass::");
   static var m_aptActive = undefined;
   var m_codePrefix = "";
   function AptApiClass(className, screen)
   {
      this.m_codePrefix = "Apt" + className + "::";
      m_aptActive = InGame();
      this.Call("RegisterScreen",screen._target);
   }
   static function InGame(Void)
   {
      if(m_aptActive == undefined)
      {
         m_aptActive = Boolean(Number(extern.InGame));
      }
      return m_aptActive;
   }
   function get Active()
   {
      return m_aptActive;
   }
   function Call(func, params)
   {
      debug.Trace("AptCall " + this.m_codePrefix + func + "(" + params + ")");
      fscommand(this.m_codePrefix + func,params);
   }
   function CallAbs(func, params)
   {
      debug.Trace("CallAbs " + func + "(",params,")");
      fscommand(func,params);
   }
   function LoadVariables(varName, obj, convertStr)
   {
      if(m_aptActive)
      {
         obj = new Object();
         _loadVariables(this.m_codePrefix + varName,obj);
      }
      if(convertStr != undefined)
      {
         this.ConvertObjectMembers(obj,convertStr);
      }
      debug.Dump(obj,"LoadVariables:" + varName);
      return obj;
   }
   function ConvertObjectMembers(obj, convertStr)
   {
      var _loc5_ = convertStr.split(",");
      var _loc6_ = _loc5_.length;
      var _loc2_ = 0;
      var _loc4_;
      var _loc1_;
      while(_loc2_ < _loc6_)
      {
         _loc4_ = _loc5_[_loc2_].split("=");
         _loc1_ = _loc4_[0];
         switch(_loc4_[1])
         {
            case "Array":
               obj[_loc1_] = obj[_loc1_].split(",");
               break;
            case "Boolean":
               obj[_loc1_] = !Number(obj[_loc1_]) ? false : true;
               break;
            case "Number":
               obj[_loc1_] = Number(obj[_loc1_]);
               break;
            default:
               trace("ERROR: AptLoadVariables - Unable to convert : " + _loc5_[_loc2_]);
         }
         _loc2_ = _loc2_ + 1;
      }
   }
   static function _loadVariables(varName, obj)
   {
      loadVariables(varName,obj);
      return obj;
   }
   function GetExtern(ext, testDefault)
   {
      var _loc2_;
      ext = this.m_codePrefix + ext;
      if(m_aptActive)
      {
         _loc2_ = extern[ext];
      }
      else
      {
         if(extern[ext] == undefined)
         {
            _loc2_ = testDefault;
         }
         else
         {
            _loc2_ = extern[ext];
         }
         debug.Trace("GetExtern(" + ext + ")=>" + _loc2_);
      }
      return _loc2_;
   }
   function SetExtern(ext, to)
   {
      ext = this.m_codePrefix + ext;
      extern[ext] = to;
      debug.Trace("SetExtern(" + ext + ")=>" + to);
   }
   function ToggleExtern(ext)
   {
      ext = this.m_codePrefix + ext;
      var _loc2_ = !Number(extern[ext]);
      extern[ext] = _loc2_;
      return _loc2_;
   }
   function PlaySound(name)
   {
      fscommand("PlaySound",name);
   }
   function IsCnc3Demo(testDefault)
   {
      return !InGame() ? testDefault : extern.Cnc3PcDemo != "0";
   }
   static function EnableEngineComponents(path)
   {
      debug.Trace("EnableEngineComponents:" + path);
      fscommand("EnableComponents",path);
   }
   static function DisableEngineComponents(path)
   {
      debug.Trace("DisableEngineComponents:" + path);
      fscommand("DisableComponents",path);
   }
}
