// Only the CLR entry thunk lives in this module. Native asset routes enter in P4.
#using "Managed.Application.netmodule"
static_assert(sizeof(void*) == 4, "NewModCompiler supports Win32 only");
namespace NewModCompiler { namespace Hosting {
public ref class CompilerEntry abstract sealed {
public:
    static int Main(array<System::String^>^ arguments) {
        return NewModCompiler::Cli::Program::Main(arguments);
    }
};
}}
