#include "../Native/DataInitializers.h"
#include <msclr/marshal.h>

namespace Recovered {
    // A small independently buildable C++/CLI component. This does not pretend
    // to implement IAssetBuilderPlugin before the remaining pipeline exists.
    public ref class PrimitiveInitializers abstract sealed {
    public:
        static float Time(System::String^ text)
        {
            msclr::interop::marshal_context context;
            return Native::InitializeTime(context.marshal_as<const char*>(text));
        }
        static float Angle(System::String^ text)
        {
            msclr::interop::marshal_context context;
            return Native::InitializeAngle(context.marshal_as<const char*>(text));
        }
    };
}
