using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Aston
{
    public class Options
    {
        public IMemberAccessStrategy MemberAccessStrategy { get; set; }
        public ICustomLiteralsProvider CustomLiteralProvider { get; set; }
    }
}
