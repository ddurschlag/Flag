using System;
using System.IO;
namespace Flag.Compile.CSharp.Templates {
    public class Templates {
        private TextWriter Writer;
        private void Write(string s){Writer.Write(s);}

        public Templates(TextWriter writer){{Writer=writer;}}

        

        public void Call(dynamic tArg) {
            Write(@"if ( ");if ( tArg.ChildVariable != null ) {
Write(tArg.ChildVariable);
}
Write(@" != null ) { ");if ( tArg.Name != null ) {
Write(tArg.Name);
}
Write(@"(");if ( tArg.ChildVariable != null ) {
Write(tArg.ChildVariable);
}
Write(@"); }
");
        }   

        

        public void CallInline(dynamic tArg) {
            Write(@"if ( ");if ( tArg.ChildVariable != null ) {
Write(tArg.ChildVariable);
}
Write(@" != null ) {
");if ( tArg.Template != null ) { Template(tArg.Template); }
Write(@"
}
");
        }   

        

        public void Class(dynamic tArg) {
            Write(@"using System;
using System.IO;
namespace ");if ( tArg.Namespace != null ) {
Write(tArg.Namespace);
}
Write(@" {
    public class ");if ( tArg.Name != null ) {
Write(tArg.Name);
}
Write(@" {
        private TextWriter Writer;
        private void Write(string s){Writer.Write(s);}

        public ");if ( tArg.Name != null ) {
Write(tArg.Name);
}
Write(@"(TextWriter writer){{Writer=writer;}}

        ");if ( tArg.Templates != null ) {
if ( tArg.Templates != null )
foreach ( var anon_0 in tArg.Templates ) {
Write(@"

        public void ");if ( anon_0.Item1 != null ) {
Write(anon_0.Item1);
}
Write(@"(dynamic tArg) {
            ");if ( anon_0.Item2 != null ) { Template(anon_0.Item2); }
Write(@"
        }   

        ");
}
}
Write(@"
    }
}");
        }   

        

        public void Instructions(dynamic tArg) {
            if ( tArg != null )
foreach ( var anon_1 in tArg ) {
if ( anon_1.Loop != null ) { Loop(anon_1.Loop); }
if ( anon_1.LoopInline != null ) { LoopInline(anon_1.LoopInline); }
if ( anon_1.Call != null ) { Call(anon_1.Call); }
if ( anon_1.CallInline != null ) { CallInline(anon_1.CallInline); }
if ( anon_1.Output != null ) { Output(anon_1.Output); }
if ( anon_1.Render != null ) { Render(anon_1.Render); }

}
        }   

        

        public void Loop(dynamic tArg) {
            Write(@"if ( ");if ( tArg.ContextVariable != null ) {
Write(tArg.ContextVariable);
}
Write(@" != null ) foreach ( var childElement in ");if ( tArg.ContextVariable != null ) {
Write(tArg.ContextVariable);
}
Write(@" ) { ");if ( tArg.Name != null ) {
Write(tArg.Name);
}
Write(@"(childElement); }
");
        }   

        

        public void LoopInline(dynamic tArg) {
            Write(@"if ( ");if ( tArg.ContextVariable != null ) {
Write(tArg.ContextVariable);
}
Write(@" != null )
foreach ( var ");if ( tArg.ChildVariable != null ) {
Write(tArg.ChildVariable);
}
Write(@" in ");if ( tArg.ContextVariable != null ) {
Write(tArg.ContextVariable);
}
Write(@" ) {
");if ( tArg.Template != null ) { Template(tArg.Template); }
Write(@"
}");
        }   

        

        public void Output(dynamic tArg) {
            Write(@"Write(@""");if ( tArg.Text != null ) {
Write(tArg.Text);
}
Write(@""");");
        }   

        

        public void Render(dynamic tArg) {
            Write(@"Write(");if ( tArg.ContextVariable != null ) {
Write(tArg.ContextVariable);
}
Write(@");");
        }   

        

        public void Template(dynamic tArg) {
            if ( tArg.Instructions != null ) { Instructions(tArg.Instructions); }

        }   

        
    }
}