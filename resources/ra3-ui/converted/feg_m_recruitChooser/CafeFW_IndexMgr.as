// WARNING: This class was decompiled without detecting uninitialized class fields.
class CafeFW_IndexMgr
{
   var mIndex = 0;
   var mMinIndex = 0;
   var mMaxIndex = 10;
   var mStepSize = 1;
   var mWrapState = true;
   var mSnapState = false;
   static var TIBINDEX_CHANGE_NONE = 0;
   static var TIBINDEX_CHANGED = 1;
   function CafeFW_IndexMgr()
   {
   }
   function Cafe2_IndexMgr(Void)
   {
      trace("~~TIBTIB~~ CafeFW_IndexMgr::constructor");
   }
   function SetInfo(curIndex, minIndex, maxIndex, curStep, wrapState, snapState)
   {
      var _loc2_ = TIBINDEX_CHANGE_NONE;
      if(maxIndex >= minIndex)
      {
         this.mMinIndex = minIndex;
         this.mMaxIndex = maxIndex;
         this.SetStep(curStep);
         this.SetWrap(wrapState);
         this.SetSnap(snapState);
         _loc2_ = this.UpdateIndex(curIndex);
      }
      else
      {
         trace("ERROR! CafeFW_IndexMgr:SetInfo()  MaxIndex is less than MinIndex");
      }
      return _loc2_;
   }
   function SetIndex(index)
   {
      return this.UpdateIndex(index);
   }
   function SetStep(stepSize)
   {
      var _loc2_ = TIBINDEX_CHANGE_NONE;
      if(stepSize > 0 && stepSize <= this.mMaxIndex - this.mMinIndex)
      {
         if(stepSize != this.mStepSize)
         {
            this.mStepSize = stepSize;
            _loc2_ = TIBINDEX_CHANGED;
         }
      }
      else
      {
         trace("ERROR! CafeFW_IndexMgr:SetStep() - invalid stepSize");
      }
      return _loc2_;
   }
   function SetWrap(wrapState)
   {
      var _loc2_ = TIBINDEX_CHANGE_NONE;
      if(wrapState != this.mWrapState)
      {
         this.mWrapState = wrapState;
         _loc2_ = TIBINDEX_CHANGED;
      }
      return _loc2_;
   }
   function SetSnap(snapState)
   {
      var _loc2_ = TIBINDEX_CHANGE_NONE;
      if(snapState != this.mSnapState)
      {
         this.mSnapState = snapState;
         _loc2_ = TIBINDEX_CHANGED;
      }
      return _loc2_;
   }
   function IncIndex(Void)
   {
      var _loc4_ = TIBINDEX_CHANGE_NONE;
      var _loc2_ = this.mIndex + this.mStepSize;
      var _loc3_ = _loc2_ - this.mMaxIndex;
      if(_loc3_ > 0)
      {
         if(this.mWrapState && this.mMinIndex != this.mMaxIndex)
         {
            _loc2_ = this.mMinIndex + (_loc3_ - this.mStepSize);
         }
         else
         {
            _loc2_ = this.mMaxIndex;
         }
      }
      return this.UpdateIndex(_loc2_);
   }
   function DecIndex(Void)
   {
      var _loc2_ = this.mIndex - this.mStepSize;
      var _loc3_ = _loc2_ - this.mMinIndex;
      if(_loc3_ < 0)
      {
         if(this.mWrapState && this.mMinIndex != this.mMaxIndex)
         {
            _loc2_ = this.mMaxIndex + (_loc3_ + this.mStepSize);
         }
         else
         {
            _loc2_ = this.mMinIndex;
         }
      }
      return this.UpdateIndex(_loc2_);
   }
   function GetMinIndex(Void)
   {
      return this.mMinIndex;
   }
   function GetMaxIndex(Void)
   {
      return this.mMaxIndex;
   }
   function GetIndex(Void)
   {
      return this.mIndex;
   }
   function GetNumItems(Void)
   {
      return this.mMaxIndex - this.mMinIndex;
   }
   function GetStepSize(Void)
   {
      return this.mStepSize;
   }
   function Snap(index)
   {
      if(this.mSnapState && index != this.mMinIndex && index != this.mMaxIndex && this.mStepSize > 1)
      {
         if(index == this.mMaxIndex - this.mStepSize)
         {
            index = this.mMaxIndex - (index - this.mMinIndex) % this.mStepSize;
         }
         else
         {
            index -= (index - this.mMinIndex) % this.mStepSize;
         }
      }
      return index;
   }
   function UpdateIndex(newIndex)
   {
      var _loc3_ = TIBINDEX_CHANGE_NONE;
      if(newIndex < this.mMinIndex)
      {
         newIndex = this.mMinIndex;
      }
      if(newIndex > this.mMaxIndex)
      {
         newIndex = this.mMaxIndex;
      }
      newIndex = this.Snap(newIndex);
      if(newIndex != this.mIndex)
      {
         this.mIndex = newIndex;
         _loc3_ = TIBINDEX_CHANGED;
      }
      return _loc3_;
   }
   static function includeCode()
   {
      trace("CafeFW_IndexMgr::includeCode()");
   }
}
