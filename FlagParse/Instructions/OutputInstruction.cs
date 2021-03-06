﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Instructions
{
    public class OutputInstruction : Instruction
    {
        public OutputInstruction(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }

        internal override void Accept(InstructionVisitor v)
        {
            v.Visit(this);
        }

        internal override T Accept<T>(InstructionVisitor<T> v)
        {
            return v.Visit(this);
        }

        public override string ToString()
        {
            return string.Concat("[OUTPUT: ", Text, "]");
        }
    }
}
