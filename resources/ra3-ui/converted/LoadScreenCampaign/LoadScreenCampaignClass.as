// WARNING: This class was decompiled without detecting uninitialized class fields.
class LoadScreenCampaignClass extends MovieClip
{
   static var m_classRoot = "LoadScreenCampaign";
   var m_playButton = undefined;
   var m_loadingString = undefined;
   static var debug = new DebugClass(m_classRoot);
   function LoadScreenCampaignClass()
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
   function RegisterPlayNowBttn(bttn)
   {
      this.m_playButton = bttn;
      this.m_playButton.ReleaseCallback = function()
      {
         _root.LoadScreenCampaign.m_apt.Call("OnBttnPlayNow");
         _root.LoadScreenCampaign.gotoAndPlay("_fadeOut");
      };
      if(!AptApiClass.InGame())
      {
         this._EnablePlayNow();
      }
   }
   function RegisterLoadingString(loadingString)
   {
      this.m_loadingString = loadingString;
   }
   function GetPercentLoaded(Void)
   {
      return this.m_percentLoaded;
   }
   function _EnablePlayNow(Void)
   {
      this.m_playButton.SetMode("");
      this.m_loadingString._visible = false;
   }
   function _SetPercentLoaded(percent)
   {
      this.m_percentLoaded = Number(percent);
   }
}
