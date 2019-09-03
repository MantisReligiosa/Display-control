using System;

namespace Helpers
{
    public static class ExceptionExtenstion
    {
        public static Exception GetInnerException(this Exception exception) => exception.InnerException == null ? exception : GetInnerException(exception.InnerException);
    }
}
