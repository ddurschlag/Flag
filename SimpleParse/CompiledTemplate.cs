using System;
using System.IO;
public class Templates
{
    private TextWriter Writer;
    private void Write(string s) { Writer.Write(s); }
    public Templates(TextWriter writer
)
    { Writer = writer; }
    public void Call(dynamic tArg)
    {
        Write(tArg["Name"]); Write(@"(")
; Write(tArg["childVariable"]); Write(@")");
    }
    public void CallInline(dynamic tArg)
    { Instruction(tArg["Instructions"]); }
    public void Class(dynamic tArg) { Write(@"usi
ng System;
using System.IO;
public class "); Write(tArg["Name"]); Write(@" {
    private TextWriter Writer;
    private void Write(string s){Writer.Write(s);}

    public "); Write(tArg["Name"]); Write(@"(TextWriter writer){{Writer=writer;}}

    "); foreach (var anon_0 in tArg["Templates"]) { Write(@"

    public void "); Write(anon_0["Item1"]); Write(@"(dynamic tArg) {
        "); Template(anon_0["Item2"]); Write(@"
    }

    "); } Write(@"
}"); }
    public void Instruction(dynamic tArg) { Loop(tArg["Loop"]); Write(@"
"); LoopInline(tArg["LoopInline"]); Write(@"
"); Call(tArg["Call"]); Write(@"
"); CallInline(tArg["CallInline"]); }
    public void Loop(dynamic tArg) { Write(@"foreach ( var childElement in "); Write(tArg["ContextVariable"]); Write(@" ) {
"); Write(tArg["Name"]); Write(@"(childElement);
}
"); }
    public void LoopInline(dynamic tArg)
    {
        Write(@"foreach ( var "); Write(tArg["ChildVariable"]);Write(@" in ");Write(tArg["ContextVariable"]);Write(@" ) {
            ");Instruction(tArg["Instructions"]);Write(@"}");}
            public void Output(dynamic tArg) {Write(@"Write(@""");Write(tArg["Text"]);Write(@""");");
    }
    public void Render(dynamic tArg)
    {
        Write(@"Write("); Write(tArg["ContextVariable"]);Write(@"); ");}public void Template(dynamic tArg) {Instruction(tArg["Instructions"]);
    }
}