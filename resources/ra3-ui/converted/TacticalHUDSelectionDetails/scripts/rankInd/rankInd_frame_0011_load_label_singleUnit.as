onClipEvent(load){
   labelText = "$" + qualifyName(_parent,"RankLabel");
   if(_parent.rankFactionLabel != undefined)
   {
      factionLabel = _parent.rankFactionLabel;
   }
   if(_parent.rank != undefined)
   {
      gotoAndPlay("_" + String(_parent.rank));
   }
}
