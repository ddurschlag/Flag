using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp
{
    public partial class Templates
    {


        public static void Call(dynamic tArg, TextWriter writer)
        {
            writer.Write(@"if ( "); if (tArg.ChildVariable != null)
            {
                writer.Write(tArg.ChildVariable);
            }
            writer.Write(@" != null ) { "); if (tArg.Name != null)
            {
                writer.Write(tArg.Name);
            }
            writer.Write(@"("); if (tArg.ChildVariable != null)
            {
                writer.Write(tArg.ChildVariable);
            }
            writer.Write(@", writer); }
");
        }



        public static void CallInline(dynamic tArg, TextWriter writer)
        {
            writer.Write(@"if ( "); if (tArg.ChildVariable != null)
            {
                writer.Write(tArg.ChildVariable);
            }
            writer.Write(@" != null ) {
"); if (tArg.Template != null) { Template(tArg.Template, writer); }
            writer.Write(@"
}
");
        }



        public static void Class(dynamic tArg, TextWriter writer)
        {
            writer.Write(@"using System;
using System.IO;
namespace "); if (tArg.Namespace != null)
            {
                writer.Write(tArg.Namespace);
            }
            writer.Write(@" {
    public partial class "); if (tArg.Name != null)
            {
                writer.Write(tArg.Name);
            }
            writer.Write(@" {
        "); if (tArg.Templates != null)
            {
                if (tArg.Templates != null)
                    foreach (var anon_0 in tArg.Templates)
                    {
                        writer.Write(@"

        public static void "); if (anon_0.Item1 != null)
                        {
                            writer.Write(anon_0.Item1);
                        }
                        writer.Write(@"(dynamic tArg, TextWriter writer) {
            "); if (anon_0.Item2 != null) { Template(anon_0.Item2, writer); }
                        writer.Write(@"
        }   

        ");
                    }
            }
            writer.Write(@"
    }
}");
        }



        public static void Instructions(dynamic tArg, TextWriter writer)
        {
            if (tArg != null)
                foreach (var anon_1 in tArg)
                {
                    if (anon_1.Loop != null) { Loop(anon_1.Loop, writer); }
                    if (anon_1.LoopInline != null) { LoopInline(anon_1.LoopInline, writer); }
                    if (anon_1.Call != null) { Call(anon_1.Call, writer); }
                    if (anon_1.CallInline != null) { CallInline(anon_1.CallInline, writer); }
                    if (anon_1.Output != null) { Output(anon_1.Output, writer); }
                    if (anon_1.Render != null) { Render(anon_1.Render, writer); }

                }
        }



        public static void Loop(dynamic tArg, TextWriter writer)
        {
            writer.Write(@"if ( "); if (tArg.ContextVariable != null)
            {
                writer.Write(tArg.ContextVariable);
            }
            writer.Write(@" != null ) foreach ( var childElement in "); if (tArg.ContextVariable != null)
            {
                writer.Write(tArg.ContextVariable);
            }
            writer.Write(@" ) { "); if (tArg.Name != null)
            {
                writer.Write(tArg.Name);
            }
            writer.Write(@"(childElement); }
");
        }



        public static void LoopInline(dynamic tArg, TextWriter writer)
        {
            writer.Write(@"if ( "); if (tArg.ContextVariable != null)
            {
                writer.Write(tArg.ContextVariable);
            }
            writer.Write(@" != null )
foreach ( var "); if (tArg.ChildVariable != null)
            {
                writer.Write(tArg.ChildVariable);
            }
            writer.Write(@" in "); if (tArg.ContextVariable != null)
            {
                writer.Write(tArg.ContextVariable);
            }
            writer.Write(@" ) {
"); if (tArg.Template != null) { Template(tArg.Template, writer); }
            writer.Write(@"
}");
        }



        public static void Output(dynamic tArg, TextWriter writer)
        {
            writer.Write(@"writer.Write(@"""); if (tArg.Text != null)
            {
                writer.Write(tArg.Text);
            }
            writer.Write(@""");");
        }



        public static void Render(dynamic tArg, TextWriter writer)
        {
            writer.Write(@"writer.Write("); if (tArg.ContextVariable != null)
            {
                writer.Write(tArg.ContextVariable);
            }
            writer.Write(@");");
        }



        public static void Template(dynamic tArg, TextWriter writer)
        {
            if (tArg.Instructions != null) { Instructions(tArg.Instructions, writer); }

        }


    }
}