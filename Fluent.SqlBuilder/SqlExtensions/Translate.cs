using System;
using System.Collections.Generic;
using System.Text;

namespace Fluent.SqlQuery.SqlExtensions
{
    internal class Translate : Attribute
    {
        public Translate(string translation)
        {
            Translation = translation;
        }

        public string Translation { get; }
    }
}
