// Diagnostic writer only. Never supplies candidate pixels or selects a codec rule.
#include <cstdio>
static FILE* traceFile=0;
extern "C" __declspec(dllexport) void __cdecl S3TraceFile(const char* path) {
    if(traceFile)fclose(traceFile);traceFile=0;
    if(path)fopen_s(&traceFile,path,"w");
}
void S3StageTrace(const char* stage,int n,const double* values) {
    if(!traceFile)return;fprintf(traceFile,"%s",stage);
    for(int i=0;i<n;i++)fprintf(traceFile,"\t%.17g",values[i]);fprintf(traceFile,"\n");fflush(traceFile);
}
extern "C" void __cdecl S3LegacyTrace(int stage,unsigned char* stack) {
    if(!traceFile)return;
    if(stage==0){unsigned int* args=(unsigned int*)stack;
        double head[2]={(double)args[1],(double)args[2]};S3StageTrace("legacy-round-mode",2,head);
        S3StageTrace("legacy-round-input",6,(double*)args[6]);
    }else{
        S3StageTrace("legacy-single-errors",3,(double*)(stack+0x50));
        double choice=(double)*(int*)(stack+0x18);S3StageTrace("legacy-single-choice",1,&choice);
    }
}
