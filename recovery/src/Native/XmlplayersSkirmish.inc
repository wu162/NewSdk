// Original SkirmishSettings Marshal 52750; BuildSpeed 52736; root wrapper27500/Tracker111721.
const Field fieldsSkirmishSettings[]={
    {"RandomCrateSpawner",nullptr,8,Kind::Hash,""},
    {"LoadAfter",nullptr,4,Kind::Reference,""}
};
const Field fieldsSkirmishBuildSpeed[]={{"Speed",nullptr,0,Kind::Float,""}};
const Child childrenSkirmishSettings[]={{"HandicapBuildSpeed","SkirmishBuildSpeed",12,ChildKind::List}};
bool GetSkirmishLayout(const std::string& name,Layout& result){
    if(name=="SkirmishSettings")result={20,402082255u,fieldsSkirmishSettings,2,childrenSkirmishSettings,1};
    else if(name=="SkirmishBuildSpeed")result={4,0,fieldsSkirmishBuildSpeed,1,nullptr,0};
    else return false;
    return true;
}
