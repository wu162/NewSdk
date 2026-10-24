// Fixed original Module: 46808-46993; original Plugin wrappers allocate 24/12/16/40-byte roots.
const Field fieldsFormationSelection[]={
    {"FormationAssistantTemplate",nullptr,4,Kind::Reference,""},
    {"MaxDragLength",nullptr,8,Kind::Float,""},
    {"MaxUnitsSelected",nullptr,12,Kind::Unsigned,""}
};
const Field fieldsFormationAssistant[]={
    {"DefaultPreviewObject",nullptr,4,Kind::Hash,""},
    {"FacingArrowTemplateHead",nullptr,8,Kind::Hash,""},
    {"FacingArrowTemplateBody",nullptr,12,Kind::Hash,""},
    {"FacingArrowTemplateBase",nullptr,16,Kind::Hash,""},
    {"RowPadding",nullptr,20,Kind::Float,""},
    {"ColumnPadding",nullptr,24,Kind::Float,""},
    {"ActivateDragDistance",nullptr,28,Kind::Float,""},
    {"ActivateTime",nullptr,32,Kind::Float,""}
};
const Child childrenFormationUnit[]={
    {"IncludeObjectFilter","ObjectFilter",4,ChildKind::List},
    {"ExcludeObjectFilter","ObjectFilter",12,ChildKind::List},
    {"PreviewObject","HashText",20,ChildKind::Pointer}
};
const Child childrenFormationTemplate[]={{"Row","FormationRow",4,ChildKind::List}};
const Child childrenFormationRow[]={{"Cell","FormationCell",0,ChildKind::List}};
const Child childrenFormationCell[]={{"Definition","ReferenceText",0,ChildKind::List}};
const Child childrenFormationAssistant[]={{"ObjectFilter","ObjectFilter",36,ChildKind::Pointer}};
bool GetFormationLayout(const std::string& name,Layout& result){
    if(name=="FormationAssistantUnitDefinition")result={24,838130914u,nullptr,0,childrenFormationUnit,3};
    else if(name=="FormationAssistantTemplate")result={12,2912385462u,nullptr,0,childrenFormationTemplate,1};
    else if(name=="FormationSelectionTemplate")result={16,3374519264u,fieldsFormationSelection,3,nullptr,0};
    else if(name=="FormationAssistant")result={40,1963196508u,fieldsFormationAssistant,8,childrenFormationAssistant,1};
    else if(name=="FormationRow")result={8,0,nullptr,0,childrenFormationRow,1};
    else if(name=="FormationCell")result={8,0,nullptr,0,childrenFormationCell,1};
    else if(name=="ObjectFilter")result={112,0,nullptr,0,nullptr,0};
    else return false;
    return true;
}
