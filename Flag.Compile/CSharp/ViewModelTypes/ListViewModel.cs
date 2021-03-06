﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp.ViewModelTypes
{
    public class ListViewModel : ViewModelType
    {
        public ListViewModel(string typeName, string enumerableTypeName, IEnumerable<ViewModelType> innerTypes)
        :base(typeName, innerTypes)
        {
            EnumerableTypeName = enumerableTypeName;
        }
        public string EnumerableTypeName { get; private set; }

        public override void Accept(ViewModelTypeVisitor v)
        {
            v.Visit(this);
        }

        public override T Accept<T>(ViewModelTypeVisitor<T> v)
        {
            return v.Visit(this);
        }
    }
}
